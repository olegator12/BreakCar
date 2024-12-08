using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public abstract class Bar : MonoBehaviour
    {
        [SerializeField] private Image _slider;

        public void Change(int current, float max)
        {
            _slider.fillAmount = current / max;
        }
    }
}