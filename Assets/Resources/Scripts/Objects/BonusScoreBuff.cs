using UnityEngine;

public class BonusScoreBuff : Bonus
{
    private Timer _destroyTimer = null;

    protected void Start()
    {
        _destroyTimer = gameObject.AddComponent<Timer>();
        _destroyTimer.time = 7.0f; //7 Seconds to destroy
        _destroyTimer.EndEvent += Kill;
        _destroyTimer.StartTimer();

        PlayerController player = FindObjectOfType<PlayerController>();
        player.maxScore += 1000;
    }

    private void Kill()
    {
        Destroy(gameObject);
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
                    //TODO: Sound
                    player.Score += 1000;
                    CreateBonusStatusText("+1000");
                    Kill();
                    break;
            }
        }
    }
}
