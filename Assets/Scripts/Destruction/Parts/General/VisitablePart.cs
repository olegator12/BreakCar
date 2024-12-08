using UnityEngine;

namespace Destruction
{
    public abstract class VisitablePart : MonoBehaviour
    {
        public abstract void Accept(IPartVisitor visitor);
    }
}