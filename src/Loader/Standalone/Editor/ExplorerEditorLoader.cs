﻿#if STANDALONE
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;

namespace UnityExplorer.Loader.Standalone
{
    public class ExplorerEditorLoader : ExplorerStandalone
    {
        public new string ExplorerFolderName => $"{ExplorerCore.DEFAULT_EXPLORER_FOLDER_NAME}~";

        public static void Initialize()
        {
            Instance = new ExplorerEditorLoader();
            OnLog += LogHandler;
            Instance.configHandler = new StandaloneConfigHandler();

            ExplorerCore.Init(Instance);
        }

        static void LogHandler(string message, LogType logType)
        {
            switch (logType)
            {
                case LogType.Assert:    Debug.LogError(message); break;
                case LogType.Error:     Debug.LogError(message); break;
                case LogType.Exception: Debug.LogError(message); break;
                case LogType.Log:       Debug.Log(message); break;
                case LogType.Warning:   Debug.LogWarning(message); break;
            }
        }

        protected override void CheckExplorerFolder()
        {
            if (explorerFolderDest == null)
                explorerFolderDest = Path.GetDirectoryName(Application.dataPath);
        }
    }
}
#endif