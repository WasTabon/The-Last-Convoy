using UnityEngine;
using Zenject;
using LastConvoy.Models;

public class SteeringWheelView : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float _maxRotationAngle = 90f;
    [SerializeField] private float _rotationSpeed = 10f;

    private PlayerCarModel _model;
    private float _currentRotation;
    private bool _isInjected;

    [Inject]
    public void Construct(PlayerCarModel model)
    {
        _model = model;
        _isInjected = true;
    }

    private void Update()
    {
        if (!_isInjected) return;
        UpdateRotation();
    }

    private void UpdateRotation()
    {
        float targetRotation = -_model.CurrentTurnInput * _maxRotationAngle;

        _currentRotation = Mathf.Lerp(_currentRotation, targetRotation, Time.deltaTime * _rotationSpeed);

        transform.localRotation = Quaternion.Euler(0f, 0f, _currentRotation);
    }
}
