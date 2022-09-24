using UnityEngine;
using DMSH.Misc;
using DMSH.Characters;

namespace DMSH.Objects.Bonuses
{
    public class BonusWeaponBuff : Bonus
    {        
        protected void Start()
        {
            InitializeBasicBonusComponents();
        }

        public override void Use(PlayerController player)
        {
            if (!audioSource.isPlaying)
            {
                Debug.Assert(player);
                audioSource.Play();
                Destroy(gameObject, audioSource.clip.length);
                player.AddWeaponBoost(Random.Range(0.5f, 4.0f));
                gameObject.GetComponent<Rigidbody2D>().isKinematic = true;
                gameObject.GetComponent<SpriteRenderer>().enabled = false;
                gameObject.GetComponent<Collider2D>().enabled = false;
            }
        }
    }
}