using System;
using UnityEngine;
using Zenject;

namespace LastConvoy.Services.Input
{
    public class InputService : IInputService, ITickable
    {
        public event Action OnFirePressed;
        public event Action OnFireReleased;
        public event Action OnPausePressed;

        public float MouseX => UnityEngine.Input.GetAxis("Mouse X");
        public float MouseY => UnityEngine.Input.GetAxis("Mouse Y");
        public bool IsFireHeld => UnityEngine.Input.GetMouseButton(0);

        public void Tick()
        {
            if (UnityEngine.Input.GetMouseButtonDown(0))
            {
                OnFirePressed?.Invoke();
            }

            if (UnityEngine.Input.GetMouseButtonUp(0))
            {
                OnFireReleased?.Invoke();
            }

            if (UnityEngine.Input.GetKeyDown(KeyCode.Escape))
            {
                OnPausePressed?.Invoke();
            }
        }
    }
}
