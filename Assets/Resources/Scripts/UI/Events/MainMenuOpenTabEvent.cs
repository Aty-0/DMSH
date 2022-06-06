using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuOpenTabEvent : MonoBehaviour
{
    public GameObject tab;

    public void OpenTab()
    {
        tab.gameObject.SetActive(true);
        transform.parent.gameObject.SetActive(false);
    }
}
