using System.Collections.Generic;
using Destruction;
using UnityEngine;

namespace Configs
{
    [CreateAssetMenu(fileName = "Car Prices Configuration", menuName = "Car Prices Configuration", order = 1)]
    public class CarPricesConfiguration : ScriptableObject
    {
        [field: SerializeField] public List<SerializedPair<CarSetup, int>> Cars { get; private set; }
    }
}