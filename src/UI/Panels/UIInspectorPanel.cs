using System.Diagnostics;
using Mono.CSharp;
using UnityExplorer.CacheObject;
using UnityExplorer.CacheObject.Views;
using UnityExplorer.Config;
using UniverseLib.UI;
using UniverseLib.UI.Models;
using UniverseLib.UI.Widgets.ScrollView;
using Object = UnityEngine.Object;

namespace UnityExplorer.UI.Panels;

public class UIInspectorPanel : UEPanel
{
    public override string Name => "UI Inspector";
    public override UIManager.Panels PanelType => UIManager.Panels.UIInspector;

    public override int MinWidth => 350;
    public override int MinHeight => 75;
    public override Vector2 DefaultAnchorMin => new(0.65f, 0.7f);
    public override Vector2 DefaultAnchorMax => new(1f, 1f);

    public override bool ShouldSaveActiveState => true;
    public override bool ShowByDefault => false;

    public static RectTransform CurrentRect;
    public static LayoutElement CurrentLayoutElement;
    public static HorizontalOrVerticalLayoutGroup CurrentLayoutGroup;
    public static Image CurrentImage;

    public static bool IsActive => Instance.Enabled;

    public static UIInspectorPanel Instance;
    
    private static GameObject Container;
    private static GameObject ScrollContainer;


    public UIInspectorPanel(UIBase owner) : base(owner)
    {
        Instance = this;
        InspectorManager.OnInspectedTabsChanged += OnInspectedTabsChanged;
    }

    private static void OnInspectedTabsChanged()
    {
        var gameObject = InspectorManager.ActiveInspector.Target.TryCast<GameObject>();
        ExplorerCore.Log($"Inspecting game object {(bool)gameObject}");
        
        if (gameObject && (bool)gameObject.GetComponent<RectTransform>())
        {
            Inspect(gameObject);
            UIManager.SetPanelActive(UIManager.Panels.UIInspector, true);
            return;
        }
        
        UIManager.SetPanelActive(UIManager.Panels.UIInspector, false);
    }

    public override void SetActive(bool active)
    {
        base.SetActive(active);

        UIManager.OverlayGroup.gameObject.SetActive(active);
    }

    public static void Inspect(GameObject gameObject)
    {
        ExplorerCore.Log("[UIInspector] Inspecting " + gameObject.name);
        
        CurrentRect = gameObject.GetComponent<RectTransform>();
        CurrentLayoutElement = gameObject.GetComponent<LayoutElement>();
        CurrentLayoutGroup = gameObject.GetComponent<HorizontalOrVerticalLayoutGroup>();
        CurrentImage = gameObject.GetComponent<Image>();
        
        ClearContainer();
        
        if(CurrentRect)
            CreateRectInspector();
        
        if(CurrentLayoutElement)
            CreateLayoutElementInspector();
        
        if(CurrentLayoutGroup)
            CreateLayoutGroupInspector();
        
        if(CurrentImage)
            CreateImageInspector();
    }

    private static void CreateRectInspector()
    {
        CreateValueInspector(CurrentRect, "anchoredPosition");
        CreateValueInspector(CurrentRect, "sizeDelta");
    }

    private static void CreateLayoutElementInspector()
    {
        CreateValueInspector(CurrentLayoutElement, nameof(LayoutElement.flexibleWidth));
        CreateValueInspector(CurrentLayoutElement, nameof(LayoutElement.flexibleHeight));
        
        CreateValueInspector(CurrentLayoutElement, nameof(LayoutElement.preferredWidth));
        CreateValueInspector(CurrentLayoutElement, nameof(LayoutElement.preferredHeight));
    }

    private static void CreateLayoutGroupInspector()
    {
        CreateValueInspector(CurrentLayoutGroup, nameof(HorizontalOrVerticalLayoutGroup.spacing));
        CreateValueInspector(CurrentLayoutGroup, nameof(HorizontalOrVerticalLayoutGroup.childControlWidth));
        CreateValueInspector(CurrentLayoutGroup, nameof(HorizontalOrVerticalLayoutGroup.childControlHeight));
        CreateValueInspector(CurrentLayoutGroup, nameof(HorizontalOrVerticalLayoutGroup.childForceExpandWidth));
        CreateValueInspector(CurrentLayoutGroup, nameof(HorizontalOrVerticalLayoutGroup.childForceExpandHeight));
        // CreateValueInspector(CurrentLayoutGroup, nameof(HorizontalOrVerticalLayoutGroup.padding));
    }

    private static void CreateImageInspector()
    {
        CreateValueInspector(CurrentImage, nameof(Image.color));
    }

    private static void CreateValueInspector(object owner, string propName)
    {
        CreateValueInspector(owner, owner.GetType().GetProperty(propName, BindingFlags.Public | BindingFlags.Instance));
    }

    private static void CreateValueInspector(object owner, PropertyInfo prop)
    {
        var cell = new ConfigEntryCell();
        cell.OnlySubContent = ShouldShowSubContent(prop.PropertyType);
        cell.CreateContent(Container);
        var entry = new ValueInspectorObject(prop, owner);
        entry.SetView(cell);
        entry.UpdateValue();
        entry.SetDataToCell(cell);
        if(cell.OnlySubContent)
            entry.OnCellSubContentToggle();
    }

    private static void ClearContainer()
    {
        RemoveAllChildren(Container);
    }
    
    public static void RemoveAllChildren(GameObject go)
    {
        var tr = go.transform;
        while (tr.childCount > 0)
        {
            Object.DestroyImmediate(tr.GetChild(0).gameObject);
        }
    }

    protected static bool ShouldShowSubContent(Type type)
    {
        return type != typeof(float) && 
               type != typeof(int) && 
               type != typeof(bool);
    }

    protected override void ConstructPanelContent()
    {
        ScrollContainer = UIFactory.CreateScrollView(ContentRoot, "VerticalContainer", out Container, out _, new Color(28/255f, 28/255f, 28/255f));
    }
    
    private class ValueInspectorObject : CacheObjectBase
    {
        public override bool ShouldAutoEvaluate => false;
        public override bool HasArguments => false;
        public override bool CanWrite => true;

        private readonly PropertyInfo _propertyInfo;
        private readonly object _propertyOwner;

        public ValueInspectorObject(PropertyInfo prop, object propOwner)
        {
            _propertyInfo = prop;
            _propertyOwner = propOwner;
            NameLabelText = prop.Name;
            FallbackType = prop.PropertyType;
        }
        
        public override void TrySetUserValue(object value)
        {
            if (_propertyInfo == null)
            {
                return;
            }
            
            _propertyInfo.SetValue(_propertyOwner, value);
            
            UpdateValue();
        }

        public void UpdateValue()
        {
            SetValueFromSource(_propertyInfo.GetValue(_propertyOwner));
        }

        protected override bool TryAutoEvaluateIfUnitialized(CacheObjectCell cell)
        {
            return true;
        }
    }
}