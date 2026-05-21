using UnityEngine;
using Zenject;
using LastConvoy.Services.Input;
using LastConvoy.StateMachine;
using LastConvoy.StateMachine.States;

public class PlayerCarPresenter : IInitializable, ITickable
{
    private readonly PlayerCarModel _model;
    private readonly IInputService _inputService;
    private readonly GameStateMachine _stateMachine;

    public PlayerCarPresenter(
        PlayerCarModel model,
        IInputService inputService,
        GameStateMachine stateMachine)
    {
        _model = model;
        _inputService = inputService;
        _stateMachine = stateMachine;
    }

    public void Initialize()
    {
        _model.Initialize();
    }

    public void Tick()
    {
        if (!_stateMachine.IsInState<GameplayState>()) return;

        float deltaTime = Time.deltaTime;
        float vertical = _inputService.Vertical;
        float horizontal = _inputService.Horizontal;

        _model.Update(vertical, horizontal, deltaTime);
    }
}
