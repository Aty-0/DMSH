using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FullscreenToggle : MonoBehaviour
{
    [SerializeField] private Toggle _toggle;

    protected void Start()
    {
        _toggle = GetComponent<Toggle>();
        _toggle.isOn = Screen.fullScreen;
        _toggle.onValueChanged.AddListener(delegate {
            ToggleValueChanged(_toggle);
        });
    }

    private void ToggleValueChanged(Toggle toggle)
    {
        Screen.fullScreen = _toggle.isOn;
    }
}
