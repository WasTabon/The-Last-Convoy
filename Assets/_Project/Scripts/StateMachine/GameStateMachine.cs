using System;
using System.Collections.Generic;
using Zenject;

namespace LastConvoy.StateMachine
{
    public class GameStateMachine : ITickable
    {
        public event Action<Type> OnStateChanged;

        private readonly Dictionary<Type, IGameState> _states = new Dictionary<Type, IGameState>();
        private IGameState _currentState;

        public void RegisterState<T>(T state) where T : IGameState
        {
            _states[typeof(T)] = state;
        }

        public void ChangeState<T>() where T : IGameState
        {
            var type = typeof(T);

            if (!_states.TryGetValue(type, out var newState))
            {
                throw new Exception($"State {type.Name} not registered");
            }

            _currentState?.Exit();
            _currentState = newState;
            _currentState.Enter();

            OnStateChanged?.Invoke(type);
        }

        public void Tick()
        {
            _currentState?.Update();
        }

        public bool IsInState<T>() where T : IGameState
        {
            return _currentState is T;
        }

        public Type GetCurrentStateType()
        {
            return _currentState?.GetType();
        }
    }
}
