using UnityEngine;

namespace LastConvoy.Configs
{
    [CreateAssetMenu(fileName = "CameraConfig", menuName = "LastConvoy/Configs/Camera")]
    public class CameraConfig : ScriptableObject
    {
        [field: SerializeField] public float MouseSensitivity { get; private set; } = 100f;
        
        [field: SerializeField] public float MinVerticalAngle { get; private set; } = -90f;
        [field: SerializeField] public float MaxVerticalAngle { get; private set; } = 90f;
        [field: SerializeField] public float MinHorizontalAngle { get; private set; } = -90f;
        [field: SerializeField] public float MaxHorizontalAngle { get; private set; } = 90f;
    }
}
