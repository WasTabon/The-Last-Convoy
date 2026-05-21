using UnityEngine;
using Zenject;

public class PlayerCarInstaller : MonoInstaller
{
    [Header("Car Config")]
    [SerializeField] private PlayerCarConfig _config;
    [SerializeField] private PlayerCarCameraConfig _cameraConfig;

    [Header("Turret Config")]
    [SerializeField] private TurretConfig _turretConfig;
    [SerializeField] private TurretWeaponConfig _turretWeaponConfig;

    [Header("References")]
    [SerializeField] private Camera _mainCamera;

    public override void InstallBindings()
    {
        if (_config == null)
        {
            Debug.LogError("[PlayerCarInstaller] Config is not assigned!");
            return;
        }

        if (_cameraConfig == null)
        {
            Debug.LogError("[PlayerCarInstaller] Camera Config is not assigned!");
            return;
        }

        if (_turretConfig == null)
        {
            Debug.LogError("[PlayerCarInstaller] Turret Config is not assigned!");
            return;
        }

        if (_turretWeaponConfig == null)
        {
            Debug.LogError("[PlayerCarInstaller] Turret Weapon Config is not assigned!");
            return;
        }

        if (_mainCamera == null)
        {
            _mainCamera = Camera.main;
            if (_mainCamera == null)
            {
                Debug.LogError("[PlayerCarInstaller] Main Camera not found!");
                return;
            }
        }

        Container.Bind<PlayerCarConfig>().FromInstance(_config).AsSingle();
        Container.Bind<PlayerCarCameraConfig>().FromInstance(_cameraConfig).AsSingle();
        Container.Bind<TurretConfig>().FromInstance(_turretConfig).AsSingle();
        Container.Bind<TurretWeaponConfig>().FromInstance(_turretWeaponConfig).AsSingle();
        Container.Bind<Camera>().FromInstance(_mainCamera).AsSingle();
        Container.Bind<Transform>().WithId("PlayerCarTransform").FromInstance(transform).AsSingle();

        Container.Bind<PlayerCarModel>().AsSingle();
        Container.Bind<TurretModel>().AsSingle();
        Container.Bind<TurretWeaponModel>().AsSingle();

        Container.BindInterfacesAndSelfTo<PlayerCarPresenter>().AsSingle();
        Container.BindInterfacesAndSelfTo<TurretPresenter>().AsSingle();
        Container.BindInterfacesAndSelfTo<TurretWeaponPresenter>().AsSingle();
    }
}
