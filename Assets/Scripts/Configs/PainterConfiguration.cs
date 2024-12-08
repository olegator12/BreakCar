using System.Collections.Generic;
using AYellowpaper.SerializedCollections;
using Destruction;
using UI;
using UnityEngine;

namespace Configs
{
    [CreateAssetMenu(fileName = "Painter Configuration", menuName = "Painter Configuration", order = 4)]
    public class PainterConfiguration : ScriptableObject
    {
        [field: SerializeField] public PainterCard CarTemplate { get; private set; }

        [field: SerializeField] public Material GlassTemplate { get; private set; }

        [field: SerializeField] public Material MatteTemplate { get; private set; }

        [field: SerializeField] public Material GlossyTemplate { get; private set; }

        [field: SerializeField] public Material ChromeTemplate { get; private set; }

        [field: SerializeField] public Sprite Clear { get; private set; }

        [field: SerializeField] public Sprite ColorPicker { get; private set; }

        [field: SerializeField] public Sprite Color { get; private set; }

        [field: SerializeField] public Sprite Window { get; private set; }

        [field: SerializeField] public List<Color> Colors { get; private set; }

        [field: SerializeField] public List<Color> Windows { get; private set; }

        [field: SerializeField] public List<SerializedPair<Sprite, Wheel>> Wheels { get; private set; }

        [field: SerializeField]
        public SerializedDictionary<WheelSize, SerializedDictionary<WheelSize, float>> WheelScales { get; private set; }
    }
}