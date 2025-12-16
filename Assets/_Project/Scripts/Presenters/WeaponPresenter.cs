using System;
using UnityEngine;
using Zenject;
using LastConvoy.Models;
using LastConvoy.Configs;
using LastConvoy.Services.Input;
using LastConvoy.StateMachine;
using LastConvoy.StateMachine.States;

namespace LastConvoy.Presenters
{
    public class WeaponPresenter : IInitializable, ITickable, IDisposable
    {
        public event Action<Vector3, Vector3> OnImpact;
        public event Action<Vector3> OnRecoilChanged;

        private readonly WeaponModel _model;
        private readonly WeaponConfig _config;
        private readonly IInputService _inputService;
        private readonly GameStateMachine _stateMachine;
        private readonly Camera _camera;

        private float _nextFireTime;
        private Vector3 _recoilRotation;
        private float _recoilPhase;

        public WeaponPresenter(
            WeaponModel model,
            WeaponConfig config,
            IInputService inputService,
            GameStateMachine stateMachine,
            Camera camera)
        {
            _model = model;
            _config = config;
            _inputService = inputService;
            _stateMachine = stateMachine;
            _camera = camera;
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

            float deltaTime = Time.deltaTime;

            _model.UpdateSpin(deltaTime);
            _model.UpdateBarrelSpeed(deltaTime);
            UpdateRecoil(deltaTime);

            if (_model.IsFiring && Time.time >= _nextFireTime)
            {
                Fire();
                _nextFireTime = Time.time + _config.FireRate;
            }
        }

        private void HandleFirePressed()
        {
            if (_stateMachine.IsInState<GameplayState>())
            {
                _model.StartSpinning();
            }
        }

        private void HandleFireReleased()
        {
            _model.StopSpinning();
        }

        private void Fire()
        {
            _model.NotifyFired();
            PerformRaycast();
        }

        private void PerformRaycast()
        {
            if (_camera == null) return;

            Ray ray = _camera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));

            if (Physics.Raycast(ray, out RaycastHit hit, _config.RaycastRange, _config.HitLayers))
            {
                OnImpact?.Invoke(hit.point, hit.normal);
            }
        }

        private void UpdateRecoil(float deltaTime)
        {
            if (_model.IsFiring)
            {
                _recoilPhase += deltaTime * _config.RecoilFrequency;

                float rotX = Mathf.Sin(_recoilPhase * 1.5f) * _config.RecoilRotationAmount;
                float rotY = Mathf.Cos(_recoilPhase * 1.2f) * _config.RecoilRotationAmount * 0.8f;

                Vector3 targetRecoil = new Vector3(rotX, rotY, 0f);
                _recoilRotation = Vector3.Lerp(_recoilRotation, targetRecoil, deltaTime * 20f);
            }
            else
            {
                _recoilRotation = Vector3.Lerp(_recoilRotation, Vector3.zero, deltaTime * _config.RecoilRecoverySpeed);
            }

            OnRecoilChanged?.Invoke(_recoilRotation);
        }
    }
}
