using System;
using UnityEngine;

namespace UI
{
    public class AdvertCell : WeaponCell
    {
        [SerializeField] private AdvertRegistration _registration;
        [SerializeField] private GameObject _background;

        public override void OnRegister(Action onClick)
        {
            if (onClick == null)
            {
                if (_background.activeSelf == true)
                    _background.SetActive(false);

                return;
            }

            //_registration.RegisterCallback(AdvertName.SuperWeapon, Choose);
        }
    }
}