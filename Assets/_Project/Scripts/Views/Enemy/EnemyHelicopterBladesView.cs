using UnityEngine;
using Zenject;

public class EnemyHelicopterBladesView : MonoBehaviour
{
    private EnemyHelicopterConfig _config;
    private EnemyHelicopterModel _model;

    private float _currentRotationSpeed;

    [Inject]
    public void Construct(EnemyHelicopterConfig config, EnemyHelicopterModel model)
    {
        _config = config;
        _model = model;
    }

    private void Start()
    {
        _currentRotationSpeed = _config.BladeRotationSpeed;
    }

    private void Update()
    {
        if (_model.State == EnemyHelicopterState.Crashing)
        {
            _currentRotationSpeed = Mathf.Lerp(_currentRotationSpeed, _config.BladeRotationSpeed * 0.3f, Time.deltaTime * 0.5f);
        }

        transform.Rotate(0f, _currentRotationSpeed * Time.deltaTime, 0f);
    }
}
