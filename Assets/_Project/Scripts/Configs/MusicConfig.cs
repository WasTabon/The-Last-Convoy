using UnityEngine;

namespace LastConvoy.Configs
{
    [CreateAssetMenu(fileName = "MusicConfig", menuName = "LastConvoy/Configs/Music")]
    public class MusicConfig : ScriptableObject
    {
        [field: SerializeField] public AudioClip MusicTrack { get; private set; }
        
        [field: SerializeField] public float MusicVolume { get; private set; } = 0.5f;
        [field: SerializeField] public float MasterVolume { get; private set; } = 1.0f;
        
        [field: SerializeField] public bool EnableInteriorEffect { get; private set; } = true;
        [field: SerializeField] public float InteriorEffectStrength { get; private set; } = 0.7f;
    }
}
