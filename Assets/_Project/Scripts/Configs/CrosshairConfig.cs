using UnityEngine;

namespace LastConvoy.Configs
{
    [CreateAssetMenu(fileName = "CrosshairConfig", menuName = "LastConvoy/Configs/Crosshair")]
    public class CrosshairConfig : ScriptableObject
    {
        [field: SerializeField] public Color CrosshairColor { get; private set; } = Color.white;
        [field: SerializeField] public float LineThickness { get; private set; } = 2f;
        [field: SerializeField] public float LineLength { get; private set; } = 20f;
        [field: SerializeField] public float CenterGap { get; private set; } = 10f;

        [field: SerializeField] public float MaxSpread { get; private set; } = 30f;
        [field: SerializeField] public float SpreadSpeed { get; private set; } = 5f;
    }
}
