using UnityEngine;

using DMSH.Characters;
using DMSH.Objects.Bonuses;

namespace DMSH.Gameplay
{
    public class BonusMagnet : MonoBehaviour
    {
        protected void OnTriggerStay2D(Collider2D collider)
        {
            var bonus = collider.gameObject.GetComponent<Bonus>();

            if (bonus != null)
            {
                var r2d = bonus.GetComponent<Rigidbody2D>();
                r2d.gravityScale = 0;
                var speed = 9.0f * Time.deltaTime;
                bonus.transform.position = Vector3.MoveTowards(bonus.transform.position, gameObject.transform.position, speed);

                var distance = Vector3.Distance(bonus.transform.position, gameObject.transform.position);
                if (distance <= 0.6f)
                {
                    bonus.Use(gameObject.transform.parent.GetComponent<PlayerController>());
                }
            }
        }
    }
}