#if ML
using System;
using System.IO;
using SFLoader;
using SFLoader.Utils;
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

namespace UnityExplorer
{
    public class ExplorerMelonMod : SonsMod, IExplorerLoader
    {
        public string ExplorerFolderName => ExplorerCore.DEFAULT_EXPLORER_FOLDER_NAME;
        public string ExplorerFolderDestination => MelonEnvironment.ModsDirectory;

        public string UnhollowedModulesFolder => MelonEnvironment.Il2CppAssembliesDirectory;

        public ConfigHandler ConfigHandler => _configHandler;
        public MelonLoaderConfigHandler _configHandler;

        public Action<object> OnLogMessage => LoggerInstance.Msg;
        public Action<object> OnLogWarning => LoggerInstance.Warning;
        public Action<object> OnLogError   => LoggerInstance.Error;

        public override void OnInitializeMod()
        {
            _configHandler = new MelonLoaderConfigHandler();
            ExplorerCore.Init(this);
        }
    }
}
#endif