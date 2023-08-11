using System.Security;
using Il2CppInterop.Common.XrefScans;
using Il2CppInterop.Runtime.XrefScans;
using UniverseLib.UI;
using UniverseLib.UI.Models;
using UniverseLib.UI.Widgets.ScrollView;
using Object = Il2CppSystem.Object;

namespace UnityExplorer.Hooks
{
    public class AddHookCell : ICell
    {
        public bool Enabled => UIRoot.activeSelf;

        public RectTransform Rect { get; set; }
        public GameObject UIRoot { get; set; }

        public float DefaultHeight => 30;

        public Text MethodNameLabel;
        public ButtonRef HookButton;

        public int CurrentDisplayedIndex;

        private void OnHookClicked()
        {
            HookCreator.AddHookClicked(CurrentDisplayedIndex);
        }

        public void Enable()
        {
            this.UIRoot.SetActive(true);
        }

        public void Disable()
        {
            this.UIRoot.SetActive(false);
        }

        public GameObject CreateContent(GameObject parent)
        {
            UIRoot = UIFactory.CreateUIObject(this.GetType().Name, parent, new Vector2(100, 30));
            Rect = UIRoot.GetComponent<RectTransform>();
            UIFactory.SetLayoutGroup<HorizontalLayoutGroup>(UIRoot, false, false, true, true, 5, childAlignment: TextAnchor.UpperLeft);
            UIFactory.SetLayoutElement(UIRoot, minWidth: 100, flexibleWidth: 9999, minHeight: 30, flexibleHeight: 600);
            UIRoot.AddComponent<ContentSizeFitter>().verticalFit = ContentSizeFitter.FitMode.PreferredSize;

            HookButton = UIFactory.CreateButton(UIRoot, "HookButton", "Hook", new Color(0.2f, 0.25f, 0.2f));
            UIFactory.SetLayoutElement(HookButton.Component.gameObject, minHeight: 25, minWidth: 100);
            HookButton.OnClick += OnHookClicked;
            
            var printBtn = UIFactory.CreateButton(UIRoot, "PrintMethodButton", "Print", new Color(0.2f, 0.25f, 0.2f));
            UIFactory.SetLayoutElement(printBtn.Component.gameObject, minHeight: 25, minWidth: 100);
            printBtn.OnClick += () =>
            {
                var method = HookCreator.GetMethod(CurrentDisplayedIndex);
                if (method == null)
                {
                    return;
                }

                PrintMethodCalls(method);
            };

            MethodNameLabel = UIFactory.CreateLabel(UIRoot, "MethodName", "NOT SET", TextAnchor.MiddleLeft);
            UIFactory.SetLayoutElement(MethodNameLabel.gameObject, minHeight: 25, flexibleWidth: 9999);

            return UIRoot;
        }
        
        [SecurityCritical]
        public static void PrintMethodCalls(MethodBase method)
        {
            try
            {
                var instances = XrefScanner.XrefScan(method);
                ExplorerCore.Log("Uses:");
                foreach (var instance in instances)
                {
                    var t = instance.Type;

                    if (t == XrefType.Global)
                    {
                        Object globalObject = instance.ReadAsObject();
                        if (globalObject == null)
                        {
                            continue;
                        }
                    
                        string usedString = globalObject.ToString();
                        ExplorerCore.Log($"\t-str: {usedString}");
                    
                        continue;
                    }
                    var methodObject = instance.TryResolve();
                    if (methodObject == null)
                    {
                        //Log($"Failed to resolve method object for {instance}");
                        continue;
                    }

                    var declType = methodObject.DeclaringType;
                    var name = declType?.Name ?? "-";
                    name += "." + methodObject.Name;
                    ExplorerCore.Log($"\t- {name}");
                }
            }
            catch (AccessViolationException ave)
            {
                ExplorerCore.Log(ave);
            }

            try
            {
                ExplorerCore.Log("Used by:");
                var instances2 = XrefScanner.UsedBy(method);
                foreach (var instance in instances2)
                {
                    var t = instance.Type;
                    if (t == XrefType.Global)
                    {
                        Object globalObject = instance.ReadAsObject();
                        if (globalObject == null)
                        {
                            continue;
                        }
                    
                        string usedString = globalObject.ToString();
                        ExplorerCore.Log($"\t-str: {usedString}");
                    
                        continue;
                    }
                
                    var methodObject = instance.TryResolve();
                    if (methodObject == null)
                    {
                        //Log($"Failed to resolve method object for {instance}");
                        continue;
                    }

                    var declType = methodObject.DeclaringType;
                    var name = declType?.Name ?? "-";
                    name += "." + methodObject.Name;
                    ExplorerCore.Log($"\t- {name}");
                }
            }
            catch (AccessViolationException ave)
            {
                ExplorerCore.Log(ave);
            }
        }
    }
}
