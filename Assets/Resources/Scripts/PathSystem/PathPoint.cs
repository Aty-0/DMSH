using UnityEngine;
using UnityEngine.Events;
using DMSH.Gameplay;

namespace DMSH.Path
{
    public enum EnemyScriptedBehavior
    {
        Nothing = 0,
        StartShot,
        StopShot,
    }

    public class PathPoint : MonoBehaviour
    {
        [Header("Other")]
        public Timer waitTimer = null;

        [Header("Curve")]
        public bool useCurve = false;
        public Vector3 curvePoint = Vector3.zero;

        [Header("Events")]
        public EnemyScriptedBehavior eventOnEndForAll = EnemyScriptedBehavior.Nothing;

        // When point is reached by enemy
        public UnityEvent eventSpecial = new UnityEvent();
    }
}