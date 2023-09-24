using System.Diagnostics;
using System.Text;
using System.Text.RegularExpressions;
using Mono.CSharp;
using RedLoader;
using UnityEngine.Rendering;
using UnityExplorer.CacheObject;
using UnityExplorer.CacheObject.Views;
using UnityExplorer.Config;
using UniverseLib.UI;
using UniverseLib.UI.Models;
using UniverseLib.UI.Widgets.ScrollView;
using Object = UnityEngine.Object;

namespace UnityExplorer.UI.Panels;

public class MaterialInspectorPanel : UEPanel
{
    public override string Name => "Material Inspector";
    public override UIManager.Panels PanelType => UIManager.Panels.MaterialInspector;

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

    public static MaterialInspectorPanel Instance;
    
    private static GameObject Container;
    private static GameObject ScrollContainer;
    
    private static List<MaterialPropertyInfo> CurrentProperties = new();
    private static Material CurrentMaterial;
    private static readonly Regex _regex = new(@"[^a-zA-Z0-9_]", RegexOptions.Compiled);


    public MaterialInspectorPanel(UIBase owner) : base(owner)
    {
        Instance = this;
        InspectorManager.OnInspectedTabsChanged += OnInspectedTabsChanged;
    }

    private static void OnInspectedTabsChanged()
    {
        var mat = InspectorManager.ActiveInspector.Target.TryCast<Material>();
        ExplorerCore.Log($"Inspecting material {(bool)mat}");
        if (mat)
        {
            UIManager.SetPanelActive(UIManager.Panels.MaterialInspector, true);
            Inspect(mat);
        }
    }

    public override void SetActive(bool active)
    {
        base.SetActive(active);

        UIManager.OverlayGroup.gameObject.SetActive(active);
    }

    public static void Inspect(Material mat)
    {
        ExplorerCore.Log("[MaterialInspector] Inspecting " + mat.name);

        ClearContainer();
        
        CreatePropertyInspectors(mat);
    }

    private static void CreatePropertyInspectors(Material mat)
    {
        CurrentProperties.Clear();
        CurrentMaterial = mat;
        
        var btn = UIFactory.CreateButton(Container, "ExportToClassBtn", "Export to Class", new Color(0.3f, 0.3f, 0.3f));
        UIFactory.SetLayoutElement(btn.Component.gameObject, minWidth: 70, minHeight: 25, flexibleWidth: 0, flexibleHeight: 0);
        btn.OnClick += () =>
        {
            var dir = new DirectoryInfo("ExportedMaterials");
            if (!dir.Exists)
                dir.Create();
            var className = SanitizeVariableName(CurrentMaterial.shader.name);
            var file = Path.Combine(dir.FullName, $"{className}.cs");
            ExportMaterial(file);
            RLog.Msg(System.Drawing.Color.LawnGreen, $"Exported material ({CurrentMaterial.shader.name}) to {file}");
        };
        
        var shader = mat.shader;
        var count = shader.GetPropertyCount();
        for (int i = 0; i < count; i++)
        {
            CreateValueInspector(mat, i);
        }
    }
    
    private static string SanitizeVariableName(string input)
    {
        if (string.IsNullOrEmpty(input))
            return string.Empty;

        string sanitized = input.Replace(' ', '_');

        if (char.IsDigit(sanitized[0]))
            sanitized = "_" + sanitized;

        sanitized = _regex.Replace(sanitized, string.Empty);

        return sanitized;
    }
    
    private static void ExportMaterial(string fileName)
    {
        var writer = new StringBuilder();
        var className = SanitizeVariableName(CurrentMaterial.shader.name);
        
        writer.AppendLine("using UnityEngine;");
        writer.AppendLine();
        writer.AppendLine($"public class {className}");
        writer.AppendLine("{");
        
        foreach (var prop in CurrentProperties)
        {
            var sanitizedProp = SanitizeVariableName(prop.Name);
            var sanitizedPropId = SanitizeVariableName(prop.Name) + "Id";
            writer.AppendLine($"\t\tprivate const int {sanitizedPropId} = Shader.PropertyToID(\"{prop.Name}\");");
            writer.AppendLine($"\t\tpublic {prop.PropertyType} {sanitizedProp}");
            writer.AppendLine("\t\t{");
            writer.AppendLine($"\t\t\tget => Material.{prop.GenerateGetter(sanitizedPropId)};");
            writer.AppendLine($"\t\t\tset => Material.{prop.GenerateSetter(sanitizedPropId, "value")};");
            writer.AppendLine("\t\t}");
            writer.AppendLine();
        }
        
        writer.AppendLine("\t\tpublic readonly Material Material;");
        writer.AppendLine();
        
        writer.AppendLine($"\t\tpublic {className}(Material material)");
        writer.AppendLine("\t\t{");
        writer.AppendLine("\t\t\tMaterial = material;");
        writer.AppendLine("\t\t}");
        
        writer.AppendLine("}");
        
        File.WriteAllText(fileName, writer.ToString());
    }

