using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class Rotator : MonoBehaviour
{
    [SerializeField] private float _rotationSpeed = 16f;

    private CancellationToken _token;
    private Transform _transform;

    private void Awake()
    {
        _transform = transform;
        _token = destroyCancellationToken;
        Rotate().Forget();
    }

    private async UniTaskVoid Rotate()
    {
        while (_token.IsCancellationRequested == false)
        {
            _transform.Rotate(Vector3.up, _rotationSpeed * Time.deltaTime);
            await UniTask.NextFrame(cancellationToken: _token);
        }
    }
}