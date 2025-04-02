using BepInEx.Configuration;

namespace REPOPresence
{
    class ConfigManager
    {
        public static ConfigManager Instance { get; private set; }
        public static ConfigEntry<long> AppID { get; private set; }
        public static ConfigEntry<bool> AllowJoin { get; private set; }
        public static ConfigEntry<string> MainMenuLargeImage { get; private set; }
        public static ConfigEntry<string> InLobbyLargeImage { get; private set; }
        public static ConfigEntry<string> InGameLargeImage { get; private set; }
        public static ConfigEntry<string> InGameLargeText { get; private set; }
        public static ConfigEntry<string> ActivityState { get; private set; }
        public static ConfigEntry<string> ActivityDetails { get; private set; }
        public static ConfigEntry<bool> Debug { get; private set; }
        public static ConfigEntry<string> ServiceStationImage { get; private set; }
        public static ConfigEntry<string> HeadmanManorImage { get; private set; }
        public static ConfigEntry<string> SwiftbroomAcademyImage { get; private set; }
        public static ConfigEntry<string> McJannekStationImage { get; private set; }
        public static ConfigEntry<string> DisposalArenaImage { get; private set; }

        public static void Init(ConfigFile config)
        {
            Instance = new ConfigManager();
            AppID = config.Bind("General", "1349755295974428692", 1349755295974428692, "Discord Application ID");
            AllowJoin = config.Bind("General", "AllowJoin", true, "Allow others to join your game");
            MainMenuLargeImage = config.Bind("Images", "MainMenuLargeImage", "embedded_cover", "Main menu large image key");
            InLobbyLargeImage = config.Bind("Images", "InLobbyLargeImage", "embedded_cover", "Lobby large image key");
            InGameLargeImage = config.Bind("Images", "InGameLargeImage", "embedded_cover", "In-game large image key");
            InGameLargeText = config.Bind("Images", "InGameLargeText", "Playing R.E.P.O", "In-game large image text");
            ActivityState = config.Bind("Activity", "ActivityState", "Playing", "Activity state text");
            ActivityDetails = config.Bind("Activity", "ActivityDetails", "In Game", "Activity details text");
            Debug = config.Bind("General", "Debug", false, "Enable debug logging");
            ServiceStationImage = config.Bind("Images", "ServiceStationImage", "service_station", "Service Station image key");
            HeadmanManorImage = config.Bind("Images", "HeadmanManorImage", "headman_manor", "Headman Manor image key");
            SwiftbroomAcademyImage = config.Bind("Images", "SwiftbroomAcademyImage", "swiftbroom_academy", "Swiftbroom Academy image key");
            McJannekStationImage = config.Bind("Images", "McJannekStationImage", "mcjannek_station", "McJannek Station image key");
            DisposalArenaImage = config.Bind("Images", "DisposalArenaImage", "disposal_arena", "Disposal Arena image key");
        }
    }
}