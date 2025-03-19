using System;
using DiscordRPC;
using DiscordRPC.Logging;
using UnityEngine;

namespace REPOPresence
{
    public class DiscordManager : MonoBehaviour
    {
        private static DiscordRpcClient client;
        private static RichPresence presence;

        private void Start()
        {
            InitializeDiscordRPC();
            SetDefaultPresence();
        }

        private void InitializeDiscordRPC()
        {
            REPOPresencePlugin.logger.LogInfo("Initializing Discord RPC...");
            client = new DiscordRpcClient(ConfigManager.AppID.Value.ToString())
            {
                Logger = new ConsoleLogger(DiscordRPC.Logging.LogLevel.Warning)
            };

            client.OnReady += (sender, e) =>
            {
                REPOPresencePlugin.logger.LogInfo($"Discord RPC Ready. Logged in as: {e.User.Username}");
            };
            client.OnError += (sender, e) =>
            {
                REPOPresencePlugin.logger.LogError($"Discord RPC Error: {e.Message}");
            };

            client.Initialize();

            presence = new RichPresence()
            {
                Details = ConfigManager.ActivityDetails.Value,
                State = ConfigManager.ActivityState.Value,
                Assets = new Assets()
                {
                    LargeImageKey = ConfigManager.MainMenuLargeImage.Value,
                    LargeImageText = "R.E.P.O"
                },
                Timestamps = new Timestamps(DateTime.UtcNow)
            };
            client.SetPresence(presence);

            Application.logMessageReceived += HandleLog;
            REPOPresencePlugin.logger.LogInfo("Discord RPC Initialized.");
        }

        private void SetDefaultPresence()
        {
            SetPresence("In Main Menu", "Just Chill", ConfigManager.MainMenuLargeImage.Value);
        }

        private void HandleLog(string logString, string stackTrace, LogType type)
        {
            if (logString.Contains("Changed level to: Level - Lobby Menu"))
            {
                SetPresence("In Lobby", "Waiting for players", ConfigManager.InLobbyLargeImage.Value);
            }
            else if (logString.Contains("Changed level to: Level - "))
            {
                string levelName = logString.Split(new string[] { "Changed level to: Level - " }, StringSplitOptions.None)[1];
                SetPresence($"In Game: {levelName}", "Playing", ConfigManager.InGameLargeImage.Value);
            }
            else if (logString.Contains("Created lobby on Network Connect") || logString.Contains("Steam: Hosting lobby"))
            {
                SetPresence("In Lobby", "Waiting for players", ConfigManager.InLobbyLargeImage.Value);
            }
            else if (logString.Contains("Leave to Main Menu"))
            {
                SetPresence("In Main Menu", "Just Chill", ConfigManager.MainMenuLargeImage.Value);
            }
            else if (logString.Contains("updated level to: Level - "))
            {
                string levelName = logString.Split(new string[] { "updated level to: Level - " }, StringSplitOptions.None)[1];
                SetPresence($"In Game: {levelName}", "Playing", ConfigManager.InGameLargeImage.Value);
            }
        }

        private void SetPresence(string details, string state, string largeImageKey)
        {
            if (presence != null)
            {
                presence.Details = details;
                presence.State = state;
                presence.Assets.LargeImageKey = largeImageKey;
                client.SetPresence(presence);
                REPOPresencePlugin.logger.LogInfo($"Presence set: {details}, {state}");
            }
        }

        private void OnDestroy()
        {
            REPOPresencePlugin.logger.LogInfo("OnDestroy called. Checking if Discord RPC needs to be disposed.");
            if (client != null)
            {
                client.Dispose();
                client = null;
                REPOPresencePlugin.logger.LogInfo("Discord RPC client disposed.");
            }
            Application.logMessageReceived -= HandleLog;
        }
    }
}