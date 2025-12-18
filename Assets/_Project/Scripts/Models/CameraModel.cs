using System;
using UnityEngine;
using LastConvoy.Configs;

namespace LastConvoy.Models
{
    public class CameraModel
    {
        public event Action<Quaternion> OnRotationChanged;
        public event Action<Vector3> OnShakeOffsetChanged;

        public float XRotation { get; private set; }
        public float YRotation { get; private set; }
        public Quaternion Rotation { get; private set; }
        public Vector3 ShakeOffset { get; private set; }

        private readonly CameraConfig _config;
        private float _shakePhase;

        public CameraModel(CameraConfig config)
        {
            _config = config;
        }

        public void Initialize(float startXRotation, float startYRotation)
        {
            XRotation = startXRotation;
            YRotation = startYRotation;
            UpdateRotation();
        }

        public void UpdateLook(float mouseX, float mouseY, float deltaTime)
        {
            float scaledMouseX = mouseX * _config.MouseSensitivity * deltaTime;
            float scaledMouseY = mouseY * _config.MouseSensitivity * deltaTime;

            XRotation -= scaledMouseY;
            XRotation = Mathf.Clamp(XRotation, _config.MinVerticalAngle, _config.MaxVerticalAngle);

            YRotation += scaledMouseX;
            YRotation = Mathf.Clamp(YRotation, _config.MinHorizontalAngle, _config.MaxHorizontalAngle);

            UpdateRotation();
        }

        public void UpdateShake(bool isFiring, float shakeIntensity, float shakeFrequency, float deltaTime)
        {
            if (isFiring)
            {
                _shakePhase += deltaTime * shakeFrequency;

                float offsetX = (Mathf.PerlinNoise(_shakePhase, 0f) * 2f - 1f) * shakeIntensity;
                float offsetY = (Mathf.PerlinNoise(0f, _shakePhase) * 2f - 1f) * shakeIntensity;

                ShakeOffset = new Vector3(offsetX, offsetY, 0f);
            }
            else
            {
                ShakeOffset = Vector3.Lerp(ShakeOffset, Vector3.zero, deltaTime * 15f);
            }

            OnShakeOffsetChanged?.Invoke(ShakeOffset);
        }

        private void UpdateRotation()
        {
            Rotation = Quaternion.Euler(XRotation, YRotation, 0f);
            OnRotationChanged?.Invoke(Rotation);
        }
    }
}
