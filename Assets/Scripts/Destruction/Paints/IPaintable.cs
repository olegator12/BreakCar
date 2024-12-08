using UnityEngine;

namespace Destruction
{
    public interface IPaintable
    {
        public WheelSize WheelSize { get; }

        public void ChangeWheels(Wheel template, float size);

        public void PaintWindows(Material material);

        public void PaintCar(Material material);

        public void ReturnWheels();

        public void ReturnWindows();

        public void ReturnPaint();
    }
}