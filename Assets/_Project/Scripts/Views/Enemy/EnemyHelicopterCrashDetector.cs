using UnityEngine;
using Zenject;

public class EnemyHelicopterCrashDetector : MonoBehaviour
{
    [SerializeField] private float _groundCheckDistance = 2f;
    [SerializeField] private LayerMask _groundLayers = -1;

    private EnemyHelicopterModel _model;
    private EnemyHelicopterConfig _config;
    private EnemyHelicopterAudioView _audioView;

    private bool _hasExploded;

    [Inject]
    public void Construct(EnemyHelicopterModel model, EnemyHelicopterConfig config)
    {
        _model = model;
        _config = config;
    }

    private void Start()
    {
        _audioView = GetComponentInParent<EnemyHelicopterAudioView>();
        if (_audioView == null)
        {
            _audioView = GetComponent<EnemyHelicopterAudioView>();
        }

        _model.OnCrashStarted += HandleCrashStarted;
    }

    private void OnDestroy()
    {
        if (_model != null)
        {
            _model.OnCrashStarted -= HandleCrashStarted;
        }
    }

    private void Update()
    {
        if (_model.State != EnemyHelicopterState.Crashing) return;
        if (_hasExploded) return;

        CheckGroundCollision();
    }

    private void HandleCrashStarted()
    {
        if (_audioView != null)
        {
            _audioView.FadeOutAndStop(_config.CrashAudioFadeDuration);
        }
    }

    private void CheckGroundCollision()
    {
        Ray ray = new Ray(transform.position, Vector3.down);

        if (Physics.Raycast(ray, out RaycastHit hit, _groundCheckDistance, _groundLayers))
        {
            if (IsOwnCollider(hit.collider)) return;

            Explode(hit.point);
        }
    }

    private bool IsOwnCollider(Collider other)
    {
        Transform current = other.transform;
        Transform root = transform.root;

        while (current != null)
        {
            if (current == root) return true;
            current = current.parent;
        }

        return false;
    }

    private void Explode(Vector3 position)
    {
        _hasExploded = true;

        _model.Explode();

        if (_config.ExplosionPrefab != null)
        {
            Instantiate(_config.ExplosionPrefab, position, Quaternion.identity);
        }
        else
        {
            Debug.LogError("[EnemyHelicopterCrashDetector] Explosion Prefab is not assigned in config!");
        }

        PlayExplosionSound(position);

        Destroy(transform.root.gameObject);
    }

    private void PlayExplosionSound(Vector3 position)
    {
        if (_config.ExplosionClip == null)
        {
            Debug.LogError("[EnemyHelicopterCrashDetector] Explosion Clip is not assigned in config!");
            return;
        }

        GameObject audioObject = new GameObject("ExplosionSound");
        audioObject.transform.position = position;

        AudioSource source = audioObject.AddComponent<AudioSource>();
        source.clip = _config.ExplosionClip;
        source.volume = _config.ExplosionVolume;
        source.spatialBlend = 0f;
        source.priority = 0;
        source.Play();

        Destroy(audioObject, _config.ExplosionClip.length + 0.5f);
    }
}
