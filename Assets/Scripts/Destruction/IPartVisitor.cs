namespace Destruction
{
    public interface IPartVisitor
    {
        public void Visit(DeformableMesh deformableMesh);

        public void Visit(Glass glass);

        public void Visit(Wheel wheel);

        public void Visit(PaintPart paint);
    }
}