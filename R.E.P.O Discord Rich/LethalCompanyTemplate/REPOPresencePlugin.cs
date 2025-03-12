using DiscordRPC.Message;
using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine.SceneManagement;
using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using DiscordRPC;
using DiscordRPC.Logging;
using Steamworks;
using REPOPresence;
using DiscordRPC.Events;
using REPO_Discord_Rich_Presence;

namespace DiscordRPC.Example
{
    [BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
    [BepInProcess("REPO.exe")]
    class Program : BaseUnityPlugin
    {
        private static Logging.LogLevel logLevel = Logging.LogLevel.Trace;
        private static int discordPipe = -1;
        private static RichPresence presence = new RichPresence()
        {
            Details = "Example Project 🎁",
            State = "csharp example",
            Assets = new Assets()
            {
                LargeImageKey = "c639821880a71b73e0156a433273846e72b74510cbaf8f90eb209973c9ef7300",
                LargeImageText = "R.E.P.O"
            },
            Timestamps = new Timestamps(DateTime.UtcNow)
        };
        private static DiscordRpcClient client;
        private static bool isRunning = true;
        private static StringBuilder word = new StringBuilder();
        private static string lastScene = "";
        public static ManualLogSource LoggerInstance;
        private ConfigEntry<bool> enableDiscordRPC;
        private static int cursorIndex = 0;
        private static string previousCommand = "";

        private static bool isSteamInitialized = false;
        private static CSteamID steamID;

        private void Awake()
        {
            enableDiscordRPC = Config.Bind("General", "EnableDiscordRPC", true, "Enable or disable Discord Rich Presence");

            if (!enableDiscordRPC.Value)
            {
                Logger.LogInfo("Discord Rich Presence is disabled.");
                return;
            }

            DontDestroyOnLoad(gameObject);
            LoggerInstance = Logger;
            LoggerInstance.LogInfo("R.E.P.O Discord Presence is starting...");

            if (!SteamAPI.Init())
            {
                LoggerInstance.LogError("Failed to initialize Steam API.");
                return;
            }
            isSteamInitialized = true;
            steamID = SteamUser.GetSteamID();

            InitializeDiscordRPC();
        }

        private static void Main(string[] args)
        {
            for (int i = 0; i < args.Length; i++)
            {
                switch (args[i])
                {
                    case "-pipe":
                        discordPipe = int.Parse(args[++i]);
                        break;

                    default: break;
                }
            }

            ReadyTaskExample();
            Console.WriteLine("Press any key to terminate");
            Console.ReadKey();
        }

        private void InitializeDiscordRPC()
        {
            client = new DiscordRpcClient("1349443275974508574", pipe: discordPipe)
            {
                Logger = new ConsoleLogger(logLevel, true)
            };

            client.OnReady += (_, e) =>
            {
                LoggerInstance.LogInfo($"Discord RPC Ready. Logged in as: {e.User.Username}");
            };
            client.OnError += (_, e) =>
            {
                LoggerInstance.LogError($"Discord RPC Error: {e.Message}");
            };

            client.Initialize();

            SetPresence("Starting R.E.P.O...", "Loading...");

            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        private static async void ReadyTaskExample()
        {
            TaskCompletionSource<User> readyCompletionSource = new TaskCompletionSource<User>();

            using var client = new DiscordRpcClient("1349443275974508574", pipe: discordPipe)
            {
                Logger = new Logging.ConsoleLogger(logLevel, true)
            };

            client.OnReady += (sender, msg) =>
            {
                readyCompletionSource.SetResult(msg.User);
            };

            client.Initialize();
            SetPresence("Starting R.E.P.O...", "Loading...");

            SceneManager.sceneLoaded += OnSceneLoaded;

            var user = await readyCompletionSource.Task;
            Console.WriteLine("Connected to discord with user {0}: {1}", user.Username, user.Avatar);
        }

        private static void SetPresence(string details, string state)
        {
            presence.Details = details;
            presence.State = state;
            client.SetPresence(presence);
        }

        private static void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            string currentScene = scene.name;
            Console.WriteLine($"Scene loaded: {currentScene}");
            lastScene = currentScene;

            if (lastScene == "Main")
            {
                SetPresence("In Main Menu", "Just Chill");
            }
            else if (lastScene.Contains("Level"))
            {
                SetPresence("In Game", lastScene);
            }
            else
            {
                SetPresence("Exploring R.E.P.O", lastScene);
            }

            if (isSteamInitialized)
            {
                var steamName = SteamFriends.GetPersonaName();
                SetPresence($"Playing as {steamName}", lastScene);
            }
        }

        private static void ProcessKey()
        {
            var key = Console.ReadKey(true);
            switch (key.Key)
            {
                case ConsoleKey.Enter:
                    Console.WriteLine();
                    ExecuteCommand(word.ToString());
                    word.Clear();
                    break;

                case ConsoleKey.Backspace:
                    if (cursorIndex > 0)
                    {
                        word.Remove(cursorIndex - 1, 1);
                        cursorIndex--;
                    }
                    break;

                case ConsoleKey.Delete:
                    if (cursorIndex < word.Length)
                    {
                        word.Remove(cursorIndex, 1);
                    }
                    break;

                case ConsoleKey.LeftArrow:
                    cursorIndex = Math.Max(0, cursorIndex - 1);
                    break;

                case ConsoleKey.RightArrow:
                    cursorIndex = Math.Min(word.Length, cursorIndex + 1);
                    break;

                case ConsoleKey.UpArrow:
                    word.Clear().Append(previousCommand);
                    break;

                default:
                    if (!Char.IsControl(key.KeyChar))
                    {
                        word.Insert(cursorIndex, key.KeyChar);
                        cursorIndex++;
                    }
                    break;
            }
            Console.SetCursorPosition(cursorIndex, Console.CursorTop);
        }

        private static void ExecuteCommand(string input)
        {
            string[] parts = input.Trim().Split(' ', 2);
            string command = parts[0].ToLowerInvariant();
            string body = parts.Length > 1 ? parts[1] : string.Empty;

            switch (command)
            {
                case "close":
                    client?.Dispose();
                    break;

                case "state":
                    presence.State = body;
                    client?.SetPresence(presence);
                    break;

                case "details":
                    presence.Details = body;
                    client?.SetPresence(presence);
                    break;

                case "large_key":
                    presence.Assets = presence.Assets ?? new Assets();
                    presence.Assets.LargeImageKey = body;
                    client?.SetPresence(presence);
                    break;

                case "large_text":
                    presence.Assets = presence.Assets ?? new Assets();
                    presence.Assets.LargeImageText = body;
                    client?.SetPresence(presence);
                    break;

                case "small_key":
                    presence.Assets = presence.Assets ?? new Assets();
                    presence.Assets.SmallImageKey = body;
                    client?.SetPresence(presence);
                    break;

                case "small_text":
                    presence.Assets = presence.Assets ?? new Assets();
                    presence.Assets.SmallImageText = body;
                    client?.SetPresence(presence);
                    break;

                case "help":
                    Console.WriteLine("Available Commands: state, details, large_key, large_text, small_key, small_text");
                    break;

                default:
                    Console.WriteLine("Unknown Command '{0}'. Try 'help' for a list of commands", command);
                    break;
            }
        }

        private void OnDestroy()
        {
            if (!enableDiscordRPC.Value)
                return;

            LoggerInstance.LogInfo("R.E.P.O Discord Presence is unloading...");
            SceneManager.sceneLoaded -= OnSceneLoaded;
            if (client != null)
            {
                client.Dispose();
                client = null;
            }
            if (isSteamInitialized)
            {
                SteamAPI.Shutdown();
            }
        }
    }
}











// 1349421320877772831                 e97e5539a92d03667c3070102377d2d8a1a183817f7d5621bb480ea36a2f7432