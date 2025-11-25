using UnityEngine;
using System.Collections.Generic;

public class HelicopterWaypointMovement : MonoBehaviour
{
    [Header("Waypoints")]
    [SerializeField] private Transform waypointsParent;
    [SerializeField] private List<Transform> waypoints = new List<Transform>();
    
    [Header("Movement")]
    [SerializeField] private float cruiseSpeed = 20f;
    [SerializeField] private float acceleration = 3f;
    [SerializeField] private float waypointReachDistance = 8f;
    
    [Header("Turning")]
    [SerializeField] private float yawSpeed = 0.8f;
    [SerializeField] private float bankingSpeed = 1.5f;
    [SerializeField] private float bankReturnSpeed = 1f;
    
    [Header("Helicopter Feel")]
    [SerializeField] private float maxPitchAngle = 12f;
    [SerializeField] private float maxRollAngle = 20f;
    [SerializeField] private float pitchSpeed = 1.5f;
    
    [Header("Oscillation")]
    [SerializeField] private float heightOscillationAmount = 0.3f;
    [SerializeField] private float heightOscillationSpeed = 1.2f;
    [SerializeField] private float swayAmount = 0.15f;
    [SerializeField] private float swaySpeed = 0.8f;
    
    [Header("Gizmos")]
    [SerializeField] private Color pathColor = Color.cyan;
    [SerializeField] private Color waypointColor = Color.yellow;
    [SerializeField] private float waypointGizmoSize = 1f;
    
    private int currentWaypointIndex = 0;
    private float currentSpeed = 0f;
    private float currentYaw = 0f;
    private float currentPitch = 0f;
    private float currentRoll = 0f;
    private float targetRoll = 0f;
    private float oscillationTime = 0f;
    private Vector3 currentVelocity = Vector3.zero;
    private float angularVelocity = 0f;

    private void Start()
    {
        CollectWaypoints();
        
        if (waypoints.Count > 0)
        {
            currentYaw = transform.eulerAngles.y;
        }
    }

    private void Update()
    {
        if (waypoints.Count < 2) return;
        
        UpdateMovement();
        UpdateRotation();
        ApplyOscillation();
    }

    private void CollectWaypoints()
    {
        waypoints.Clear();
        
        if (waypointsParent == null) return;
        
        foreach (Transform child in waypointsParent)
        {
            waypoints.Add(child);
        }
    }

    private void UpdateMovement()
    {
        Transform targetWaypoint = waypoints[currentWaypointIndex];
        Vector3 toTarget = targetWaypoint.position - transform.position;
        float distanceToWaypoint = toTarget.magnitude;
        
        if (distanceToWaypoint < waypointReachDistance)
        {
            currentWaypointIndex = (currentWaypointIndex + 1) % waypoints.Count;
        }
        
        currentSpeed = Mathf.Lerp(currentSpeed, cruiseSpeed, Time.deltaTime * acceleration);
        
        Vector3 forward = Quaternion.Euler(0, currentYaw, 0) * Vector3.forward;
        Vector3 targetVelocity = forward * currentSpeed;
        
        currentVelocity = Vector3.Lerp(currentVelocity, targetVelocity, Time.deltaTime * acceleration);
        
        transform.position += currentVelocity * Time.deltaTime;
    }

    private void UpdateRotation()
    {
        Transform targetWaypoint = waypoints[currentWaypointIndex];
        Vector3 toTarget = targetWaypoint.position - transform.position;
        toTarget.y = 0;
        
        if (toTarget.sqrMagnitude < 0.01f) return;
        
        float targetYaw = Mathf.Atan2(toTarget.x, toTarget.z) * Mathf.Rad2Deg;
        
        float yawDifference = Mathf.DeltaAngle(currentYaw, targetYaw);
        
        float maxYawChange = yawSpeed * 60f * Time.deltaTime;
        float yawChange = Mathf.Clamp(yawDifference * yawSpeed * Time.deltaTime * 2f, -maxYawChange, maxYawChange);
        
        float previousYaw = currentYaw;
        currentYaw += yawChange;
        
        angularVelocity = Mathf.Lerp(angularVelocity, (currentYaw - previousYaw) / Time.deltaTime, Time.deltaTime * 3f);
        
        targetRoll = Mathf.Clamp(angularVelocity * 0.4f, -maxRollAngle, maxRollAngle);
        
        if (Mathf.Abs(targetRoll) > Mathf.Abs(currentRoll))
        {
            currentRoll = Mathf.Lerp(currentRoll, targetRoll, Time.deltaTime * bankingSpeed);
        }
        else
        {
            currentRoll = Mathf.Lerp(currentRoll, targetRoll, Time.deltaTime * bankReturnSpeed);
        }
        
        float speedRatio = currentSpeed / cruiseSpeed;
        float targetPitch = speedRatio * maxPitchAngle;
        currentPitch = Mathf.Lerp(currentPitch, targetPitch, Time.deltaTime * pitchSpeed);
        
        transform.rotation = Quaternion.Euler(currentPitch, currentYaw, currentRoll);
    }

    private void ApplyOscillation()
    {
        oscillationTime += Time.deltaTime;
        
        float heightOffset = Mathf.Sin(oscillationTime * heightOscillationSpeed) * heightOscillationAmount;
        heightOffset += Mathf.Sin(oscillationTime * heightOscillationSpeed * 2.3f) * heightOscillationAmount * 0.3f;
        
        float swayX = Mathf.Sin(oscillationTime * swaySpeed) * swayAmount;
        float swayZ = Mathf.Sin(oscillationTime * swaySpeed * 0.7f + 1.5f) * swayAmount * 0.5f;
        
        Vector3 localOscillation = new Vector3(swayX, heightOffset, swayZ);
        Vector3 worldOscillation = transform.TransformDirection(localOscillation);
        
        transform.position += worldOscillation * Time.deltaTime;
    }

    private void OnValidate()
    {
        if (Application.isPlaying)
        {
            CollectWaypoints();
        }
    }

    private void OnDrawGizmos()
    {
        List<Transform> gizmoWaypoints = new List<Transform>();
        
        if (waypointsParent != null)
        {
            foreach (Transform child in waypointsParent)
            {
                gizmoWaypoints.Add(child);
            }
        }
        else if (waypoints != null && waypoints.Count > 0)
        {
            gizmoWaypoints = waypoints;
        }
        
        if (gizmoWaypoints.Count < 2) return;
        
        Gizmos.color = pathColor;
        for (int i = 0; i < gizmoWaypoints.Count; i++)
        {
            if (gizmoWaypoints[i] == null) continue;
            
            int nextIndex = (i + 1) % gizmoWaypoints.Count;
            if (gizmoWaypoints[nextIndex] == null) continue;
            
            Gizmos.DrawLine(gizmoWaypoints[i].position, gizmoWaypoints[nextIndex].position);
        }
        
        Gizmos.color = waypointColor;
        for (int i = 0; i < gizmoWaypoints.Count; i++)
        {
            if (gizmoWaypoints[i] == null) continue;
            
            Gizmos.DrawWireSphere(gizmoWaypoints[i].position, waypointGizmoSize);
            
#if UNITY_EDITOR
            UnityEditor.Handles.Label(gizmoWaypoints[i].position + Vector3.up * 2f, $"WP {i}");
#endif
        }
    }
}