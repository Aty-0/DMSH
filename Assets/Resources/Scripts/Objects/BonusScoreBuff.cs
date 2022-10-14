using UnityEngine;
using DMSH.Characters;
using DMSH.Misc.Animated;

namespace DMSH.Objects.Bonuses
{
    public class BonusScoreBuff : Bonus
    {
        protected override void Use(PlayerController player)
        {
            if (!AudioSource.isPlaying)
            {
                Debug.Assert(player);
                player.Score += 1000;
                CreateBonusStatusText("+1000");
                AudioSource.Play();

                Rigidbody.isKinematic = true;
                Renderer.enabled = false;
                Collider.enabled = false;
                
                Kill();
            }
        }

        private void CreateBonusStatusText(string text)
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