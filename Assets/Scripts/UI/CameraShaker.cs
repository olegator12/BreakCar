using DG.Tweening;
using UnityEngine;

namespace UI
{
    public class CameraShaker : ICameraShaker
    {
        private readonly Camera _main;
        private readonly Transform _cameraPoint;
        private readonly float _duration;
        private readonly float _strength;

        public CameraShaker(Camera main, Transform cameraPoint, float duration, float strength)
        {
            _main = main;
            _cameraPoint = cameraPoint;
            _duration = duration;
            _strength = strength;
        }

        public void Shake(float strengthModifier)
        {
            _cameraPoint.DOShakePosition(_duration, _strength * strengthModifier);
        }

        public void SetFOV(float value)
        {
            _main.fieldOfView = value;
        }
    }
}