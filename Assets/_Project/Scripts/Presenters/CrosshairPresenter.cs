using UnityEngine;
using Zenject;
using LastConvoy.Models;
using LastConvoy.StateMachine;
using LastConvoy.StateMachine.States;

namespace LastConvoy.Presenters
{
    public class CrosshairPresenter : ITickable
    {
        private readonly CrosshairModel _model;
        private readonly WeaponModel _weaponModel;
        private readonly GameStateMachine _stateMachine;

        public CrosshairPresenter(
            CrosshairModel model,
            WeaponModel weaponModel,
            GameStateMachine stateMachine)
        {
            _model = model;
            _weaponModel = weaponModel;
            _stateMachine = stateMachine;
        }

        public void Tick()
        {
            if (!_stateMachine.IsInState<GameplayState>()) return;

            _model.UpdateSpread(_weaponModel.IsFiring, Time.deltaTime);
        }
    }
}
