using UnityEngine;
using UnityEngine.UI;

using DMSH.Objects;

namespace DMSH.Characters
{
    public class Boss : Enemy
    {
        [Header("Boss")]
        public string bossName = "NoName";

        [Header("UI")]
        [SerializeField]
        private GameObject _bossUI;
        [SerializeField]
        private Text _bossName;
        [SerializeField] 
        private Text _bossLifes;
        [SerializeField] 
        private Image _bossHealthBar;

        protected override void EnemyStart()
        {
            _bossUI.SetActive(!_bossUI.activeSelf);
            _bossLifes.text = $"Lifes:{_lifes}";
            _bossName.text = bossName;

            Debug.Assert(_bossUI.activeSelf == true, "Boss UI is not active");
        }

        public override void OnDamage()
        {
            _bossLifes.text = $"Lifes:{_lifes}";
            _bossHealthBar.fillAmount = _health / 100;
            _playerController.Score += 1000;
        }

        public override void OnDie()
        {
            // Add score
            _playerController.Score += 10000;

            // Clear scene from bullets
            foreach (Bullet bullet in FindObjectsOfType<Bullet>())
            {
                if (bullet.isEnemyBullet &&
                    bullet.collisionDestroyBullet &&
                    bullet.pathSystem == null)
                {
                    bullet.Unspawn();
                }
            }
        }

        public override void OnDieCompletely()
        {
            _bossUI.SetActive(false);
        }
    }
}