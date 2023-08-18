﻿using Il2CppInterop.Runtime;

using UnityExplorer.Config;
using UnityExplorer.CSConsole;
using UnityExplorer.Inspectors;
using UnityExplorer.UI.Panels;
using UnityExplorer.UI.Widgets;
using UniverseLib.Input;
using UniverseLib.UI;
using UniverseLib.UI.Models;

#if SONS
using RedLoader;
using Sons.Ai.Vail;
using TheForest.Utils;
using InputSystem = Sons.Input.InputSystem;
#endif

namespace UnityExplorer.UI
{
    public static class UIManager
    {
        public enum Panels
        {
            ObjectExplorer,
            Inspector,
            CSConsole,
            Options,
            ConsoleLog,
            AutoCompleter,
            UIInspectorResults,
            HookManager,
            Clipboard,
            Freecam
        }

        public enum VerticalAnchor
        {
            Top,
            Bottom
        }

        public static VerticalAnchor NavbarAnchor = VerticalAnchor.Top;

        public static bool Initializing { get; internal set; } = true;

        internal static UIBase UiBase { get; private set; }
        public static GameObject UIRoot => UiBase?.RootObject;
        public static RectTransform UIRootRect { get; private set; }
        public static Canvas UICanvas { get; private set; }

        internal static readonly Dictionary<Panels, UEPanel> UIPanels = new();

        public static RectTransform NavBarRect;
        public static GameObject NavbarTabButtonHolder;
        private static readonly Vector2 NAVBAR_DIMENSIONS = new(1160f, 35f);

        private static ButtonRef closeBtn;
        private static TimeScaleWidget timeScaleWidget;

        private static int lastScreenWidth;
        private static int lastScreenHeight;

        public static bool ShouldRenderGame = true;

        public static bool ShowMenu
        {
            get => UiBase != null && UiBase.Enabled;
            set
            {
                if (UiBase == null || !UIRoot || UiBase.Enabled == value)
                    return;

                UniversalUI.SetUIActive(ExplorerCore.GUID, value);
                UniversalUI.SetUIActive(MouseInspector.UIBaseGUID, value);

                if (!ShouldRenderGame)
                {
                    if (value)
                    {
                        ShouldRenderGame = true;
                        ToggleRendering();
                    }
                    else
                    {
                        ToggleRendering();
                        ShouldRenderGame = false;
                    }
                }
            }
        }

        // Initialization

        internal static void InitUI()
        {
            UiBase = UniversalUI.RegisterUI<ExplorerUIBase>(ExplorerCore.GUID, Update);

#if SONS
            var cursorState = UIRoot.AddComponent<Sons.Input.InputCursorState>();
            cursorState._hardwareCursor = true;
            cursorState._enabled = true;
            cursorState._priority = 999;
            
            UIRoot.AddComponent<Sons.Input.InputActionMapState>();
            
            UIRoot.AddComponent<UnityEngine.EventSystems.EventSystem>();
            UIRoot.AddComponent<UnityEngine.EventSystems.StandaloneInputModule>();
            UIRoot.AddComponent<UnityEngine.EventSystems.BaseInput>();
#endif

            UIRootRect = UIRoot.GetComponent<RectTransform>();
            UICanvas = UIRoot.GetComponent<Canvas>();

            DisplayManager.Init();

            Display display = DisplayManager.ActiveDisplay;
            lastScreenWidth = display.renderingWidth;
            lastScreenHeight = display.renderingHeight;

            // Create UI.
            CreateTopNavBar();
            // This could be automated with Assembly.GetTypes(),
            // but the order is important and I'd have to write something to handle the order.
            UIPanels.Add(Panels.AutoCompleter, new AutoCompleteModal(UiBase));
            UIPanels.Add(Panels.ObjectExplorer, new ObjectExplorerPanel(UiBase));
            UIPanels.Add(Panels.Inspector, new InspectorPanel(UiBase));
            UIPanels.Add(Panels.CSConsole, new CSConsolePanel(UiBase));
            UIPanels.Add(Panels.HookManager, new HookManagerPanel(UiBase));
            UIPanels.Add(Panels.Freecam, new FreeCamPanel(UiBase));
            UIPanels.Add(Panels.Clipboard, new ClipboardPanel(UiBase));
            UIPanels.Add(Panels.ConsoleLog, new LogPanel(UiBase));
            UIPanels.Add(Panels.Options, new OptionsPanel(UiBase));
            UIPanels.Add(Panels.UIInspectorResults, new MouseInspectorResultsPanel(UiBase));

            MouseInspector.inspectorUIBase = UniversalUI.RegisterUI(MouseInspector.UIBaseGUID, null);
            new MouseInspector(MouseInspector.inspectorUIBase);

            // Call some initialize methods
            Notification.Init();
            ConsoleController.Init();

            // Failsafe fix, in some games all dropdowns displayed values are blank on startup for some reason.
            foreach (Dropdown dropdown in UIRoot.GetComponentsInChildren<Dropdown>(true))
                dropdown.RefreshShownValue();

            Initializing = false;

            if (ConfigManager.Hide_On_Startup.Value)
                ShowMenu = false;
        }

