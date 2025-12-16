using System;

namespace LastConvoy.Services.Input
{
    public interface IInputService
    {
        event Action OnFirePressed;
        event Action OnFireReleased;
        event Action OnPausePressed;

        float MouseX { get; }
        float MouseY { get; }
        bool IsFireHeld { get; }
    }
}
