using UnityEngine;
using Zenject;
using LastConvoy.Models;

namespace LastConvoy.Views.Camera
{
    public class CameraView : MonoBehaviour
    {
        private CameraModel _model;
        private Vector3 _originalLocalPosition;

        [Inject]
        public void Construct(CameraModel model)
        {
            _model = model;
        }

        private void Awake()
        {
            _originalLocalPosition = transform.localPosition;
        }

        private void OnEnable()
        {
            _model.OnRotationChanged += HandleRotationChanged;
            _model.OnShakeOffsetChanged += HandleShakeOffsetChanged;
        }

        private void OnDisable()
        {
            _model.OnRotationChanged -= HandleRotationChanged;
            _model.OnShakeOffsetChanged -= HandleShakeOffsetChanged;
        }

        private void HandleRotationChanged(Quaternion rotation)
        {
            transform.localRotation = rotation;
        }

        private void HandleShakeOffsetChanged(Vector3 shakeOffset)
        {
            transform.localPosition = _originalLocalPosition + shakeOffset;
        }
    }
}
