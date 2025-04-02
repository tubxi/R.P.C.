using System;
using DiscordRPC;
using DiscordRPC.Logging;
using UnityEngine;

namespace REPOPresence
{
    class DiscordManager : MonoBehaviour
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
            client = new DiscordRpcClient("1349755295974428692")
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
                Details = "In Game",
                State = "Playing",
                Assets = new Assets()
                {
                    LargeImageKey = "embedded_cover",
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
            REPOPresencePlugin.logger.LogInfo("Setting default presence: Main Menu");
            SetPresence("In Main Menu", "Just Chill", "embedded_cover");
        }

        private void HandleLog(string logString, string stackTrace, LogType type)
        {
            if (logString.Contains("Changed level to: Level - Lobby Menu"))
            {
                REPOPresencePlugin.logger.LogInfo("Setting presence: In Lobby");
                SetPresence("In Lobby", "Waiting for players", "embedded_cover");
            }
            else if (logString.Contains("Changed level to: Level - "))
            {
                string levelName = logString.Split(new string[] { "Changed level to: Level - " }, StringSplitOptions.None)[1];
                string imageKey = GetLevelImageKey(levelName);
                REPOPresencePlugin.logger.LogInfo($"Setting presence: In Game - {levelName}");
                SetPresence($"In Game: {levelName}", "Playing", imageKey);
            }
            else if (logString.Contains("Created lobby on Network Connect") || logString.Contains("Steam: Hosting lobby"))
            {
                REPOPresencePlugin.logger.LogInfo("Setting presence: In Lobby");
                SetPresence("In Lobby", "Waiting for players", "embedded_cover");
            }
            else if (logString.Contains("Leave to Main Menu"))
            {
                REPOPresencePlugin.logger.LogInfo("Setting presence: Main Menu");
                SetPresence("In Main Menu", "Just Chill", "embedded_cover");
            }
            else if (logString.Contains("updated level to: Level - "))
            {
                string levelName = logString.Split(new string[] { "updated level to: Level - " }, StringSplitOptions.None)[1];
                string imageKey = GetLevelImageKey(levelName);
                REPOPresencePlugin.logger.LogInfo($"Setting presence: In Game - {levelName}");
                SetPresence($"In Game: {levelName}", "Playing", imageKey);
            }
        }

        private string GetLevelImageKey(string levelName)
        {
            switch (levelName)
            {
                case "Lobby Menu":
                    return "embedded_cover";
                case "Manor":
                    return "headman_manor";
                case "Wizard":
                    return "swiftbroom_academy";
                case "Service Station":
                    return "service_station";
                case "Arctic":
                    return "mcjannek_station";
                case "Arena":
                    return "disposal_arena";
                default:
                    return "embedded_cover";
            }
        }

        private void SetPresence(string details, string state, string largeImageKey)
        {
            presence.Details = details;
            presence.State = state;
            presence.Assets.LargeImageKey = largeImageKey;
            client.SetPresence(presence);
            REPOPresencePlugin.logger.LogInfo($"Presence set: {details}, {state}, {largeImageKey}");
        }

        private void OnDestroy()
        {
            client.Dispose();
        }
    }
}