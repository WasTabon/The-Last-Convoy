using System;
using LastConvoy.Configs;

namespace LastConvoy.Models
{
    public class WeaponModel
    {
        public event Action OnFiringStarted;
        public event Action OnFiringStopped;
        public event Action<float> OnSpinProgressChanged;
        public event Action OnFired;

        public float SpinProgress { get; private set; }
        public bool IsFiring { get; private set; }
        public bool IsSpinning { get; private set; }
        public float CurrentBarrelSpeed { get; private set; }

        private readonly WeaponConfig _config;

        public WeaponModel(WeaponConfig config)
        {
            _config = config;
        }

        public void StartSpinning()
        {
            IsSpinning = true;
        }

        public void StopSpinning()
        {
            IsSpinning = false;
            if (IsFiring)
            {
                IsFiring = false;
                OnFiringStopped?.Invoke();
            }
        }

        public void UpdateSpin(float deltaTime)
        {
            float previousProgress = SpinProgress;

            if (IsSpinning)
            {
                SpinProgress = Math.Min(1f, SpinProgress + deltaTime / _config.SpinUpTime);

                if (SpinProgress >= 1f && !IsFiring)
                {
                    IsFiring = true;
                    OnFiringStarted?.Invoke();
                }
            }
            else
            {
                SpinProgress = Math.Max(0f, SpinProgress - deltaTime / (_config.SpinUpTime * 0.5f));
            }

            if (Math.Abs(previousProgress - SpinProgress) > 0.001f)
            {
                OnSpinProgressChanged?.Invoke(SpinProgress);
            }
        }

        public void UpdateBarrelSpeed(float deltaTime)
        {
            float targetSpeed = SpinProgress * _config.MaxBarrelRotationSpeed;
            CurrentBarrelSpeed = CurrentBarrelSpeed + (targetSpeed - CurrentBarrelSpeed) * deltaTime * 5f;
        }

        public void NotifyFired()
        {
            OnFired?.Invoke();
        }
    }
}
