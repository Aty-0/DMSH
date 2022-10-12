using Bagheads.UnityConsole;
using Bagheads.UnityConsole.Data;

using System;

using UnityEngine;

using DMSH.Characters;

namespace DMSH.Misc.Log
{
    public class LogHandler : MonoBehaviour
    {
        [Header("Draw")]
        public bool drawLogMessages = false;
        public bool drawConsole = false;

        [Header("Misc")]
        [SerializeField]
        private Font _font;

        [SerializeField]
        private int _fontSize;

        [SerializeField]
        private bool _savedGameActiveState;

        [SerializeField]
        private bool _Resume = true;

        [SerializeField]
        private bool _fpsCounter = false;

        protected void Start()
        {
            IntegrateKonsoleCommands();
        }

        private void IntegrateKonsoleCommands()
        {
            Konsole.RemoveAllAnonymousCommands();
            Konsole.IntegrateInExistingCanvas(new IntegrationOptions
            {
                FontSize = _fontSize,
                DefaultTextFont = _font,
            });
            Konsole.RegisterCommand("debugDrawFps", "Show/hide FPS indicator", _ => _fpsCounter = !_fpsCounter);
            Konsole.RegisterCommand("d_tc", "Stop all audio sources", _ =>
            {
                foreach (var audioSource in FindObjectsOfType<AudioSource>())
                {
                    audioSource.Stop();
                }

                _Resume = !_Resume;
            });

            Konsole.RegisterCommand("g_killPlayer", context =>
            {
                FindObjectOfType<PlayerController>().Kill();

                drawConsole = false;
                Konsole.ToggleConsole();
                Cursor.visible = true;
            });

            Konsole.RegisterCommand("g_god", context =>
            {
                GlobalSettings.cheatGod = !GlobalSettings.cheatGod;
                context.Log($"{TextTags.Bold("GodMode")} is {(GlobalSettings.cheatGod ? TextTags.WithColor(Color.green, "Enabled") : TextTags.WithColor(Color.green, "Disabled"))}");
            });

            Konsole.RegisterCommand("g_infboost", context =>
            {
                GlobalSettings.cheatInfiniteBoost = !GlobalSettings.cheatInfiniteBoost;
                context.Log($"{TextTags.Bold("InfiniteBoost")} is {(GlobalSettings.cheatInfiniteBoost ? TextTags.WithColor(Color.green, "Enabled") : TextTags.WithColor(Color.green, "Disabled"))}");
            });

            Konsole.RegisterCommand("g_killAllEnemy", _ =>
            {
                foreach (var enemy in FindObjectsOfType<Enemy>())
                {
                    enemy.Kill(true);
                }
            });

            Konsole.RegisterCommand("noclip", context =>
            {
                PlayerController.Player.boxCollider2D.enabled = !PlayerController.Player.boxCollider2D.enabled;
                context.Log($"{TextTags.Bold("Noclip")} is {(PlayerController.Player.boxCollider2D.enabled ? "Enabled" : "Disabled")}");
            });

            // ?
            Konsole.RegisterCommand("tm", _ => drawLogMessages = !drawLogMessages);

            Konsole.RegisterCommand("testLog", _ => Debug.Log("Hi"));
            Konsole.RegisterCommand("testassert", _ => Debug.Assert(false, "Assert Hi"));
            Konsole.RegisterCommand("testexception", _ => Debug.LogException(new NotImplementedException()));

            Konsole.RegisterCommand("debugDrawPlayerDGUI", context =>
            {
                GlobalSettings.debugDrawPlayerDGUI = !GlobalSettings.debugDrawPlayerDGUI;
                context.Log($"{TextTags.Bold("DebugDrawPlayerDGUI")} is {(GlobalSettings.debugDrawPlayerDGUI ? TextTags.WithColor(Color.green, "Enabled") : TextTags.WithColor(Color.green, "Disabled"))}");
            });

            Konsole.RegisterCommand("debugDrawPSAllPoints", context =>
            {
                GlobalSettings.debugDrawPSAllPoints = !GlobalSettings.debugDrawPSAllPoints;
                context.Log($"{TextTags.Bold("debugDrawPSAllPoints")} is {(GlobalSettings.debugDrawPSAllPoints ? TextTags.WithColor(Color.green, "Enabled") : TextTags.WithColor(Color.green, "Disabled"))}");
            });

            Konsole.RegisterCommand("debugDrawPSCurrentMovement", context =>
            {
                GlobalSettings.debugDrawPSCurrentMovement = !GlobalSettings.debugDrawPSCurrentMovement;
                context.Log($"{TextTags.Bold("debugDrawPSCurrentMovement")} is {(GlobalSettings.debugDrawPSCurrentMovement ? TextTags.WithColor(Color.green, "Enabled") : TextTags.WithColor(Color.green, "Disabled"))}");
            });

            Konsole.RegisterCommand("debugDrawPSObjectInfo", context =>
            {
                GlobalSettings.debugDrawPSObjectInfo = !GlobalSettings.debugDrawPSObjectInfo;
                context.Log($"{TextTags.Bold("debugDrawPSObjectInfo")} is {(GlobalSettings.debugDrawPSObjectInfo ? TextTags.WithColor(Color.green, "Enabled") : TextTags.WithColor(Color.green, "Disabled"))}");
            });

            Konsole.RegisterCommand("debugDrawWeaponPoints", context =>
            {
                GlobalSettings.debugDrawWeaponPoints = !GlobalSettings.debugDrawWeaponPoints;
                context.Log($"{TextTags.Bold("debugDrawWeaponPoints")} is {(GlobalSettings.debugDrawWeaponPoints ? TextTags.WithColor(Color.green, "Enabled") : TextTags.WithColor(Color.green, "Disabled"))}");
            });

            Konsole.RegisterCommand("debugDrawInvWallSI", context =>
            {
                GlobalSettings.debugDrawInvWallSI = !GlobalSettings.debugDrawInvWallSI;
                context.Log($"{TextTags.Bold("debugDrawInvWallSI")} is {(GlobalSettings.debugDrawInvWallSI ? TextTags.WithColor(Color.green, "Enabled") : TextTags.WithColor(Color.green, "Disabled"))}");
            });
        }

        protected void OnGUI()
        {
            GUIStyle textStyle = new GUIStyle(GUI.skin.label);
            textStyle.font = _font;
            textStyle.fontSize = _fontSize;

            if (Event.current.type == EventType.KeyUp)
            {
                if (Event.current.keyCode == KeyCode.Escape
                    && Event.current.isKey
                    && Konsole.ConsoleInstance.IsShouldBeVisible)
                {
                    Konsole.ToggleConsole();
                    drawConsole = Konsole.ConsoleInstance.IsShouldBeVisible;
                }

                if (Event.current.keyCode == KeyCode.BackQuote && Event.current.isKey)
                {
                    drawConsole = Konsole.ConsoleInstance.IsShouldBeVisible;
                    Cursor.visible = !_Resume || (drawConsole || !GlobalSettings.gameActiveAsBool);

                    if (_Resume)
                    {
                        if (drawConsole)
                        {
                            _savedGameActiveState = GlobalSettings.gameActiveAsBool;
                        }

                        GlobalSettings.SetGameActive(_savedGameActiveState == true && !drawConsole);
                    }
                }
            }

            if (_fpsCounter)
            {
                GUILayout.BeginArea(new Rect(0, 30, 500, 500));
                GUILayout.Label($"FPS:{(int) (1f / Time.unscaledDeltaTime)}", textStyle);
                GUILayout.EndArea();
            }
        }
    }
}