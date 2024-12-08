using UnityEngine;

namespace Configs
{
    [CreateAssetMenu(fileName = "General Settings", menuName = "General Settings", order = 0)]
    public class GeneralSettings : ScriptableObject
    {
        [field: SerializeField] public float TouchInterval { get; private set; } = 0.4f;

        [field: SerializeField] public float BoostTouchInterval { get; private set; } = 0.2f;

        [field: SerializeField] public float BoostTimeOut { get; private set; } = 8f;

        [field: SerializeField] public float BoostDamageMultiplier { get; private set; } = 2f;

        [field: SerializeField] public float PlatformRotationSpeed { get; private set; } = 0.3f;
    }
}