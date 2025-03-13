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
        public static ConfigEntry<bool> JoinOnlyInPublicLobby { get; private set; }
        public static ConfigEntry<bool> DisplayLivingPlayers { get; private set; }

        public static ConfigEntry<string> MainMenuLargeImage { get; private set; }
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
            JoinOnlyInPublicLobby = config.Bind(
                "Party",
                "JoinOnlyInPublicLobby",
                false,
                "Only allow players to join your game from Discord if your lobby is public."
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
                "In Game",
                "The state of the rich presence."
            );

            MainMenuLargeImage = config.Bind(
                "Presence.MainMenu",
                "LargeImage",
                "mainmenu",
                "The large image key for the rich presence."
            );

            InGameLargeImage = config.Bind(
                "Presence.InGame",
                "LargeImage",
                "ingame",
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