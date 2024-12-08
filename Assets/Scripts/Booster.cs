using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using TMPro;
using UI;
using UnityEngine;
using YG;
using Random = UnityEngine.Random;

public class Booster : MonoBehaviour
{
    private const float Delay = 0.1f;
    private const string RU = "ru";

    private readonly TimeSpan _time = TimeSpan.FromSeconds(Delay);

    [SerializeField] private EventButton _speed;
    [SerializeField] private EventButton _ammo;
    [SerializeField] private EventButton _damage;
    [SerializeField] private GameObject _timer;
    [SerializeField] private TMP_Text _text;

    private Dictionary<GameObject, (Action openCallback, Action hideCallback)> _events;
    private List<GameObject> _gameObjects;
    private GameObject _current;
    private bool _canWork;

    public bool IsTimerActive => _timer.activeSelf;

    public void Initialize(
        Action onClick,
        (Action openCallback, Action hideCallback) speedHandler,
        (Action openCallback, Action hideCallback) ammoHandler,
        (Action openCallback, Action hideCallback) damageHandler)
    {
        _canWork = YandexGame.EnvironmentData.language != RU;
        _speed.Initialize(onClick);
        _ammo.Initialize(onClick);
        _damage.Initialize(onClick);

        _gameObjects = new List<GameObject>()
        {
            _speed.gameObject,
            _ammo.gameObject,
            _damage.gameObject,
        };

        _events = new Dictionary<GameObject, (Action openCallback, Action hideCallback)>()
        {
            { _gameObjects[0], speedHandler },
            { _gameObjects[1], ammoHandler },
            { _gameObjects[2], damageHandler },
        };
    }

    public void Hide()
    {
        if (_canWork == false)
            return;

        _current.SetActive(false);
        _events[_current].openCallback.Invoke();
    }

    public void HideEmpty()
    {
        if (_canWork == false)
            return;

        if (_current == null)
            return;

        _current.SetActive(false);
    }

    public void Show()
    {
        if (_canWork == false)
            return;

        _events[_current].hideCallback.Invoke();
        _current = _gameObjects[Random.Range(0, _gameObjects.Count)];
        _current.SetActive(true);
    }

    public void ShowEmpty()
    {
        if (_canWork == false)
            return;

        _current = _gameObjects[Random.Range(0, _gameObjects.Count)];
        _current.SetActive(true);
    }

    public void ShowTimer(float time)
    {
        if (_canWork == false)
            return;

        Wait(time).Forget();
    }

    private async UniTaskVoid Wait(float time)
    {
        _text.SetText($"{time:F1}");
        _timer.SetActive(true);

        while (time > 0f)
        {
            await UniTask.Delay(_time, cancellationToken: destroyCancellationToken);
            time -= Delay;
            _text.SetText($"{time:F1}");
        }

        _timer.SetActive(false);
    }
}