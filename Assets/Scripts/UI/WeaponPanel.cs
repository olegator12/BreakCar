using System;
using System.Collections.Generic;
using UnityEngine;
using Weapons;

namespace UI
{
    public class WeaponPanel : MonoBehaviour
    {
        [SerializeField] private UIWindow _window;
        [SerializeField] private WeaponCell _left;
        [SerializeField] private WeaponCell _right;
        [SerializeField] private AdvertCell _advert;

        private Dictionary<WeaponType, Sprite> _icons;

        public void Initialize(Dictionary<WeaponType, Sprite> icons)
        {
            _icons = icons;
            _window.Initialize();
            _left.Initialize();
            _right.Initialize();
            _advert.Initialize();
        }

        public void RegisterCallbacks(
            Action<WeaponType> onChoosing,
            Action<WeaponType> onChoosingAdvert,
            Action onClickAdvert = null)
        {
            _left.RegisterCallback(onChoosing);
            _right.RegisterCallback(onChoosing);
            _advert.RegisterCallback(onChoosingAdvert, onClickAdvert);
        }

        public void Show(
            WeaponType left,
            WeaponType right,
            WeaponType advert)
        {
            _window.Open();
            _left.Change(left, _icons[left]);
            _right.Change(right, _icons[right]);
            _advert.Change(advert, _icons[advert]);
        }

        public void Close()
        {
            _window.Close();
        }
    }
}