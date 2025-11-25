using UnityEngine;

public class CombatMusicManager : MonoBehaviour
{
    [Header("Music Track")]
    [SerializeField] private AudioClip musicTrack;
    
    [Header("Volume Settings")]
    [SerializeField] private float musicVolume = 0.5f;
    [SerializeField] private float masterVolume = 1.0f;
    
    [Header("Helicopter Interior Feel")]
    [SerializeField] private bool enableInteriorEffect = true;
    [SerializeField] private float interiorEffectStrength = 0.7f;
    
    private AudioSource musicSource;
    
    // Filters for music
    private AudioLowPassFilter musicLowPass;
    private AudioHighPassFilter musicHighPass;
    private AudioReverbFilter musicReverb;
    private AudioEchoFilter musicEcho;
    private AudioChorusFilter musicChorus;

    void Start()
    {
        SetupAudioSource();
        StartMusic();
    }

    void Update()
    {
        UpdateFilters();
    }

    void SetupAudioSource()
    {
        // Music source
        GameObject audioObj = new GameObject("Music_Track");
        audioObj.transform.SetParent(transform);
        audioObj.transform.localPosition = Vector3.zero;
        
        musicSource = audioObj.AddComponent<AudioSource>();
        musicSource.playOnAwake = false;
        musicSource.loop = true;
        musicSource.spatialBlend = 0f; // 2D sound
        musicSource.clip = musicTrack;
        musicSource.volume = musicVolume * masterVolume;
        musicSource.priority = 64;
        
        ConfigureMusicFilters();
    }

    void ConfigureMusicFilters()
    {
        if (musicSource == null) return;
        
        // Low-pass filter - creates muffled "in-helicopter" feel
        musicLowPass = musicSource.gameObject.AddComponent<AudioLowPassFilter>();
        musicLowPass.cutoffFrequency = 5000f;
        musicLowPass.lowpassResonanceQ = 1.0f;
        
        // High-pass filter - removes too much bass rumble
        musicHighPass = musicSource.gameObject.AddComponent<AudioHighPassFilter>();
        musicHighPass.cutoffFrequency = 200f;
        musicHighPass.highpassResonanceQ = 1.0f;
        
        // Reverb - adds space/interior acoustics (helicopter cabin feel)
        musicReverb = musicSource.gameObject.AddComponent<AudioReverbFilter>();
        musicReverb.reverbPreset = AudioReverbPreset.Room;
        musicReverb.dryLevel = -200f;
        musicReverb.room = -600f;
        musicReverb.roomHF = -800f;
        musicReverb.decayTime = 0.6f;
        musicReverb.decayHFRatio = 0.5f;
        musicReverb.reflectionsLevel = -1200;
        musicReverb.reflectionsDelay = 0.01f;
        musicReverb.reverbLevel = -800;
        musicReverb.reverbDelay = 0.02f;
        musicReverb.diffusion = 70f;
        musicReverb.density = 60f;
        
        // Echo - subtle mechanical echo from helicopter interior
        musicEcho = musicSource.gameObject.AddComponent<AudioEchoFilter>();
        musicEcho.delay = 50f;
        musicEcho.decayRatio = 0.3f;
        musicEcho.wetMix = 0.15f;
        musicEcho.dryMix = 0.85f;
        
        // Chorus - adds richness and width to music
        musicChorus = musicSource.gameObject.AddComponent<AudioChorusFilter>();
        musicChorus.dryMix = 0.85f;
        musicChorus.wetMix1 = 0.35f;
        musicChorus.wetMix2 = 0.25f;
        musicChorus.wetMix3 = 0.15f;
        musicChorus.delay = 25f;
        musicChorus.rate = 0.5f;
        musicChorus.depth = 0.06f;
    }

    void StartMusic()
    {
        if (musicSource != null && musicTrack != null)
        {
            musicSource.Play();
        }
    }

    void UpdateFilters()
    {
        if (!enableInteriorEffect) return;
        
        float intensity = interiorEffectStrength;
        
        // Adjust filters dynamically based on interior effect strength
        if (musicLowPass != null)
        {
            // Lower cutoff frequency = more muffled "interior" sound
            float targetFreq = Mathf.Lerp(10000f, 4000f, intensity);
            musicLowPass.cutoffFrequency = Mathf.Lerp(
                musicLowPass.cutoffFrequency,
                targetFreq,
                Time.deltaTime * 2f
            );
        }
        
        if (musicReverb != null)
        {
            // More reverb = stronger interior acoustic feel
            float targetRoom = Mathf.Lerp(-1000f, -600f, intensity);
            musicReverb.room = Mathf.Lerp(
                musicReverb.room,
                targetRoom,
                Time.deltaTime * 2f
            );
        }
        
        if (musicEcho != null)
        {
            // More echo = more mechanical interior reflections
            float targetWetMix = Mathf.Lerp(0.05f, 0.15f, intensity);
            musicEcho.wetMix = Mathf.Lerp(
                musicEcho.wetMix,
                targetWetMix,
                Time.deltaTime * 2f
            );
        }
    }

    // Public methods to control music
    public void SetMasterVolume(float volume)
    {
        masterVolume = Mathf.Clamp01(volume);
        if (musicSource != null)
        {
            musicSource.volume = musicVolume * masterVolume;
        }
    }

    public void SetMusicVolume(float volume)
    {
        musicVolume = Mathf.Clamp01(volume);
        if (musicSource != null)
        {
            musicSource.volume = musicVolume * masterVolume;
        }
    }

    public void SetInteriorEffect(bool enabled)
    {
        enableInteriorEffect = enabled;
        
        // If disabling, reset filters to neutral
        if (!enabled)
        {
            if (musicLowPass != null) musicLowPass.cutoffFrequency = 22000f;
            if (musicReverb != null) musicReverb.room = -10000f;
            if (musicEcho != null) musicEcho.wetMix = 0f;
        }
    }

    public void SetInteriorEffectStrength(float strength)
    {
        interiorEffectStrength = Mathf.Clamp01(strength);
    }

    public void PauseMusic()
    {
        if (musicSource != null && musicSource.isPlaying)
        {
            musicSource.Pause();
        }
    }

    public void ResumeMusic()
    {
        if (musicSource != null && !musicSource.isPlaying)
        {
            musicSource.UnPause();
        }
    }

    public void StopMusic()
    {
        if (musicSource != null)
        {
            musicSource.Stop();
        }
    }

    void OnValidate()
    {
        masterVolume = Mathf.Clamp01(masterVolume);
        musicVolume = Mathf.Clamp01(musicVolume);
        interiorEffectStrength = Mathf.Clamp01(interiorEffectStrength);
        
        // Update volume in real-time in editor
        if (Application.isPlaying && musicSource != null)
        {
            musicSource.volume = musicVolume * masterVolume;
        }
    }
}