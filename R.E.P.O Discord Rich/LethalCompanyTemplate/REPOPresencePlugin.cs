using BepInEx;
using BepInEx.Logging;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace REPOPresence
{
    [BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
    [BepInProcess("REPO.exe")]
    public class REPOPresencePlugin : BaseUnityPlugin
    {
        internal static ManualLogSource logger;
        private ConfigManager configManager;

        private void Awake()
        {
            logger = Logger;
            ConfigManager.Init(Config);

            GameObject discordGameObject = new GameObject();
            discordGameObject.AddComponent<DiscordManager>();
            DontDestroyOnLoad(discordGameObject);
            discordGameObject.hideFlags = HideFlags.HideAndDontSave;

            logger.LogInfo($"Plugin {MyPluginInfo.PLUGIN_GUID} is loaded!");
        }
    }
}









