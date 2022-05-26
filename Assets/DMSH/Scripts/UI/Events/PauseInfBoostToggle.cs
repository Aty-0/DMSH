using UnityEngine;
using UnityEngine.UI;

public class PauseInfBoostToggle : MonoBehaviour
{
    [SerializeField] private PlayerController _controller;
    [SerializeField] private Toggle _toggle;
    private void Start()
    {
        _toggle = GetComponent<Toggle>();
        _toggle.isOn = _controller.CheatInfBoost;
        _toggle.onValueChanged.AddListener(delegate {
            ToggleValueChanged(_toggle);
        });
    }

    public void ToggleValueChanged(Toggle change)
    {
        _controller.CheatInfBoost = _toggle.isOn;
    }
}
