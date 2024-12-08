using UnityEngine;

namespace Destruction
{
    [RequireComponent(typeof(PaintWindow))]
    public class Window : Glass
    {
        public override CarPart Name => CarPart.Window;
    }
}