        // Main UI Update loop

        public static void Update()
        {
            if (!UIRoot)
                return;

            // If we are doing a Mouse Inspect, we don't need to update anything else.
            if (MouseInspector.Instance.TryUpdate())
                return;

            // Update Notification modal
            Notification.Update();

            // Check forceUnlockMouse toggle
            if (InputManager.GetKeyDown(ConfigManager.Force_Unlock_Toggle.Value))
                UniverseLib.Config.ConfigManager.Force_Unlock_Mouse = !UniverseLib.Config.ConfigManager.Force_Unlock_Mouse;

            // update the timescale value
            timeScaleWidget.Update();

            // check screen dimension change
            Display display = DisplayManager.ActiveDisplay;
            if (display.renderingWidth != lastScreenWidth || display.renderingHeight != lastScreenHeight)
                OnScreenDimensionsChanged();
        }

        // Panels

        public static UEPanel GetPanel(Panels panel) => UIPanels[panel];

        public static T GetPanel<T>(Panels panel) where T : UEPanel => (T)UIPanels[panel];

        public static void TogglePanel(Panels panel)
        {
            UEPanel uiPanel = GetPanel(panel);
            SetPanelActive(panel, !uiPanel.Enabled);
        }

        public static void SetPanelActive(Panels panelType, bool active)
        {
            GetPanel(panelType).SetActive(active);
        }

        public static void SetPanelActive(UEPanel panel, bool active)
        {
            panel.SetActive(active);
        }

        // navbar

        public static void SetNavBarAnchor()
        {
            switch (NavbarAnchor)
            {
                case VerticalAnchor.Top:
                    NavBarRect.anchorMin = new Vector2(0.5f, 1f);
                    NavBarRect.anchorMax = new Vector2(0.5f, 1f);
                    NavBarRect.anchoredPosition = new Vector2(NavBarRect.anchoredPosition.x, 0);
                    NavBarRect.sizeDelta = NAVBAR_DIMENSIONS;
                    break;

                case VerticalAnchor.Bottom:
                    NavBarRect.anchorMin = new Vector2(0.5f, 0f);
                    NavBarRect.anchorMax = new Vector2(0.5f, 0f);
                    NavBarRect.anchoredPosition = new Vector2(NavBarRect.anchoredPosition.x, 35);
                    NavBarRect.sizeDelta = NAVBAR_DIMENSIONS;
                    break;
            }
        }

        // listeners

        private static void OnScreenDimensionsChanged()
        {
            Display display = DisplayManager.ActiveDisplay;
            lastScreenWidth = display.renderingWidth;
            lastScreenHeight = display.renderingHeight;

            foreach (KeyValuePair<Panels, UEPanel> panel in UIPanels)
            {
                panel.Value.EnsureValidSize();
                panel.Value.EnsureValidPosition();
                panel.Value.Dragger.OnEndResize();
            }
        }

        private static void OnCloseButtonClicked()
        {
            ShowMenu = false;
        }

        private static void Master_Toggle_OnValueChanged(KeyCode val)
        {
            closeBtn.ButtonText.text = val.ToString();
        }

        private static void ToggleRendering()
        {
            var objects = Resources.FindObjectsOfTypeAll(Il2CppType.Of<Camera>());
            ShouldRenderGame = !ShouldRenderGame;
            foreach (var obj in objects)
            {
                var cam = obj.Cast<Camera>();
                if (!cam.name.Contains("Main"))
                    continue;
                cam.enabled = ShouldRenderGame;
                break;
            }
        }

        // UI Construction

