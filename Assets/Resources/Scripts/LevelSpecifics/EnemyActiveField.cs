using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyActiveField : MonoBehaviour
{
    [SerializeField] private PlayerController _playerController;

    protected void Start()
    {
        _playerController = FindObjectOfType<PlayerController>();
        _playerController.screenHandler.onScreenResolutionChange.Add(OnResolutionChange);
    }

    private void OnResolutionChange()
    {
        Vector3 ViewportToWorldPointX = new Vector2(_playerController.gameCamera.ViewportToWorldPoint(new Vector2(1, 0)).x, 0);
        Vector3 ViewportToWorldPointY = new Vector2(0, _playerController.gameCamera.ViewportToWorldPoint(new Vector2(0, 1)).y);        
        gameObject.transform.localScale = new Vector3(ViewportToWorldPointX.x * 2, ViewportToWorldPointY.y * 2, 1);
    }

    protected void OnTriggerEnter2D(Collider2D collider)
    {
        Component[] components = collider.gameObject.GetComponents<Component>();
        
        foreach (Component component in components)
        {
            switch (component)
            {
                case Enemy enemy:
                    enemy.ignoreHits = false;
                    break;
            }
        }
    }

    protected void OnTriggerExit2D(Collider2D collider)
    {
        Component[] components = collider.gameObject.GetComponents<Component>();
        
        foreach (Component component in components)
        {
            switch (component)
            {
                case Enemy enemy:
                    enemy.ignoreHits = true;
                    break;
            }
        }
    }
}
