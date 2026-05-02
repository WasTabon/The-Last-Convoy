using UnityEngine;

[CreateAssetMenu(fileName = "PlayerCarConfig", menuName = "LastConvoy/Configs/PlayerCar")]
public class PlayerCarConfig : ScriptableObject
{
    [Header("Speed")]
    [SerializeField] private float maxForwardSpeed = 25f;
    [SerializeField] private float maxReverseSpeed = 10f;
    [SerializeField] private float acceleration = 15f;
    [SerializeField] private float brakeForce = 20f;
    [SerializeField] private float deceleration = 8f;

    [Header("Turning")]
    [SerializeField] private float turnSpeed = 80f;
    [SerializeField] private float turnSpeedReduction = 0.3f;
    [SerializeField] private float minSpeedToTurn = 1f;

    public float MaxForwardSpeed => maxForwardSpeed;
    public float MaxReverseSpeed => maxReverseSpeed;
    public float Acceleration => acceleration;
    public float BrakeForce => brakeForce;
    public float Deceleration => deceleration;
    public float TurnSpeed => turnSpeed;
    public float TurnSpeedReduction => turnSpeedReduction;
    public float MinSpeedToTurn => minSpeedToTurn;
}
