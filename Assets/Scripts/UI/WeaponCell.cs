using System;
using UnityEngine;
using UnityEngine.UI;
using Weapons;

namespace UI
{
    [RequireComponent(typeof(Button))]
    [RequireComponent(typeof(Image))]
    public class WeaponCell : MonoBehaviour
    {
        private Button _button;
        private Image _icon;

        private Action<WeaponType> _onChoosing;
        private Action _onClick;
        private WeaponType _name;

        private void OnDestroy()
        {
            if (_button == null)
                return;

            _button.onClick.RemoveListener(OnClick);
        }

        public void Initialize()
        {
            _button = GetComponent<Button>();
            _icon = GetComponent<Image>();

            _button.onClick.AddListener(OnClick);
        }

        public void Change(WeaponType name, Sprite sprite)
        {
            _name = name;
            _icon.sprite = sprite;
        }

        public void RegisterCallback(Action<WeaponType> onChoosing, Action onClick = null)
        {
            _onChoosing = onChoosing;
            _onClick = onClick ?? Choose;

            OnRegister(onClick);
        }

        public void Choose()
        {
            _onChoosing.Invoke(_name);
        }

        public virtual void OnRegister(Action onClick)
        {
        }

        private void OnClick()
        {
            _onClick.Invoke();
        }
    }
}