using UnityEngine;
using System.Collections.Generic;

namespace LastConvoy.Views.Effects
{
    public class BulletImpactPool : MonoBehaviour
    {
        [SerializeField] private ParticleSystem _impactEffectPrefab;
        [SerializeField] private int _poolSize = 100;
        [SerializeField] private float _effectDuration = 1.5f;

        private readonly Queue<ParticleSystem> _availableEffects = new Queue<ParticleSystem>();
        private readonly List<ActiveEffect> _activeEffects = new List<ActiveEffect>();
        private Transform _poolContainer;

        private class ActiveEffect
        {
            public ParticleSystem Effect;
            public float DeactivateTime;

            public ActiveEffect(ParticleSystem effect, float deactivateTime)
            {
                Effect = effect;
                DeactivateTime = deactivateTime;
            }
        }

        private void Awake()
        {
            InitializePool();
        }

        private void Update()
        {
            UpdateActiveEffects();
        }

        private void InitializePool()
        {
            _poolContainer = new GameObject("ImpactEffectsPool").transform;
            _poolContainer.SetParent(transform);
            _poolContainer.localPosition = Vector3.zero;

            if (_impactEffectPrefab == null)
            {
                Debug.LogError("Impact Effect Prefab is not assigned!");
                return;
            }

            for (int i = 0; i < _poolSize; i++)
            {
                ParticleSystem effect = Instantiate(_impactEffectPrefab, _poolContainer);
                effect.gameObject.name = $"ImpactEffect_{i}";
                effect.gameObject.SetActive(false);
                _availableEffects.Enqueue(effect);
            }
        }

        public void PlayImpactEffect(Vector3 position, Vector3 normal)
        {
            if (_availableEffects.Count == 0)
            {
                RecycleOldestEffect();
            }

            if (_availableEffects.Count > 0)
            {
                ParticleSystem effect = _availableEffects.Dequeue();

                effect.transform.position = position;
                effect.transform.rotation = Quaternion.LookRotation(normal);

                effect.gameObject.SetActive(true);
                effect.Play();

                _activeEffects.Add(new ActiveEffect(effect, Time.time + _effectDuration));
            }
        }

        public void PlayImpactEffect(Vector3 position, Quaternion rotation)
        {
            if (_availableEffects.Count == 0)
            {
                RecycleOldestEffect();
            }

            if (_availableEffects.Count > 0)
            {
                ParticleSystem effect = _availableEffects.Dequeue();

                effect.transform.position = position;
                effect.transform.rotation = rotation;

                effect.gameObject.SetActive(true);
                effect.Play();

                _activeEffects.Add(new ActiveEffect(effect, Time.time + _effectDuration));
            }
        }

        private void UpdateActiveEffects()
        {
            float currentTime = Time.time;

            for (int i = _activeEffects.Count - 1; i >= 0; i--)
            {
                if (currentTime >= _activeEffects[i].DeactivateTime)
                {
                    DeactivateEffect(i);
                }
            }
        }

        private void DeactivateEffect(int index)
        {
            ParticleSystem effect = _activeEffects[index].Effect;

            effect.Stop();
            effect.gameObject.SetActive(false);

            _availableEffects.Enqueue(effect);
            _activeEffects.RemoveAt(index);
        }

        private void RecycleOldestEffect()
        {
            if (_activeEffects.Count > 0)
            {
                DeactivateEffect(0);
            }
        }

        public int GetAvailableCount()
        {
            return _availableEffects.Count;
        }

        public int GetActiveCount()
        {
            return _activeEffects.Count;
        }

        private void OnDestroy()
        {
            _availableEffects.Clear();
            _activeEffects.Clear();
        }
    }
}
