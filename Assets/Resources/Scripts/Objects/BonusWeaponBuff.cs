using UnityEngine;

using DMSH.Misc;
using DMSH.Characters;

namespace DMSH.Objects.Bonuses
{
    public class BonusWeaponBuff : Bonus
    {
        protected override void Use(PlayerController player)
        {
            if (!AudioSource.isPlaying)
            {
                Debug.Assert(player);
                AudioSource.Play();

                player.weapon.AddWeaponBoost(Random.Range(0.5f, 4.0f));
                Rigidbody.isKinematic = true;
                Renderer.enabled = false;
                Collider.enabled = false;

                Kill();
            }
        }
    }
}