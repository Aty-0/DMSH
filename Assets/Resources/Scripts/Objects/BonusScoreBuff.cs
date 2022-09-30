using UnityEngine;
using DMSH.Misc;
using DMSH.Characters;

namespace DMSH.Objects.Bonuses
{
    public class BonusScoreBuff : Bonus
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
                player.Score += 1000;
                CreateBonusStatusText("+1000");
                Destroy(gameObject, audioSource.clip.length);
                audioSource.Play();
                gameObject.GetComponent<Rigidbody2D>().isKinematic = true;
                gameObject.GetComponent<SpriteRenderer>().enabled = false;
                gameObject.GetComponent<Collider2D>().enabled = false;
            }
        }

        public void CreateBonusStatusText(string text)
        {
            GameObject textGO = new GameObject();
            textGO.name = $"BonusStatusText{textGO.GetInstanceID()}";
            textGO.transform.position = transform.position + new Vector3(0, 0, 1);
            textGO.transform.localScale = new Vector3(0.25f, 0.25f, 1);
            DamageStatusText dst = textGO.AddComponent<DamageStatusText>();
            dst.text = text;
            dst.fontSize = 6.0f;
        }
    }
}