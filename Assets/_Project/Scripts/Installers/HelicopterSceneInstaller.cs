using System.Collections.Generic;
using UnityEngine;
using Zenject;
using LastConvoy.Configs;
using LastConvoy.Models;
using LastConvoy.Presenters;
using LastConvoy.StateMachine;
using LastConvoy.StateMachine.States;
using LastConvoy.Views.Effects;

namespace LastConvoy.Installers
{
    public class HelicopterSceneInstaller : MonoInstaller
    {
        [Header("Configs")]
        [SerializeField] private WeaponConfig _weaponConfig;
        [SerializeField] private HelicopterConfig _helicopterConfig;
        [SerializeField] private CameraConfig _cameraConfig;
        [SerializeField] private CrosshairConfig _crosshairConfig;
        [SerializeField] private MusicConfig _musicConfig;

        [Header("Scene References")]
        [SerializeField] private Transform _waypointsParent;
        [SerializeField] private Camera _mainCamera;
        [SerializeField] private BulletImpactPool _impactPool;

        public override void InstallBindings()
        {
            BindConfigs();
            BindModels();
            BindPresenters();
            BindSceneReferences();
        }

        public override void Start()
        {
            base.Start();
            InitializeStateMachine();
        }

        private void BindConfigs()
        {
            Container.Bind<WeaponConfig>().FromInstance(_weaponConfig).AsSingle();
            Container.Bind<HelicopterConfig>().FromInstance(_helicopterConfig).AsSingle();
            Container.Bind<CameraConfig>().FromInstance(_cameraConfig).AsSingle();
            Container.Bind<CrosshairConfig>().FromInstance(_crosshairConfig).AsSingle();
            Container.Bind<MusicConfig>().FromInstance(_musicConfig).AsSingle();
        }

        private void BindModels()
        {
            Container.Bind<WeaponModel>().AsSingle();
            Container.Bind<HelicopterModel>().AsSingle();
            Container.Bind<CameraModel>().AsSingle();
            Container.Bind<CrosshairModel>().AsSingle();
        }

        private void BindPresenters()
        {
            Container.BindInterfacesAndSelfTo<WeaponPresenter>().AsSingle();
            Container.BindInterfacesAndSelfTo<HelicopterPresenter>().AsSingle();
            Container.BindInterfacesAndSelfTo<CameraPresenter>().AsSingle();
            Container.BindInterfacesAndSelfTo<CrosshairPresenter>().AsSingle();
        }

        private void BindSceneReferences()
        {
            Container.Bind<Camera>().FromInstance(_mainCamera).AsSingle();
            Container.Bind<BulletImpactPool>().FromInstance(_impactPool).AsSingle();

            List<Transform> waypoints = new List<Transform>();
            if (_waypointsParent != null)
            {
                foreach (Transform child in _waypointsParent)
                {
                    waypoints.Add(child);
                }
            }
            Container.Bind<List<Transform>>().WithId("Waypoints").FromInstance(waypoints).AsSingle();
        }

        private void InitializeStateMachine()
        {
            var stateMachine = Container.Resolve<GameStateMachine>();
            var gameplayState = Container.Resolve<GameplayState>();
            var pauseState = Container.Resolve<PauseState>();

            stateMachine.RegisterState(gameplayState);
            stateMachine.RegisterState(pauseState);

            stateMachine.ChangeState<GameplayState>();
        }
    }
}
