using UnityEngine;
using Zenject;
using LastConvoy.Models;
using LastConvoy.Presenters;
using LastConvoy.Views.Effects;

namespace LastConvoy.Views.Weapon
{
    public class WeaponView : MonoBehaviour
    {
        [SerializeField] private Transform _barrelTransform;
        [SerializeField] private Transform _minigunTransform;
        [SerializeField] private ParticleSystem _muzzleFlash;
        [SerializeField] private ParticleSystem _bulletShells;

        private WeaponModel _model;
        private WeaponPresenter _presenter;
        private BulletImpactPool _impactPool;
        private bool _isInjected;

        [Inject]
        public void Construct(
            WeaponModel model,
            WeaponPresenter presenter,
            BulletImpactPool impactPool)
        {
            _model = model;
            _presenter = presenter;
            _impactPool = impactPool;
            _isInjected = true;
        }

        private void OnEnable()
        {
            if (!_isInjected) return;
            
            _model.OnFiringStarted += HandleFiringStarted;
            _model.OnFiringStopped += HandleFiringStopped;
            _presenter.OnImpact += HandleImpact;
        }

        private void OnDisable()
        {
            if (!_isInjected) return;
            
            _model.OnFiringStarted -= HandleFiringStarted;
            _model.OnFiringStopped -= HandleFiringStopped;
            _presenter.OnImpact -= HandleImpact;
        }

        private void Update()
        {
            if (!_isInjected) return;
            
            RotateBarrel();
            RotateMinigunToCamera();
        }

        private void RotateBarrel()
        {
            if (_barrelTransform == null) return;
            _barrelTransform.Rotate(0f, 0f, -_model.CurrentBarrelSpeed * Time.deltaTime);
        }

        private void RotateMinigunToCamera()
        {
            if (_minigunTransform == null) return;

            _minigunTransform.rotation = UnityEngine.Camera.main.transform.rotation;
        }

        private void HandleFiringStarted()
        {
            if (_muzzleFlash != null) _muzzleFlash.Play();
            if (_bulletShells != null) _bulletShells.Play();
        }

        private void HandleFiringStopped()
        {
            if (_muzzleFlash != null) _muzzleFlash.Stop();
            if (_bulletShells != null) _bulletShells.Stop();
        }

        private void HandleImpact(Vector3 position, Vector3 normal)
        {
            if (_impactPool != null)
            {
                _impactPool.PlayImpactEffect(position, normal);
            }
        }
    }
}