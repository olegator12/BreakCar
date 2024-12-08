using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Weapons
{
    public abstract class TrajectoryWeapon : RangeWeapon, IRevertible, ICancellable
    {
        private const int MinValue = -1;
        private const int MaxValue = 1;

        [SerializeField] private Transform _shootPoint;
        [SerializeField] private AnimationCurve _trajectory;
        [SerializeField] private int _pointsPerTrajectory = 25;

        private GameObject _gameObject;
        private Projectile _current;

        public abstract bool CanStick { get; }

        public override void OnInitialize()
        {
            _gameObject = gameObject;
        }

        public override void Attack(Vector3 targetPosition, Vector3 normal, Func<bool> onContact)
        {
            if (_current != null)
            {
                onContact.Invoke();
                MoveProjectile(targetPosition, normal);
                return;
            }

            onContact.Invoke();
            CreateProjectile();
            MoveProjectile(targetPosition, normal);
        }

        public override void Prepare()
        {
            CreateProjectile();
            SetFOV();
            _gameObject.SetActive(true);
        }

        public override Vector3 GetSpawnPosition(Vector3 _, Vector3 __)
        {
            return _shootPoint.position;
        }

        public void Revert()
        {
            _gameObject.SetActive(false);
        }

        public void Cancel()
        {
            CreateProjectile();
        }

        public abstract Vector3 CalculateEvaluation(Vector3 position, float evaluation, int variant);

        private void MoveProjectile(Vector3 targetPosition, Vector3 normal)
        {
            _current.SetParent(euler: _shootPoint.localEulerAngles);
            _current.Move(CalculateTrajectory(targetPosition), normal, CanStick, Shake);
            _current = null;
        }

        private void CreateProjectile()
        {
            _current = CreateProjectile(Vector3.zero, Vector3.zero);
            _current.Freeze();
            _current.SetParent(_shootPoint);
        }

        private Vector3[] CalculateTrajectory(Vector3 targetPosition)
        {
            Vector3 startPosition = _shootPoint.position;
            Vector3 direction = targetPosition - startPosition;
            Vector3[] result = new Vector3[_pointsPerTrajectory + 2];
            Vector3 step = direction / _pointsPerTrajectory;
            float segmentCount = _pointsPerTrajectory;
            int variant = Random.Range(MinValue, MaxValue);
            result[0] = startPosition;
            result[^1] = targetPosition;

            for (int i = 0; i < _pointsPerTrajectory; i++)
            {
                Vector3 position = startPosition + step * i;
                position = CalculateEvaluation(position, _trajectory.Evaluate(i / segmentCount), variant);
                result[i + 1] = position;
            }

            return result;
        }
    }
}