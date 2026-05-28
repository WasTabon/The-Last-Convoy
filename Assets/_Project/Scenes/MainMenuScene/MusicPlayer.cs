using UnityEngine;

public class MusicPlayer : MonoBehaviour
{
    [Header("Music")]
    [SerializeField] private AudioClip _musicClip;
    [SerializeField] private float _startTime = 0f;
    [SerializeField] private float _volume = 0.5f;
    [SerializeField] private bool _loop = true;
    [SerializeField] private bool _playOnStart = true;

    private AudioSource _audioSource;

    private void Awake()
    {
        SetupAudio();
    }

    private void Start()
    {
        if (_playOnStart)
        {
            Play();
        }
    }

    private void SetupAudio()
    {
        _audioSource = gameObject.AddComponent<AudioSource>();
        _audioSource.clip = _musicClip;
        _audioSource.volume = _volume;
        _audioSource.loop = _loop;
        _audioSource.playOnAwake = false;
        _audioSource.spatialBlend = 0f;
    }

    public void Play()
    {
        if (_audioSource == null || _musicClip == null) return;

        _audioSource.time = Mathf.Clamp(_startTime, 0f, _musicClip.length);
        _audioSource.Play();
    }

    public void Stop()
    {
        if (_audioSource != null)
        {
            _audioSource.Stop();
        }
    }

    public void Pause()
    {
        if (_audioSource != null)
        {
            _audioSource.Pause();
        }
    }

    public void SetTime(float time)
    {
        if (_audioSource != null && _musicClip != null)
        {
            _audioSource.time = Mathf.Clamp(time, 0f, _musicClip.length);
        }
    }
}
