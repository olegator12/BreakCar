using UnityEngine;

namespace Destruction
{
    [RequireComponent(typeof(PaintPart))]
    public class Body : SimpleDeformable
    {
        [field: SerializeField] public bool IsForwardDeformation = false;

        public override CarPart Name => CarPart.Body;
    }
}