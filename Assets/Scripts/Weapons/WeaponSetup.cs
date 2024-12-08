using System;
using System.Collections.Generic;
using Configs;
using UI;
using UnityEngine;

namespace Weapons
{
    public class WeaponSetup : MonoBehaviour
    {
        [SerializeField] private List<Weapon> _weapons;
        [SerializeField] private WeaponConfiguration _configuration;

        private Dictionary<WeaponType, IWeapon> _castedWeapons;
        private ICameraShaker _cameraShaker;

        public void Initialize(Transform poolHolder, ICameraShaker cameraShaker, ObjectPool<SpawnableSound> soundPool)
        {
            Dictionary<WeaponType, WeaponSettings> configuration = _configuration.GetConfiguration();
            _castedWeapons = new Dictionary<WeaponType, IWeapon>();

            foreach (Weapon item in _weapons)
            {
                IWeapon weapon = (IWeapon)item;
                item.Initialize(configuration[weapon.Name], cameraShaker);
                _castedWeapons.Add(weapon.Name, weapon);

                if (weapon is IRangeWeapon rangeWeapon)
                    rangeWeapon.RegisterHolder(poolHolder);

                if (weapon is IRotatable rotatable)
                    rotatable.InitializeSound(soundPool);
            }
        }

        public void Clear()
        {
            foreach (Weapon weapon in _weapons)
            {
                if (weapon is IRangeWeapon rangeWeapon == false)
                    continue;

                rangeWeapon.Clear();
            }
        }

        public IWeapon GetWeapon(WeaponType name)
        {
            return _castedWeapons[name];
        }
    }
}