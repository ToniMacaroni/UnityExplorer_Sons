#if ML
using System;
using System.IO;
using MelonLoader;
using MelonLoader.Utils;
using SonsSdk;
using UnityExplorer;
using UnityExplorer.Config;
using UnityExplorer.Loader.ML;
using UnityExplorer.UI;
using UniverseLib.Input;
using UniverseLib.UI;

#if CPP
[assembly: MelonPlatformDomain(MelonPlatformDomainAttribute.CompatibleDomains.IL2CPP)]
#else
[assembly: MelonPlatformDomain(MelonPlatformDomainAttribute.CompatibleDomains.MONO)]
#endif

[assembly: SonsModInfo(typeof(ExplorerMelonMod), ExplorerCore.NAME, ExplorerCore.VERSION, ExplorerCore.AUTHOR)]
[assembly: MelonColor(ConsoleColor.DarkMagenta)]

namespace UnityExplorer
{
    public class ExplorerMelonMod : SonsMod, IExplorerLoader
    {
        public string ExplorerFolderName => ExplorerCore.DEFAULT_EXPLORER_FOLDER_NAME;
        public string ExplorerFolderDestination => MelonHandler.ModsDirectory;

        public string UnhollowedModulesFolder => MelonEnvironment.Il2CppAssembliesDirectory;

        public ConfigHandler ConfigHandler => _configHandler;
        public MelonLoaderConfigHandler _configHandler;

        public Action<object> OnLogMessage => LoggerInstance.Msg;
        public Action<object> OnLogWarning => LoggerInstance.Warning;
        public Action<object> OnLogError   => LoggerInstance.Error;

        public override void OnInitializeMelon()
        {
            _configHandler = new MelonLoaderConfigHandler();
            ExplorerCore.Init(this);
        }
    }
}
#endif