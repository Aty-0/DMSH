using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenHandler : MonoBehaviour
{
    public List<Action> onScreenResolutionChange = new List<Action>();
    [SerializeField] private int _lastScreenWidth = 0;
    [SerializeField] private int _lastScreenHeight = 0;

    protected void Update()
    {
        if (_lastScreenWidth != Screen.width || _lastScreenHeight != Screen.height)
            foreach (Action action in onScreenResolutionChange)
                action?.Invoke();
    }
}
