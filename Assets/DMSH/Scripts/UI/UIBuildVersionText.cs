using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Text))]
public class UIBuildVersionText : MonoBehaviour
{
    protected void Start()
    {
        GetComponent<Text>().text = $"{Application.productName} Build:{Application.buildGUID} Version:{Application.version} Unity version:{Application.unityVersion}";
    }
}