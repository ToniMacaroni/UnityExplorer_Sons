global using System;
global using System.Collections.Generic;
global using System.IO;
global using System.Linq;
global using System.Reflection;
global using UnityEngine;
global using UnityEngine.UI;
global using UniverseLib;
global using UniverseLib.Utility;
using ForestNanosuit;
using RedLoader;
using UnityExplorer.Config;
using UnityExplorer.CSConsole;
using UnityExplorer.Inspectors;
using UnityExplorer.ObjectExplorer;
using UnityExplorer.Runtime;
using UnityExplorer.UI;
using UnityExplorer.UI.Panels;
using UniverseLib.Input;
using Color = System.Drawing.Color;

namespace UnityExplorer
{
    public static class ExplorerCore
    {
        public const string NAME = "UnityExplorer";
        public const string VERSION = "4.9.3";
        public const string AUTHOR = "Sinai";
        public const string GUID = "com.sinai.unityexplorer";

        public static IExplorerLoader Loader { get; private set; }
        public static string ExplorerFolder => Path.Combine(Loader.ExplorerFolderDestination, Loader.ExplorerFolderName);
        public const string DEFAULT_EXPLORER_FOLDER_NAME = "UnityExplorer";

        public static HarmonyLib.Harmony Harmony { get; } = new(GUID);

        /// <summary>
        /// Initialize UnityExplorer with the provided Loader implementation.
        /// </summary>
        public static void Init(IExplorerLoader loader)
        {
            if (Loader != null)
                throw new Exception("UnityExplorer is already loaded.");

            Loader = loader;

            Log($"{NAME} {VERSION} initializing...");

            CheckLegacyExplorerFolder();
            Directory.CreateDirectory(ExplorerFolder);
            ConfigManager.Init(Loader.ConfigHandler);

            Universe.Init(ConfigManager.Startup_Delay_Time.Value, LateInit, Log, new()
            {
                Disable_EventSystem_Override = ConfigManager.Disable_EventSystem_Override.Value,
                Force_Unlock_Mouse = ConfigManager.Force_Unlock_Mouse.Value,
                Unhollowed_Modules_Folder = loader.UnhollowedModulesFolder
            });

            UERuntimeHelper.Init();
            ExplorerBehaviour.Setup();
            UnityCrashPrevention.Init();
        }

        // Do a delayed setup so that objects aren't destroyed instantly.
        // This can happen for a multitude of reasons.
        // Default delay is 1 second which is usually enough.
        static void LateInit()
        {
            SceneHandler.Init();

            Log($"Creating UI...");

            UIManager.InitUI();

            Log($"{NAME} {VERSION} ({Universe.Context}) initialized.");
            
            MaterialSync.Start();
            
            RLog.MsgDrawingCallbackHandler += OnRLogMsg;
            RLog.ErrorCallbackHandler += OnRLogError;
            RLog.WarningCallbackHandler += OnRLogWarning;
            // InspectorManager.Inspect(typeof(Tests.TestClass));
        }

        internal static void Shutdown()
        {
            RLog.MsgDrawingCallbackHandler -= OnRLogMsg;
            RLog.ErrorCallbackHandler -= OnRLogError;
            RLog.WarningCallbackHandler -= OnRLogWarning;
        }

        private static void OnRLogMsg(Color namesectionColor, Color textColor, string namesection, string text)
        {
            if(!ConfigManager.Enable_Loader_Logs.Value)
            {
                return;
            }

            LogPanel.Log($"[{namesection}] {text}", LogType.Log);
        }
        
        private static void OnRLogWarning(string namesection, string text)
        {
            if(!ConfigManager.Enable_Loader_Logs.Value)
            {
                return;
            }
            
            LogPanel.Log($"[{namesection}] {text}", LogType.Warning);
        }

        private static void OnRLogError(string namesection, string text)
        {
            if(!ConfigManager.Enable_Loader_Logs.Value)
            {
                return;
            }
            
            LogPanel.Log($"[{namesection}] {text}", LogType.Error);
        }
        
        internal static void Update()
        {
            // check master toggle
            if (InputManager.GetKeyDown(ConfigManager.Master_Toggle.Value))
            {
                UIManager.ShowMenu = !UIManager.ShowMenu;
            }

            if (InputManager.GetKeyDown(ConfigManager.Debug_Box_Toggle_Key.Value))
            {
                GameObjectInspector.ToggleBoxDebug();
            }

            if (InputManager.GetKeyDown(ConfigManager.Log_Panel_Toggle_Key.Value))
            {
                UIManager.TogglePanel(UIManager.Panels.ConsoleLog);
            }
            
            ConsoleController.CheckQuickScripts();
        }


        #region LOGGING

        public static void Log(object message)
            => Log(message, LogType.Log);

        public static void LogWarning(object message)
            => Log(message, LogType.Warning);

        public static void LogError(object message)
            => Log(message, LogType.Error);

        public static void LogUnity(object message, LogType logType)
        {
            if (!ConfigManager.Log_Unity_Debug.Value)
                return;

            Log($"[Unity] {message}", logType);
        }

        private static void Log(object message, LogType logType)
        {
            string log = message?.ToString() ?? "";

            LogPanel.Log(log, logType);

            switch (logType)
            {
                case LogType.Assert:
                case LogType.Log:
                    Loader.OnLogMessage(log);
                    break;

                case LogType.Warning:
                    Loader.OnLogWarning(log);
                    break;

                case LogType.Error:
                case LogType.Exception:
                    Loader.OnLogError(log);
                    break;
            }
        }

        #endregion


        #region LEGACY FOLDER MIGRATION

        // Can be removed eventually. For migration from <4.7.0
        static void CheckLegacyExplorerFolder()
        {
            return;
            string legacyPath = Path.Combine(Loader.ExplorerFolderDestination, "UnityExplorer");
            if (Directory.Exists(legacyPath))
            {
                LogWarning($"Attempting to migrate old 'UnityExplorer/' folder to 'sinai-dev-UnityExplorer/'...");

                // If new folder doesn't exist yet, let's just use Move().
                if (!Directory.Exists(ExplorerFolder))
                {
                    try
                    {
                        Directory.Move(legacyPath, ExplorerFolder);
                        Log("Migrated successfully.");
                    }
                    catch (Exception ex)
                    {
                        LogWarning($"Exception migrating folder: {ex}");
                    }
                }
                else // We have to merge
                {
                    try
                    {
                        CopyAll(new(legacyPath), new(ExplorerFolder));
                        Directory.Delete(legacyPath, true);
                        Log("Migrated successfully.");
                    }
                    catch (Exception ex)
                    {
                        LogWarning($"Exception migrating folder: {ex}");
                    }
                }
            }
        }

        public static void CopyAll(DirectoryInfo source, DirectoryInfo target)
        {
            Directory.CreateDirectory(target.FullName);

            // Copy each file into it's new directory.
            foreach (FileInfo fi in source.GetFiles())
                fi.MoveTo(Path.Combine(target.ToString(), fi.Name));

            // Copy each subdirectory using recursion.
            foreach (DirectoryInfo diSourceSubDir in source.GetDirectories())
            {
                DirectoryInfo nextTargetSubDir = target.CreateSubdirectory(diSourceSubDir.Name);
                CopyAll(diSourceSubDir, nextTargetSubDir);
            }
        }

        #endregion
    }
}
