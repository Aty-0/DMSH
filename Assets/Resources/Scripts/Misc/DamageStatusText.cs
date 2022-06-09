using System.Collections;
using UnityEngine;
using TMPro;

public class DamageStatusText : MonoBehaviour
{
    public string text = string.Empty;
    [SerializeField] private TextMeshPro _textTMP;
    [SerializeField] private Coroutine _fadeAnimationCoroutine;
    [SerializeField] private Coroutine _goUpAnimationCoroutine;

    protected void Start()
    {
        _textTMP = gameObject.AddComponent<TextMeshPro>();
        _textTMP.text = text;

        //TODO: Load TMP font asset
        //      Need to load it one time 
        //Font font = Resources.Load<Font>("Fonts/mom");
        //textTMP.font = font;
        _textTMP.fontSize = 12;

        _fadeAnimationCoroutine = StartCoroutine(DamageStatusTextFadeAnimation());
        _goUpAnimationCoroutine = StartCoroutine(DamageStatusTextGoUpAnimation());
    }

    private void OnDestroyDamageText()
    {
        StopCoroutine(_fadeAnimationCoroutine);
        StopCoroutine(_goUpAnimationCoroutine);
        Destroy(gameObject);
    }

    private IEnumerator DamageStatusTextGoUpAnimation()
    {
        Vector2 initPos = _textTMP.transform.position;
        while (initPos.y < initPos.y + 2.0f)
        {
            initPos.y += 0.02f * GlobalSettings.gameActive;
            _textTMP.transform.position = initPos;
            yield return new WaitForSeconds(0.01f);
        }
    }

    private IEnumerator DamageStatusTextFadeAnimation()
    {
        float alpha = 1.0f;
        while (alpha > 0.0f)
        {
            alpha -= 0.01f * GlobalSettings.gameActive;
            _textTMP.color = new Color(1, 1, 1, alpha);
            yield return new WaitForSeconds(0.01f);
        }

        OnDestroyDamageText();
    }
}
