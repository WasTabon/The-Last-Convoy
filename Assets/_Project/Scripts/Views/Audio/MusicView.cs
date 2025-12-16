using UnityEngine;
using Zenject;
using LastConvoy.Configs;

namespace LastConvoy.Views.Audio
{
    public class MusicView : MonoBehaviour
    {
        private MusicConfig _config;

        private AudioSource _musicSource;
        private AudioLowPassFilter _musicLowPass;
        private AudioHighPassFilter _musicHighPass;
        private AudioReverbFilter _musicReverb;
        private AudioEchoFilter _musicEcho;
        private AudioChorusFilter _musicChorus;

        [Inject]
        public void Construct(MusicConfig config)
        {
            _config = config;
        }

        private void Start()
        {
            SetupAudioSource();
            StartMusic();
        }

        private void Update()
        {
            UpdateFilters();
        }

        private void SetupAudioSource()
        {
            GameObject audioObj = new GameObject("Music_Track");
            audioObj.transform.SetParent(transform);
            audioObj.transform.localPosition = Vector3.zero;

            _musicSource = audioObj.AddComponent<AudioSource>();
            _musicSource.playOnAwake = false;
            _musicSource.loop = true;
            _musicSource.spatialBlend = 0f;
            _musicSource.clip = _config.MusicTrack;
            _musicSource.volume = _config.MusicVolume * _config.MasterVolume;
            _musicSource.priority = 64;

            ConfigureMusicFilters();
        }

        private void ConfigureMusicFilters()
        {
            if (_musicSource == null) return;

            _musicLowPass = _musicSource.gameObject.AddComponent<AudioLowPassFilter>();
            _musicLowPass.cutoffFrequency = 5000f;
            _musicLowPass.lowpassResonanceQ = 1.0f;

            _musicHighPass = _musicSource.gameObject.AddComponent<AudioHighPassFilter>();
            _musicHighPass.cutoffFrequency = 200f;
            _musicHighPass.highpassResonanceQ = 1.0f;

            _musicReverb = _musicSource.gameObject.AddComponent<AudioReverbFilter>();
            _musicReverb.reverbPreset = AudioReverbPreset.Room;
            _musicReverb.dryLevel = -200f;
            _musicReverb.room = -600f;
            _musicReverb.roomHF = -800f;
            _musicReverb.decayTime = 0.6f;
            _musicReverb.decayHFRatio = 0.5f;
            _musicReverb.reflectionsLevel = -1200;
            _musicReverb.reflectionsDelay = 0.01f;
            _musicReverb.reverbLevel = -800;
            _musicReverb.reverbDelay = 0.02f;
            _musicReverb.diffusion = 70f;
            _musicReverb.density = 60f;

            _musicEcho = _musicSource.gameObject.AddComponent<AudioEchoFilter>();
            _musicEcho.delay = 50f;
            _musicEcho.decayRatio = 0.3f;
            _musicEcho.wetMix = 0.15f;
            _musicEcho.dryMix = 0.85f;

            _musicChorus = _musicSource.gameObject.AddComponent<AudioChorusFilter>();
            _musicChorus.dryMix = 0.85f;
            _musicChorus.wetMix1 = 0.35f;
            _musicChorus.wetMix2 = 0.25f;
            _musicChorus.wetMix3 = 0.15f;
            _musicChorus.delay = 25f;
            _musicChorus.rate = 0.5f;
            _musicChorus.depth = 0.06f;
        }

        private void StartMusic()
        {
            if (_musicSource != null && _config.MusicTrack != null)
            {
                _musicSource.Play();
            }
        }

        private void UpdateFilters()
        {
            if (!_config.EnableInteriorEffect) return;

            float intensity = _config.InteriorEffectStrength;

            if (_musicLowPass != null)
            {
                float targetFreq = Mathf.Lerp(10000f, 4000f, intensity);
                _musicLowPass.cutoffFrequency = Mathf.Lerp(
                    _musicLowPass.cutoffFrequency,
                    targetFreq,
                    Time.deltaTime * 2f
                );
            }

            if (_musicReverb != null)
            {
                float targetRoom = Mathf.Lerp(-1000f, -600f, intensity);
                _musicReverb.room = Mathf.Lerp(
                    _musicReverb.room,
                    targetRoom,
                    Time.deltaTime * 2f
                );
            }

            if (_musicEcho != null)
            {
                float targetWetMix = Mathf.Lerp(0.05f, 0.15f, intensity);
                _musicEcho.wetMix = Mathf.Lerp(
                    _musicEcho.wetMix,
                    targetWetMix,
                    Time.deltaTime * 2f
                );
            }
        }

        public void SetMasterVolume(float volume)
        {
            if (_musicSource != null)
            {
                _musicSource.volume = _config.MusicVolume * Mathf.Clamp01(volume);
            }
        }

        public void PauseMusic()
        {
            if (_musicSource != null && _musicSource.isPlaying)
            {
                _musicSource.Pause();
            }
        }

        public void ResumeMusic()
        {
            if (_musicSource != null && !_musicSource.isPlaying)
            {
                _musicSource.UnPause();
            }
        }

        public void StopMusic()
        {
            if (_musicSource != null)
            {
                _musicSource.Stop();
            }
        }
    }
}
