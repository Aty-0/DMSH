using UnityEngine;
using UnityEngine.Events;

using DMSH.Misc;

namespace DMSH.Path
{
    public enum EnemyScriptedBehavior
    {
        Nothing = 0,
        StartShot,
        StopShot,
        // TODO: etc...
    }

    public class PathPoint : MonoBehaviour
    {
        [Header("Other")]
        public Timer waitTimer;

        [Header("Curve")]
        public bool useCurve;
        public Vector3 curvePoint = Vector3.zero;

        [Header("Events")]
        public EnemyScriptedBehavior eventOnEndForAll;

        // When point is reached by enemy
        public UnityEvent eventSpecial = new UnityEvent();
    }
}