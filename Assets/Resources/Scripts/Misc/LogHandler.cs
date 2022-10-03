using System;
using UnityEngine;

using DMSH.Characters;

using Konsole;
using Konsole.Data;

namespace DMSH.Misc.Log
{
    public class LogHandler : MonoBehaviour
    {
        [Header("Draw")]
        public bool drawLogMessages = false;
        public bool drawConsole = false;

        [Header("Misc")]
        [SerializeField] private Font _font;
        [SerializeField] private int _fontSize;
        [SerializeField] private int _savedGameActiveState;
        [SerializeField] private bool _Resume = true;
        [SerializeField] private bool _fpsCounter = false;

        protected void Start()
        {
            IntegrateKonsoleCommands();
        }

        private void IntegrateKonsoleCommands()
        {
            Konsole.Konsole.IntegrateInExistingCanvas(new IntegrationOptions
            {
                FontSize = _fontSize,
                DefaultTextFont = _font,
            });
            Konsole.Konsole.RegisterCommand("fps", "Show/hide FPS indicator", _ => _fpsCounter = !_fpsCounter);
            Konsole.Konsole.RegisterCommand("tc", "Stop all audio sources", _ =>
            {
                foreach (var audioSource in FindObjectsOfType<AudioSource>())
                {
                    audioSource.Stop();
                }
                _Resume = !_Resume;
            });
            Konsole.Konsole.RegisterCommand("killplayer", _ => FindObjectOfType<PlayerController>().Kill());
            Konsole.Konsole.RegisterCommand("god", context =>
            {
                GlobalSettings.cheatGod = !GlobalSettings.cheatGod;
                context.Log($"{TextTags.Bold("GodMode")} is {(GlobalSettings.cheatGod ? TextTags.WithColor(Color.green, "Enabled") : TextTags.WithColor(Color.green, "Disabled"))}");
            });
            
            Konsole.Konsole.RegisterCommand("infboost", context =>
            {
                GlobalSettings.cheatInfiniteBoost = !GlobalSettings.cheatInfiniteBoost;
                context.Log($"{TextTags.Bold("InfiniteBoost")} is {(GlobalSettings.cheatInfiniteBoost ? TextTags.WithColor(Color.green, "Enabled") : TextTags.WithColor(Color.green, "Disabled"))}");
            });
            
            Konsole.Konsole.RegisterCommand("infboost", _ =>
            {
                foreach (var enemy in FindObjectsOfType<Enemy>())
                {
                    enemy.Kill(true);
                }
            });

            // ?
            Konsole.Konsole.RegisterCommand("tmessages", _ => drawLogMessages = !drawLogMessages);

            Konsole.Konsole.RegisterCommand("testLog", _ => Debug.Log("Hi"));
            Konsole.Konsole.RegisterCommand("testassert", _ => Debug.Assert(false, "Assert Hi"));
            Konsole.Konsole.RegisterCommand("testexception", _ => Debug.LogException(new NotImplementedException()));

            Konsole.Konsole.RegisterCommand("debugDrawPlayerDGUI", context =>
            {
                GlobalSettings.debugDrawPlayerDGUI = !GlobalSettings.debugDrawPlayerDGUI;
                context.Log($"{TextTags.Bold("DebugDrawPlayerDGUI")} is {(GlobalSettings.debugDrawPlayerDGUI ? TextTags.WithColor(Color.green, "Enabled") : TextTags.WithColor(Color.green, "Disabled"))}");
            });

            Konsole.Konsole.RegisterCommand("debugDrawPSAllPoints", context =>
            {
                GlobalSettings.debugDrawPSAllPoints = !GlobalSettings.debugDrawPSAllPoints;
                context.Log($"{TextTags.Bold("debugDrawPSAllPoints")} is {(GlobalSettings.debugDrawPSAllPoints ? TextTags.WithColor(Color.green, "Enabled") : TextTags.WithColor(Color.green, "Disabled"))}");
            });

            Konsole.Konsole.RegisterCommand("debugDrawPSCurrentMovement", context =>
            {
                GlobalSettings.debugDrawPSCurrentMovement = !GlobalSettings.debugDrawPSCurrentMovement;
                context.Log($"{TextTags.Bold("debugDrawPSCurrentMovement")} is {(GlobalSettings.debugDrawPSCurrentMovement ? TextTags.WithColor(Color.green, "Enabled") : TextTags.WithColor(Color.green, "Disabled"))}");
            });

            Konsole.Konsole.RegisterCommand("debugDrawPSObjectInfo", context =>
            {
                GlobalSettings.debugDrawPSObjectInfo = !GlobalSettings.debugDrawPSObjectInfo;
                context.Log($"{TextTags.Bold("debugDrawPSObjectInfo")} is {(GlobalSettings.debugDrawPSObjectInfo ? TextTags.WithColor(Color.green, "Enabled") : TextTags.WithColor(Color.green, "Disabled"))}");
            });

            Konsole.Konsole.RegisterCommand("debugDrawWeaponPoints", context =>
            {
                GlobalSettings.debugDrawWeaponPoints = !GlobalSettings.debugDrawWeaponPoints;
                context.Log($"{TextTags.Bold("debugDrawWeaponPoints")} is {(GlobalSettings.debugDrawWeaponPoints ? TextTags.WithColor(Color.green, "Enabled") : TextTags.WithColor(Color.green, "Disabled"))}");
            });
        }

        protected void OnGUI()
        {
            GUIStyle textStyle = new GUIStyle(GUI.skin.label);
            textStyle.font = _font;
            textStyle.fontSize = _fontSize;

            if (Event.current.type == EventType.KeyUp)
            {
                if (Event.current.keyCode == KeyCode.Escape && Event.current.isKey)
                    drawConsole = false;

                if (Event.current.keyCode == KeyCode.BackQuote && Event.current.isKey)
                {
                    drawConsole = !drawConsole;
                    Konsole.Konsole.ToggleConsole();
                    Cursor.visible = !_Resume || (drawConsole || !Convert.ToBoolean(_savedGameActiveState));

                    if (_Resume)
                    {
                        if (drawConsole)
                        {
                            //FIX ME: I'm too lazy to fix type
                            _savedGameActiveState = GlobalSettings.gameActiveAsInt;
                        }

                        GlobalSettings.SetGameActive(_savedGameActiveState == 1 && !drawConsole);
                    }
                }
            }

            if (_fpsCounter)
            {
                GUILayout.BeginArea(new Rect(0, 30, 500, 500));
                GUILayout.Label($"FPS:{(int)(1f / Time.unscaledDeltaTime)}", textStyle);
                GUILayout.EndArea();
            }

            if (drawLogMessages)
            {
                textStyle.normal.textColor = new Color(255, 97, 0);

                GUILayout.BeginArea(new Rect(Screen.width - 100, 0, 500, 500));
                GUILayout.Label($"gameActive: {GlobalSettings.gameActiveAsBool}", textStyle);
                GUILayout.EndArea();
            }
        }
    }
}