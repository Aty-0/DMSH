using UnityEngine;

using DMSH.Characters;

namespace DMSH.Objects.Bonuses
{
    [CreateAssetMenu(menuName = "DMSH/Bonus/Weapon")]
    public class BonusWeaponBuff : IBonusBehaviour
    {
        [Header("Weapon Bonus")]
        [SerializeField]
        private float m_addBoostFrom = 0.5f;
        
        [SerializeField]
        private float m_addBoostTo = 4.0f;
        
        public override void Use(Bonus caller)
        {
            if (caller.AudioSource.isPlaying)
                return;
            
            var player = PlayerController.Player;
            Debug.Assert(player);
            caller.AudioSource.Play();

            player.Weapon.AddWeaponBoost(Random.Range(m_addBoostFrom, m_addBoostTo));
            caller.Rigidbody.isKinematic = true;
            caller.Renderer.enabled = false;
            caller.Collider.enabled = false;

            caller.Kill();
        }
    }
}