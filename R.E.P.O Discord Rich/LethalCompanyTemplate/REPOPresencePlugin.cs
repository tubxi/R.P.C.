using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using DiscordRPC;
using DiscordRPC.Logging;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace REPOPresence
{
    [BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
    [BepInProcess("REPO.exe")]
    public class REPOPresencePlugin : BaseUnityPlugin
    {
        public static REPOPresencePlugin Instance { get; private set; }
        private DiscordRpcClient _discordClient;
        private DateTime _startTime;
        private string _lastScene = "";
        public static ManualLogSource LoggerInstance;

        private ConfigEntry<bool> enableDiscordRPC;

        private void Awake()
        {
            enableDiscordRPC = Config.Bind("General", "EnableDiscordRPC", true, "Enable or disable Discord Rich Presence");

            if (!enableDiscordRPC.Value)
            {
                Logger.LogInfo("Discord Rich Presence is disabled.");
                return;
            }

            if (Instance != null)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            DontDestroyOnLoad(gameObject);
            LoggerInstance = Logger;
            LoggerInstance.LogInfo("R.E.P.O Discord Presence is starting...");

            InitializeDiscordRPC();
        }

        private void InitializeDiscordRPC()
        {
            _startTime = DateTime.UtcNow;

            string discordClientId = "1349421320877772831";
            _discordClient = new DiscordRpcClient(discordClientId);
            _discordClient.Logger = new ConsoleLogger() { Level = DiscordRPC.Logging.LogLevel.Warning };

            _discordClient.OnReady += (_, e) =>
            {
                LoggerInstance.LogInfo($"Discord RPC Ready. Logged in as: {e.User.Username}");
            };
            _discordClient.OnError += (_, e) =>
            {
                LoggerInstance.LogError($"Discord RPC Error: {e.Message}");
            };

            _discordClient.Initialize();
            SetPresence("Starting R.E.P.O...", "Loading...");

            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            string currentScene = scene.name;
            LoggerInstance.LogInfo($"Scene loaded: {currentScene}");
            _lastScene = currentScene;

            if (_lastScene == "Main")
            {
                SetPresence("In Main Menu", "Just Chill");
            }
            else if (_lastScene.Contains("Level"))
            {
                SetPresence("In Game", _lastScene);
            }
            else
            {
                SetPresence("Exploring R.E.P.O", _lastScene);
            }
        }

        private void SetPresence(string details, string state)
        {
            var presence = new RichPresence
            {
                Details = details,
                State = state,
                Assets = new Assets
                {
                    LargeImageKey = "e97e5539a92d03667c3070102377d2d8a1a183817f7d5621bb480ea36a2f7432",
                    LargeImageText = "R.E.P.O"
                },
                Timestamps = new Timestamps(_startTime)
            };
            _discordClient.SetPresence(presence);
        }

        private void OnDestroy()
        {
            if (!enableDiscordRPC.Value)
                return;

            LoggerInstance.LogInfo("R.E.P.O Discord Presence is unloading...");
            SceneManager.sceneLoaded -= OnSceneLoaded;
            if (_discordClient != null)
            {
                _discordClient.Dispose();
                _discordClient = null;
            }
        }
    }
}















// 1349421320877772831                 e97e5539a92d03667c3070102377d2d8a1a183817f7d5621bb480ea36a2f7432