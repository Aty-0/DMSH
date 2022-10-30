using DMSH.Characters;
using DMSH.Misc;

using Scripts.Utils;

using UnityEngine;
using UnityEngine.InputSystem;

namespace DMSH.Scripts.Controls
{
    [RequireComponent(typeof(PlayerInput))]
    public class PlayerControl : Instance<PlayerControl>
    {
        [SerializeField]
        private PlayerController m_player;

        [SerializeField]
        private PlayerInput m_input;
        public PlayerInput Input => m_input;

        protected void Start()
        {
            if (m_input is null)
            {
                m_input = GetComponent<PlayerInput>();
                if (m_input is null)
                {
                    Debug.LogError($"Critical bug. Where is {nameof(PlayerInput)}?", this);
                }
            }

            if (m_player is null)
            {
                m_player = PlayerController.Player;
                if (m_player is null)
                {
                    Debug.LogError($"Player is not set, tried to find it and failed!", this);
                }
                else
                {
                    Debug.LogWarning($"Player is not set, tried to find it and found ({m_player.name})!", this);
                }
            }
        }

        /// <summary>Call from <c>PlayerInput</c></summary>
        public void On_UseBoost(InputAction.CallbackContext context)
        {
            if (m_player != null && GlobalSettings.gameActiveAsBool)
            {
                m_player.UseBoost();
            }
        }

        /// <summary>Call from <c>PlayerInput</c></summary>
        public void On_Shot(InputAction.CallbackContext context)
        {
            if (GlobalSettings.gameActiveAsBool && context.action.IsPressed())
            {
                m_player.Weapon.Shot();
            }
            else
            {
                m_player.Weapon.StopShooting();
            }
        }

        /// <summary>Call from <c>PlayerInput</c></summary>
        public void On_Move(InputAction.CallbackContext context)
        {
            if (m_player is null)
                return;

            m_player.MoveDirection = context.ReadValue<Vector2>();
        }

        /// <summary>Call from <c>PlayerInput</c></summary>
        public void On_Pause(InputAction.CallbackContext input)
        {
            if (m_player is null)
                return;

            if (!input.performed)
                return;

            m_player.ShowPauseScreen();
        }
    }
}