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

        public static Vector2 Vec2Random(float min, float max)
        {
            var rnd = Random.Range(min, max);
            return new Vector2(rnd, rnd);
        }

        public static Vector3 Vec3Random(float min, float max)
        {
            var rnd = Random.Range(min, max);
            return new Vector3(rnd, rnd, rnd);
        }
        public static Vector3 Vec2RandomInVec3(float min, float max)
        {
            var rnd = Random.Range(min, max);
            return new Vector3(rnd, rnd, 0);
        }
    }
}