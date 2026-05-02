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
    private readonly Transform _carTransform;

    public PlayerCarPresenter(
        PlayerCarModel model,
        IInputService inputService,
        GameStateMachine stateMachine,
        [Inject(Id = "PlayerCarTransform")] Transform carTransform)
    {
        _model = model;
        _inputService = inputService;
        _stateMachine = stateMachine;
        _carTransform = carTransform;
    }

    public void Initialize()
    {
        if (_carTransform == null)
        {
            Debug.LogError("[PlayerCarPresenter] Car Transform is not assigned!");
            return;
        }

        _model.Initialize(_carTransform.position, _carTransform.rotation);
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
