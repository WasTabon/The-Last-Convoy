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
        private CameraModel _cameraModel;
        private BulletImpactPool _impactPool;

        [Inject]
        public void Construct(
            WeaponModel model,
            WeaponPresenter presenter,
            CameraModel cameraModel,
            BulletImpactPool impactPool)
        {
            _model = model;
            _presenter = presenter;
            _cameraModel = cameraModel;
            _impactPool = impactPool;
        }

        private void OnEnable()
        {
            _model.OnFiringStarted += HandleFiringStarted;
            _model.OnFiringStopped += HandleFiringStopped;
            _presenter.OnImpact += HandleImpact;
            _presenter.OnRecoilChanged += HandleRecoilChanged;
        }

        private void OnDisable()
        {
            _model.OnFiringStarted -= HandleFiringStarted;
            _model.OnFiringStopped -= HandleFiringStopped;
            _presenter.OnImpact -= HandleImpact;
            _presenter.OnRecoilChanged -= HandleRecoilChanged;
        }

        private void Update()
        {
            RotateBarrel();
        }

        private void RotateBarrel()
        {
            if (_barrelTransform == null) return;

            _barrelTransform.Rotate(0f, 0f, -_model.CurrentBarrelSpeed * Time.deltaTime);
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

        private void HandleRecoilChanged(Vector3 recoilRotation)
        {
            if (_minigunTransform == null) return;

            Quaternion targetRotation = _cameraModel.Rotation *
                Quaternion.Euler(recoilRotation.x, recoilRotation.y, 0f);

            _minigunTransform.rotation = Quaternion.Slerp(
                _minigunTransform.rotation,
                targetRotation,
                Time.deltaTime * 15f
            );
        }
    }
}
