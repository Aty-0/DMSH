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
        private GameObject m_projectilePrefab;

        private readonly List<(bool inUse, TPoolElement go)> _pool = new(32);

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
                var createdGo = Instantiate(m_projectilePrefab);
#if UNITY_EDITOR
                createdGo.transform.SetParent(GameObject.Find("Pools").transform);
                createdGo.name = $"{TypeName}_{i}";
#endif
                createdGo.SetActive(false);
                _pool.Add((false, createdGo.GetComponent<TPoolElement>()));

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

            var createdGo = Instantiate(Get.m_projectilePrefab);
#if UNITY_EDITOR
            createdGo.transform.SetParent(GameObject.Find("Pools").transform);
            createdGo.name = $"Projectile_{Get._pool.Count}";
#endif
            var component = createdGo.GetComponent<TPoolElement>();
            Get._pool.Add((true, component));
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
    }
}