using System;
using System.Collections.Generic;
using Economic;
using UI;
using UnityEngine;
using YG;

public class Advert : AdvertRegistration
{
    private const string NullException = "Callback not registered";
    private const int MoneyRewardCount = 200;

    [SerializeField] private CardHolder _card;

    private Dictionary<AdvertName, Action> _callbacks;
    private Wallet _wallet;

    private void OnDestroy()
    {
        YandexGame.RewardVideoEvent -= OnRewarded;
    }

    public void Initialize(Wallet wallet)
    {
        _callbacks = new Dictionary<AdvertName, Action>();
        _wallet = wallet;

        YandexGame.RewardVideoEvent += OnRewarded;
    }

    public override void RegisterCallback(AdvertName name, Action callback)
    {
        _callbacks[name] = callback;
    }

    private void OnRewarded(int name)
    {
        if (_callbacks.TryGetValue((AdvertName)name, out Action callback) == false)
            throw new ArgumentNullException(NullException);

        switch ((AdvertName)name)
        {
            case AdvertName.TwoHundredMoney:
            case AdvertName.TwoHundredMoneyCar:
                AccrualMoney();
                break;
        }

        callback.Invoke();
    }

    private void AccrualMoney()
    {
        _wallet.AddMoney(MoneyRewardCount);
        _wallet.ApplyModification();
        _card.Draw();
    }
}