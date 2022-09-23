using System;
using System.Collections;
using UnityEngine;

namespace DMSH.Misc
{
    public class Timer : MonoBehaviour
    {
        public bool isEnded
        {
            private set => _isEnded = value;
            get => _isEnded;
        }

        [Header("Time")]
        public float time = 0.0f;

        [Tooltip("In seconds")]
        public float tick = 1.0f;

        [Header("Debug")]
        [SerializeField] private bool _isEnded = false;
        [SerializeField] private float _currentTime = 0.0f;

        public event Action StartEvent;
        public event Action UpdateEvent;
        public event Action EndEvent;

        private Coroutine _tickCoroutine = null;

        protected void Start()
        {
            _currentTime = time;
        }

        public void StartTimer()
        {
            _isEnded = false;
            _tickCoroutine = StartCoroutine(Tick());
        }

        public void StopTimer()
        {
            StopCoroutine(_tickCoroutine);
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

        private IEnumerator Tick()
        {
            StartEvent?.Invoke();
            while (_currentTime >= 0)
            {
                _currentTime -= tick * GlobalSettings.gameActiveAsInt;

                UpdateEvent?.Invoke();
                yield return new WaitForSeconds(tick);
            }
            _isEnded = true;
            EndEvent?.Invoke();
        }
    }
}