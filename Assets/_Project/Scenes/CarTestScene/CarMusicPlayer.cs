using UnityEngine;

public class CarMusicPlayer : MonoBehaviour
{
    [Header("Music")]
    [SerializeField] private AudioClip _musicTrack;
    [SerializeField] private float _musicVolume = 0.6f;

    [Header("Car Speaker Feel")]
    [SerializeField] private float _lowPassCutoff = 4000f;
    [SerializeField] private float _highPassCutoff = 120f;
    [SerializeField] private float _distortionLevel = 0.1f;

    private AudioSource _musicSource;
    private AudioLowPassFilter _lowPass;
    private AudioHighPassFilter _highPass;
    private AudioDistortionFilter _distortion;
    private AudioChorusFilter _chorus;
    private AudioReverbFilter _reverb;

    private void Start()
    {
        SetupAudio();
        PlayMusic();
    }

    private void SetupAudio()
    {
        GameObject audioObj = new GameObject("CarMusic");
        audioObj.transform.SetParent(transform);
        audioObj.transform.localPosition = Vector3.zero;

        _musicSource = audioObj.AddComponent<AudioSource>();
        _musicSource.clip = _musicTrack;
        _musicSource.loop = true;
        _musicSource.playOnAwake = false;
        _musicSource.spatialBlend = 0f;
        _musicSource.volume = _musicVolume;
        _musicSource.priority = 64;

        _lowPass = audioObj.AddComponent<AudioLowPassFilter>();
        _lowPass.cutoffFrequency = _lowPassCutoff;
        _lowPass.lowpassResonanceQ = 1.5f;

        _highPass = audioObj.AddComponent<AudioHighPassFilter>();
        _highPass.cutoffFrequency = _highPassCutoff;
        _highPass.highpassResonanceQ = 1.2f;

        _distortion = audioObj.AddComponent<AudioDistortionFilter>();
        _distortion.distortionLevel = _distortionLevel;

        _chorus = audioObj.AddComponent<AudioChorusFilter>();
        _chorus.dryMix = 0.9f;
        _chorus.wetMix1 = 0.2f;
        _chorus.wetMix2 = 0.15f;
        _chorus.wetMix3 = 0.1f;
        _chorus.delay = 15f;
        _chorus.rate = 0.4f;
        _chorus.depth = 0.04f;

        _reverb = audioObj.AddComponent<AudioReverbFilter>();
        _reverb.reverbPreset = AudioReverbPreset.Room;
        _reverb.dryLevel = 0f;
        _reverb.room = -400f;
        _reverb.roomHF = -500f;
        _reverb.decayTime = 0.4f;
        _reverb.decayHFRatio = 0.7f;
        _reverb.reflectionsLevel = -800;
        _reverb.reflectionsDelay = 0.005f;
        _reverb.reverbLevel = -600;
        _reverb.reverbDelay = 0.01f;
        _reverb.diffusion = 80f;
        _reverb.density = 70f;
    }

    private void PlayMusic()
    {
        if (_musicSource != null && _musicTrack != null)
        {
            _musicSource.Play();
        }
    }

    public void SetVolume(float volume)
    {
        _musicVolume = Mathf.Clamp01(volume);
        if (_musicSource != null)
        {
            _musicSource.volume = _musicVolume;
        }
    }

    public void Pause()
    {
        if (_musicSource != null && _musicSource.isPlaying)
        {
            _musicSource.Pause();
        }
    }

    public void Resume()
    {
        if (_musicSource != null && !_musicSource.isPlaying)
        {
            _musicSource.UnPause();
        }
    }

    public void Stop()
    {
        if (_musicSource != null)
        {
            _musicSource.Stop();
        }
    }
}
