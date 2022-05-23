using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIMainMenuStart : MonoBehaviour
{
    public float speed = 10.0f; //11
    protected void Start()
    {
        if (GlobalSettings.mainMenuAwakeAnimation)
            foreach(Text text in FindObjectsOfType<Text>())
                StartCoroutine(BasicAnimationsPack.SmoothAwakeText(text, speed));      
    }
}
