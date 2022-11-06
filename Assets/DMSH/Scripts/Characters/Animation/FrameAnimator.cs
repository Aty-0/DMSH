using DMSH.Gameplay;
using DMSH.Misc;
using DMSH.Path;

using Scripts.Utils;

using UnityEngine;

namespace DMSH.Characters.Animation
{
    public class FrameAnimator : MonoBehaviour
    {
        [SerializeField]
        private AnimationData m_data;

        [SerializeField]
        private MovableObject m_owner;

        [SerializeField]
        private Weapon m_ownerWeapon;

        [SerializeField]
        private SpriteRenderer m_renderer;
        public SpriteRenderer SpriteRenderer => m_renderer;

        private DirectionUtils.DirectionEnum _prevFrameDirection;
        private float _beforeNextFrame;
        private int _spriteIndex;
        private bool _shootState;
        private bool _isPaused;

        // unity

        protected void Start()
        {
            if (m_data != null)
            {
                SetNextFrame(0);
            }

            if (m_ownerWeapon != null)
            {
                m_ownerWeapon.onShot.Add(OnOwnerShooting);
            }
        }

        protected void Update()
        {
            if (GlobalSettings.IsPaused || _isPaused)
                return;
            
            _beforeNextFrame -= Time.deltaTime;
            if (_beforeNextFrame <= 0)
            {
                SetNextFrame(_spriteIndex + 1);
            }
        }

        // public

        public void ApplyNewAnimations(AnimationData data)
        {
            ResetToIdle();
            m_data = data;
            SetNextFrame(0);
        }

        public void SetPause(bool isPaused)
        {
            _isPaused = isPaused;
        }

        // private

        private void SetNextFrame(int index, bool recalcFps = true)
        {
            var moveDirection = m_owner.MoveDirection;
            var newFrameDirection = moveDirection.ToDirectionOnlySides();
            var frameDirectionIsChanged = newFrameDirection != _prevFrameDirection;
            _prevFrameDirection = newFrameDirection;

            var isStanding = moveDirection.sqrMagnitude < Mathf.Epsilon;
            var animationType = _shootState
                ? AnimationData.AnimationTypeEnum.Attack
                : (isStanding ? AnimationData.AnimationTypeEnum.Idle : AnimationData.AnimationTypeEnum.Move);

            if (m_data.TryGetAnimationData(_prevFrameDirection, animationType, out var animationDataStruct))
            {
                if (index > animationDataStruct.Sprites.Length - 1 || frameDirectionIsChanged)
                {
                    _shootState = false;
                    _spriteIndex = 0;
                }
                else
                {
                    _spriteIndex = index;
                }

                if (recalcFps)
                {
                    _beforeNextFrame = 1 / animationDataStruct.FPS;
                }

                m_renderer.sprite = animationDataStruct.Sprites[_spriteIndex];
            }
            else if(_shootState)
            {
                _shootState = false;
                SetNextFrame(_spriteIndex, false);
            }
        }

        private void ResetToIdle()
        {
            _beforeNextFrame = 0;
            _spriteIndex = 0;
            _shootState = false;
        }

        private void OnOwnerShooting()
        {
            _shootState = true;
            SetNextFrame(_spriteIndex);
        }
    }
}