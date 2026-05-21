using System;
using UnityEngine;
using Zenject;
using LastConvoy.Services.Input;
using LastConvoy.StateMachine;
using LastConvoy.StateMachine.States;

public class TurretWeaponPresenter : IInitializable, ITickable, IDisposable
{
    public event Action<Vector3, Vector3> OnImpact;

    public Transform AimPoint { get; set; }

    private readonly TurretWeaponModel _model;
    private readonly TurretWeaponConfig _config;
    private readonly IInputService _inputService;
    private readonly GameStateMachine _stateMachine;

    public TurretWeaponPresenter(
        TurretWeaponModel model,
        TurretWeaponConfig config,
        IInputService inputService,
        GameStateMachine stateMachine)
    {
        _model = model;
        _config = config;
        _inputService = inputService;
        _stateMachine = stateMachine;
    }

    public void Initialize()
    {
        _inputService.OnFirePressed += HandleFirePressed;
        _inputService.OnFireReleased += HandleFireReleased;
    }

    public void Dispose()
    {
        _inputService.OnFirePressed -= HandleFirePressed;
        _inputService.OnFireReleased -= HandleFireReleased;
    }

    public void Tick()
    {
        if (!_stateMachine.IsInState<GameplayState>()) return;

        if (_model.TryFire(Time.time))
        {
            PerformRaycast();
        }
    }

    private void HandleFirePressed()
    {
        if (_stateMachine.IsInState<GameplayState>())
        {
            _model.StartFiring();
        }
    }

    private void HandleFireReleased()
    {
        _model.StopFiring();
    }

    private void PerformRaycast()
    {
        if (AimPoint == null)
        {
            Debug.LogError("[TurretWeaponPresenter] AimPoint is not set!");
            return;
        }

        Ray ray = new Ray(AimPoint.position, AimPoint.forward);

        if (Physics.Raycast(ray, out RaycastHit hit, _config.RaycastRange, _config.HitLayers))
        {
            OnImpact?.Invoke(hit.point, hit.normal);

            IDamageable damageable = hit.collider.GetComponent<IDamageable>();
            if (damageable != null)
            {
                damageable.TakeDamage(_config.DamagePerShot);
            }
        }
    }
}
