using UnityEngine;

namespace Destruction
{
    [RequireComponent(typeof(PaintPart))]
    public class Roof : SimpleDeformable
    {
        public override CarPart Name => CarPart.Roof;
    }
}