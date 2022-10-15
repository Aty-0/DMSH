using UnityEngine;

using DMSH.Characters;

using Scripts.Utils.Pools;

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
            BonusStatsTextPool
                .GetOrCreate()
                .SpawnAt(caller.transform.position + new Vector3(0, 0, 1), $"+{m_addScore}");
            
            caller.AudioSource.Play();

            caller.Rigidbody.isKinematic = true;
            caller.Renderer.enabled = false;
            caller.Collider.enabled = false;

            caller.Kill();
        }
    }
}