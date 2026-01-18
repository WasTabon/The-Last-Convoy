using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class EnemyHelicopterInstaller : MonoInstaller
{
    [Header("Config")]
    [SerializeField] private EnemyHelicopterConfig _config;

    [Header("Waypoints")]
    [SerializeField] private Transform _waypointsParent;

    public override void InstallBindings()
    {
        List<Transform> waypoints = CollectWaypoints();

        Container.Bind<EnemyHelicopterConfig>().FromInstance(_config).AsSingle();
        Container.Bind<List<Transform>>().FromInstance(waypoints).AsSingle();
        Container.Bind<EnemyHelicopterModel>().AsSingle();
        Container.BindInterfacesAndSelfTo<EnemyHelicopterPresenter>().AsSingle();
    }

    private List<Transform> CollectWaypoints()
    {
        List<Transform> waypoints = new List<Transform>();

        if (_waypointsParent == null)
        {
            Debug.LogError("[EnemyHelicopterInstaller] Waypoints Parent is not assigned!");
            return waypoints;
        }

        foreach (Transform child in _waypointsParent)
        {
            waypoints.Add(child);
        }

        if (waypoints.Count == 0)
        {
            Debug.LogError("[EnemyHelicopterInstaller] No waypoints found in Waypoints Parent!");
        }

        return waypoints;
    }
}
