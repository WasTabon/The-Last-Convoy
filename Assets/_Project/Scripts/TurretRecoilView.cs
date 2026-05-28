using UnityEngine;
using Zenject;

public class TurretRecoilView : MonoBehaviour
{
    [Header("Recoil Settings")]
    [SerializeField] private float _recoilDistance = 0.2f;
    [SerializeField] private float _returnSpeed = 15f;

    private TurretWeaponModel _model;
    private Vector3 _originalLocalPosition;
    private float _currentRecoil;
    private bool _isInjected;

    [Inject]
    public void Construct(TurretWeaponModel model)
    {
        _model = model;
        _isInjected = true;
    }

    private void Awake()
    {
        _originalLocalPosition = transform.localPosition;
    }

    private void Start()
    {
        if (!_isInjected) return;
        _model.OnFired += HandleFired;
    }

    private void OnDestroy()
    {
        if (!_isInjected) return;
        if (_model != null)
        {
            _model.OnFired -= HandleFired;
        }
    }

    private void Update()
    {
        if (!_isInjected) return;
        UpdateRecoil();
    }

    private void HandleFired()
    {
        _currentRecoil = -_recoilDistance;
    }

    private void UpdateRecoil()
    {
        if (Mathf.Abs(_currentRecoil) < 0.001f)
        {
            _currentRecoil = 0f;
            transform.localPosition = _originalLocalPosition;
            return;
        }

        _currentRecoil = Mathf.Lerp(_currentRecoil, 0f, Time.deltaTime * _returnSpeed);

        transform.localPosition = _originalLocalPosition + new Vector3(0f, 0f, _currentRecoil);
    }
}
