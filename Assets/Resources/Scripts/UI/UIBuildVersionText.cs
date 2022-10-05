using UnityEngine;
using UnityEngine.UI;

public class UIBuildVersionText : MonoBehaviour
{
    [SerializeField]
    private Text _text;

    protected void Start()
    {
        _text = GetComponent<Text>();
        _text.text = $"{Application.productName} Build:{Application.buildGUID } Version:{Application.version } Unity version:{Application.unityVersion}";
    }
}
