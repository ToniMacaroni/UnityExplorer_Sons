using MelonLoader.Utils;
using UnityExplorer.CacheObject;
using UnityExplorer.Config;

namespace UnityExplorer.Runtime
{
    // Not really that necessary anymore, can eventually just be refactored away into the few classes that use this class.

    public abstract class UERuntimeHelper
    {
        public static UERuntimeHelper Instance;

        public static void Init()
        {
#if CPP
            Instance = new Il2CppHelper();
#else
            Instance = new MonoHelper();
#endif
            Instance.SetupEvents();

            LoadBlacklistString(ConfigManager.Reflection_Signature_Blacklist.Value);
            ConfigManager.Reflection_Signature_Blacklist.OnValueChanged += (string val) =>
            {
                LoadBlacklistString(val);
            };
        }

        public abstract void SetupEvents();

        private static readonly HashSet<string> currentBlacklist = new();

        public virtual string[] DefaultReflectionBlacklist => new string[0];

        public static void LoadBlacklistString(string blacklist)
        {
            try
            {
                if (string.IsNullOrEmpty(blacklist) && !Instance.DefaultReflectionBlacklist.Any())
                    return;

                try
                {
                    string[] sigs = blacklist.Split(';');
                    foreach (string sig in sigs)
                    {
                        string s = sig.Trim();
                        if (string.IsNullOrEmpty(s))
                            continue;
                        if (!currentBlacklist.Contains(s))
                            currentBlacklist.Add(s);
                    }
                }
                catch (Exception ex)
                {
                    ExplorerCore.LogWarning($"Exception parsing blacklist string: {ex.ReflectionExToString()}");
                }

                foreach (string sig in Instance.DefaultReflectionBlacklist)
                {
                    if (!currentBlacklist.Contains(sig))
                        currentBlacklist.Add(sig);
                }

                Mono.CSharp.IL2CPP.Blacklist.SignatureBlacklist = currentBlacklist;
            }
            catch (Exception ex)
            {
                ExplorerCore.LogWarning($"Exception setting up reflection blacklist: {ex.ReflectionExToString()}");
            }
        }

        public static bool IsBlacklisted(MemberInfo member)
        {
            if (string.IsNullOrEmpty(member.DeclaringType?.Namespace))
                return false;

            string sig = $"{member.DeclaringType.FullName}.{member.Name}";

            return currentBlacklist.Contains(sig);
        }
        
        public static void DumpComponentData(object target)
        {
            var dumpComp = new DumpObjectController(target);

            var types = ReflectionUtility.GetAllBaseTypes(target.GetActualType()).ToList();
            if (!types.Contains(target.GetActualType()))
                types.Add(target.GetActualType());

            BindingFlags flags = BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance;

            var writer = new StringWriter();

            void PrintTypeInfo(Type t, string name, object value)
            {
                if (t == null || name == null || value == null)
                    return;

                ExplorerCore.Log($"Printing {name}");

                if (t == typeof(string))
                {
                    string prop1 = value as string;
                    writer.WriteLine($"string {name} = \"{prop1}\"");
                }
                else if (t == typeof(float))
                {
                    float prop2 = (float)value;
                    writer.WriteLine($"float {name} = {prop2.ToString("F1")}");
                }
                else if (t == typeof(UnityEngine.Object))
                {
                    writer.WriteLine($"Transform {name} = {((Transform)value).name}");
                }
                else
                {
                    writer.WriteLine($"{t.Name} {name} = {value}");
                }
            }

            foreach (var declaringType in types)
            {
                foreach (PropertyInfo prop in declaringType.GetProperties(flags))
                    if (prop.DeclaringType == declaringType && prop.CanRead)
                    {
                        if (prop.DeclaringType == null)
                        {
                            continue;
                        }

                        try
                        {
                            var p = new CacheProperty(prop);
                            p.Owner = dumpComp;
                            p.Evaluate();
                            PrintTypeInfo(prop.PropertyType, prop.Name, p.Value);
                        }
                        catch (Exception e)
                        {
                            ExplorerCore.LogWarning($"Exception getting property {prop.Name}: {e.ReflectionExToString()}");
                        }
                    }

                // foreach (FieldInfo field in declaringType.GetFields(flags))
                //     if (field.DeclaringType == declaringType)
                //     {
                //         ExplorerCore.Log($"Printing field {field.Name} of type {field.FieldType.FullName}");
                //         PrintTypeInfo(field.FieldType, field.Name, field.GetValue(comp));
                //     }
            }

            ExplorerCore.Log("Done writing");

            var sanitizedGameObjectName = "Object";
            if (target is UnityEngine.Object comp)
            {
                sanitizedGameObjectName = comp.name.Replace(" ", "_");
            }

            var outName = $"{sanitizedGameObjectName}_{target.GetActualType().Name}.txt";

            var newDir = new DirectoryInfo(MelonEnvironment.GameRootDirectory).CreateSubdirectory("ComponentDumps");

            File.WriteAllText(Path.Combine(newDir.FullName, outName), writer.ToString());
            ExplorerCore.Log("Dumped component to file!");
        }
        
        private class DumpObjectController : ICacheObjectController
        {
            private readonly object _target;
            public CacheObjectBase ParentCacheObject { get; }
            public object Target => _target;
            public Type TargetType { get; }
            public bool CanWrite { get; }
            
            public DumpObjectController(object target)
            {
                _target = target;
            }
        }
    }
}
