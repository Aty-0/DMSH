using System;
using System.Collections.Generic;

using UnityEngine;

namespace Scripts.Utils.Pools
{
    public class GenericReusableList<T>
    {
        private readonly int _initialListElementsCount;
        private static GenericReusableList<T> _pool;
        private readonly List<(bool isUsed, PooledList list)> _cache;

        private GenericReusableList(int cacheSize, int initialListElementsCount)
        {
            _initialListElementsCount = initialListElementsCount;
            _cache = new List<(bool, PooledList)>(cacheSize);
            for (int i = 0; i < _cache.Count; i++)
            {
                _cache.Add((false, new PooledList(initialListElementsCount)));
            }
        }

        public static void CreatePool(int initialSize, int initialListElementsCount = 4)
        {
            if (_pool != null)
            {
                Debug.LogWarning($"Pool already exists! skip");
                return;
            }

            _pool = new GenericReusableList<T>(initialSize, initialListElementsCount);
        }

        public static PooledList GetOrCreateList()
        {
#if UNITY_EDITOR
            if (_pool == null)
            {
                UnityEngine.Debug.LogError($"You've missed {nameof(CreatePool)} method! Call it before using this method!");
            }
#endif

            for (var i = 0; i < _pool._cache.Count; i++)
            {
                var data = _pool._cache[i];
                if (!data.isUsed)
                {
                    data.isUsed = true;
                    _pool._cache[i] = data;
                    return data.list;
                }
            }

            var newList = new PooledList(_pool._initialListElementsCount);
            _pool._cache.Add((true, newList));

            return newList;
        }

        public class PooledList : List<T>, IDisposable
        {
            public PooledList(int initialSize) : base(initialSize)
            {
            }

            public void Dispose()
            {
                for (var i = 0; i < _pool._cache.Count; i++)
                {
                    var data = _pool._cache[i];
                    if (data.list == this)
                    {
                        data.isUsed = false;
                        _pool._cache[i] = data;
                        Clear();
                        return;
                    }
                }
            }
        }
    }
}