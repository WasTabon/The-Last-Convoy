using UnityEngine;
using System.Collections.Generic;

public class BulletImpactPool : MonoBehaviour
{
    [Header("Impact Effect Settings")]
    [SerializeField] private ParticleSystem impactEffectPrefab;
    [SerializeField] private int poolSize = 100;
    [SerializeField] private float effectDuration = 1.5f;
    
    private Queue<ParticleSystem> availableEffects = new Queue<ParticleSystem>();
    private List<ActiveEffect> activeEffects = new List<ActiveEffect>();
    
    private Transform poolContainer;
    
    private class ActiveEffect
    {
        public ParticleSystem effect;
        public float deactivateTime;
        
        public ActiveEffect(ParticleSystem effect, float deactivateTime)
        {
            this.effect = effect;
            this.deactivateTime = deactivateTime;
        }
    }
    
    void Awake()
    {
        InitializePool();
    }
    
    void Update()
    {
        UpdateActiveEffects();
    }
    
    void InitializePool()
    {
        // Create container for pool objects
        poolContainer = new GameObject("ImpactEffectsPool").transform;
        poolContainer.SetParent(transform);
        poolContainer.localPosition = Vector3.zero;
        
        if (impactEffectPrefab == null)
        {
            Debug.LogError("Impact Effect Prefab is not assigned!");
            return;
        }
        
        // Pre-instantiate pool objects
        for (int i = 0; i < poolSize; i++)
        {
            ParticleSystem effect = Instantiate(impactEffectPrefab, poolContainer);
            effect.gameObject.name = $"ImpactEffect_{i}";
            effect.gameObject.SetActive(false);
            availableEffects.Enqueue(effect);
        }
        
        Debug.Log($"Bullet Impact Pool initialized with {poolSize} effects.");
    }
    
    public void PlayImpactEffect(Vector3 position, Vector3 normal)
    {
        if (availableEffects.Count == 0)
        {
            // Pool is empty, reuse oldest active effect
            RecycleOldestEffect();
        }
        
        if (availableEffects.Count > 0)
        {
            ParticleSystem effect = availableEffects.Dequeue();
            
            // Position and orient the effect
            effect.transform.position = position;
            effect.transform.rotation = Quaternion.LookRotation(normal);
            
            // Activate and play
            effect.gameObject.SetActive(true);
            effect.Play();
            
            // Add to active effects list
            activeEffects.Add(new ActiveEffect(effect, Time.time + effectDuration));
        }
    }
    
    public void PlayImpactEffect(Vector3 position, Quaternion rotation)
    {
        if (availableEffects.Count == 0)
        {
            RecycleOldestEffect();
        }
        
        if (availableEffects.Count > 0)
        {
            ParticleSystem effect = availableEffects.Dequeue();
            
            effect.transform.position = position;
            effect.transform.rotation = rotation;
            
            effect.gameObject.SetActive(true);
            effect.Play();
            
            activeEffects.Add(new ActiveEffect(effect, Time.time + effectDuration));
        }
    }
    
    void UpdateActiveEffects()
    {
        float currentTime = Time.time;
        
        // Check which effects should be deactivated
        for (int i = activeEffects.Count - 1; i >= 0; i--)
        {
            if (currentTime >= activeEffects[i].deactivateTime)
            {
                DeactivateEffect(i);
            }
        }
    }
    
    void DeactivateEffect(int index)
    {
        ParticleSystem effect = activeEffects[index].effect;
        
        // Stop and deactivate
        effect.Stop();
        effect.gameObject.SetActive(false);
        
        // Return to pool
        availableEffects.Enqueue(effect);
        
        // Remove from active list
        activeEffects.RemoveAt(index);
    }
    
    void RecycleOldestEffect()
    {
        if (activeEffects.Count > 0)
        {
            // Force deactivate the oldest effect
            DeactivateEffect(0);
        }
    }
    
    public int GetAvailableCount()
    {
        return availableEffects.Count;
    }
    
    public int GetActiveCount()
    {
        return activeEffects.Count;
    }
    
    void OnDestroy()
    {
        // Clean up
        availableEffects.Clear();
        activeEffects.Clear();
    }
}