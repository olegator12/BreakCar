using UnityEngine;

namespace Destruction
{
    [RequireComponent(typeof(MeshRenderer))]
    public class PaintPart : VisitablePart
    {
        [SerializeField] private MeshRenderer _renderer;
        [SerializeField] private Material[] _startMaterials;

        public void Initialize()
        {
            _renderer = GetComponent<MeshRenderer>();
            _startMaterials = _renderer.materials;
            OnInitialize();
        }

        public virtual void OnInitialize()
        {
        }

        public void Paint(Material material)
        {
            Material[] materials = _renderer.materials;

            for (int i = 0; i < _renderer.materials.Length; i++)
                materials[i] = material;

            _renderer.materials = materials;

            OnPainting(material);
        }

        public void Return()
        {
            _renderer.materials = _startMaterials;
            OnReturn();
        }

        public virtual void OnPainting(Material material)
        {
        }

        public virtual void OnReturn()
        {
        }

        public override void Accept(IPartVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}