    private static void CreateValueInspector(Material mat, int index)
    {
        var prop = MaterialPropertyInfo.Create(mat, index);
        CurrentProperties.Add(prop);
        
        var cell = new ConfigEntryCell();
        cell.OnlySubContent = ShouldShowSubContent(prop.PropertyType);
        cell.CreateContent(Container);
        var entry = new ValueInspectorObject(prop, mat);
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

    private class MaterialPropertyInfo
    {
        public string Name;
        public ShaderPropertyType Type;

        public MaterialPropertyInfo(string name, ShaderPropertyType type)
        {
            Name = name;
            Type = type;
        }
        
        public static MaterialPropertyInfo Create(Material mat, int index)
        {
            return Create(mat.shader, index);
        }
        
        public static MaterialPropertyInfo Create(Shader shader, int index)
        {
            var name = shader.GetPropertyName(index);
            var type = shader.GetPropertyType(index);
            return new MaterialPropertyInfo(name, type);
        }
        
        public void SetValue(Material mat, object value)
        {
            switch (Type)
            {
                case ShaderPropertyType.Color:
                    mat.SetColor(Name, value.TryCast<Color>());
                    break;
                case ShaderPropertyType.Vector:
                    mat.SetVector(Name, value.TryCast<Vector4>());
                    break;
                case ShaderPropertyType.Float:
                    mat.SetFloat(Name, (float) value);
                    break;
                case ShaderPropertyType.Range:
                    mat.SetFloat(Name, (float) value);
                    break;
                case ShaderPropertyType.Texture:
                    mat.SetTexture(Name, value.TryCast<Texture>());
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
        
        public string GenerateSetter(string propName, string valueName)
        {
            return Type switch {
                ShaderPropertyType.Color => $"SetColor({propName}, {valueName})",
                ShaderPropertyType.Vector => $"SetVector({propName}, {valueName})",
                ShaderPropertyType.Float => $"SetFloat({propName}, {valueName})",
                ShaderPropertyType.Range => $"SetFloat({propName}, {valueName})",
                ShaderPropertyType.Texture => $"SetTexture({propName}, {valueName})",
                _ => throw new ArgumentOutOfRangeException()
            };
        }
        
        public string GenerateGetter(string propName)
        {
            return Type switch {
                ShaderPropertyType.Color => $"GetColor({propName})",
                ShaderPropertyType.Vector => $"GetVector({propName})",
                ShaderPropertyType.Float => $"GetFloat({propName})",
                ShaderPropertyType.Range => $"GetFloat({propName})",
                ShaderPropertyType.Texture => $"GetTexture({propName})",
                _ => throw new ArgumentOutOfRangeException()
            };
        }
        
        public object GetValue(Material mat)
        {
            switch (Type)
            {
                case ShaderPropertyType.Color:
                    return mat.GetColor(Name);
                case ShaderPropertyType.Vector:
                    return mat.GetVector(Name);
                case ShaderPropertyType.Float:
                    return mat.GetFloat(Name);
                case ShaderPropertyType.Range:
                    return mat.GetFloat(Name);
                case ShaderPropertyType.Texture:
                    return mat.GetTexture(Name);
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
        
        public Type PropertyType
        {
            get
            {
                switch (Type)
                {
                    case ShaderPropertyType.Color:
                        return typeof(Color);
                    case ShaderPropertyType.Vector:
                        return typeof(Vector4);
                    case ShaderPropertyType.Float:
                        return typeof(float);
                    case ShaderPropertyType.Range:
                        return typeof(float);
                    case ShaderPropertyType.Texture:
                        return typeof(Texture);
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }
    }
    
    private class ValueInspectorObject : CacheObjectBase
    {
        public override bool ShouldAutoEvaluate => false;
        public override bool HasArguments => false;
        public override bool CanWrite => true;

        private readonly MaterialPropertyInfo _propertyInfo;
        private readonly Material Material;

        public ValueInspectorObject(MaterialPropertyInfo prop, Material mat)
        {
            _propertyInfo = prop;
            NameLabelText = prop.Name;
            FallbackType = prop.PropertyType;
            Material = mat;
        }
        
        public override void TrySetUserValue(object value)
        {
            if (_propertyInfo == null)
            {
                return;
            }
            
            _propertyInfo.SetValue(Material, value);
            
            UpdateValue();
        }

        public void UpdateValue()
        {
            SetValueFromSource(_propertyInfo.GetValue(Material));
        }

        protected override bool TryAutoEvaluateIfUnitialized(CacheObjectCell cell)
        {
            return true;
        }
    }
}