using UnityEngine;
using DMSH.Characters;
using DMSH.Misc;

namespace DMSH.Objects.Bonuses
{
    public class Bonus : MonoBehaviour
    {
        public Timer destroyTimer = null;
        public AudioSource audioSource = null;


        protected void OnTriggerEnter2D(Collider2D collider)
        {
            var player = collider.gameObject.GetComponent<PlayerController>();

            if (player)
                Use(player);
        }

        public virtual void Use(PlayerController player)
        {

        }

        public virtual void Kill()
        {
            Destroy(gameObject);
        }

        protected void InitializeBasicBonusComponents()
        {
            destroyTimer = gameObject.AddComponent<Timer>();
            destroyTimer.time = 7.0f; // 7 Seconds to destroy
            destroyTimer.EndEvent += Kill;
            destroyTimer.StartTimer();

            audioSource = gameObject.GetComponent<AudioSource>();
        }
    }
}