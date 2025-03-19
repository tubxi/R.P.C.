using BepInEx.Configuration;

namespace REPOPresence
{
    public class ConfigManager
    {
        public static ConfigManager Instance { get; private set; }

        public static void Init(ConfigFile config)
        {
            Instance = new ConfigManager(config);
        }

        public static ConfigEntry<long> AppID { get; private set; }
        public static ConfigEntry<bool> AllowJoin { get; private set; }
        public static ConfigEntry<string> MainMenuLargeImage { get; private set; }
        public static ConfigEntry<string> InLobbyLargeImage { get; private set; }
        public static ConfigEntry<string> InGameLargeImage { get; private set; }
        public static ConfigEntry<string> InGameLargeText { get; private set; }
        public static ConfigEntry<string> ActivityState { get; private set; }
        public static ConfigEntry<string> ActivityDetails { get; private set; }
        public static ConfigEntry<bool> Debug { get; private set; }

        private ConfigManager(ConfigFile config)
        {
            AppID = config.Bind(
                "General",
                "1349755295974428692",
                1349755295974428692,
                "The Discord App ID for this game."
            );

            AllowJoin = config.Bind(
                "Party",
                "AllowJoin",
                true,
                "Allow players to join your game from Discord."
            );

            ActivityDetails = config.Bind(
                "Presence",
                "ActivityDetails",
                "Playing REPO",
                "The details of the rich presence."
            );
            ActivityState = config.Bind(
                "Presence",
                "ActivityState",
                "In Main Menu",
                "The state of the rich presence."
            );

            MainMenuLargeImage = config.Bind(
                "Presence.MainMenu",
                "LargeImage",
                "57dabf5f530a90fdca068426714bc61f20b3e22ae4a5526b32d490d86fc8a33c",
                "The large image key for the rich presence."
            );

            InLobbyLargeImage = config.Bind(
                "Presence.InLobby",
                "LargeImage",
                "57dabf5f530a90fdca068426714bc61f20b3e22ae4a5526b32d490d86fc8a33c",
                "The large image key for the rich presence."
            );

            InGameLargeImage = config.Bind(
                "Presence.InGame",
                "LargeImage",
                "57dabf5f530a90fdca068426714bc61f20b3e22ae4a5526b32d490d86fc8a33c",
                "The large image key for the rich presence."
            );
            InGameLargeText = config.Bind(
                "Presence.InGame",
                "LargeText",
                "In Game",
                "The large image tooltip for the rich presence."
            );

            Debug = config.Bind(
                "Debug",
                "Debug logs",
                false,
                "Enable debug logging."
            );
        }
    }
}