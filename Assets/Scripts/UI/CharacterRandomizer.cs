using System.Collections.Generic;
using System.Linq;
using Configs;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace UI
{
    public class CharacterRandomizer : MonoBehaviour
    {
        private const int MinValue = 0;

        [SerializeField] private CharactersProvider _provider;
        [SerializeField] private Image _firstCharacter;
        [SerializeField] private Image _secondCharacter;
        [SerializeField] private Image _firstReason;
        [SerializeField] private Image _secondReason;

        public void Randomize()
        {
            Sprite character = GetRandom(null, _provider.Characters);
            Sprite reason = GetRandom(null, _provider.Reasons);

            _firstCharacter.sprite = character;
            _secondCharacter.sprite = GetRandom(character, _provider.Characters);

            _firstReason.sprite = reason;
            _secondReason.sprite = GetRandom(reason, _provider.Reasons);
        }

        private Sprite GetRandom(Sprite exclude, List<Sprite> keep)
        {
            List<Sprite> available = exclude == null ? keep : keep.Where(item => item != exclude).ToList();
            return available[Random.Range(MinValue, available.Count)];
        }
    }
}