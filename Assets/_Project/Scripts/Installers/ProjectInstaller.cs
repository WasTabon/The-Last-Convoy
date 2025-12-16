using Zenject;
using LastConvoy.Services.Input;
using LastConvoy.StateMachine;
using LastConvoy.StateMachine.States;

namespace LastConvoy.Installers
{
    public class ProjectInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            BindServices();
            BindStateMachine();
        }

        private void BindServices()
        {
            Container.BindInterfacesAndSelfTo<InputService>().AsSingle();
        }

        private void BindStateMachine()
        {
            Container.BindInterfacesAndSelfTo<GameStateMachine>().AsSingle();

            Container.Bind<GameplayState>().AsSingle();
            Container.Bind<PauseState>().AsSingle();
        }
    }
}
