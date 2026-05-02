using UnityEngine;
using Zenject;

public class PlayerCarInstaller : MonoInstaller
{
    [Header("Config")]
    [SerializeField] private PlayerCarConfig _config;
    [SerializeField] private PlayerCarCameraConfig _cameraConfig;

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

        Container.Bind<PlayerCarConfig>().FromInstance(_config).AsSingle();
        Container.Bind<PlayerCarCameraConfig>().FromInstance(_cameraConfig).AsSingle();
        Container.Bind<Transform>().WithId("PlayerCarTransform").FromInstance(transform).AsSingle();
        Container.Bind<PlayerCarModel>().AsSingle();
        Container.BindInterfacesAndSelfTo<PlayerCarPresenter>().AsSingle();
    }
}
