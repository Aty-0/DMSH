using DMSH.Misc;

using Scripts.Utils.Pools;

using System.Collections;

using UnityEngine;

using TMPro;

namespace DMSH.UI
{
    public class UI_BonusStatText : MonoBehaviour
    {
        [SerializeField]
        private TextMeshPro m_textTMP;

        private Coroutine _fadeAnimationCoroutine;
        private Coroutine _goUpAnimationCoroutine;

        // public
        
        public void SpawnAt(Vector3 position, string text)
        {
            transform.position = position;
            
            m_textTMP.text = text;

            if (_fadeAnimationCoroutine != null)
            {
                Debug.LogWarning($"{nameof(_fadeAnimationCoroutine)} already launched! Logic bug!", this);
            }
            
            _fadeAnimationCoroutine = StartCoroutine(DamageStatusTextFadeAnimation());

            if (_goUpAnimationCoroutine != null)
            {
                Debug.LogWarning($"{nameof(_goUpAnimationCoroutine)} already launched! Logic bug!", this);
            }
            
            _goUpAnimationCoroutine = StartCoroutine(DamageStatusTextGoUpAnimation());
        }
        
        // private 

        private void OnDestroyDamageText()
        {
            StopCoroutine(_fadeAnimationCoroutine);
            _fadeAnimationCoroutine = null;
            StopCoroutine(_goUpAnimationCoroutine);
            _goUpAnimationCoroutine = null;

            BonusStatsTextPool.TryRelease(this);
        }

        private IEnumerator DamageStatusTextGoUpAnimation()
        {
            Vector2 initPos = m_textTMP.transform.position;
            while (initPos.y < initPos.y + 2.0f)
            {
                initPos.y += 0.02f * GlobalSettings.GameActiveAsInt;
                m_textTMP.transform.position = initPos;
                yield return new WaitForSeconds(0.01f);
            }
        }

        private IEnumerator DamageStatusTextFadeAnimation()
        {
            float alpha = 1.0f;
            while (alpha > 0.0f)
            {
                alpha -= 0.01f * GlobalSettings.GameActiveAsInt;
                m_textTMP.color = new Color(1, 1, 1, alpha);
                yield return new WaitForSeconds(0.01f);
            }

            OnDestroyDamageText();
        }
    }
}