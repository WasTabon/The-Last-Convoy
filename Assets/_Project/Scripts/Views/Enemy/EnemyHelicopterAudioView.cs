using UnityEngine;
using Zenject;

public class EnemyHelicopterAudioView : MonoBehaviour
{
    [SerializeField] private Transform _rotorAudioPosition;

    private EnemyHelicopterModel _model;
    private EnemyHelicopterConfig _config;

    private AudioSource _rotorSource;
    private AudioLowPassFilter _rotorLowPass;

    [Inject]
    public void Construct(EnemyHelicopterModel model, EnemyHelicopterConfig config)
    {
        _model = model;
        _config = config;
    }

    private void Awake()
    {
        SetupAudio();
    }

    private void Start()
    {
        StartAudio();
    }

    private void Update()
    {
        UpdateAudio();
    }

    private void SetupAudio()
    {
        GameObject audioObj = new GameObject("Audio_EnemyRotor");

        if (_rotorAudioPosition != null)
        {
            audioObj.transform.SetParent(_rotorAudioPosition);
        }
        else
        {
            audioObj.transform.SetParent(transform);
            Debug.LogWarning("[EnemyHelicopterAudioView] Rotor Audio Position not assigned, using transform root");
        }

        audioObj.transform.localPosition = Vector3.zero;

        _rotorSource = audioObj.AddComponent<AudioSource>();
        _rotorSource.clip = _config.RotorLoopClip;
        _rotorSource.loop = true;
        _rotorSource.playOnAwake = false;
        _rotorSource.volume = _config.RotorBaseVolume;
        _rotorSource.spatialBlend = _config.AudioSpatialBlend;
        _rotorSource.dopplerLevel = _config.AudioDopplerLevel;
        _rotorSource.minDistance = _config.AudioMinDistance;
        _rotorSource.maxDistance = _config.AudioMaxDistance;
        _rotorSource.rolloffMode = AudioRolloffMode.Custom;
        _rotorSource.spread = 90f;
        _rotorSource.priority = 100;

        _rotorLowPass = audioObj.AddComponent<AudioLowPassFilter>();
        _rotorLowPass.cutoffFrequency = _config.LowPassMaxFrequency;
        _rotorLowPass.lowpassResonanceQ = 1.2f;
    }

    private void StartAudio()
    {
        if (_config.RotorLoopClip == null)
        {
            Debug.LogError("[EnemyHelicopterAudioView] Rotor Loop Clip is not assigned in config!");
            return;
        }

        _rotorSource.Play();
    }

    private void UpdateAudio()
    {
        float speedRatio = _model.SpeedRatio;
        float turnIntensity = _model.TurnIntensity;

        float targetPitch = Mathf.Lerp(_config.RotorMinPitch, _config.RotorMaxPitch, speedRatio);
        _rotorSource.pitch = Mathf.Lerp(_rotorSource.pitch, targetPitch, Time.deltaTime * 3f);

        float volumeBoost = turnIntensity * 0.15f;
        float targetVolume = _config.RotorBaseVolume + speedRatio * 0.1f + volumeBoost;
        _rotorSource.volume = Mathf.Lerp(_rotorSource.volume, Mathf.Clamp01(targetVolume), Time.deltaTime * 3f);

        float targetCutoff = Mathf.Lerp(_config.LowPassMinFrequency, _config.LowPassMaxFrequency, speedRatio);
        _rotorLowPass.cutoffFrequency = Mathf.Lerp(_rotorLowPass.cutoffFrequency, targetCutoff, Time.deltaTime * 5f);
    }

    public void StopAudio()
    {
        if (_rotorSource != null && _rotorSource.isPlaying)
        {
            _rotorSource.Stop();
        }
    }

    public void FadeOutAndStop(float duration)
    {
        StartCoroutine(FadeOutCoroutine(duration));
    }

    private System.Collections.IEnumerator FadeOutCoroutine(float duration)
    {
        float startVolume = _rotorSource.volume;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            _rotorSource.volume = Mathf.Lerp(startVolume, 0f, elapsed / duration);
            yield return null;
        }

        _rotorSource.Stop();
    }
}
