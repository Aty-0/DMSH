using System.Runtime.CompilerServices;

using UnityEngine;

namespace Scripts.Utils
{
    public static class MathUtils
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2 RadianToVector2(float radian)
        {
            return new Vector2(Mathf.Cos(radian), Mathf.Sin(radian));
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Vector2ToRadian(Vector2 direction)
        {
            return (float)Mathf.Atan2(direction.x, -direction.y);
        }
    }
}