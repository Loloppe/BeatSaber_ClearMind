using IPA;
using Conf = IPA.Config.Config;
using IPA.Config.Stores;
using BeatSaberMarkupLanguage.GameplaySetup;
using IPALogger = IPA.Logging.Logger;
using HarmonyLib;
using System.Reflection;

namespace ClearMind
{
    [Plugin(RuntimeOptions.DynamicInit)]
    public class Plugin
    {
        internal static Plugin Instance;
        internal static IPALogger Log;
        internal static Harmony harmony;

        [Init]
        public Plugin(Conf conf, IPALogger logger)
        {
            Instance = this;
            Log = logger;
            Config.Instance = conf.Generated<Config>();
            harmony = new Harmony("Loloppe.BeatSaber.ClearMind");
            BeatSaberMarkupLanguage.Util.MainMenuAwaiter.MainMenuInitializing += MainMenuInit;
        }

        [OnEnable]
        public void OnEnable()
        {
            harmony.PatchAll(Assembly.GetExecutingAssembly());
        }

        public void MainMenuInit()
        {
            GameplaySetup.Instance.AddTab("ClearMind", "ClearMind.UI.settings.bsml", Config.Instance, MenuType.All);
        }

        [OnDisable]
        public void OnDisable()
        {
            harmony.UnpatchSelf();
        }
    }
}
