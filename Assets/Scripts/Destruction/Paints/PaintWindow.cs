using System.Collections.Generic;
using UnityEngine;

namespace Destruction
{
    public class PaintWindow : PaintPart
    {
        [SerializeField] private List<MeshRenderer> _renderers;
        [SerializeField] private Material _start;
        [SerializeField] private Transform _holder;

        public void RegisterHolder(Transform holder)
        {
            _holder = holder;
        }

        public override void OnInitialize()
        {
            for (int i = 0; i < _holder.childCount; i++)
                _renderers.Add(_holder.GetChild(i).GetComponent<MeshRenderer>());

            _start = _renderers[0].materials[0];
        }

        public override void OnPainting(Material material)
        {
            foreach (MeshRenderer meshRenderer in _renderers)
            {
                Material[] materials = meshRenderer.materials;
                materials[0] = material;
                meshRenderer.materials = materials;
            }
        }

        public override void OnReturn()
        {
            OnPainting(_start);
        }
    }
}