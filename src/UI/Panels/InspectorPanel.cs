﻿using UnityExplorer.Inspectors;
using UniverseLib.UI;

namespace UnityExplorer.UI.Panels
{
    public class InspectorPanel : UEPanel
    {
        public static InspectorPanel Instance { get; private set; }

        public override string Name => "Inspector";
        public override UIManager.Panels PanelType => UIManager.Panels.Inspector;
        public override bool ShouldSaveActiveState => false;

        public override int MinWidth => 810;
        public override int MinHeight => 350;
        public override Vector2 DefaultAnchorMin => new(0.35f, 0.175f);
        public override Vector2 DefaultAnchorMax => new(0.8f, 0.925f);

        public GameObject NavbarHolder;
        public Dropdown MouseInspectDropdown;
        public GameObject ContentHolder;
        public RectTransform ContentRect;

        public static float CurrentPanelWidth => Instance.Rect.rect.width;
        public static float CurrentPanelHeight => Instance.Rect.rect.height;

        public InspectorPanel(UIBase owner) : base(owner)
        {
            Instance = this;
        }

        public override void Update()
        {
            InspectorManager.Update();
        }

        public override void OnFinishResize()
        {
            base.OnFinishResize();

            InspectorManager.PanelWidth = this.Rect.rect.width;
            InspectorManager.OnPanelResized(Rect.rect.width);
        }

        protected override void ConstructPanelContent()
        {
            GameObject closeHolder = this.TitleBar.transform.Find("CloseHolder").gameObject;

            // Inspect under mouse dropdown on title bar

            GameObject mouseDropdown = UIFactory.CreateDropdown(closeHolder, "MouseInspectDropdown", out MouseInspectDropdown, "Mouse Inspect", 14,
                MouseInspector.OnDropdownSelect);
            UIFactory.SetLayoutElement(mouseDropdown, minHeight: 25, minWidth: 140);
            MouseInspectDropdown.options.Add(new Dropdown.OptionData("Mouse Inspect"));
            MouseInspectDropdown.options.Add(new Dropdown.OptionData("World"));
            MouseInspectDropdown.options.Add(new Dropdown.OptionData("UI"));
            mouseDropdown.transform.SetSiblingIndex(0);

            // add close all button to titlebar

            UniverseLib.UI.Models.ButtonRef closeAllBtn = UIFactory.CreateButton(closeHolder.gameObject, "CloseAllBtn", "Close All",
                new Color(0.3f, 0.2f, 0.2f));
            UIFactory.SetLayoutElement(closeAllBtn.Component.gameObject, minHeight: 25, minWidth: 80);
            closeAllBtn.Component.transform.SetSiblingIndex(closeAllBtn.Component.transform.GetSiblingIndex() - 1);
            closeAllBtn.OnClick += InspectorManager.CloseAllTabs;

            // this.UIRoot.GetComponent<Mask>().enabled = false;

            UIFactory.SetLayoutGroup<VerticalLayoutGroup>(this.ContentRoot, true, true, true, true, 4, padLeft: 5, padRight: 5);

            this.NavbarHolder = UIFactory.CreateGridGroup(this.ContentRoot, "Navbar", new Vector2(200, 22), new Vector2(4, 4),
                new Color(0.05f, 0.05f, 0.05f));
            //UIFactory.SetLayoutElement(NavbarHolder, flexibleWidth: 9999, minHeight: 0, preferredHeight: 0, flexibleHeight: 9999);
            NavbarHolder.AddComponent<ContentSizeFitter>().verticalFit = ContentSizeFitter.FitMode.PreferredSize;

            this.ContentHolder = UIFactory.CreateVerticalGroup(this.ContentRoot, "ContentHolder", true, true, true, true, 0, default,
                new Color(0.1f, 0.1f, 0.1f));
            UIFactory.SetLayoutElement(ContentHolder, flexibleHeight: 9999);
            ContentRect = ContentHolder.GetComponent<RectTransform>();

            this.SetActive(false);
        }
    }
}