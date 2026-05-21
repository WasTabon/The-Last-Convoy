using UnityEngine;

public class EnemyCarHitbox : MonoBehaviour, IDamageable
{
    [SerializeField] private EnemyCarController _controller;

    private void Awake()
    {
        if (_controller == null)
        {
            _controller = GetComponentInParent<EnemyCarController>();
        }

        if (_controller == null)
        {
            Debug.LogError("[EnemyCarHitbox] EnemyCarController not found in parent!");
        }
    }

    public void TakeDamage(float damage)
    {
        if (_controller != null)
        {
            _controller.TakeDamage(damage);
        }
    }
}
