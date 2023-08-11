using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Vectrosity
{
	public class VectorManager
	{
		public static float minBrightnessDistance = 500f;

		public static float maxBrightnessDistance = 250f;

		private static int brightnessLevels = 32;

		public static float distanceCheckFrequency = 0.2f;

		private static Color fogColor;

		public static bool useDraw3D = false;

		private static List<VectorLine> vectorLines;

		private static List<RefInt> objectNumbers;

		public static int _arrayCount = 0;

		private static List<VectorLine> vectorLines2;

		private static List<RefInt> objectNumbers2;

		private static int _arrayCount2 = 0;

		private static List<Transform> transforms3;

		private static List<VectorLine> vectorLines3;

		private static List<int> oldDistances;

		private static List<Color> colors;

		private static List<RefInt> objectNumbers3;

		private static int _arrayCount3 = 0;

		private static Dictionary<string, Mesh> meshTable;

		public static int arrayCount => _arrayCount;

		public static int arrayCount2 => _arrayCount2;

		public static void SetBrightnessParameters(float fadeOutDistance, float fullBrightDistance, int levels, float frequency, Color color)
		{
			minBrightnessDistance = fadeOutDistance * fadeOutDistance;
			maxBrightnessDistance = fullBrightDistance * fullBrightDistance;
			brightnessLevels = levels;
			distanceCheckFrequency = frequency;
			fogColor = color;
		}

		public static float GetBrightnessValue(Vector3 pos)
		{
			if (!VectorLine.camTransformExists)
			{
				VectorLine.SetCamera3D();
			}
			return Mathf.InverseLerp(minBrightnessDistance, maxBrightnessDistance, (pos - VectorLine.camTransformPosition).sqrMagnitude);
		}

		public static void ObjectSetup(GameObject go, VectorLine line, Visibility visibility, Brightness brightness)
		{
			ObjectSetup(go, line, visibility, brightness, true);
		}

		public static void ObjectSetup(GameObject go, VectorLine line, Visibility visibility, Brightness brightness, bool makeBounds)
		{
			VisibilityControl visibilityControl = go.GetComponent<VisibilityControl>();
			VisibilityControlStatic visibilityControlStatic = go.GetComponent<VisibilityControlStatic>();
			VisibilityControlAlways visibilityControlAlways = go.GetComponent<VisibilityControlAlways>();
			BrightnessControl brightnessControl = go.GetComponent<BrightnessControl>();
			
			if (visibility == Visibility.None)
			{
				if ((bool)visibilityControl)
				{
					Object.Destroy(visibilityControl);
				}
				if ((bool)visibilityControlStatic)
				{
					Object.Destroy(visibilityControlStatic);
				}
				if ((bool)visibilityControlAlways)
				{
					Object.Destroy(visibilityControlAlways);
				}
			}
			switch (visibility)
			{
			case Visibility.Dynamic:
				if (visibilityControl == null)
				{
					visibilityControl = go.AddComponent<VisibilityControl>();
					visibilityControl.Setup(line, makeBounds);
					if (brightnessControl != null)
					{
						brightnessControl.SetUseLine(false);
					}
				}
				break;
			case Visibility.Static:
				if (visibilityControlStatic == null)
				{
					visibilityControlStatic = go.AddComponent<VisibilityControlStatic>();
					visibilityControlStatic.Setup(line, makeBounds);
					if (brightnessControl != null)
					{
						brightnessControl.SetUseLine(false);
					}
				}
				break;
			case Visibility.Always:
				if (visibilityControlAlways == null)
				{
					visibilityControlAlways = go.AddComponent<VisibilityControlAlways>();
					visibilityControlAlways.Setup(line);
					if (brightnessControl != null)
					{
						brightnessControl.SetUseLine(false);
					}
				}
				break;
			}
			if (brightness == Brightness.Fog)
			{
				if (brightnessControl == null)
				{
					brightnessControl = go.AddComponent<BrightnessControl>();
					if (visibilityControl == null && visibilityControlStatic == null && visibilityControlAlways == null)
					{
						brightnessControl.Setup(line, true);
					}
					else
					{
						brightnessControl.Setup(line, false);
					}
				}
			}
			else if ((bool)brightnessControl)
			{
				Object.Destroy(brightnessControl);
			}
		}

		public static void VisibilityStaticSetup(VectorLine line, out RefInt objectNum)
		{
			if (vectorLines == null)
			{
				vectorLines = new List<VectorLine>();
				objectNumbers = new List<RefInt>();
			}
			vectorLines.Add(line);
			objectNum = new RefInt(_arrayCount++);
			objectNumbers.Add(objectNum);
			VectorLine.LineManagerEnable();
		}

		public static void VisibilityStaticRemove(int objectNumber)
		{
			if (objectNumber >= vectorLines.Count)
			{
				Debug.LogError("VectorManager: object number exceeds array length in VisibilityStaticRemove");
				return;
			}
			for (int i = objectNumber + 1; i < _arrayCount; i++)
			{
				objectNumbers[i].i--;
			}
			vectorLines.RemoveAt(objectNumber);
			objectNumbers.RemoveAt(objectNumber);
			_arrayCount--;
			VectorLine.LineManagerDisable();
		}

		public static void VisibilitySetup(Transform thisTransform, VectorLine line, out RefInt objectNum)
		{
			if (vectorLines2 == null)
			{
				vectorLines2 = new List<VectorLine>();
				objectNumbers2 = new List<RefInt>();
			}
			line.drawTransform = thisTransform;
			vectorLines2.Add(line);
			objectNum = new RefInt(_arrayCount2++);
			objectNumbers2.Add(objectNum);
			VectorLine.LineManagerEnable();
		}

		public static void VisibilityRemove(int objectNumber)
		{
			if (objectNumber >= vectorLines2.Count)
			{
				Debug.LogError("VectorManager: object number exceeds array length in VisibilityRemove");
				return;
			}
			for (int i = objectNumber + 1; i < _arrayCount2; i++)
			{
				objectNumbers2[i].i--;
			}
			vectorLines2.RemoveAt(objectNumber);
			objectNumbers2.RemoveAt(objectNumber);
			_arrayCount2--;
			VectorLine.LineManagerDisable();
		}

		public static void CheckDistanceSetup(Transform thisTransform, VectorLine line, Color color, RefInt objectNum)
		{
			VectorLine.LineManagerEnable();
			if (vectorLines3 == null)
			{
				vectorLines3 = new List<VectorLine>();
				transforms3 = new List<Transform>();
				oldDistances = new List<int>();
				colors = new List<Color>();
				objectNumbers3 = new List<RefInt>();
				VectorLine.LineManagerCheckDistance();
			}
			transforms3.Add(thisTransform);
			vectorLines3.Add(line);
			oldDistances.Add(-1);
			colors.Add(color);
			objectNum.i = _arrayCount3++;
			objectNumbers3.Add(objectNum);
		}

		public static void DistanceRemove(int objectNumber)
		{
			if (objectNumber >= vectorLines3.Count)
			{
				Debug.LogError("VectorManager: object number exceeds array length in DistanceRemove");
				return;
			}
			for (int i = objectNumber + 1; i < _arrayCount3; i++)
			{
				objectNumbers3[i].i--;
			}
			transforms3.RemoveAt(objectNumber);
			vectorLines3.RemoveAt(objectNumber);
			oldDistances.RemoveAt(objectNumber);
			colors.RemoveAt(objectNumber);
			objectNumbers3.RemoveAt(objectNumber);
			_arrayCount3--;
		}

		public static void CheckDistance()
		{
			for (int i = 0; i < _arrayCount3; i++)
			{
				SetDistanceColor(i);
			}
		}

		public static void SetOldDistance(int objectNumber, int val)
		{
			oldDistances[objectNumber] = val;
		}

		public static void SetDistanceColor(int i)
		{
			if (vectorLines3[i].active)
			{
				float brightnessValue = GetBrightnessValue(transforms3[i].position);
				int num = (int)(brightnessValue * (float)brightnessLevels);
				if (num != oldDistances[i])
				{
					vectorLines3[i].SetColor(Color.Lerp(fogColor, colors[i], brightnessValue));
				}
				oldDistances[i] = num;
			}
		}

		public static void DrawArrayLine(int i)
		{
			if (useDraw3D)
			{
				vectorLines[i].Draw3D();
			}
			else
			{
				vectorLines[i].Draw();
			}
		}

		public static void DrawArrayLine2(int i)
		{
			if (useDraw3D)
			{
				vectorLines2[i].Draw3D();
			}
			else
			{
				vectorLines2[i].Draw();
			}
		}

		public static void DrawArrayLines()
		{
			if (useDraw3D)
			{
				for (int i = 0; i < _arrayCount; i++)
				{
					vectorLines[i].Draw3D();
				}
			}
			else
			{
				for (int j = 0; j < _arrayCount; j++)
				{
					vectorLines[j].Draw();
				}
			}
		}

		public static void DrawArrayLines2()
		{
			if (useDraw3D)
			{
				for (int i = 0; i < _arrayCount2; i++)
				{
					vectorLines2[i].Draw3D();
				}
			}
			else
			{
				for (int j = 0; j < _arrayCount2; j++)
				{
					vectorLines2[j].Draw();
				}
			}
		}

		public static Bounds GetBounds(VectorLine line)
		{
			if (line.points3 == null)
			{
				Debug.LogError("VectorManager: GetBounds can only be used with a Vector3 array");
				return default(Bounds);
			}
			return GetBounds(line.points3);
		}

		public static Bounds GetBounds(List<Vector3> points3)
		{
			Bounds result = default(Bounds);
			Vector3 min = new Vector3(float.MaxValue, float.MaxValue, float.MaxValue);
			Vector3 max = new Vector3(float.MinValue, float.MinValue, float.MinValue);
			int count = points3.Count;
			for (int i = 0; i < count; i++)
			{
				if (points3[i].x < min.x)
				{
					min.x = points3[i].x;
				}
				else if (points3[i].x > max.x)
				{
					max.x = points3[i].x;
				}
				if (points3[i].y < min.y)
				{
					min.y = points3[i].y;
				}
				else if (points3[i].y > max.y)
				{
					max.y = points3[i].y;
				}
				if (points3[i].z < min.z)
				{
					min.z = points3[i].z;
				}
				else if (points3[i].z > max.z)
				{
					max.z = points3[i].z;
				}
			}
			result.min = min;
			result.max = max;
			return result;
		}

		private static Mesh MakeBoundsMesh(Bounds bounds)
		{
			Mesh mesh = new Mesh();
			mesh.vertices = new Vector3[8]
			{
				bounds.center + new Vector3(0f - bounds.extents.x, bounds.extents.y, bounds.extents.z),
				bounds.center + new Vector3(bounds.extents.x, bounds.extents.y, bounds.extents.z),
				bounds.center + new Vector3(0f - bounds.extents.x, bounds.extents.y, 0f - bounds.extents.z),
				bounds.center + new Vector3(bounds.extents.x, bounds.extents.y, 0f - bounds.extents.z),
				bounds.center + new Vector3(0f - bounds.extents.x, 0f - bounds.extents.y, bounds.extents.z),
				bounds.center + new Vector3(bounds.extents.x, 0f - bounds.extents.y, bounds.extents.z),
				bounds.center + new Vector3(0f - bounds.extents.x, 0f - bounds.extents.y, 0f - bounds.extents.z),
				bounds.center + new Vector3(bounds.extents.x, 0f - bounds.extents.y, 0f - bounds.extents.z)
			};
			return mesh;
		}

		public static void SetupBoundsMesh(GameObject go, VectorLine line)
		{
			MeshFilter meshFilter = go.GetComponent<MeshFilter>();
			if (meshFilter == null)
			{
				meshFilter = go.AddComponent<MeshFilter>();
			}
			MeshRenderer meshRenderer = go.GetComponent<MeshRenderer>();
			if (meshRenderer == null)
			{
				meshRenderer = go.AddComponent<MeshRenderer>();
			}
			meshRenderer.enabled = true;
			if (meshTable == null)
			{
				meshTable = new Dictionary<string, Mesh>();
			}
			if (!meshTable.ContainsKey(line.name))
			{
				meshTable.Add(line.name, MakeBoundsMesh(GetBounds(line)));
				meshTable[line.name].name = line.name + " Bounds";
			}
			meshFilter.mesh = meshTable[line.name];
		}
	}
}
