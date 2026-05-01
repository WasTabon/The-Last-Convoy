using UnityEngine;
using Zenject;

public class EnemyHelicopterHitbox : MonoBehaviour, IDamageable
{
    private EnemyHelicopterModel _model;

    [Inject]
    public void Construct(EnemyHelicopterModel model)
    {
        _model = model;
    }

    public void TakeDamage(float damage)
    {
        if (_model == null)
        {
            Debug.LogError("[EnemyHelicopterHitbox] Model is null! Check Zenject injection.");
            return;
        }

       Debug.Log("Take damage");
        
        _model.TakeDamage(damage);
    }
}
