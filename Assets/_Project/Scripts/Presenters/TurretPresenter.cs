using UnityEngine;
using Zenject;
using LastConvoy.Services.Input;
using LastConvoy.StateMachine;
using LastConvoy.StateMachine.States;

public class TurretPresenter : IInitializable, ITickable
{
    private readonly TurretModel _model;
    private readonly IInputService _inputService;
    private readonly GameStateMachine _stateMachine;

    public TurretPresenter(
        TurretModel model,
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
        float mouseX = _inputService.MouseX;
        float mouseY = _inputService.MouseY;

        _model.UpdateInput(mouseX, mouseY, deltaTime);
        _model.UpdateRotation(deltaTime);
    }
}
