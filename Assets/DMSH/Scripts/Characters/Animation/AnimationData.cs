using Scripts.Utils;

using System;

using UnityEngine;

namespace DMSH.Characters.Animation
{
    [CreateAssetMenu(menuName = "DMSH/Animation")]
    public class AnimationData : ScriptableObject
    {
        public AnimationDataStruct[] Animations;
        
        // public

        public bool TryGetAnimationData(DirectionUtils.DirectionEnum direction, AnimationTypeEnum type, out AnimationDataStruct data)
        {
            for (var i = 0; i < Animations.Length; i++)
            {
                if (Animations[i].Direction == direction && Animations[i].Type == type)
                {
                    data = Animations[i];
                    return true;
                }
            }

            data = default;
            return false;
        }
        
        // data

        [Serializable]
        public struct AnimationDataStruct
        {
            public string AnimationName;
            public float FPS;
            public DirectionUtils.DirectionEnum Direction;
            public AnimationTypeEnum Type;
            public Sprite[] Sprites;
        }

        public enum AnimationTypeEnum
        {
            Idle = 0,
            Move = 1,
            Attack = 2,
        }
    }
}