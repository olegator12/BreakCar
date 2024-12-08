using System;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class PainterCard : MonoBehaviour
    {
        [SerializeField] private Button _button;
        [SerializeField] private Image _icon;

        private AudioSource _sound;
        private Action<int> _onClick;
        private int _id;

        private void OnDestroy()
        {
            _button.onClick.RemoveListener(OnClick);
        }

        public void Initialize(Sprite sprite, Color color, Action<int> onClick, int id, AudioSource sound)
        {
            _icon.sprite = sprite;
            _icon.color = color;
            _onClick = onClick;
            _id = id;
            _sound = sound;

            _button.onClick.AddListener(OnClick);
        }

        private void OnClick()
        {
            _sound.Play();
            _onClick.Invoke(_id);
        }
    }
}