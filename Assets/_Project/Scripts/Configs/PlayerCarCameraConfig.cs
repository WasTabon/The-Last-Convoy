using UnityEngine;

[CreateAssetMenu(fileName = "PlayerCarCameraConfig", menuName = "LastConvoy/Configs/PlayerCarCamera")]
public class PlayerCarCameraConfig : ScriptableObject
{
    [Header("Position")]
    [SerializeField] private Vector3 cameraOffset = new Vector3(0f, 1.2f, 0.3f);

    [Header("Shake")]
    [SerializeField] private float shakeAmountBySpeed = 0.02f;
    [SerializeField] private float shakeFrequency = 15f;
    [SerializeField] private float maxShakeAmount = 0.05f;

    [Header("Tilt")]
    [SerializeField] private float tiltAmountByTurn = 2f;
    [SerializeField] private float tiltSmoothing = 5f;

    public Vector3 CameraOffset => cameraOffset;
    public float ShakeAmountBySpeed => shakeAmountBySpeed;
    public float ShakeFrequency => shakeFrequency;
    public float MaxShakeAmount => maxShakeAmount;
    public float TiltAmountByTurn => tiltAmountByTurn;
    public float TiltSmoothing => tiltSmoothing;
}
