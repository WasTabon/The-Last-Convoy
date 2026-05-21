using UnityEngine;
using Zenject;
using LastConvoy.Views.Effects;

public class TurretWeaponView : MonoBehaviour
{
    [SerializeField] private ParticleSystem _muzzleFlash;
    [SerializeField] private Transform _muzzlePosition;
    [SerializeField] private Transform _aimPoint;
    [SerializeField] private BulletImpactPool _impactPool;

    private TurretWeaponModel _model;
    private TurretWeaponPresenter _presenter;
    private TurretWeaponConfig _config;
    private AudioSource _fireSource;
    private bool _isInjected;

    [Inject]
    public void Construct(TurretWeaponModel model, TurretWeaponPresenter presenter, TurretWeaponConfig config)
    {
        _model = model;
        _presenter = presenter;
        _config = config;
        _isInjected = true;
    }

    private void Start()
    {
        if (!_isInjected) return;

        SetupAudio();
        SetupAimPoint();

        _model.OnFired += HandleFired;
        _presenter.OnImpact += HandleImpact;
    }

    private void OnDestroy()
    {
        if (!_isInjected) return;

        if (_model != null)
        {
            _model.OnFired -= HandleFired;
        }

        if (_presenter != null)
        {
            _presenter.OnImpact -= HandleImpact;
        }
    }

    private void SetupAimPoint()
    {
        if (_aimPoint != null)
        {
            _presenter.AimPoint = _aimPoint;
        }
        else if (_muzzlePosition != null)
        {
            _presenter.AimPoint = _muzzlePosition;
        }
        else
        {
            Debug.LogError("[TurretWeaponView] AimPoint and MuzzlePosition are not assigned!");
        }
    }

    private void SetupAudio()
    {
        GameObject audioObj = new GameObject("TurretFireAudio");
        
        if (_muzzlePosition != null)
        {
            audioObj.transform.SetParent(_muzzlePosition);
        }
        else
        {
            audioObj.transform.SetParent(transform);
        }
        
        audioObj.transform.localPosition = Vector3.zero;

        _fireSource = audioObj.AddComponent<AudioSource>();
        _fireSource.playOnAwake = false;
        _fireSource.spatialBlend = 0.5f;
        _fireSource.volume = _config.FireVolume;
    }

    private void HandleFired()
    {
        if (_muzzleFlash != null)
        {
            _muzzleFlash.Play();
        }

        PlayFireSound();
    }

    private void PlayFireSound()
    {
        if (_fireSource == null || _config.FireClip == null) return;

        _fireSource.pitch = Random.Range(_config.FirePitchMin, _config.FirePitchMax);
        _fireSource.PlayOneShot(_config.FireClip, _config.FireVolume);
    }

    private void HandleImpact(Vector3 position, Vector3 normal)
    {
        if (_impactPool != null)
        {
            _impactPool.PlayImpactEffect(position, normal);
        }
    }
}
