using UnityEngine;

using DMSH.Characters;
using DMSH.Misc.Animated;

namespace DMSH.Objects.Bonuses
{
    [CreateAssetMenu(menuName = "DMSH/Bonus/Score")]
    public class BonusScoreBuff : IBonusBehaviour
    {
        [Header("Score Bonus")]
        [SerializeField]
        private int m_addScore = 1000;

        public override void Use(Bonus caller)
        {
            if (caller.AudioSource.isPlaying)
                return;

            var player = PlayerController.Player;
            Debug.Assert(player);

            player.Score += m_addScore;
            CreateBonusStatusText(caller, $"+{m_addScore}");
            caller.AudioSource.Play();

            caller.Rigidbody.isKinematic = true;
            caller.Renderer.enabled = false;
            caller.Collider.enabled = false;

            caller.Kill();
        }

        private static void CreateBonusStatusText(Bonus caller, string text)
        {
            GameObject textGO = new GameObject();
            textGO.name = $"BonusStatusText{textGO.GetInstanceID()}";
            textGO.transform.position = caller.transform.position + new Vector3(0, 0, 1);
            textGO.transform.localScale = new Vector3(0.25f, 0.25f, 1);
            DamageStatusText dst = textGO.AddComponent<DamageStatusText>();
            dst.text = text;
            dst.fontSize = 6.0f;
        }
    }
}