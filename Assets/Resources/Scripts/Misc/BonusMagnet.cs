using UnityEngine;

using DMSH.Characters;
using DMSH.Objects.Bonuses;

namespace DMSH.Misc
{
    public class BonusMagnet : MonoBehaviour
    {
        protected void OnTriggerStay2D(Collider2D collider)
        {
            Component[] components = collider.gameObject.GetComponents<Component>();

            foreach (Component component in components)
            {
                switch (component)
                {
                    case Bonus bonus:
                        Rigidbody2D r2d = bonus.GetComponent<Rigidbody2D>();
                        r2d.gravityScale = 0;
                        float distance = Vector3.Distance(bonus.transform.position, gameObject.transform.position);
                        float speed = 9.0f * distance * Time.deltaTime;
                        bonus.transform.position = Vector3.MoveTowards(bonus.transform.position, gameObject.transform.position, speed);

                        // FIX ME: Some object in down direction sometime is not captured
                        if (distance <= 0.6f)
                            bonus.Use(gameObject.transform.parent.GetComponent<PlayerController>());


                        break;
                }
            }
        }
    }
}