        private static void CreateTopNavBar()
        {
            GameObject navbarPanel = UIFactory.CreateUIObject("MainNavbar", UIRoot);
            UIFactory.SetLayoutGroup<HorizontalLayoutGroup>(navbarPanel, false, false, true, true, 5, 4, 4, 4, 4, TextAnchor.MiddleCenter);
            navbarPanel.AddComponent<Image>().color = new Color(0.1f, 0.1f, 0.1f);
            NavBarRect = navbarPanel.GetComponent<RectTransform>();
            NavBarRect.pivot = new Vector2(0.5f, 1f);

            NavbarAnchor = ConfigManager.Main_Navbar_Anchor.Value;
            SetNavBarAnchor();
            ConfigManager.Main_Navbar_Anchor.OnValueChanged += (VerticalAnchor val) =>
            {
                NavbarAnchor = val;
                SetNavBarAnchor();
            };

            // UnityExplorer title

            string titleTxt = $"UE <i><color=grey>{ExplorerCore.VERSION}</color></i>";
            Text title = UIFactory.CreateLabel(navbarPanel, "Title", titleTxt, TextAnchor.MiddleCenter, default, true, 14);
            UIFactory.SetLayoutElement(title.gameObject, minWidth: 75, flexibleWidth: 0);

            // panel tabs

            NavbarTabButtonHolder = UIFactory.CreateUIObject("NavTabButtonHolder", navbarPanel);
            UIFactory.SetLayoutElement(NavbarTabButtonHolder, minHeight: 25, flexibleHeight: 999, flexibleWidth: 999);
            UIFactory.SetLayoutGroup<HorizontalLayoutGroup>(NavbarTabButtonHolder, false, true, true, true, 4, 2, 2, 2, 2);

            // Time scale widget
            timeScaleWidget = new(navbarPanel);

            //spacer
            GameObject spacer = UIFactory.CreateUIObject("Spacer", navbarPanel);
            UIFactory.SetLayoutElement(spacer, minWidth: 15);

            // Hide menu button
            var renderBtn = UIFactory.CreateButton(navbarPanel, "RenderButton", "Render");
            UIFactory.SetLayoutElement(renderBtn.Component.gameObject, minHeight: 25, minWidth: 60, flexibleWidth: 0);
            RuntimeHelper.SetColorBlock(renderBtn.Component, new Color(0.35f, 0.63f, 0.33f),
                new Color(0.35f, 0.79f, 0.16f), new Color(0.33f, 0.44f, 0.31f));
            renderBtn.OnClick += () =>
            {
                ToggleRendering();
                
                if (ShouldRenderGame)
                {
                    RuntimeHelper.SetColorBlock(renderBtn.Component, new Color(0.35f, 0.63f, 0.33f),
                        new Color(0.35f, 0.79f, 0.16f), new Color(0.33f, 0.44f, 0.31f));
                }
                else
                {
                    RuntimeHelper.SetColorBlock(renderBtn.Component, new Color(0.34f, 0.34f, 0.34f),
                        new Color(0.35f, 0.63f, 0.33f), new Color(0.21f, 0.21f, 0.21f));
                }
            };

#if SONS
            var vailBtn = UIFactory.CreateButton(navbarPanel, "VailButton", "Vail");
            UIFactory.SetLayoutElement(vailBtn.Component.gameObject, minHeight: 25, minWidth: 60, flexibleWidth: 0);
            RuntimeHelper.SetColorBlock(vailBtn.Component, new Color(0.35f, 0.63f, 0.33f),
                new Color(0.35f, 0.79f, 0.16f), new Color(0.33f, 0.44f, 0.31f));
            vailBtn.OnClick += () =>
            {
                if (VailWorldSimulation._instance == null)
                    return;

                VailWorldSimulation.SetPaused(!VailWorldSimulation._instance._aiPaused);
                
                if (!VailWorldSimulation._instance._aiPaused)
                {
                    RuntimeHelper.SetColorBlock(vailBtn.Component, new Color(0.35f, 0.63f, 0.33f),
                        new Color(0.35f, 0.79f, 0.16f), new Color(0.33f, 0.44f, 0.31f));
                }
                else
                {
                    RuntimeHelper.SetColorBlock(vailBtn.Component, new Color(0.34f, 0.34f, 0.34f),
                        new Color(0.35f, 0.63f, 0.33f), new Color(0.21f, 0.21f, 0.21f));
                }
            };
#endif
            
            closeBtn = UIFactory.CreateButton(navbarPanel, "CloseButton", ConfigManager.Master_Toggle.Value.ToString());
            UIFactory.SetLayoutElement(closeBtn.Component.gameObject, minHeight: 25, minWidth: 60, flexibleWidth: 0);
            RuntimeHelper.SetColorBlock(closeBtn.Component, new Color(0.63f, 0.32f, 0.31f),
                new Color(0.81f, 0.25f, 0.2f), new Color(0.6f, 0.18f, 0.16f));

            ConfigManager.Master_Toggle.OnValueChanged += Master_Toggle_OnValueChanged;
            closeBtn.OnClick += OnCloseButtonClicked;
        }
    }
}
