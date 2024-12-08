using System.Collections.Generic;
using UnityEngine;

namespace Configs
{
    [CreateAssetMenu(fileName = "Characters Provider", menuName = "Characters Provider", order = -1)]
    public class CharactersProvider : ScriptableObject
    {
        [field: SerializeField] public List<Sprite> Characters { get; private set; }

        [field: SerializeField] public List<Sprite> Reasons { get; private set; }
    }
}