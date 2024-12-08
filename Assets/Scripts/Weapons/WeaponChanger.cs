using System;
using System.Collections.Generic;
using System.Linq;
using UI;
using YG;
using Random = UnityEngine.Random;

namespace Weapons
{
    public class WeaponChanger
    {
        private const int MinValue = 0;

        private readonly Func<WeaponType, IWeapon> _weaponRequestHelper;
        private readonly WeaponPanel _panel;

        private List<WeaponType> _availableTypes;

        public WeaponChanger(
            Func<WeaponType, IWeapon> weaponRequestHelper,
            WeaponPanel panel)
        {
            _weaponRequestHelper = weaponRequestHelper;
            _panel = panel;

            _panel.RegisterCallbacks(OnChoosing, OnChoosing);
        }

        public event Action<IWeapon> Changed;

        public void ShowPanel()
        {
            _availableTypes = YandexGame.savesData.openedWeapons.Cast<WeaponType>().ToList();
            bool havePriority = YandexGame.savesData.priorityWeapon >= MinValue;
            int priority = YandexGame.savesData.priorityWeapon;

            List<WeaponType> availableWeapons = _availableTypes.ToList();
            WeaponType left = GetRandomType(availableWeapons);
            availableWeapons.Remove(left);
            WeaponType right = GetRandomType(availableWeapons);
            availableWeapons.Remove(right);
            WeaponType advert = GetRandomType(availableWeapons);

            if (havePriority == true && (int)left != priority && (int)right != priority)
            {
                advert = (WeaponType)priority;
                YandexGame.savesData.priorityWeapon = -1;
            }

            _panel.Show(left, right, advert);
        }

        private void OnChoosing(WeaponType name)
        {
            _panel.Close();
            Changed?.Invoke(_weaponRequestHelper.Invoke(name));
        }

        private WeaponType GetRandomType(List<WeaponType> collection)
        {
            return collection[Random.Range(MinValue, collection.Count)];
        }
    }
}