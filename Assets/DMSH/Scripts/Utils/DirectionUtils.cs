using UnityEngine;

namespace Scripts.Utils
{
    public static class DirectionUtils
    {
        public static DirectionEnum ToDirection(this Vector2 vectorDirection)
        {
            const float SIDE_COEFFICIENT = 0.3f;

            return vectorDirection switch
            {
                {y: > 0, x: < SIDE_COEFFICIENT and > -SIDE_COEFFICIENT} => DirectionEnum.Top,
                {y: < 0, x: < SIDE_COEFFICIENT and > -SIDE_COEFFICIENT} => DirectionEnum.Down,
                {x: < -SIDE_COEFFICIENT} => DirectionEnum.Left,
                {x: > SIDE_COEFFICIENT} => DirectionEnum.Right,
                _ => DirectionEnum.Unset
            };
        }
        
        public static DirectionEnum ToDirectionOnlySides(this Vector2 vectorDirection)
        {
            return vectorDirection switch
            {
                {x: < 0} => DirectionEnum.Left,
                {x: > 0} => DirectionEnum.Right,
                _ => DirectionEnum.Unset
            };
        }

        public enum DirectionEnum : byte
        {
            Unset = 0,
            Left = 1,
            Right = 2,
            Top = 3,
            Down = 4,
        }
    }
}