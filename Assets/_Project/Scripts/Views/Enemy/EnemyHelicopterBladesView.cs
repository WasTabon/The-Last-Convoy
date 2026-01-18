using UnityEngine;
using Zenject;

public class EnemyHelicopterBladesView : MonoBehaviour
{
    [SerializeField] private bool _isMain = true;
    private EnemyHelicopterConfig _config;

    [Inject]
    public void Construct(EnemyHelicopterConfig config)
    {
        _config = config;
    }

    private void Update()
    {
        if (_isMain)
            transform.Rotate(0f, _config.BladeRotationSpeed * Time.deltaTime, 0f);
        else
            transform.Rotate(_config.BladeRotationSpeed * Time.deltaTime, 0f, 0f);
    }
}
