using Destruction;
using UnityEngine;

namespace Configs
{
    [CreateAssetMenu(fileName = "Car Configuration", menuName = "Car Configuration", order = 3)]
    public class CarConfiguration : UpdatableConfiguration<CarPart, PartSettings>
    {
        [field: SerializeField]
        [field: Range(0f, 1f)]
        public float MinDestroyedPercent { get; private set; } = 0.7f;
    }
}