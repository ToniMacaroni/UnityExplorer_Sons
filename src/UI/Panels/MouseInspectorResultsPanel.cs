﻿using UnityExplorer.Inspectors.MouseInspectors;
using UniverseLib.UI;
using UniverseLib.UI.Widgets.ButtonList;
using UniverseLib.UI.Widgets.ScrollView;

namespace UnityExplorer.UI.Panels
{
    public class MouseInspectorResultsPanel : UEPanel
    {
        public override UIManager.Panels PanelType => UIManager.Panels.UIInspectorResults;

        public override string Name => "UI Inspector Results";

        public override int MinWidth => 500;
        public override int MinHeight => 500;
        public override Vector2 DefaultAnchorMin => new(0.5f, 0.5f);
        public override Vector2 DefaultAnchorMax => new(0.5f, 0.5f);
        
        public override bool CanDragAndResize => true;
        public override bool NavButtonWanted => false;
        public override bool ShouldSaveActiveState => false;
        public override bool ShowByDefault => false;

        private ButtonListHandler<GameObject, ButtonCell> dataHandler;
        private ScrollPool<ButtonCell> buttonScrollPool;

        public MouseInspectorResultsPanel(UIBase owner) : base(owner)
        {
        }

        public void ShowResults()
        {
            dataHandler.RefreshData();
            buttonScrollPool.Refresh(true, true);
        }

        private List<GameObject> GetEntries() => UiInspector.LastHitObjects;

        private bool ShouldDisplayCell(object cell, string filter) => true;

        private void OnCellClicked(int index)
        {
            if (index >= UiInspector.LastHitObjects.Count)
                return;

            InspectorManager.Inspect(UiInspector.LastHitObjects[index]);
        }

        private void SetCell(ButtonCell cell, int index)
        {
            if (index >= UiInspector.LastHitObjects.Count)
                return;

            GameObject obj = UiInspector.LastHitObjects[index];
            cell.Button.ButtonText.text = $"<color=cyan>{obj.name}</color> ({obj.transform.GetTransformPath(true)})";
        }

        public override void SetDefaultSizeAndPosition()
        {
            base.SetDefaultSizeAndPosition();

            this.Rect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 500f);
            this.Rect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 500f);
        }

        protected override void ConstructPanelContent()
        {
            dataHandler = new ButtonListHandler<GameObject, ButtonCell>(buttonScrollPool, GetEntries, SetCell, ShouldDisplayCell, OnCellClicked);

            buttonScrollPool = UIFactory.CreateScrollPool<ButtonCell>(this.ContentRoot, "ResultsList", out GameObject scrollObj,
                out GameObject scrollContent);

            buttonScrollPool.Initialize(dataHandler);
            UIFactory.SetLayoutElement(scrollObj, flexibleHeight: 9999);
        }
    }
}
