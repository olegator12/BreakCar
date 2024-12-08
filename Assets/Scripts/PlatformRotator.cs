using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class PlatformRotator
{
    private const float RoundDegrees = 359f;
    private const float RoundOffSet = 330f;
    private const float MinValue = 0f;

    private readonly float _rotationSpeed;
    private readonly Transform _transform;
    private readonly Vector3 _startEulerAngles;

    private CancellationTokenSource _cancellation;
    private Quaternion _targetRotation;
    private Vector3 _targetAxis;

    public PlatformRotator(Transform platform, float rotationSpeed)
    {
        _transform = platform;
        _rotationSpeed = rotationSpeed;
        _targetAxis = new Vector3(MinValue, RoundDegrees, MinValue);
        _targetRotation = Quaternion.Euler(_targetAxis);
        _startEulerAngles = _transform.eulerAngles;
    }

    public void StartRotation()
    {
        if (_cancellation != null)
            return;

        _cancellation = new CancellationTokenSource();
        Rotate().Forget();
    }

    public void StopRotation()
    {
        if (_transform != null)
            _transform.eulerAngles = _startEulerAngles;

        if (_cancellation == null)
            return;

        if (_cancellation.IsCancellationRequested == false)
            _cancellation.Cancel();

        _cancellation.Dispose();
        _cancellation = null;
    }

    private async UniTaskVoid Rotate()
    {
        while (_cancellation.IsCancellationRequested == false)
        {
            _transform.Rotate(Vector3.up, _rotationSpeed * Time.deltaTime);

            if (_transform.eulerAngles.y >= RoundOffSet)
            {
                _targetAxis.y += RoundDegrees;
                _targetRotation = Quaternion.Euler(_targetAxis);
            }

            await UniTask.NextFrame(PlayerLoopTiming.Update, _cancellation.Token);
        }
    }
}