using System.Collections.Generic;
using UnityEngine;

public class EnemyCarSpawner : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject _enemyCarPrefab;
    [SerializeField] private Transform _playerCar;
    [SerializeField] private Transform _waypointsParent;

    [Header("Spawn Settings")]
    [SerializeField] private float _spawnInterval = 10f;
    [SerializeField] private int _maxEnemies = 5;
    [SerializeField] private float _spawnDistanceMin = 50f;
    [SerializeField] private float _spawnDistanceMax = 80f;
    [SerializeField] private float _spawnHeightAboveGround = 3f;
    [SerializeField] private float _spawnSideOffset = 10f;

    [Header("Ground Detection")]
    [SerializeField] private float _groundCheckHeight = 100f;
    [SerializeField] private LayerMask _groundMask = ~0;

    private List<EnemyCarController> _activeEnemies = new List<EnemyCarController>();
    private float _nextSpawnTime;
    private Transform[] _waypoints;

    private void Start()
    {
        SetupWaypoints();
        _nextSpawnTime = Time.time + _spawnInterval;
    }

    private void SetupWaypoints()
    {
        if (_waypointsParent == null)
        {
            Debug.LogError("[EnemyCarSpawner] Waypoints Parent is not assigned!");
            return;
        }

        _waypoints = new Transform[_waypointsParent.childCount];
        for (int i = 0; i < _waypointsParent.childCount; i++)
        {
            _waypoints[i] = _waypointsParent.GetChild(i);
        }
    }

    private void Update()
    {
        CleanupDestroyedEnemies();

        if (Time.time >= _nextSpawnTime)
        {
            TrySpawnEnemy();
            _nextSpawnTime = Time.time + _spawnInterval;
        }
    }

    private void CleanupDestroyedEnemies()
    {
        _activeEnemies.RemoveAll(enemy => enemy == null);
    }

    private void TrySpawnEnemy()
    {
        if (_activeEnemies.Count >= _maxEnemies) return;
        if (_playerCar == null) return;
        if (_enemyCarPrefab == null) return;

        Vector3 spawnPosition = CalculateSpawnPosition();
        
        if (spawnPosition == Vector3.zero) return;

        SpawnEnemy(spawnPosition);
    }

    private Vector3 CalculateSpawnPosition()
    {
        Vector3 playerForward = _playerCar.forward;
        playerForward.y = 0;
        playerForward.Normalize();

        float distance = Random.Range(_spawnDistanceMin, _spawnDistanceMax);
        float sideOffset = Random.Range(-_spawnSideOffset, _spawnSideOffset);

        Vector3 spawnPoint = _playerCar.position + playerForward * distance;
        spawnPoint += _playerCar.right * sideOffset;

        Ray ray = new Ray(spawnPoint + Vector3.up * _groundCheckHeight, Vector3.down);
        if (Physics.Raycast(ray, out RaycastHit hit, _groundCheckHeight * 2f, _groundMask))
        {
            return hit.point + Vector3.up * _spawnHeightAboveGround;
        }

        return Vector3.zero;
    }

    private void SpawnEnemy(Vector3 position)
    {
        int nearestWaypointIndex = FindNearestWaypointIndex(position);
        
        Vector3 lookDirection = Vector3.zero;
        if (_waypoints != null && _waypoints.Length > 0)
        {
            lookDirection = _waypoints[nearestWaypointIndex].position - position;
            lookDirection.y = 0;
        }

        Quaternion rotation = lookDirection.sqrMagnitude > 0.01f 
            ? Quaternion.LookRotation(lookDirection) 
            : Quaternion.identity;

        GameObject enemyObj = Instantiate(_enemyCarPrefab, position, rotation);
        
        EnemyCarController controller = enemyObj.GetComponent<EnemyCarController>();
        if (controller != null)
        {
            controller.Initialize(_playerCar, _waypointsParent, nearestWaypointIndex);
            _activeEnemies.Add(controller);
        }
    }

    private int FindNearestWaypointIndex(Vector3 position)
    {
        if (_waypoints == null || _waypoints.Length == 0) return 0;

        int nearestIndex = 0;
        float nearestDistance = float.MaxValue;

        for (int i = 0; i < _waypoints.Length; i++)
        {
            float distance = Vector3.Distance(position, _waypoints[i].position);
            if (distance < nearestDistance)
            {
                nearestDistance = distance;
                nearestIndex = i;
            }
        }

        return nearestIndex;
    }

    public int GetActiveEnemyCount()
    {
        return _activeEnemies.Count;
    }
}
