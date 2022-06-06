using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonBackToGame : MonoBehaviour
{
    [SerializeField] private PlayerController _controller;
    public void ButtonBackToGameEvent()
    {
        _controller?.ShowPauseScreen();
    }
}
