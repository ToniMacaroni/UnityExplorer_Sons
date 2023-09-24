using HarmonyLib;
using Mono.CSharp;
using System.Collections;
using System.Text;
using RedLoader;
using SonsSdk;
using UnityExplorer.UI.Panels;
using Color = System.Drawing.Color;

namespace UnityExplorer.CSConsole
{
    public class ScriptInteraction : InteractiveBase
    {
        public static object CurrentTarget
            => InspectorManager.ActiveInspector?.Target;

        public static object[] AllTargets
            => InspectorManager.Inspectors.Select(it => it.Target).ToArray();

        public static void Log(object message)
            => ExplorerCore.Log(message);

        public static void Inspect(object obj)
            => InspectorManager.Inspect(obj);

        public static void Inspect(Type type)
            => InspectorManager.Inspect(type);

        public static Coroutine Start(IEnumerator ienumerator)
            => RuntimeHelper.StartCoroutine(ienumerator);

        public static void Stop(Coroutine coro)
            => RuntimeHelper.StopCoroutine(coro);

        public static void Copy(object obj)
            => ClipboardPanel.Copy(obj);

        public static object Paste()
            => ClipboardPanel.Current;

        public static void GetUsing()
            => Log(Evaluator.GetUsing());

        public static List<Transform> Bones()
        {
            var go = InspectorManager.ActiveInspector.Target as GameObject;
            var smr = go.GetComponent<SkinnedMeshRenderer>();
            return smr.bones.ToList();
        }

        public static Transform CreateSphere(float size)
        {
            return GameObject.CreatePrimitive(PrimitiveType.Sphere).transform;
        }

        public static void ExportBoneWeights(string filename)
        {
            var go = (GameObject)CurrentTarget;
            var smr = go.GetComponent<SkinnedMeshRenderer>();
            
            if (!smr)
            {
                Debug.LogError("Current gameobject has no skinned mesh renderer.");
                return;
            }

            BoneWeight[] boneWeights = smr.sharedMesh.boneWeights;
            using (StreamWriter writer = new StreamWriter(filename, false))
            {
                foreach (var weight in boneWeights)
                {
                    writer.WriteLine($"{weight.boneIndex0},{weight.weight0},{weight.boneIndex1},{weight.weight1},{weight.boneIndex2},{weight.weight2},{weight.boneIndex3},{weight.weight3}");
                }
            }
            
            Transform[] bones = smr.bones;
            using (StreamWriter writer = new StreamWriter(filename + "_bones", false))
            {
                foreach (var bone in bones)
                {
                    Transform current = bone;
                    string hierarchyPath = current.name;
                
                    while (current.parent != null && current != smr.rootBone)
                    {
                        current = current.parent;
                        hierarchyPath = current.name + "/" + hierarchyPath;
                    }

                    writer.WriteLine(hierarchyPath);
                }
            }
        }

        public static void ExportToObj(string saveFileName)
        {
            var go = (GameObject)CurrentTarget;
            Mesh mesh;
            var smr = go.GetComponent<SkinnedMeshRenderer>();
            if (smr)
            {
                mesh = smr.sharedMesh;
            }
            else
            {
                var mf = go.GetComponent<MeshFilter>();
                mesh = mf.sharedMesh;
            }

            if (!mesh)
            {
                RLog.Error("Current gameobject has no mesh!");
            }

            using (StreamWriter writer = new StreamWriter(saveFileName))
            {
                writer.WriteLine("# Exported Mesh from Unity");

                // Write vertices
                foreach (var v in mesh.vertices)
                {
                    writer.WriteLine($"v {v.x} {v.y} {v.z}");
                }

                // Write UVs
                foreach (var uv in mesh.uv)
                {
                    writer.WriteLine($"vt {uv.x} {uv.y}");
                }

                // Write normals
                foreach (var n in mesh.normals)
                {
                    writer.WriteLine($"vn {n.x} {n.y} {n.z}");
                }

                writer.WriteLine("g Mesh");
            
                // Write triangles
                for (int i = 0; i < mesh.triangles.Length; i += 3)
                {
                    writer.WriteLine($"f {mesh.triangles[i] + 1}/{mesh.triangles[i] + 1}/{mesh.triangles[i] + 1} {mesh.triangles[i + 1] + 1}/{mesh.triangles[i + 1] + 1}/{mesh.triangles[i + 1] + 1} {mesh.triangles[i + 2] + 1}/{mesh.triangles[i + 2] + 1}/{mesh.triangles[i + 2] + 1}");
                }
            }
        }
        
        public static GameObject CurrentGameObject => (GameObject)CurrentTarget;

        public static RectTransform rect => CurrentGameObject.GetComponent<RectTransform>();
        
        public static T Get<T>() where T : Component
        {
            return CurrentGameObject.GetComponent<T>();
        }

        public static void GetVars()
        {
            string vars = Evaluator.GetVars()?.Trim();
            if (string.IsNullOrEmpty(vars))
                ExplorerCore.LogWarning("No variables seem to be defined!");
            else
                Log(vars);
        }

        public static void ExportTexture(Texture texture, string path)
        {
            RenderTexture renderTexture = texture as RenderTexture;

            Texture2D texture2D = new Texture2D(renderTexture.width, renderTexture.height, TextureFormat.ARGB32, false);
            RenderTexture.active = renderTexture;
            texture2D.ReadPixels(new Rect(0, 0, renderTexture.width, renderTexture.height), 0, 0);
            texture2D.Apply();

            byte[] bytes = texture2D.EncodeToPNG();
            File.WriteAllBytes(path, bytes);
        }
        
        public static void PrintMaterial(Material mat)
        {
            if (mat == null)
            {
                Debug.LogError("Material is null.");
                return;
            }

            var list = new List<string>();

            Shader shader = mat.shader;
            int propertyCount = shader.GetPropertyCount();
        
            for (int i = 0; i < propertyCount; i++)
            {
                string propertyName = shader.GetPropertyName(i);
                var propertyType = shader.GetPropertyType(i);
                
                list.Add($"Property Name: {propertyName}, Type: {propertyType}");
            }
            
            PrettyPrint.Print(mat.name, list, text =>
            {
                RLog.Msg(Color.PaleGreen, text);
            });
        }

        public static void GetClasses()
        {
            if (AccessTools.Field(typeof(Evaluator), "source_file")
                    .GetValue(Evaluator) is CompilationSourceFile sourceFile
                && sourceFile.Containers.Any())
            {
                StringBuilder sb = new();
                sb.Append($"There are {sourceFile.Containers.Count} defined classes:");
                foreach (TypeDefinition type in sourceFile.Containers.Where(it => it is TypeDefinition))
                {
                    sb.Append($"\n\n{type.MemberName.Name}:");
                    foreach (MemberCore member in type.Members)
                        sb.Append($"\n\t- {member.AttributeTargets}: \"{member.MemberName.Name}\" ({member.ModFlags})");
                }
                Log(sb.ToString());
            }
            else
                ExplorerCore.LogWarning("No classes seem to be defined.");

        }
    }
}
