using UnityEngine;
using Zenject;
using LastConvoy.Models;
using LastConvoy.Configs;

namespace LastConvoy.Views.Helicopter
{
    public class HelicopterAudioView : MonoBehaviour
    {
        [SerializeField] private Transform _mainRotorPosition;

        private HelicopterModel _model;
        private HelicopterConfig _config;

        private AudioSource _mainRotorSource;
        private AudioSource _windSource;
        private AudioLowPassFilter _mainRotorLowPass;
        private AudioHighPassFilter _windHighPass;

        [Inject]
        public void Construct(HelicopterModel model, HelicopterConfig config)
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
            _mainRotorSource = CreateAudioSource("MainRotor", _mainRotorPosition);
            ConfigureMainRotorAudio();

            _windSource = CreateAudioSource("Wind", null);
            ConfigureWindAudio();
        }

        private AudioSource CreateAudioSource(string name, Transform position)
        {
            GameObject audioObj = new GameObject($"Audio_{name}");

            if (position != null)
            {
                audioObj.transform.SetParent(position);
            }
            else
            {
                audioObj.transform.SetParent(transform);
            }

            audioObj.transform.localPosition = Vector3.zero;

            AudioSource source = audioObj.AddComponent<AudioSource>();
            source.playOnAwake = false;
            source.loop = true;
            source.spatialBlend = 1f;
            source.dopplerLevel = 0.3f;
            source.spread = 60f;
            source.rolloffMode = AudioRolloffMode.Custom;
            source.maxDistance = 100f;
            source.minDistance = 2f;

            return source;
        }

        private void ConfigureMainRotorAudio()
        {
            if (_mainRotorSource == null) return;

            _mainRotorSource.clip = _config.MainRotorLoop;
            _mainRotorSource.volume = _config.BaseRotorVolume;
            _mainRotorSource.spatialBlend = 0.7f;
            _mainRotorSource.spread = 120f;
            _mainRotorSource.minDistance = 3f;
            _mainRotorSource.maxDistance = 150f;

            _mainRotorLowPass = _mainRotorSource.gameObject.AddComponent<AudioLowPassFilter>();
            _mainRotorLowPass.cutoffFrequency = 4500f;
            _mainRotorLowPass.lowpassResonanceQ = 1.2f;
        }

        private void ConfigureWindAudio()
        {
            if (_windSource == null) return;

            _windSource.clip = _config.WindLoop;
            _windSource.volume = 0f;
            _windSource.spatialBlend = 0f;
            _windSource.priority = 64;

            _windHighPass = _windSource.gameObject.AddComponent<AudioHighPassFilter>();
            _windHighPass.cutoffFrequency = 800f;
            _windHighPass.highpassResonanceQ = 1.5f;

            var windLowPass = _windSource.gameObject.AddComponent<AudioLowPassFilter>();
            windLowPass.cutoffFrequency = 8000f;
        }

        private void StartAudio()
        {
            if (_mainRotorSource != null && _config.MainRotorLoop != null)
            {
                _mainRotorSource.Play();
            }

            if (_windSource != null && _config.WindLoop != null)
            {
                _windSource.Play();
            }
        }

        private void UpdateAudio()
        {
            float speedRatio = _model.SpeedRatio;
            float turnIntensity = _model.TurnIntensity;

            if (_mainRotorSource != null)
            {
                float rotorVolume = _config.BaseRotorVolume + speedRatio * 0.2f + turnIntensity * 0.1f;
                _mainRotorSource.volume = Mathf.Clamp(rotorVolume, 0f, 0.7f);

                if (_mainRotorLowPass != null)
                {
                    _mainRotorLowPass.cutoffFrequency = Mathf.Lerp(3500f, 6000f, speedRatio);
                }
            }

            if (_windSource != null)
            {
                float windVolume = _config.BaseWindVolume + speedRatio * _config.WindVolumeBySpeed;
                windVolume += turnIntensity * 0.2f;
                _windSource.volume = Mathf.Clamp(windVolume, 0f, 0.3f);

                float windPitch = 0.8f + speedRatio * 0.4f;
                _windSource.pitch = windPitch;

                if (_windHighPass != null)
                {
                    _windHighPass.cutoffFrequency = Mathf.Lerp(600f, 1200f, speedRatio);
                }
            }
        }
    }
}
