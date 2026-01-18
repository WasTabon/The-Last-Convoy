using UnityEngine;
using LastConvoy.Services.Input;

namespace LastConvoy.StateMachine.States
{
    public class PauseState : IGameState
    {
        private readonly IInputService _inputService;
        private readonly GameStateMachine _stateMachine;

        public PauseState(IInputService inputService, GameStateMachine stateMachine)
        {
            _inputService = inputService;
            _stateMachine = stateMachine;
        }

        public void Enter()
        {
            Time.timeScale = 0f;
            AudioListener.pause = true;
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;

            _inputService.OnPausePressed += HandleResume;
        }

        public void Exit()
        {
            _inputService.OnPausePressed -= HandleResume;
        }

        public void Update()
        {
        }

        private void HandleResume()
        {
            _stateMachine.ChangeState<GameplayState>();
        }
    }
}
