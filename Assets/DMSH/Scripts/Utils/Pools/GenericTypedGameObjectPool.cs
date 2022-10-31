using System.Collections;
using System.Collections.Generic;

using UnityEngine;

namespace Scripts.Utils.Pools
{
    public abstract class GenericTypedGameObjectPool<TPoolElement, TPool> : Instance<TPool>
        where TPool : GenericTypedGameObjectPool<TPoolElement, TPool>
        where TPoolElement : Component
    {
        private const int PRECACHE_CHUNK_SIZE = 8;
        private static readonly string TypeName = typeof(TPoolElement).Name;

        public virtual int PrecacheCount => 32;

        [SerializeField]
        private GameObject m_pooledPrefab;

        [SerializeField]
        private Transform m_parentTo;

        private readonly List<(bool inUse, TPoolElement go)> _pool = new(32);
        private readonly Dictionary<GameObject, TPoolElement> _goHashmap = new(32);

        // unity

        private void Awake()
        {
            StartCoroutine(PrecacheCoroutine());
        }

        private IEnumerator PrecacheCoroutine()
        {
            var chunkSize = PRECACHE_CHUNK_SIZE;
            for (var i = 0; i < PrecacheCount; i++)
            {
                var createdGo = Instantiate(m_pooledPrefab);
#if UNITY_EDITOR
                createdGo.transform.SetParent(
                    m_parentTo == null
                        ? GameObject.Find("Pools").transform
                        : m_parentTo);

                createdGo.name = $"{TypeName}_{i}";
#endif
                createdGo.SetActive(false);
                var poolElement = createdGo.GetComponent<TPoolElement>();
                _pool.Add((false, poolElement));
                _goHashmap.Add(createdGo, poolElement);

                chunkSize--;
                if (chunkSize < 0)
                {
                    chunkSize = PRECACHE_CHUNK_SIZE;
                    yield return null;
                }
            }
        }

        // public API

        public static TPoolElement GetOrCreate()
        {
            for (var i = 0; i < Get._pool.Count; i++)
            {
                var data = Get._pool[i];
                if (!data.inUse)
                {
                    data.inUse = true;
                    Get._pool[i] = data;
                    data.go.gameObject.SetActive(true);
                    return data.go;
                }
            }

            var createdGo = Instantiate(Get.m_pooledPrefab);
#if UNITY_EDITOR
            createdGo.transform.SetParent(
                Get.m_parentTo == null
                    ? GameObject.Find("Pools").transform
                    : Get.m_parentTo);
            createdGo.name = $"Projectile_{Get._pool.Count}";
#endif
            var component = createdGo.GetComponent<TPoolElement>();
            Get._pool.Add((true, component));
            Get._goHashmap.Add(createdGo, component);
            return component;
        }

        public static bool TryRelease(TPoolElement go)
        {
            var pool = Get._pool;
            for (var i = 0; i < pool.Count; i++)
            {
                var dataTuple = pool[i];
                if (dataTuple.go == go)
                {
                    dataTuple.go.gameObject.SetActive(false);
                    dataTuple.inUse = false;
                    pool[i] = dataTuple;
                    return true;
                }
            }

            return false;
        }

        public static bool TryGetByGo(GameObject key, out TPoolElement element) =>
            Get._goHashmap.TryGetValue(key, out element);
    }
}