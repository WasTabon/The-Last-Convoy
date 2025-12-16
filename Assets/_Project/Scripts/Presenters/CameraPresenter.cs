using UnityEngine;
using Zenject;
using LastConvoy.Models;
using LastConvoy.Configs;
using LastConvoy.Services.Input;
using LastConvoy.StateMachine;
using LastConvoy.StateMachine.States;

namespace LastConvoy.Presenters
{
    public class CameraPresenter : IInitializable, ITickable
    {
        private readonly CameraModel _model;
        private readonly WeaponModel _weaponModel;
        private readonly WeaponConfig _weaponConfig;
        private readonly IInputService _inputService;
        private readonly GameStateMachine _stateMachine;

        public CameraPresenter(
            CameraModel model,
            WeaponModel weaponModel,
            WeaponConfig weaponConfig,
            IInputService inputService,
            GameStateMachine stateMachine)
        {
            _model = model;
            _weaponModel = weaponModel;
            _weaponConfig = weaponConfig;
            _inputService = inputService;
            _stateMachine = stateMachine;
        }

        public void Initialize()
        {
            _model.Initialize(0f, 0f);
        }

        public void Tick()
        {
            if (!_stateMachine.IsInState<GameplayState>()) return;

            float deltaTime = Time.deltaTime;

            _model.UpdateLook(_inputService.MouseX, _inputService.MouseY, deltaTime);
            _model.UpdateShake(
                _weaponModel.IsFiring,
                _weaponConfig.ShakeIntensity,
                _weaponConfig.ShakeFrequency,
                deltaTime
            );
        }
    }
}
