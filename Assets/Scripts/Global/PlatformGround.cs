using UnityEngine;

public class PlatformGround : Ground, ISticky
{
    private Transform _transform;

    private void Awake()
    {
        _transform = transform;
    }

    public bool TryStick(Transform projectile)
    {
        projectile.SetParent(_transform);
        return true;
    }
}