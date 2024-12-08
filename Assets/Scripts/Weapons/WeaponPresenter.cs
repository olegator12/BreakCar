using System;
using System.Threading;
using Configs;
using Cysharp.Threading.Tasks;
using Destruction;
using UI;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using YG;

namespace Weapons
{
    public class WeaponPresenter
    {
        private readonly GeneralSettings _settings;
        private readonly InputSystem _input;
        private readonly Caster _caster;
        private readonly WeaponChanger _changer;
        private readonly Ammunition _ammunition;
        private readonly AmmunitionBar _ammunitionBar;
        private readonly Booster _booster;
        private readonly TimeSpan _boostCoolDown;
        private readonly bool _isMobile;
        private readonly float _damageModifier;
        private readonly TutorialAnimation _tutorial;

        private IWeapon _weapon;
        private PartHighlighter _highlighter;
        private CancellationTokenSource _cancellation;
        private TimeSpan _touchDelay;
        private TimeSpan _touchInterval;
        private TimeSpan _boostInterval;
        private bool _isTouching;
        private bool _didTouch;
        private bool _isFreeAmmo;

        public WeaponPresenter(
            GeneralSettings settings,
            AdvertRegistration registration,
            Booster boostButton,
            Caster caster,
            WeaponChanger changer,
            AmmunitionBar ammunitionBar,
            TutorialAnimation tutorial)
        {
            _input = new InputSystem();
            _ammunition = new Ammunition();

            _settings = settings;
            _caster = caster;
            _changer = changer;
            _ammunitionBar = ammunitionBar;
            _booster = boostButton;
            _tutorial = tutorial;
            _boostCoolDown = TimeSpan.FromSeconds(_settings.BoostTimeOut);
            _isMobile = YandexGame.EnvironmentData.isDesktop == false;
            _damageModifier = settings.BoostDamageMultiplier;

            registration.RegisterCallback(AdvertName.Boost, OnBoostCollected);
            _booster.Initialize(
                () => YandexGame.RewVideoShow((int)AdvertName.Boost),
                (SetBoostDelay, SetTouchDelay),
                (SetFree, SetPaid),
                (ModifyDamage, SetDefaultDamage));
        }

        public void Enable()
        {
            _input.Enable();
            _input.Player.Touch.performed += OnTouched;
            _input.Player.Touch.canceled += OnTouchEnded;

            _changer.Changed += OnWeaponChanged;
            _ammunition.Changed += OnAmmunitionChanged;
            _ammunition.Spent += OnAmmunitionSpent;
        }

        public void Disable()
        {
            Dispose();
            _ammunition.SpendAll();
            _booster.HideEmpty();
            _weapon?.UpdateDamage();

            if (_weapon is IRevertible revertible)
                revertible.Revert();

            _weapon = null;

            _input.Disable();
            _input.Player.Touch.performed -= OnTouched;
            _input.Player.Touch.canceled -= OnTouchEnded;

            _changer.Changed -= OnWeaponChanged;
            _ammunition.Changed -= OnAmmunitionChanged;
            _ammunition.Spent -= OnAmmunitionSpent;
        }

        public void Register(PartHighlighter highlighter)
        {
            _highlighter = highlighter;
        }

        private void OnTouched(InputAction.CallbackContext _)
        {
            if (_weapon == null)
                return;

            if (_isTouching == true)
                return;

            if (EventSystem.current.IsPointerOverGameObject() == true)
                return;

            if (_weapon is IUpdatable updatable)
                updatable.UpdateDelay(_touchDelay);

            if (_weapon is IRotatable == true)
                ReadRotatable().Forget();
            else
                ReadTouch().Forget();
        }

        private void OnTouchEnded(InputAction.CallbackContext _)
        {
            Dispose();
            _isTouching = false;

            if (_weapon is ICancellable cancellable)
                cancellable.Cancel();
        }

        private void OnWeaponChanged(IWeapon weapon)
        {
            if (_tutorial.IsActive == true)
                _tutorial.Stop();

            _weapon?.UpdateDamage();
            _weapon = weapon;
            _booster.ShowEmpty();
            _touchInterval = TimeSpan.FromSeconds(_settings.TouchInterval * _weapon.DelayModifier);
            _boostInterval = TimeSpan.FromSeconds(_settings.BoostTouchInterval * _weapon.DelayModifier);
            _touchDelay = _booster.IsTimerActive ? _boostInterval : _touchInterval;
            _ammunition.Load(_weapon.Uses);
            _weapon.Prepare();

            if (weapon.Name == WeaponType.Knife)
                _highlighter.HighlightWheels();

            if (weapon.Name == WeaponType.EmergencyHammer)
                _highlighter.HighlightGlasses();
        }

