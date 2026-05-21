using UnityEngine;
using Zenject;

public class PlayerCarCameraView : MonoBehaviour
{
    private PlayerCarModel _model;
    private PlayerCarCameraConfig _config;
    private bool _isInjected;

    private Vector3 _baseLocalPosition;
    private float _shakePhase;
    private float _currentTilt;

    [Inject]
    public void Construct(PlayerCarModel model, PlayerCarCameraConfig config)
    {
        _model = model;
        _config = config;
        _isInjected = true;
    }

    private void Start()
    {
        if (!_isInjected) return;

        _baseLocalPosition = _config.CameraOffset;
        transform.localPosition = _baseLocalPosition;
        transform.localRotation = Quaternion.identity;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void LateUpdate()
    {
        if (!_isInjected) return;

        UpdateShake();
        UpdateTilt();
    }

    private void UpdateShake()
    {
        float speedRatio = _model.SpeedRatio;

        if (speedRatio < 0.1f)
        {
            transform.localPosition = Vector3.Lerp(transform.localPosition, _baseLocalPosition, Time.deltaTime * 10f);
            return;
        }

        _shakePhase += Time.deltaTime * _config.ShakeFrequency;

        float shakeAmount = Mathf.Min(speedRatio * _config.ShakeAmountBySpeed, _config.MaxShakeAmount);

        float shakeX = (Mathf.PerlinNoise(_shakePhase, 0f) * 2f - 1f) * shakeAmount;
        float shakeY = (Mathf.PerlinNoise(0f, _shakePhase) * 2f - 1f) * shakeAmount;

        Vector3 shakeOffset = new Vector3(shakeX, shakeY, 0f);
        transform.localPosition = _baseLocalPosition + shakeOffset;
    }

    private void UpdateTilt()
    {
        float targetTilt = 0f;

        if (!_model.IsStationary)
        {
            float horizontalInput = Input.GetAxis("Horizontal");
            targetTilt = -horizontalInput * _config.TiltAmountByTurn;
        }

        _currentTilt = Mathf.Lerp(_currentTilt, targetTilt, Time.deltaTime * _config.TiltSmoothing);

        Vector3 currentEuler = transform.localEulerAngles;
        transform.localRotation = Quaternion.Euler(currentEuler.x, currentEuler.y, _currentTilt);
    }
}
