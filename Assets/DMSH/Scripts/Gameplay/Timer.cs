using System;
using System.Collections;

using UnityEngine;

using DMSH.Misc;

namespace DMSH.Gameplay
{
    public class Timer : MonoBehaviour
    {
        public bool isEnded
        {
            private set => _isEnded = value;
            get => _isEnded;
        }

        [Header("Time")]
        public float time;

        [Tooltip("In seconds")]
        public float tick = 1.0f;

        [Header("Debug")]
        [SerializeField]
        private bool _isEnded;
        [SerializeField]
        private float _currentTime;

        public event Action StartEvent;
        public event Action UpdateEvent;
        public event Action EndEvent;

        private Coroutine _tickCoroutine;

        protected void Start()
        {
            _currentTime = time;
        }

        public void StartTimer()
        {
            _isEnded = false;
            if (_tickCoroutine != null)
            {
                Debug.LogWarning("Already started! Bug in logic!", this);
                return;
            }

            _tickCoroutine = StartCoroutine(Tick());
        }

        public void StopTimer()
        {
            if (_tickCoroutine != null)
            {
                StopCoroutine(_tickCoroutine);
                _tickCoroutine = null;
            }
        }

        public void EndTimer()
        {
            _isEnded = true;
        }

        public void ResetTimer()
        {
            _isEnded = false;
            _currentTime = time;
        }

        /// <summary>
        /// Clear prev states and launch it as first time
        /// </summary>
        public void RestartTimer()
        {
            if (_tickCoroutine != null)
            {
                StopTimer();
            }

            ResetTimer();
            StartTimer();
        }

        private IEnumerator Tick()
        {
            try
            {
                StartEvent?.Invoke();
                while (_currentTime >= 0)
                {
                    _currentTime -= tick * GlobalSettings.GameActiveAsInt;

                    UpdateEvent?.Invoke();
                    yield return new WaitForSeconds(tick);
                }

                _isEnded = true;
                EndEvent?.Invoke();
            }
            finally
            {
                _isEnded = true;
                _tickCoroutine = null;
            }
        }
    }
}