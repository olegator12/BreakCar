using System;
using System.Collections.Generic;
using UnityEngine;
using YG;

namespace Weapons
{
    [Serializable]
    public class WeaponSettings
    {
        [SerializeField] private List<DamageType> _damageTypes = new ();
        [SerializeField] private float _fov = 65f;
        [SerializeField] private float _desktopFOVScale = 0.92f;

        [field: SerializeField] public int Uses { get; private set; } = 10;

        [field: SerializeField] public float DamageValue { get; private set; }

        [field: SerializeField] public float Speed { get; private set; } = 0.25f;

        [field: SerializeField] public float DelayModifier { get; private set; } = 1f;

        [field: SerializeField] public float CameraShakeModifier { get; private set; } = 0.5f;

        [field: SerializeField] public float DeformationModifier { get; private set; } = 1f;

        [field: SerializeField] public float RotationForce { get; private set; } = 0.2f;

        [field: SerializeField] public bool IsAutomaticGun { get; private set; } = false;

        [field: SerializeField] public Sprite Icon { get; private set; }

        public float FOV => YandexGame.EnvironmentData.isDesktop ? _fov * _desktopFOVScale : _fov;

        public IReadOnlyList<DamageType> DamageTypes => _damageTypes;
    }
}