namespace LastConvoy.StateMachine
{
    public interface IGameState
    {
        void Enter();
        void Exit();
        void Update();
    }
}
