using UnityEngine;

public class BonusWeaponBuff : Bonus
{
    private Timer _destroyTimer = null;
    private AudioSource _audioSource = null;

    protected void Start()
    {
        _destroyTimer = gameObject.AddComponent<Timer>();
        _destroyTimer.time = 7.0f; //7 Seconds to destroy
        _destroyTimer.EndEvent += Kill;
        _destroyTimer.StartTimer();

        _audioSource = gameObject.GetComponent<AudioSource>();
    }
    private void Kill()
    {
        Destroy(gameObject);
    }

    public override void Use(PlayerController player)
    {
        if (!_audioSource.isPlaying)
        {
            Debug.Assert(player);
            _audioSource.Play();
            Destroy(gameObject, _audioSource.clip.length);
            player.AddWeaponBoost(Random.Range(0.5f, 4.0f));
            gameObject.GetComponent<Rigidbody2D>().isKinematic = true;
            gameObject.GetComponent<SpriteRenderer>().enabled = false;
            gameObject.GetComponent<Collider2D>().enabled = false;
        }
    }

    protected void OnTriggerEnter2D(Collider2D collider)
    {
        Component[] components = collider.gameObject.GetComponents<Component>();

        foreach (Component component in components)
        {
            switch (component)
            {
                case PlayerController player:
                    Use(player);
                    break;
            }
        }
    }
}
