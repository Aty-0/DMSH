using TMPro;

using UnityEngine;

[RequireComponent(typeof(TMP_Text))]
public class UIBuildVersionText : MonoBehaviour
{
    protected void Start()
    {
        GetComponent<TMP_Text>().text = $"{Application.productName} Build:{Application.buildGUID} Version:{Application.version} Unity version:{Application.unityVersion}";
    }
}