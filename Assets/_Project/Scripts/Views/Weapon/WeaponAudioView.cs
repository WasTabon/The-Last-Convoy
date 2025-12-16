using UnityEngine;
using Zenject;
using LastConvoy.Models;
using LastConvoy.Configs;

namespace LastConvoy.Views.Weapon
{
    public class WeaponAudioView : MonoBehaviour
    {
        [SerializeField] private Transform _muzzlePosition;

        private WeaponModel _model;
        private WeaponConfig _config;

        private AudioSource _spinSource;
        private AudioSource _fireSource;
        private AudioLowPassFilter _fireLowPass;
        private AudioDistortionFilter _fireDistortion;

        [Inject]
        public void Construct(WeaponModel model, WeaponConfig config)
        {
            _model = model;
            _config = config;
        }

        private void Awake()
        {
            SetupAudio();
        }

        private void OnEnable()
        {
            _model.OnFiringStarted += HandleFiringStarted;
            _model.OnFiringStopped += HandleFiringStopped;
            _model.OnSpinProgressChanged += HandleSpinProgressChanged;
        }

        private void OnDisable()
        {
            _model.OnFiringStarted -= HandleFiringStarted;
            _model.OnFiringStopped -= HandleFiringStopped;
            _model.OnSpinProgressChanged -= HandleSpinProgressChanged;
        }

        private void Update()
        {
            UpdateSpinAudio();
            UpdateFireAudio();
        }

        private void SetupAudio()
        {
            _spinSource = CreateAudioSource("Spin");
            _spinSource.clip = _config.SpinClip;
            _spinSource.loop = true;
            _spinSource.spatialBlend = 0.8f;
            _spinSource.volume = 0f;
            _spinSource.minDistance = _config.AudioMinDistance;
            _spinSource.maxDistance = _config.AudioMaxDistance;

            _fireSource = CreateAudioSource("Fire");
            _fireSource.clip = _config.FireLoopClip;
            _fireSource.loop = true;
            _fireSource.spatialBlend = 0.9f;
            _fireSource.volume = _config.BaseFireVolume;
            _fireSource.minDistance = _config.AudioMinDistance;
            _fireSource.maxDistance = _config.AudioMaxDistance;

            _fireLowPass = _fireSource.gameObject.AddComponent<AudioLowPassFilter>();
            _fireLowPass.cutoffFrequency = 8000f;

            _fireDistortion = _fireSource.gameObject.AddComponent<AudioDistortionFilter>();
            _fireDistortion.distortionLevel = 0.15f;

            var fireHighPass = _fireSource.gameObject.AddComponent<AudioHighPassFilter>();
            fireHighPass.cutoffFrequency = 150f;
        }

        private AudioSource CreateAudioSource(string name)
        {
            GameObject audioObj = new GameObject($"Audio_{name}");

            if (_muzzlePosition != null)
            {
                audioObj.transform.SetParent(_muzzlePosition);
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
            source.dopplerLevel = 0.5f;
            source.rolloffMode = AudioRolloffMode.Custom;
            source.priority = 64;

            return source;
        }

        private void HandleFiringStarted()
        {
            if (_spinSource != null && _spinSource.isPlaying)
            {
                _spinSource.Stop();
            }

            if (_fireSource != null && _config.FireLoopClip != null)
            {
                _fireSource.Play();
            }
        }

        private void HandleFiringStopped()
        {
            if (_fireSource != null && _fireSource.isPlaying)
            {
                _fireSource.Stop();
            }
        }

        private void HandleSpinProgressChanged(float progress)
        {
            if (_spinSource == null || _config.SpinClip == null) return;

            bool shouldPlaySpin = progress > 0.01f && !_model.IsFiring;

            if (shouldPlaySpin && !_spinSource.isPlaying)
            {
                _spinSource.Play();
            }
            else if (!shouldPlaySpin && _spinSource.isPlaying)
            {
                _spinSource.Stop();
            }
        }

        private void UpdateSpinAudio()
        {
            if (_spinSource == null || !_spinSource.isPlaying) return;

            float targetPitch = Mathf.Lerp(_config.SpinPitchMin, _config.SpinPitchMax, _model.SpinProgress);
            _spinSource.pitch = Mathf.Lerp(_spinSource.pitch, targetPitch, Time.deltaTime * 5f);

            float targetVolume = _config.BaseSpinVolume * Mathf.Clamp01(_model.SpinProgress);
            _spinSource.volume = Mathf.Lerp(_spinSource.volume, targetVolume, Time.deltaTime * 3f);
        }

        private void UpdateFireAudio()
        {
            if (_fireSource == null || !_model.IsFiring) return;

            _fireSource.pitch = Random.Range(_config.FirePitchMin, _config.FirePitchMax);
            _fireSource.volume = _config.BaseFireVolume + Random.Range(-0.05f, 0.05f);

            if (_fireLowPass != null)
            {
                _fireLowPass.cutoffFrequency = Mathf.Lerp(
                    _fireLowPass.cutoffFrequency,
                    7000f + Random.Range(-500f, 500f),
                    Time.deltaTime * 10f
                );
            }

            if (_fireDistortion != null)
            {
                _fireDistortion.distortionLevel = 0.15f + Random.Range(0f, 0.05f);
            }
        }
    }
}
