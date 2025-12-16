using UnityEngine;
using Zenject;
using LastConvoy.Models;

namespace LastConvoy.Views.Helicopter
{
    public class HelicopterView : MonoBehaviour
    {
        private HelicopterModel _model;

        [Inject]
        public void Construct(HelicopterModel model)
        {
            _model = model;
        }

        private void OnEnable()
        {
            _model.OnPositionChanged += HandlePositionChanged;
            _model.OnRotationChanged += HandleRotationChanged;
        }

        private void OnDisable()
        {
            _model.OnPositionChanged -= HandlePositionChanged;
            _model.OnRotationChanged -= HandleRotationChanged;
        }

        private void HandlePositionChanged(Vector3 position)
        {
            transform.position = position;
        }

        private void HandleRotationChanged(Quaternion rotation)
        {
            transform.rotation = rotation;
        }
    }
}
