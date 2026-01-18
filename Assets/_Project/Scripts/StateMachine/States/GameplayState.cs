using UnityEngine;
using LastConvoy.Services.Input;

namespace LastConvoy.StateMachine.States
{
    public class GameplayState : IGameState
    {
        private readonly IInputService _inputService;
        private readonly GameStateMachine _stateMachine;

        public GameplayState(IInputService inputService, GameStateMachine stateMachine)
        {
            _inputService = inputService;
            _stateMachine = stateMachine;
        }

        public void Enter()
        {
            Time.timeScale = 1f;
            AudioListener.pause = false;
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;

            _inputService.OnPausePressed += HandlePause;
        }

        public void Exit()
        {
            _inputService.OnPausePressed -= HandlePause;
        }

        public void Update()
        {
        }

        private void HandlePause()
        {
            _stateMachine.ChangeState<PauseState>();
        }
    }
}
