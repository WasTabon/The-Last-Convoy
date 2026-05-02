using UnityEngine;
using Zenject;

public class PlayerCarView : MonoBehaviour
{
    private PlayerCarModel _model;

    [Inject]
    public void Construct(PlayerCarModel model)
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
