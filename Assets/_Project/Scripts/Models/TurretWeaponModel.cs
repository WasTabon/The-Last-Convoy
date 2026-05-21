using System;

public class TurretWeaponModel
{
    public event Action OnFired;
    public event Action OnStartedFiring;
    public event Action OnStoppedFiring;

    public bool IsFiring { get; private set; }

    private readonly TurretWeaponConfig _config;
    private float _lastFireTime;
    private bool _wasFiring;

    public TurretWeaponModel(TurretWeaponConfig config)
    {
        _config = config;
    }

    public void StartFiring()
    {
        IsFiring = true;

        if (!_wasFiring)
        {
            _wasFiring = true;
            OnStartedFiring?.Invoke();
        }
    }

    public void StopFiring()
    {
        IsFiring = false;

        if (_wasFiring)
        {
            _wasFiring = false;
            OnStoppedFiring?.Invoke();
        }
    }

    public bool TryFire(float currentTime)
    {
        if (!IsFiring) return false;

        if (currentTime - _lastFireTime < _config.FireRate) return false;

        _lastFireTime = currentTime;
        OnFired?.Invoke();
        return true;
    }
}
