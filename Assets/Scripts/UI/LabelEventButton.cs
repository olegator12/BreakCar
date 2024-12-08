using TMPro;
using UnityEngine;

namespace UI
{
    public class LabelEventButton : EventButton
    {
        [SerializeField] private TMP_Text _text;

        public void SetText(string value)
        {
            _text.SetText(value);
        }
    }
}