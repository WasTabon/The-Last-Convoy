using UnityEngine;
using System.Collections.Generic;

public class TimedParticleAudioPlayer : MonoBehaviour
{
    [Header("Particle System")]
    [SerializeField] private Transform particleParent;
    [SerializeField] private bool playOnStart = true;
    
    [Header("Timing Settings")]
    [SerializeField] private float minInterval = 2f;
    [SerializeField] private float maxInterval = 5f;
    [SerializeField] private bool randomizeInterval = true;
    
    [Header("Audio Settings")]
    [SerializeField] private AudioClip[] soundClips;
    [SerializeField] private float volume = 1f;
    [SerializeField] private float minPitch = 0.9f;
    [SerializeField] private float maxPitch = 1.1f;
    [SerializeField] private bool randomizePitch = true;
    
    [Header("Audio Source Settings")]
    [SerializeField] private float spatialBlend = 1f;
    [SerializeField] private float maxDistance = 50f;
    [SerializeField] private float minDistance = 1f;
    
    private List<ParticleSystem> particleSystems = new List<ParticleSystem>();
    private AudioSource audioSource;
    private float nextPlayTime = 0f;
    private bool isPlaying = false;

    void Awake()
    {
        SetupAudioSource();
        CollectParticleSystems();
    }

    void Start()
    {
        if (playOnStart)
        {
            StartPlaying();
        }
    }

    void Update()
    {
        if (isPlaying && Time.time >= nextPlayTime)
        {
            PlayEffect();
            ScheduleNextPlay();
        }
    }

    void SetupAudioSource()
    {
        GameObject audioObj = new GameObject("AudioSource");
        audioObj.transform.SetParent(transform);
        audioObj.transform.localPosition = Vector3.zero;
        
        audioSource = audioObj.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;
        audioSource.loop = false;
    }

    void CollectParticleSystems()
    {
        if (particleParent == null)
        {
            Debug.LogWarning("Particle Parent is not assigned!");
            return;
        }
        
        ParticleSystem[] systems = particleParent.GetComponentsInChildren<ParticleSystem>(true);
        particleSystems.AddRange(systems);
        
        foreach (ParticleSystem ps in particleSystems)
        {
            ps.gameObject.SetActive(false);
        }
        
        Debug.Log($"Found {particleSystems.Count} particle systems in children.");
    }

    void PlayEffect()
    {
        PlayRandomParticle();
        PlayRandomSound();
    }

    void PlayRandomParticle()
    {
        if (particleSystems.Count == 0) return;
        
        ParticleSystem randomPS = particleSystems[Random.Range(0, particleSystems.Count)];
        
        if (randomPS != null)
        {
            randomPS.gameObject.SetActive(true);
            randomPS.Play();
            
            float duration = randomPS.main.duration + randomPS.main.startLifetime.constantMax;
            StartCoroutine(DeactivateParticleAfterDelay(randomPS, duration));
        }
    }

    void PlayRandomSound()
    {
        if (soundClips == null || soundClips.Length == 0 || audioSource == null) return;
        
        AudioClip randomClip = soundClips[Random.Range(0, soundClips.Length)];
        
        if (randomClip != null)
        {
            if (randomizePitch)
            {
                audioSource.pitch = Random.Range(minPitch, maxPitch);
            }
            else
            {
                audioSource.pitch = 1f;
            }
            
            audioSource.PlayOneShot(randomClip, volume);
        }
    }

    System.Collections.IEnumerator DeactivateParticleAfterDelay(ParticleSystem ps, float delay)
    {
        yield return new WaitForSeconds(delay);
        
        if (ps != null)
        {
            ps.Stop();
            ps.gameObject.SetActive(false);
        }
    }

    void ScheduleNextPlay()
    {
        float interval = randomizeInterval ? 
            Random.Range(minInterval, maxInterval) : 
            minInterval;
        
        nextPlayTime = Time.time + interval;
    }

    public void StartPlaying()
    {
        isPlaying = true;
        ScheduleNextPlay();
    }

    public void StopPlaying()
    {
        isPlaying = false;
    }

    public void PlayImmediately()
    {
        PlayEffect();
        ScheduleNextPlay();
    }

    public void SetInterval(float min, float max)
    {
        minInterval = Mathf.Max(0.1f, min);
        maxInterval = Mathf.Max(minInterval, max);
    }

    public void SetVolume(float newVolume)
    {
        volume = Mathf.Clamp01(newVolume);
    }

    public void SetPitchRange(float min, float max)
    {
        minPitch = Mathf.Clamp(min, 0.1f, 3f);
        maxPitch = Mathf.Clamp(max, minPitch, 3f);
    }

    public bool IsPlaying()
    {
        return isPlaying;
    }

    void OnValidate()
    {
        minInterval = Mathf.Max(0.1f, minInterval);
        maxInterval = Mathf.Max(minInterval, maxInterval);
        volume = Mathf.Clamp01(volume);
        minPitch = Mathf.Clamp(minPitch, 0.1f, 3f);
        maxPitch = Mathf.Clamp(maxPitch, minPitch, 3f);
    }

    void OnDestroy()
    {
        StopAllCoroutines();
    }
}