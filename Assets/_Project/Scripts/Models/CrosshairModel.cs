using System;
using UnityEngine;
using LastConvoy.Configs;

namespace LastConvoy.Models
{
    public class CrosshairModel
    {
        public event Action<float> OnSpreadChanged;

        public float CurrentSpread { get; private set; }

        private readonly CrosshairConfig _config;

        public CrosshairModel(CrosshairConfig config)
        {
            _config = config;
        }

        public void UpdateSpread(bool isFiring, float deltaTime)
        {
            float targetSpread = isFiring ? _config.MaxSpread : 0f;
            float previousSpread = CurrentSpread;

            CurrentSpread = Mathf.Lerp(CurrentSpread, targetSpread, deltaTime * _config.SpreadSpeed);

            if (Mathf.Abs(previousSpread - CurrentSpread) > 0.01f)
            {
                OnSpreadChanged?.Invoke(CurrentSpread);
            }
        }
    }
}
