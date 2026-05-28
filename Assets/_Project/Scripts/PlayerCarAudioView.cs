using UnityEngine;
using Zenject;
using LastConvoy.Models;

public class PlayerCarAudioView : MonoBehaviour
{
    [Header("Audio")]
    [SerializeField] private AudioClip _engineIdleLoop;
    [SerializeField] private float _baseVolume = 0.5f;

    [Header("Pitch Settings")]
    [SerializeField] private float _pitchMin = 0.8f;
    [SerializeField] private float _pitchMax = 1.5f;

    [Header("Volume by Speed")]
    [SerializeField] private float _volumeMin = 0.4f;
    [SerializeField] private float _volumeMax = 0.7f;

    [Header("Speed Reference")]
    [SerializeField] private float _maxSpeed = 25f;

    private PlayerCarModel _model;
    private AudioSource _engineSource;
    private bool _isInjected;

    [Inject]
    public void Construct(PlayerCarModel model)
    {
        _model = model;
        _isInjected = true;
    }

    private void Start()
    {
        SetupAudio();
        PlayEngine();
    }

    private void Update()
    {
        if (!_isInjected) return;
        UpdateEngineSound();
    }

    private void SetupAudio()
    {
        GameObject audioObj = new GameObject("EngineAudio");
        audioObj.transform.SetParent(transform);
        audioObj.transform.localPosition = Vector3.zero;

        _engineSource = audioObj.AddComponent<AudioSource>();
        _engineSource.clip = _engineIdleLoop;
        _engineSource.loop = true;
        _engineSource.playOnAwake = false;
        _engineSource.spatialBlend = 0.5f;
        _engineSource.volume = _baseVolume;
        _engineSource.priority = 64;

        var lowPass = audioObj.AddComponent<AudioLowPassFilter>();
        lowPass.cutoffFrequency = 5000f;
    }

    private void PlayEngine()
    {
        if (_engineSource != null && _engineIdleLoop != null)
        {
            _engineSource.Play();
        }
    }

    private void UpdateEngineSound()
    {
        if (_engineSource == null) return;

        float speedRatio = Mathf.Clamp01(_model.CurrentSpeed / _maxSpeed);

        _engineSource.pitch = Mathf.Lerp(_pitchMin, _pitchMax, speedRatio);
        _engineSource.volume = Mathf.Lerp(_volumeMin, _volumeMax, speedRatio);
    }
}
