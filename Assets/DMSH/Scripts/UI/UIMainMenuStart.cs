using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIMainMenuStart : MonoBehaviour
{
    public float speed = 10.0f;
    protected void Start()
    {
        if (GlobalSettings.MainMenuAwakeAnimation)
            StartCoroutine(BasicAnimationsPack.SmoothAwakeText(FindObjectsOfType<Text>(), speed));
        
    }
}
