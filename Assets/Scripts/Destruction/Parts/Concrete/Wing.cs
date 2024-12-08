using UnityEngine;

namespace Destruction
{
    [RequireComponent(typeof(PaintPart))]
    public class Wing : SimpleKnocker
    {
        public override CarPart Name => CarPart.Wing;
    }
}