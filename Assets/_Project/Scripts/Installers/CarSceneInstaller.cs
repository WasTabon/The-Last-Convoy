using UnityEngine;
using Zenject;
using LastConvoy.StateMachine;
using LastConvoy.StateMachine.States;

public class CarSceneInstaller : MonoInstaller
{
    public override void InstallBindings()
    {
    }

    public override void Start()
    {
        base.Start();
        InitializeStateMachine();
    }

    private void InitializeStateMachine()
    {
        var stateMachine = Container.Resolve<GameStateMachine>();
        var gameplayState = Container.Resolve<GameplayState>();
        var pauseState = Container.Resolve<PauseState>();

        stateMachine.RegisterState(gameplayState);
        stateMachine.RegisterState(pauseState);

        stateMachine.ChangeState<GameplayState>();
    }
}
