using System;
using DiscordRPC;
using DiscordRPC.Logging;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace REPOPresence
{
    public class DiscordManager : MonoBehaviour
    {
        private static DiscordRpcClient client;
        private static RichPresence presence;

        private void Start()
        {
            InitializeDiscordRPC();
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

            SceneManager.sceneLoaded += OnSceneLoaded;
            REPOPresencePlugin.logger.LogInfo("Discord RPC Initialized.");
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            string currentScene = scene.name;
            REPOPresencePlugin.logger.LogInfo($"Scene loaded: {currentScene}");

            if (currentScene == "Main")
            {
                SetPresence("In Main Menu", "Just Chill", ConfigManager.MainMenuLargeImage.Value);
            }
            else if (currentScene.Contains("Level"))
            {
                SetPresence("In Game", currentScene, ConfigManager.InGameLargeImage.Value);
            }
            else
            {
                SetPresence("Exploring REPO", currentScene, ConfigManager.InGameLargeImage.Value);
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
        }
    }
}