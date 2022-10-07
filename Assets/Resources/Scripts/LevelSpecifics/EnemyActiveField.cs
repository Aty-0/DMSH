using UnityEngine;
using DMSH.Characters;
using DMSH.Misc.Screen;

namespace DMSH.LevelSpecifics
{
    public class EnemyActiveField : MonoBehaviour
    {
        [SerializeField] 
        private PlayerController _playerController;

        [SerializeField] 
        private ResizableGameElements _resizableGameElements;

        protected void Start()
        {
            _playerController = FindObjectOfType<PlayerController>();

            _resizableGameElements = _playerController.resizableGameElements;
            _resizableGameElements.screenHandler.onScreenResolutionChange.Add(OnResolutionChange);
        }

        private void OnResolutionChange()
        {
            gameObject.transform.localScale = new Vector3(_resizableGameElements.resolutionInWorldPoint.x * 2, _resizableGameElements.resolutionInWorldPoint.y * 2, 1);
        }

        protected void OnTriggerEnter2D(Collider2D collider)
        {
            var enemy = collider.GetComponent<Enemy>();

            if (enemy)
            {
                enemy.ignoreHits = false;
            }
        }

        protected void OnTriggerExit2D(Collider2D collider)
        {
            var enemy = collider.GetComponent<Enemy>();

            if (enemy)
            {
                enemy.ignoreHits = true;
            }

        }
    
    }
}