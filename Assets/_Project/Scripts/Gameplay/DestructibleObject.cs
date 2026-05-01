using UnityEngine;

public class DestructibleObject : MonoBehaviour, IDamageable
{
    [SerializeField] private DestructibleObjectConfig _config;

    private float _currentHealth;
    private bool _isDestroyed;

    private void Start()
    {
        if (_config == null)
        {
            Debug.LogError($"[DestructibleObject] Config is not assigned on {gameObject.name}!");
            return;
        }

        _currentHealth = _config.MaxHealth;
    }

    public void TakeDamage(float damage)
    {
        if (_isDestroyed) return;

        if (_config == null)
        {
            Debug.LogError($"[DestructibleObject] Config is not assigned on {gameObject.name}!");
            return;
        }

        _currentHealth -= damage;

        if (_currentHealth <= 0)
        {
            Explode();
        }
    }

    private void Explode()
    {
        _isDestroyed = true;

        Vector3 position = transform.position;

        if (_config.ExplosionPrefab != null)
        {
            Instantiate(_config.ExplosionPrefab, position, Quaternion.identity);
        }
        else
        {
            Debug.LogError($"[DestructibleObject] Explosion Prefab is not assigned in config!");
        }

        PlayExplosionSound(position);

        Destroy(gameObject);
    }

    private void PlayExplosionSound(Vector3 position)
    {
        if (_config.ExplosionClip == null)
        {
            Debug.LogError($"[DestructibleObject] Explosion Clip is not assigned in config!");
            return;
        }

        GameObject audioObject = new GameObject("DestructibleExplosionSound");
        audioObject.transform.position = position;

        AudioSource source = audioObject.AddComponent<AudioSource>();
        source.clip = _config.ExplosionClip;
        source.volume = _config.ExplosionVolume;
        source.spatialBlend = _config.AudioSpatialBlend;
        source.minDistance = _config.AudioMinDistance;
        source.maxDistance = _config.AudioMaxDistance;
        source.rolloffMode = AudioRolloffMode.Linear;
        source.priority = 32;
        source.Play();

        Destroy(audioObject, _config.ExplosionClip.length + 0.5f);
    }
}
