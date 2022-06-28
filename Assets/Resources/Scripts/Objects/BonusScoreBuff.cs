using UnityEngine;

public class BonusScoreBuff : Bonus
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

        PlayerController player = FindObjectOfType<PlayerController>();
        player.maxScore += 1000;
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
            player.Score += 1000;
            CreateBonusStatusText("+1000");
            Destroy(gameObject, _audioSource.clip.length);
            _audioSource.Play();
            gameObject.GetComponent<Rigidbody2D>().isKinematic = true;
            gameObject.GetComponent<SpriteRenderer>().enabled = false;
            gameObject.GetComponent<Collider2D>().enabled = false;
        }
    }

    public void CreateBonusStatusText(string text)
    {
        GameObject textGO = new GameObject();
        textGO.name = $"BonusStatusText{textGO.GetInstanceID()}";
        textGO.transform.position = transform.position + new Vector3(0,0,1);
        textGO.transform.localScale = new Vector3(0.25f, 0.25f, 1);
        DamageStatusText dst = textGO.AddComponent<DamageStatusText>();
        dst.text = text;        
        dst.fontSize = 6.0f;        
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
