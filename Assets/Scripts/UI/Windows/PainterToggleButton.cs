using System.Collections.Generic;
using UnityEngine;

namespace UI
{
    public class PainterToggleButton : WindowToggleButton<PainterWindow>
    {
        [SerializeField] private GameObject _highlight;
        [SerializeField] private List<GameObject> _other;

        public override void OnInteract()
        {
            foreach (GameObject highlight in _other)
                highlight.SetActive(false);

            _highlight.SetActive(true);
        }
    }
}