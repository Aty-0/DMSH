using DMSH.Objects;

using System.Collections;
using System.Collections.Generic;

using UnityEngine;

namespace Scripts.Utils.Pools
{
    public class ProjectilePool : Instance<ProjectilePool>
    {
        private const int PRECACHE_CHUNK_SIZE = 8;

        [SerializeField]
        private int m_precacheCount = 256;

        [SerializeField]
        private GameObject m_projectilePrefab;

        private readonly List<(bool inUse, Bullet go)> _pool = new(128);

        // unity

        private void Awake()
        {
            StartCoroutine(PrecacheCoroutine());
        }

        private IEnumerator PrecacheCoroutine()
        {
            var chunkSize = PRECACHE_CHUNK_SIZE;
            for (var i = 0; i < m_precacheCount; i++)
            {
                var createdGo = Instantiate(m_projectilePrefab);
#if UNITY_EDITOR
                createdGo.transform.SetParent(GameObject.Find("Pools").transform);
                createdGo.name = $"Projectile_{i}";
#endif
                createdGo.SetActive(false);
                _pool.Add((false, createdGo.GetComponent<Bullet>()));

                chunkSize--;
                if (chunkSize < 0)
                {
                    chunkSize = PRECACHE_CHUNK_SIZE;
                    yield return null;
                }
            }
        }

        // public API

        public static Bullet GetOrCreate()
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
            var component = createdGo.GetComponent<Bullet>();
            Get._pool.Add((true, component));
            return component;
        }

        public static bool TryRelease(Bullet go)
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