        private void OnAmmunitionChanged(int current, int max)
        {
            _ammunitionBar.Change(current, max);
        }

        private void OnAmmunitionSpent()
        {
            if (_weapon is IRevertible revertible)
                revertible.Revert();

            _changer.ShowPanel();
            _weapon = null;
            _booster.HideEmpty();
        }

        private async void OnBoostCollected()
        {
            _booster.Hide();
            _booster.ShowTimer((float)_boostCoolDown.TotalSeconds);
            await UniTask.Delay(_boostCoolDown);
            _booster.Show();
        }

        private void Dispose()
        {
            if (_cancellation == null)
                return;

            if (_cancellation.IsCancellationRequested == false)
                _cancellation.Cancel();

            _cancellation.Dispose();
            _cancellation = null;
        }

        private async UniTaskVoid ReadTouch()
        {
            if (_isTouching == true)
                return;

            _isTouching = true;
            _cancellation = new CancellationTokenSource();
            await UniTask.WaitUntil(CanAttack, cancellationToken: _cancellation.Token);
            DropTouchDelay().Forget();

            while (_cancellation?.IsCancellationRequested == false)
            {
                Vector2 screenPosition = _input.Player.ScreenPosition.ReadValue<Vector2>();

                if (_ammunition.CanSpend() == false)
                    break;

                if (_caster.TryDemolish(screenPosition, out RaycastHit hit, _weapon is TrajectoryWeapon == true) ==
                    false)
                {
                    await Wait();
                    continue;
                }

                _weapon.Attack(hit.point, hit.normal, TrySpend);

                if (_weapon is IMeleeWeapon weapon == false)
                {
                    await Wait();
                    continue;
                }

                if (hit.collider.TryGetComponent(out IBreakable breakable) == false)
                {
                    await Wait();
                    continue;
                }

                if (breakable is Body { IsForwardDeformation: false } body)
                {
                    body.PlaySound();
                    await Wait();
                    continue;
                }

                breakable.Break(
                    hit.point,
                    hit.normal,
                    _caster.GetCastDirection(hit.point).normalized,
                    _weapon.DeformationModifier,
                    weapon.DamageTypes,
                    weapon.DamageValue);

                await Wait();
            }
        }

        private async UniTaskVoid ReadRotatable()
        {
            if (_isTouching == true)
                return;

            _isTouching = true;
            _cancellation = new CancellationTokenSource();
            IRotatable rotatable = (IRotatable)_weapon;
            _weapon.Attack(Vector3.zero, Vector3.zero, TrySpend);

            while (_cancellation?.IsCancellationRequested == false)
            {
                if (_ammunition.CanSpend() == false)
                    break;

                Vector2 screenPosition = _input.Player.ScreenPosition.ReadValue<Vector2>();
                rotatable.Read(screenPosition);

                if (_isMobile == false &&
                    _caster.TryDemolish(
                        screenPosition,
                        out RaycastHit hit,
                        _weapon is TrajectoryWeapon == true) == true)
                {
                    rotatable.Read(hit.point);
                }

                await UniTask.NextFrame(PlayerLoopTiming.Update, _cancellation.Token);
            }
        }

        private UniTask Wait()
        {
            return UniTask.Delay(_touchDelay, cancellationToken: _cancellation.Token);
        }

        private bool CanAttack()
        {
            return _didTouch == false;
        }

        private bool TrySpend()
        {
            if (_ammunition.CanSpend() == false)
                return false;

            if (_isFreeAmmo == false)
                _ammunition.Spend();

            return true;
        }

        private async UniTaskVoid DropTouchDelay()
        {
            _didTouch = true;
            await UniTask.Delay(_touchDelay);
            _didTouch = false;
        }

        private void SetTouchDelay()
        {
            _touchDelay = _touchInterval;
            UpdateRotatableDelay();
        }

        private void SetBoostDelay()
        {
            _touchDelay = _boostInterval;
            UpdateRotatableDelay();
        }

        private void UpdateRotatableDelay()
        {
            if (_weapon is IUpdatable updatable == false)
                return;

            updatable.UpdateDelay(_touchDelay);
        }

        private void SetFree()
        {
            _isFreeAmmo = true;
        }

        private void SetPaid()
        {
            _isFreeAmmo = false;
        }

        private void SetDefaultDamage()
        {
            _weapon.UpdateDamage();
        }

        private void ModifyDamage()
        {
            _weapon.UpdateDamage(_damageModifier);
        }
    }
}