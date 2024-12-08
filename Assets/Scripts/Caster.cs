using UnityEngine;

public class Caster
{
    private const float MaxDistance = 120;
    private const float Radius = 0.4f;

    private readonly Camera _camera;
    private readonly Transform _transform;
    private readonly LayerMask _general;
    private readonly LayerMask _trajectoryMask;

    public Caster(Camera camera, LayerMask general, LayerMask trajectoryMask)
    {
        _camera = camera;
        _transform = camera.transform;
        _general = general;
        _trajectoryMask = trajectoryMask;
    }

    public bool TryDemolish(Vector2 screenPosition, out RaycastHit hit, bool isTrajectory)
    {
        return Physics.SphereCast(
            _camera.ScreenPointToRay(screenPosition),
            Radius,
            out hit,
            MaxDistance,
            isTrajectory == false ? _general : _trajectoryMask);
    }

    public Vector3 GetCastDirection(Vector3 contact)
    {
        return contact - _transform.position;
    }
}