using UniverseLib.UI;
using UniverseLib.UI.Models;
using UniverseLib.UI.Widgets.ButtonList;

namespace UnityExplorer.UI.Widgets
{
    public class ComponentCell : ButtonCell
    {
        public Toggle BehaviourToggle;
        public ButtonRef DestroyButton;
        public ButtonRef DumpButton;

        public Action<bool, int> OnBehaviourToggled;
        public Action<int> OnDestroyClicked;
        public Action<int> OnDumpClicked;

        private void BehaviourToggled(bool val)
        {
            OnBehaviourToggled?.Invoke(val, CurrentDataIndex);
        }

        private void DestroyClicked()
        {
            OnDestroyClicked?.Invoke(CurrentDataIndex);
        }

        private void DumpClicked()
        {
            ExplorerCore.Log("OnDumpClicked1");
            OnDumpClicked?.Invoke(CurrentDataIndex);
        }

        public override GameObject CreateContent(GameObject parent)
        {
            GameObject root = base.CreateContent(parent);

            // Add mask to button so text doesnt overlap on Close button
            //this.Button.Component.gameObject.AddComponent<Mask>().showMaskGraphic = true;
            this.Button.ButtonText.horizontalOverflow = HorizontalWrapMode.Wrap;

            // Behaviour toggle

            GameObject toggleObj = UIFactory.CreateToggle(UIRoot, "BehaviourToggle", out BehaviourToggle, out Text behavText);
            UIFactory.SetLayoutElement(toggleObj, minHeight: 25, minWidth: 25);
            BehaviourToggle.onValueChanged.AddListener(BehaviourToggled);
            // put at first object
            toggleObj.transform.SetSiblingIndex(0);

            // Destroy button

            DestroyButton = UIFactory.CreateButton(UIRoot, "DestroyButton", "X", new Color(0.3f, 0.2f, 0.2f));
            UIFactory.SetLayoutElement(DestroyButton.Component.gameObject, minHeight: 21, minWidth: 25);
            DestroyButton.OnClick += DestroyClicked;
            
            DumpButton = UIFactory.CreateButton(UIRoot, "DumpButton", "D", new Color(0.2f, 0.25f, 0.3f));
            UIFactory.SetLayoutElement(DumpButton.Component.gameObject, minHeight: 21, minWidth: 25);
            DumpButton.OnClick += DumpClicked;

            return root;
        }
    }
}
