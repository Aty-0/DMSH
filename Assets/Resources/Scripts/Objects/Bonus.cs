using UnityEngine;
using DMSH.Characters;

namespace DMSH.Objects.Bonuses
{
    public class Bonus : MonoBehaviour
    {
        protected void OnTriggerEnter2D(Collider2D collider)
        {
            var player = collider.gameObject.GetComponent<PlayerController>();

            if (player)
                Use(player);
        }

        public virtual void Use(PlayerController player)
        {

        }
    }
}