using UnityEngine;

public class BonusWeaponBuff : Bonus
{
    private Timer _destroyTimer = null;

    protected void Start()
    {
        _destroyTimer = gameObject.AddComponent<Timer>();
        _destroyTimer.time = 7.0f; //7 Seconds to destroy
        _destroyTimer.EndEvent += Kill;
        _destroyTimer.StartTimer();
    }

    private void Kill()
    {
        Destroy(gameObject);
    }

    protected void OnTriggerEnter2D(Collider2D collider)
    {
        Component[] components = collider.gameObject.GetComponents<Component>();

        foreach (Component component in components)
        {
            switch (component)
            {
                case PlayerController player:
                    //TODO: Sound
                    player.AddWeaponBoost(Random.Range(0.5f, 4.0f));
                    Kill();
                    break;
            }
        }
    }
}
