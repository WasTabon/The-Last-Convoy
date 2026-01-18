using UnityEngine;
using Zenject;

public class EnemyHelicopterBladesView : MonoBehaviour
{
    private EnemyHelicopterConfig _config;

    [Inject]
    public void Construct(EnemyHelicopterConfig config)
    {
        _config = config;
    }

    private void Update()
    {
        transform.Rotate(0f, _config.BladeRotationSpeed * Time.deltaTime, 0f);
    }
}
