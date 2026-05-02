using UnityEngine;
using Zenject;

public class TurretView : MonoBehaviour
{
    [SerializeField] private Transform _horizontalPivot;
    [SerializeField] private Transform _verticalPivot;

    private TurretModel _model;

    [Inject]
    public void Construct(TurretModel model)
    {
        _model = model;
    }

    private void Start()
    {
        if (_horizontalPivot == null)
        {
            Debug.LogError("[TurretView] Horizontal Pivot is not assigned!");
        }

        if (_verticalPivot == null)
        {
            Debug.LogError("[TurretView] Vertical Pivot is not assigned!");
        }
    }

    private void OnEnable()
    {
        _model.OnAnglesChanged += HandleAnglesChanged;
    }

    private void OnDisable()
    {
        _model.OnAnglesChanged -= HandleAnglesChanged;
    }

    private void HandleAnglesChanged(float horizontalAngle, float verticalAngle)
    {
        if (_horizontalPivot != null)
        {
            _horizontalPivot.localRotation = Quaternion.Euler(0f, horizontalAngle, 0f);
        }

        if (_verticalPivot != null)
        {
            _verticalPivot.localRotation = Quaternion.Euler(verticalAngle, 0f, 0f);
        }
    }
}
