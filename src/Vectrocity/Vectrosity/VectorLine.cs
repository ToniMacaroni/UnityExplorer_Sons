using System;
using System.Collections.Generic;
using Il2CppInterop.Runtime.Injection;
using UnityEngine;
using UnityEngine.UI;

namespace Vectrosity
{
	[Serializable]
	public class VectorLine
	{
		private enum FunctionName
		{
			SetColors,
			SetWidths,
			MakeCurve,
			MakeSpline,
			MakeEllipse
		}

		private Vector3[] m_lineVertices;

		private Vector2[] m_lineUVs;

		private Color32[] m_lineColors;

		private List<int> m_lineTriangles;

		private int m_vertexCount;

		private GameObject m_go;

		private RectTransform m_rectTransform;

		private IVectorObject m_vectorObject;

		private Color32 m_color;

		private CanvasState m_canvasState;

		private bool m_is2D;

		private List<Vector2> m_points2;

		private List<Vector3> m_points3;

		private int m_pointsCount;

		private Vector3[] m_screenPoints;

		private float[] m_lineWidths;

		private float m_lineWidth;

		private float m_maxWeldDistance;

		private float[] m_distances;

		private string m_name;

		private Material m_material;

		private Texture m_originalTexture;

		private Texture m_texture;

		private bool m_active = true;

		private LineType m_lineType;

		
		private float m_capLength;

		
		private bool m_smoothWidth = false;

		
		private bool m_smoothColor = false;

		
		private Joins m_joins;

		
		private bool m_isAutoDrawing = false;

		
		private int m_drawStart = 0;

		
		private int m_drawEnd = 0;

		
		private int m_endPointsUpdate;

		
		private bool m_useNormals = false;

		
		private bool m_useTangents = false;

		
		private bool m_normalsCalculated = false;

		
		private bool m_tangentsCalculated = false;

		
		private EndCap m_capType = EndCap.None;

		
		private string m_endCap;

		
		private bool m_useCapColors = false;

		
		private Color32 m_frontColor;

		
		private Color32 m_backColor;

		
		private float m_lineUVBottom;

		
		private float m_lineUVTop;

		
		private float m_frontCapUVBottom;

		
		private float m_frontCapUVTop;

		
		private float m_backCapUVBottom;

		
		private float m_backCapUVTop;

		
		private bool m_continuousTexture = false;

		
		private Transform m_drawTransform;

		
		private bool m_viewportDraw;

		
		private float m_textureScale;

		
		private bool m_useTextureScale = false;

		
		private float m_textureOffset;

		
		private bool m_useMatrix = false;

		
		private Matrix4x4 m_matrix;

		
		private bool m_collider = false;

		
		private bool m_trigger = false;

		
		//private PhysicsMaterial2D m_physicsMaterial;

		
		private bool m_alignOddWidthToPixels = false;

		private static Vector3 v3zero = Vector3.zero;

		private static Canvas m_canvas;

		private static Transform camTransform;

		private static Camera cam3D;

		private static Vector3 oldPosition;

		private static Vector3 oldRotation;

		private static bool lineManagerCreated = false;

		private static LineManager m_lineManager;

		private static Dictionary<string, CapInfo> capDictionary;

		private static int endianDiff1;

		private static int endianDiff2;

		private static byte[] byteBlock;

		private static string[] functionNames = new string[5] { "VectorLine.SetColors: Length of color", "VectorLine.SetWidths: Length of line widths", "MakeCurve", "MakeSpline", "MakeEllipse" };

		public Vector3[] lineVertices => m_lineVertices;

		public Vector2[] lineUVs => m_lineUVs;

		public Color32[] lineColors => m_lineColors;

		public List<int> lineTriangles => m_lineTriangles;

		public RectTransform rectTransform
		{
			get
			{
				if (m_go != null)
				{
					return m_rectTransform;
				}
				return null;
			}
		}

		public Color32 color
		{
			get
			{
				return m_color;
			}
			set
			{
				m_color = value;
				SetColor(value);
			}
		}

		public bool is2D => m_is2D;

		public List<Vector2> points2
		{
			get
			{
				if (!m_is2D)
				{
					Debug.LogError("Line \"" + name + "\" uses points3 rather than points2");
					return null;
				}
				return m_points2;
			}
			set
			{
				if (value == null)
				{
					Debug.LogError("List for Line \"" + name + "\" must not be null");
				}
				else
				{
					m_points2 = value;
				}
			}
		}

		public List<Vector3> points3
		{
			get
			{
				if (m_is2D)
				{
					Debug.LogError("Line \"" + name + "\" uses points2 rather than points3");
					return null;
				}
				return m_points3;
			}
			set
			{
				if (value == null)
				{
					Debug.LogError("List for Line \"" + name + "\" must not be null");
				}
				else
				{
					m_points3 = value;
				}
			}
		}

		private int pointsCount => (!m_is2D) ? m_points3.Count : m_points2.Count;

		public float lineWidth
		{
			get
			{
				return m_lineWidth;
			}
			set
			{
				m_lineWidth = value;
				float num = value * 0.5f;
				for (int i = 0; i < m_lineWidths.Length; i++)
				{
					m_lineWidths[i] = num;
				}
				m_maxWeldDistance = value * 2f * (value * 2f);
			}
		}

		public float maxWeldDistance
		{
			get
			{
				return Mathf.Sqrt(m_maxWeldDistance);
			}
			set
			{
				m_maxWeldDistance = value * value;
			}
		}

		public string name
		{
			get
			{
				return m_name;
			}
			set
			{
				m_name = value;
				if (m_go != null)
				{
					m_go.name = value;
				}
				if (m_vectorObject != null)
				{
					m_vectorObject.SetName(value);
				}
			}
		}

		public Material material
		{
			get
			{
				return m_material;
			}
			set
			{
				if (m_vectorObject != null)
				{
					m_vectorObject.SetMaterial(value);
				}
				m_material = value;
			}
		}

		public Texture texture
		{
			get
			{
				return m_texture;
			}
			set
			{
				if (m_capType != EndCap.None)
				{
					m_originalTexture = value;
					return;
				}
				if (m_vectorObject != null)
				{
					m_vectorObject.SetTexture(value);
				}
				m_texture = value;
			}
		}

		public int layer
		{
			get
			{
				if (m_go != null)
				{
					return m_go.layer;
				}
				return 0;
			}
			set
			{
				if (m_go != null)
				{
					m_go.layer = Mathf.Clamp(value, 0, 31);
				}
			}
		}

		public bool active
		{
			get
			{
				return m_active;
			}
			set
			{
				m_active = value;
				if (m_vectorObject != null)
				{
					m_vectorObject.Enable(value);
				}
			}
		}

		public LineType lineType
		{
			get
			{
				return m_lineType;
			}
			set
			{
				if (value == m_lineType)
				{
					return;
				}
				m_lineType = value;
				if (value == LineType.Points || (value == LineType.Discrete && m_joins == Joins.Fill))
				{
					m_joins = Joins.None;
				}
				if (value == LineType.Discrete)
				{
					drawStart = m_drawStart;
					drawEnd = m_drawEnd;
				}
				if (value != 0 && m_points2.Count > 16383)
				{
					Resize(16383);
				}
				ResetLine();
			}
		}

		public float capLength
		{
			get
			{
				return m_capLength;
			}
			set
			{
				if (m_lineType == LineType.Points)
				{
					Debug.LogError("LineType.Points can't use capLength");
				}
				else
				{
					m_capLength = value;
				}
			}
		}

		public bool smoothWidth
		{
			get
			{
				return m_smoothWidth;
			}
			set
			{
				m_smoothWidth = m_lineType != LineType.Points && value;
			}
		}

		public bool smoothColor
		{
			get
			{
				return m_smoothColor;
			}
			set
			{
				bool flag = m_smoothColor;
				m_smoothColor = m_lineType != LineType.Points && value;
				if (m_smoothColor != flag)
				{
					int segmentNumber = GetSegmentNumber();
					for (int i = 0; i < segmentNumber; i++)
					{
						SetColor(GetColor(i), i);
					}
				}
			}
		}

		public Joins joins
		{
			get
			{
				return m_joins;
			}
			set
			{
				if (m_lineType == LineType.Points || (m_lineType == LineType.Discrete && value == Joins.Fill))
				{
					return;
				}
				if ((m_joins == Joins.Fill && value != 0) || (m_joins != 0 && value == Joins.Fill))
				{
					m_joins = value;
					ClearTriangles();
					SetupTriangles(0);
				}
				m_joins = value;
				if (m_joins == Joins.Weld)
				{
					if (m_canvasState == CanvasState.OnCanvas)
					{
						Draw();
					}
					else if (m_canvasState == CanvasState.OffCanvas)
					{
						Draw3D();
					}
				}
			}
		}

		public bool isAutoDrawing => m_isAutoDrawing;

		public int drawStart
		{
			get
			{
				return m_drawStart;
			}
			set
			{
				if (m_lineType == LineType.Discrete && ((uint)value & (true ? 1u : 0u)) != 0)
				{
					value++;
				}
				m_drawStart = Mathf.Clamp(value, 0, pointsCount - 1);
			}
		}

		public int drawEnd
		{
			get
			{
				return m_drawEnd;
			}
			set
			{
				if (m_lineType == LineType.Discrete && value != 0 && (value & 1) == 0)
				{
					value++;
				}
				m_drawEnd = Mathf.Clamp(value, 0, pointsCount - 1);
			}
		}

		public int endPointsUpdate
		{
			get
			{
				if (m_lineType != LineType.Discrete)
				{
					return m_endPointsUpdate;
				}
				return (m_endPointsUpdate != 0) ? (m_endPointsUpdate + 1) : 0;
			}
			set
			{
				if (m_lineType == LineType.Discrete && value > 1 && (value & 1) == 0)
				{
					value--;
				}
				m_endPointsUpdate = Mathf.Max(0, value);
			}
		}

		public string endCap
		{
			get
			{
				return m_endCap;
			}
			set
			{
				if (m_lineType == LineType.Points)
				{
					Debug.LogError("LineType.Points can't use end caps");
					return;
				}
				if (value == null || value == "")
				{
					RemoveEndCap();
					return;
				}
				if (capDictionary == null || !capDictionary.ContainsKey(value))
				{
					Debug.LogError("End cap \"" + value + "\" is not set up");
					return;
				}
				m_endCap = value;
				m_capType = capDictionary[value].capType;
				if (m_capType != EndCap.None)
				{
					SetupEndCap(capDictionary[value].uvHeights);
				}
			}
		}

		public bool continuousTexture
		{
			get
			{
				return m_continuousTexture;
			}
			set
			{
				m_continuousTexture = value;
				if (!value)
				{
					ResetTextureScale();
				}
			}
		}

		public Transform drawTransform
		{
			get
			{
				return m_drawTransform;
			}
			set
			{
				m_drawTransform = value;
			}
		}

		public bool useViewportCoords
		{
			get
			{
				return m_viewportDraw;
			}
			set
			{
				if (m_is2D)
				{
					m_viewportDraw = value;
				}
				else
				{
					Debug.LogError("Line must use Vector2 points in order to use viewport coords");
				}
			}
		}

		
		public float textureScale
		{
			get
			{
				return m_textureScale;
			}
			set
			{
				m_textureScale = value;
				if (m_textureScale == 0f)
				{
					m_useTextureScale = false;
					ResetTextureScale();
				}
				else
				{
					m_useTextureScale = true;
				}
			}
		}

		public float textureOffset
		{
			get
			{
				return m_textureOffset;
			}
			set
			{
				m_textureOffset = value;
				SetTextureScale();
			}
		}

		public Matrix4x4 matrix
		{
			get
			{
				return m_matrix;
			}
			set
			{
				m_matrix = value;
				m_useMatrix = m_matrix != Matrix4x4.identity;
			}
		}

		public int drawDepth
		{
			get
			{
				if (m_canvasState == CanvasState.OffCanvas)
				{
					Debug.LogError("VectorLine.drawDepth can't be used with lines made with Draw3D");
					return 0;
				}
				return m_go.transform.GetSiblingIndex();
			}
			set
			{
				if (m_canvasState == CanvasState.OffCanvas)
				{
					Debug.LogError("VectorLine.drawDepth can't be used with lines made with Draw3D");
				}
				else
				{
					m_go.transform.SetSiblingIndex(value);
				}
			}
		}

		public bool alignOddWidthToPixels
		{
			get
			{
				return m_alignOddWidthToPixels;
			}
			set
			{
				float num = ((!value) ? 0f : 0.5f);
				m_rectTransform.anchoredPosition = new Vector2(num, num);
				m_alignOddWidthToPixels = value;
			}
		}

		public static Canvas canvas
		{
			get
			{
				if (m_canvas == null)
				{
					SetupVectorCanvas();
				}
				return m_canvas;
			}
		}

		public static Vector3 camTransformPosition => camTransform.position;

		public static bool camTransformExists => camTransform != null;

		public static LineManager lineManager
		{
			get
			{
				if (!lineManagerCreated)
				{
					lineManagerCreated = true;
					GameObject gameObject = new GameObject("LineManager");
					m_lineManager = gameObject.AddComponent<LineManager>();
					m_lineManager.enabled = false;
					UnityEngine.Object.DontDestroyOnLoad(m_lineManager);
				}
				return m_lineManager;
			}
		}

		public VectorLine(string name, List<Vector3> points, float width)
		{
			m_points3 = points;
			SetupLine(name, null, width, LineType.Discrete, Joins.None, false);
		}

		public VectorLine(string name, List<Vector3> points, Texture texture, float width)
		{
			m_points3 = points;
			SetupLine(name, texture, width, LineType.Discrete, Joins.None, false);
		}

		public VectorLine(string name, List<Vector3> points, float width, LineType lineType)
		{
			m_points3 = points;
			SetupLine(name, null, width, lineType, Joins.None, false);
		}

		public VectorLine(string name, List<Vector3> points, Texture texture, float width, LineType lineType)
		{
			m_points3 = points;
			SetupLine(name, texture, width, lineType, Joins.None, false);
		}

		public VectorLine(string name, List<Vector3> points, float width, LineType lineType, Joins joins)
		{
			m_points3 = points;
			SetupLine(name, null, width, lineType, joins, false);
		}

		public VectorLine(string name, List<Vector3> points, Texture texture, float width, LineType lineType, Joins joins)
		{
			m_points3 = points;
			SetupLine(name, texture, width, lineType, joins, false);
		}

		public VectorLine(string name, List<Vector2> points, float width)
		{
			m_points2 = points;
			SetupLine(name, null, width, LineType.Discrete, Joins.None, true);
		}

		public VectorLine(string name, List<Vector2> points, Texture texture, float width)
		{
			m_points2 = points;
			SetupLine(name, texture, width, LineType.Discrete, Joins.None, true);
		}

		public VectorLine(string name, List<Vector2> points, float width, LineType lineType)
		{
			m_points2 = points;
			SetupLine(name, null, width, lineType, Joins.None, true);
		}

		public VectorLine(string name, List<Vector2> points, Texture texture, float width, LineType lineType)
		{
			m_points2 = points;
			SetupLine(name, texture, width, lineType, Joins.None, true);
		}

		public VectorLine(string name, List<Vector2> points, float width, LineType lineType, Joins joins)
		{
			m_points2 = points;
			SetupLine(name, null, width, lineType, joins, true);
		}

		public VectorLine(string name, List<Vector2> points, Texture texture, float width, LineType lineType, Joins joins)
		{
			m_points2 = points;
			SetupLine(name, texture, width, lineType, joins, true);
		}

		public static string Version()
		{
			return "Vectrosity version 5.2.2";
		}

		protected void SetupLine(string lineName, Texture texture, float width, LineType lineType, Joins joins, bool use2D)
		{
			m_is2D = use2D;
			m_lineType = lineType;
			if (joins == Joins.Fill && m_lineType != 0)
			{
				Debug.LogError("VectorLine: Must use LineType.Continuous if using Joins.Fill for \"" + lineName + "\"");
				return;
			}
			if (joins == Joins.Weld && m_lineType == LineType.Points)
			{
				Debug.LogError("VectorLine: LineType.Points can't use Joins.Weld for \"" + lineName + "\"");
				return;
			}
			if ((m_is2D && m_points2 == null) || (!m_is2D && m_points3 == null))
			{
				Debug.LogError("VectorLine: the points array is null for \"" + lineName + "\"");
				return;
			}
			if (m_is2D)
			{
				m_pointsCount = ((m_points2.Capacity <= 0 || m_points2.Count != 0) ? m_points2.Count : m_points2.Capacity);
				int num = m_pointsCount - m_points2.Count;
				for (int i = 0; i < num; i++)
				{
					m_points2.Add(Vector2.zero);
				}
			}
			else
			{
				m_pointsCount = ((m_points3.Capacity <= 0 || m_points3.Count != 0) ? m_points3.Count : m_points3.Capacity);
				int num2 = m_pointsCount - m_points3.Count;
				for (int j = 0; j < num2; j++)
				{
					m_points3.Add(Vector3.zero);
				}
			}
			if (SetVertexCount())
			{
				m_go = new GameObject(name);
				m_canvasState = CanvasState.None;
				layer = LayerMask.NameToLayer("UI");
				m_rectTransform = m_go.AddComponent<RectTransform>();
				SetupTransform(m_rectTransform);
				m_texture = texture;
				m_lineVertices = new Vector3[m_vertexCount];
				m_lineUVs = new Vector2[m_vertexCount];
				m_lineColors = new Color32[m_vertexCount];
				m_lineUVBottom = 0f;
				m_lineUVTop = 1f;
				SetUVs(0, GetSegmentNumber());
				m_lineTriangles = new List<int>();
				name = lineName;
				color = Color.white;
				m_maxWeldDistance = width * 2f * (width * 2f);
				m_joins = joins;
				m_lineWidths = new float[1];
				m_lineWidths[0] = width * 0.5f;
				m_lineWidth = width;
				if (!m_is2D)
				{
					m_screenPoints = new Vector3[m_vertexCount];
				}
				m_drawStart = 0;
				m_drawEnd = m_pointsCount - 1;
				SetupTriangles(0);
			}
		}

		private void SetupTriangles(int startVert)
		{
			int num = 0;
			int num2 = 0;
			if (pointsCount > 0)
			{
				if (m_lineType == LineType.Points)
				{
					num = pointsCount * 6;
					num2 = pointsCount * 4;
				}
				else if (m_lineType == LineType.Continuous)
				{
					num = ((m_joins != 0) ? ((pointsCount - 1) * 6) : ((pointsCount - 1) * 12));
					num2 = (pointsCount - 1) * 4;
				}
				else
				{
					num = pointsCount / 2 * 6;
					num2 = pointsCount * 2;
				}
			}
			if (m_capType != EndCap.None)
			{
				num += 12;
			}
			if (m_lineTriangles.Count > num)
			{
				m_lineTriangles.RemoveRange(num, m_lineTriangles.Count - num);
				if (m_joins == Joins.Fill)
				{
					SetLastFillTriangles();
				}
				else if (m_vectorObject != null)
				{
					m_vectorObject.UpdateTris();
				}
				return;
			}
			if (m_joins == Joins.Fill)
			{
				if (startVert >= 4)
				{
					int num3 = m_lineTriangles.Count - 6;
					m_lineTriangles[num3] = startVert - 3;
					m_lineTriangles[num3 + 1] = startVert;
					m_lineTriangles[num3 + 2] = startVert + 3;
					m_lineTriangles[num3 + 3] = startVert - 2;
					m_lineTriangles[num3 + 4] = startVert;
					m_lineTriangles[num3 + 5] = startVert + 3;
				}
				for (int i = startVert; i < num2; i += 4)
				{
					m_lineTriangles.Add(i);
					m_lineTriangles.Add(i + 1);
					m_lineTriangles.Add(i + 3);
					m_lineTriangles.Add(i + 1);
					m_lineTriangles.Add(i + 2);
					m_lineTriangles.Add(i + 3);
					m_lineTriangles.Add(i + 1);
					m_lineTriangles.Add(i + 4);
					m_lineTriangles.Add(i + 7);
					m_lineTriangles.Add(i + 2);
					m_lineTriangles.Add(i + 4);
					m_lineTriangles.Add(i + 7);
				}
				SetLastFillTriangles();
			}
			else
			{
				for (int j = startVert; j < num2; j += 4)
				{
					m_lineTriangles.Add(j);
					m_lineTriangles.Add(j + 1);
					m_lineTriangles.Add(j + 3);
					m_lineTriangles.Add(j + 1);
					m_lineTriangles.Add(j + 2);
					m_lineTriangles.Add(j + 3);
				}
			}
			if (m_vectorObject != null)
			{
				m_vectorObject.UpdateTris();
			}
		}

		private void SetLastFillTriangles()
		{
			if (pointsCount < 2)
			{
				return;
			}
			int num = (pointsCount - 1) * 12 + ((m_capType != EndCap.None) ? 12 : 0);
			bool flag = false;
			if ((m_is2D && m_points2[0] == m_points2[points2.Count - 1]) || (!m_is2D && m_points3[0] == m_points3[points3.Count - 1]))
			{
				if (m_lineTriangles[num - 4] != 3 && m_lineTriangles[num - 1] != 3)
				{
					flag = true;
				}
				m_lineTriangles[num - 6] = m_vertexCount - 3;
				m_lineTriangles[num - 5] = 0;
				m_lineTriangles[num - 4] = 3;
				m_lineTriangles[num - 3] = m_vertexCount - 2;
				m_lineTriangles[num - 2] = 0;
				m_lineTriangles[num - 1] = 3;
			}
			else
			{
				if (m_lineTriangles[num - 4] == 3 && m_lineTriangles[num - 1] == 3)
				{
					flag = true;
				}
				m_lineTriangles[num - 6] = 0;
				m_lineTriangles[num - 5] = 0;
				m_lineTriangles[num - 4] = 0;
				m_lineTriangles[num - 3] = 0;
				m_lineTriangles[num - 2] = 0;
				m_lineTriangles[num - 1] = 0;
			}
			if (flag && m_vectorObject != null)
			{
				m_vectorObject.UpdateTris();
			}
		}

		private void SetupEndCap(float[] uvHeights)
		{
			int num = m_vertexCount + 8;
			if (num > 65534)
			{
				Debug.LogError("VectorLine: exceeded maximum vertex count of 65534 for \"" + m_name + "\"...use fewer points");
				return;
			}
			ResizeMeshArrays(num);
			int num2 = 0;
			if (m_joins == Joins.Fill)
			{
				for (int i = num - 8; i < num; i += 4)
				{
					m_lineTriangles.Insert(num2, i);
					m_lineTriangles.Insert(1 + num2, i + 1);
					m_lineTriangles.Insert(2 + num2, i + 3);
					m_lineTriangles.Insert(3 + num2, i + 1);
					m_lineTriangles.Insert(4 + num2, i + 2);
					m_lineTriangles.Insert(5 + num2, i + 3);
					num2 += 6;
				}
			}
			else
			{
				for (int j = num - 8; j < num; j += 4)
				{
					m_lineTriangles.Insert(num2, j);
					m_lineTriangles.Insert(1 + num2, j + 1);
					m_lineTriangles.Insert(2 + num2, j + 3);
					m_lineTriangles.Insert(3 + num2, j + 1);
					m_lineTriangles.Insert(4 + num2, j + 2);
					m_lineTriangles.Insert(5 + num2, j + 3);
					num2 += 6;
				}
			}
			int num3 = ((num >= 12) ? (num - 12) : 0);
			for (int k = num - 8; k < num - 4; k++)
			{
				ref Color32 reference = ref m_lineColors[k];
				reference = m_lineColors[0];
				ref Color32 reference2 = ref m_lineColors[k + 4];
				reference2 = m_lineColors[num3];
			}
			m_lineUVBottom = uvHeights[0];
			m_lineUVTop = uvHeights[1];
			m_backCapUVBottom = uvHeights[2];
			m_backCapUVTop = uvHeights[3];
			m_frontCapUVBottom = uvHeights[4];
			m_frontCapUVTop = uvHeights[5];
			SetUVs(0, GetSegmentNumber());
			SetEndCapUVs();
			if (m_vectorObject != null)
			{
				m_vectorObject.UpdateTris();
				m_vectorObject.UpdateUVs();
			}
			SetEndCapColors();
			m_originalTexture = m_texture;
			m_texture = capDictionary[m_endCap].texture;
			if (m_vectorObject != null)
			{
				m_vectorObject.SetTexture(m_texture);
			}
		}

		private void ResetLine()
		{
			SetVertexCount();
			m_lineVertices = new Vector3[m_vertexCount];
			m_lineUVs = new Vector2[m_vertexCount];
			m_lineColors = new Color32[m_vertexCount];
			if (!m_is2D)
			{
				m_screenPoints = new Vector3[m_vertexCount];
			}
			SetUVs(0, GetSegmentNumber());
			SetColor(m_color);
			int segmentNumber = GetSegmentNumber();
			SetupWidths(segmentNumber);
			ClearTriangles();
			SetupTriangles(0);
			if (m_vectorObject != null)
			{
				m_vectorObject.UpdateMeshAttributes();
			}
			if (m_canvasState == CanvasState.OnCanvas)
			{
				Draw();
			}
			else if (m_canvasState == CanvasState.OffCanvas)
			{
				Draw3D();
			}
		}

		private void SetEndCapUVs()
		{
			ref Vector2 reference = ref m_lineUVs[m_vertexCount + 3];
			reference = new Vector2(0f, m_frontCapUVTop);
			ref Vector2 reference2 = ref m_lineUVs[m_vertexCount];
			reference2 = new Vector2(1f, m_frontCapUVTop);
			ref Vector2 reference3 = ref m_lineUVs[m_vertexCount + 1];
			reference3 = new Vector2(1f, m_frontCapUVBottom);
			ref Vector2 reference4 = ref m_lineUVs[m_vertexCount + 2];
			reference4 = new Vector2(0f, m_frontCapUVBottom);
			if (capDictionary[m_endCap].capType == EndCap.Mirror)
			{
				ref Vector2 reference5 = ref m_lineUVs[m_vertexCount + 7];
				reference5 = new Vector2(0f, m_frontCapUVBottom);
				ref Vector2 reference6 = ref m_lineUVs[m_vertexCount + 4];
				reference6 = new Vector2(1f, m_frontCapUVBottom);
				ref Vector2 reference7 = ref m_lineUVs[m_vertexCount + 5];
				reference7 = new Vector2(1f, m_frontCapUVTop);
				ref Vector2 reference8 = ref m_lineUVs[m_vertexCount + 6];
				reference8 = new Vector2(0f, m_frontCapUVTop);
			}
			else
			{
				ref Vector2 reference9 = ref m_lineUVs[m_vertexCount + 7];
				reference9 = new Vector2(0f, m_backCapUVTop);
				ref Vector2 reference10 = ref m_lineUVs[m_vertexCount + 4];
				reference10 = new Vector2(1f, m_backCapUVTop);
				ref Vector2 reference11 = ref m_lineUVs[m_vertexCount + 5];
				reference11 = new Vector2(1f, m_backCapUVBottom);
				ref Vector2 reference12 = ref m_lineUVs[m_vertexCount + 6];
				reference12 = new Vector2(0f, m_backCapUVBottom);
			}
		}

		private void RemoveEndCap()
		{
			if (m_capType != EndCap.None)
			{
				m_endCap = null;
				m_capType = EndCap.None;
				ResizeMeshArrays(m_vertexCount);
				m_lineTriangles.RemoveRange(0, 12);
				m_lineUVBottom = 0f;
				m_lineUVTop = 1f;
				SetUVs(0, GetSegmentNumber());
				if (m_useTextureScale)
				{
					SetTextureScale();
				}
				texture = m_originalTexture;
				m_vectorObject.UpdateMeshAttributes();
			}
		}

		private static void SetupTransform(RectTransform rectTransform)
		{
			rectTransform.offsetMin = Vector2.zero;
			rectTransform.offsetMax = Vector2.zero;
			rectTransform.anchorMin = Vector2.zero;
			rectTransform.anchorMax = Vector2.zero;
			rectTransform.pivot = Vector2.zero;
			rectTransform.anchoredPosition = Vector2.zero;
		}

		private void ResizeMeshArrays(int newCount)
		{
			Array.Resize(ref m_lineVertices, newCount);
			Array.Resize(ref m_lineUVs, newCount);
			Array.Resize(ref m_lineColors, newCount);
		}

		public void Resize(int newCount)
		{
			if (newCount < 0)
			{
				Debug.LogError("VectorLine.Resize: the new count must be >= 0");
			}
			else
			{
				if (newCount == pointsCount)
				{
					return;
				}
				if (m_is2D)
				{
					if (newCount > m_pointsCount)
					{
						for (int i = 0; i < newCount - m_pointsCount; i++)
						{
							m_points2.Add(Vector2.zero);
						}
					}
					else
					{
						m_points2.RemoveRange(newCount, m_pointsCount - newCount);
					}
				}
				else if (newCount > m_pointsCount)
				{
					for (int j = 0; j < newCount - m_pointsCount; j++)
					{
						m_points3.Add(v3zero);
					}
				}
				else
				{
					m_points3.RemoveRange(newCount, m_pointsCount - newCount);
				}
				Resize();
			}
		}

		private void Resize()
		{
			int num = m_pointsCount;
			int num2 = m_pointsCount;
			if (m_lineType != LineType.Points)
			{
				num2 = ((m_lineType != 0) ? (m_pointsCount / 2) : Mathf.Max(0, m_pointsCount - 1));
			}
			bool flag = m_drawEnd == m_pointsCount - 1 || m_drawEnd < 1;
			if (!SetVertexCount())
			{
				return;
			}
			m_pointsCount = pointsCount;
			int num3 = m_lineVertices.Length - ((m_capType != EndCap.None) ? 8 : 0);
			if (num3 < m_vertexCount)
			{
				if (num3 == 0)
				{
					num3 = 4;
				}
				while (num3 < m_pointsCount)
				{
					num3 *= 2;
				}
				num3 = Mathf.Min(num3, MaxPoints());
				ResizeMeshArrays((m_capType != EndCap.None) ? (num3 * 4 + 8) : (num3 * 4));
				if (!m_is2D)
				{
					Array.Resize(ref m_screenPoints, num3 * 4);
				}
			}
			if (m_lineWidths.Length > 1)
			{
				if (m_lineType != LineType.Points)
				{
					num3 = ((m_lineType != 0) ? (num3 / 2) : (num3 - 1));
				}
				if (num3 > m_lineWidths.Length)
				{
					Array.Resize(ref m_lineWidths, num3);
				}
			}
			if (flag)
			{
				m_drawEnd = m_pointsCount - 1;
			}
			m_drawStart = Mathf.Clamp(m_drawStart, 0, m_pointsCount - 1);
			m_drawEnd = Mathf.Clamp(m_drawEnd, 0, m_pointsCount - 1);
			if (m_pointsCount > num2)
			{
				SetColor(m_color, num2, GetSegmentNumber());
				SetUVs(num2, GetSegmentNumber());
				if (m_lineWidths.Length > 1)
				{
					SetWidth(m_lineWidth, num2, GetSegmentNumber());
				}
			}
			if (m_pointsCount < num)
			{
				ZeroVertices(m_pointsCount, num);
			}
			if (m_capType != EndCap.None)
			{
				SetEndCapUVs();
				SetEndCapColors();
			}
			SetupTriangles(num2 * 4);
			if (m_vectorObject != null)
			{
				m_vectorObject.UpdateMeshAttributes();
			}
		}

		private void SetUVs(int startIndex, int endIndex)
		{
			Vector2 vector = new Vector2(0f, m_lineUVTop);
			Vector2 vector2 = new Vector2(1f, m_lineUVTop);
			Vector2 vector3 = new Vector2(1f, m_lineUVBottom);
			Vector2 vector4 = new Vector2(0f, m_lineUVBottom);
			int num = startIndex * 4;
			for (int i = startIndex; i < endIndex; i++)
			{
				m_lineUVs[num] = vector;
				m_lineUVs[num + 1] = vector2;
				m_lineUVs[num + 2] = vector3;
				m_lineUVs[num + 3] = vector4;
				num += 4;
			}
			if (m_vectorObject != null)
			{
				m_vectorObject.UpdateUVs();
			}
		}

		private bool SetVertexCount()
		{
			m_vertexCount = Mathf.Max(0, GetSegmentNumber() * 4);
			if (m_lineType == LineType.Discrete && ((uint)pointsCount & (true ? 1u : 0u)) != 0)
			{
				m_vertexCount += 4;
			}
			if (m_vertexCount > 65534)
			{
				Debug.LogError("VectorLine: exceeded maximum vertex count of 65534 for \"" + name + "\"...use fewer points (maximum is 16383 points for continuous lines and points, and 32767 points for discrete lines)");
				return false;
			}
			return true;
		}

		private int MaxPoints()
		{
			if (m_lineType == LineType.Discrete)
			{
				return 32767;
			}
			return 16383;
		}

		public void AddNormals()
		{
			m_useNormals = true;
			m_normalsCalculated = false;
		}

		public void AddTangents()
		{
			if (!m_useNormals)
			{
				m_useNormals = true;
				m_normalsCalculated = false;
			}
			m_useTangents = true;
			m_tangentsCalculated = false;
		}

		public Vector4[] CalculateTangents(Vector3[] normals)
		{
			if (!m_useNormals)
			{
				m_vectorObject.UpdateNormals();
				m_useNormals = true;
				m_normalsCalculated = true;
			}
			int num = m_vectorObject.VertexCount();
			Vector3[] array = new Vector3[num];
			Vector3[] array2 = new Vector3[num];
			int count = m_lineTriangles.Count;
			for (int i = 0; i < count; i += 3)
			{
				int num2 = m_lineTriangles[i];
				int num3 = m_lineTriangles[i + 1];
				int num4 = m_lineTriangles[i + 2];
				Vector3 vector = m_lineVertices[num2];
				Vector3 vector2 = m_lineVertices[num3];
				Vector3 vector3 = m_lineVertices[num4];
				Vector2 vector4 = m_lineUVs[num2];
				Vector2 vector5 = m_lineUVs[num3];
				Vector2 vector6 = m_lineUVs[num4];
				float num5 = vector2.x - vector.x;
				float num6 = vector3.x - vector.x;
				float num7 = vector2.y - vector.y;
				float num8 = vector3.y - vector.y;
				float num9 = vector2.z - vector.z;
				float num10 = vector3.z - vector.z;
				float num11 = vector5.x - vector4.x;
				float num12 = vector6.x - vector4.x;
				float num13 = vector5.y - vector4.y;
				float num14 = vector6.y - vector4.y;
				float num15 = 1f / (num11 * num14 - num12 * num13);
				Vector3 vector7 = new Vector3((num14 * num5 - num13 * num6) * num15, (num14 * num7 - num13 * num8) * num15, (num14 * num9 - num13 * num10) * num15);
				Vector3 vector8 = new Vector3((num11 * num6 - num12 * num5) * num15, (num11 * num8 - num12 * num7) * num15, (num11 * num10 - num12 * num9) * num15);
				array[num2] += vector7;
				array[num3] += vector7;
				array[num4] += vector7;
				array2[num2] += vector8;
				array2[num3] += vector8;
				array2[num4] += vector8;
			}
			Vector4[] array3 = new Vector4[num];
			for (int j = 0; j < m_vertexCount; j++)
			{
				Vector3 vector9 = normals[j];
				Vector3 vector10 = array[j];
				ref Vector4 reference = ref array3[j];
				reference = (vector10 - vector9 * Vector3.Dot(vector9, vector10)).normalized;
				array3[j].w = ((!(Vector3.Dot(Vector3.Cross(vector9, vector10), array2[j]) < 0f)) ? 1f : (-1f));
			}
			return array3;
		}

		public static GameObject SetupVectorCanvas()
		{
			GameObject gameObject = GameObject.Find("VectorCanvas");
			Canvas canvas;
			if (gameObject != null)
			{
				canvas = gameObject.GetComponent<Canvas>();
			}
			else
			{
				gameObject = new GameObject("VectorCanvas");
				gameObject.layer = LayerMask.NameToLayer("UI");
				canvas = gameObject.AddComponent<Canvas>();
			}
			canvas.renderMode = RenderMode.ScreenSpaceOverlay;
			canvas.sortingOrder = 1;
			m_canvas = canvas;
			return gameObject;
		}

		public static void SetCanvasCamera(Camera cam)
		{
			SetCanvasCamera(cam, 0);
		}

		public static void SetCanvasCamera(Camera cam, int id)
		{
			if (id < 0)
			{
				Debug.LogError("VectorLine.SetCanvasCamera: id must be >= 0");
				return;
			}
			if (m_canvas == null)
			{
				SetupVectorCanvas();
			}
			m_canvas.renderMode = RenderMode.ScreenSpaceCamera;
			m_canvas.worldCamera = cam;
		}

		public void SetCanvas(GameObject canvasObject)
		{
			SetCanvas(canvasObject, true);
		}

		public void SetCanvas(GameObject canvasObject, bool worldPositionStays)
		{
			Canvas component = canvasObject.GetComponent<Canvas>();
			if (component == null)
			{
				Debug.LogError("VectorLine.SetCanvas: canvas object must have a Canvas component");
			}
			else
			{
				SetCanvas(component, worldPositionStays);
			}
		}

		public void SetCanvas(Canvas canvas)
		{
			SetCanvas(canvas, true);
		}

		public void SetCanvas(Canvas canvas, bool worldPositionStays)
		{
			if (m_canvasState == CanvasState.OffCanvas)
			{
				Debug.LogError("VectorLine.SetCanvas only works with lines made with Draw, not Draw3D.");
			}
			else if (canvas == null)
			{
				Debug.LogError("VectorLine.SetCanvas: canvas must not be null");
			}
			else if (canvas.renderMode == RenderMode.WorldSpace)
			{
				Debug.LogError("VectorLine.SetCanvas: canvas must be screen space overlay or screen space camera");
			}
			else
			{
				m_go.transform.SetParent(canvas.transform, worldPositionStays);
			}
		}

		public void SetMask(GameObject maskObject)
		{
			Mask component = maskObject.GetComponent<Mask>();
			if (component == null)
			{
				Debug.LogError("VectorLine.SetMask: mask object must have a Mask component");
			}
			else
			{
				SetMask(component);
			}
		}

		public void SetMask(Mask mask)
		{
			if (m_canvasState == CanvasState.OffCanvas)
			{
				Debug.LogError("VectorLine.SetMask only works with lines made with Draw, not Draw3D.");
			}
			else if (mask == null)
			{
				Debug.LogError("VectorLine.SetMask: mask must not be null");
			}
			else
			{
				m_go.transform.SetParent(mask.transform, true);
			}
		}

		private bool CheckCamera3D()
		{
			if (!m_is2D && !cam3D)
			{
				SetCamera3D();
				if (!cam3D)
				{
					Debug.LogError("No camera available...use VectorLine.SetCamera3D to assign a camera");
					return false;
				}
			}
			return true;
		}

		public static void SetCamera3D()
		{
			if (Camera.main == null)
			{
				Debug.LogError("VectorLine.SetCamera3D: no camera tagged \"Main Camera\" found. Please call SetCamera3D with a specific camera instead.");
			}
			else
			{
				SetCamera3D(Camera.main);
			}
		}

		public static void SetCamera3D(GameObject cameraObject)
		{
			Camera component = cameraObject.GetComponent<Camera>();
			if (component == null)
			{
				Debug.LogError("VectorLine.SetCamera3D: camera object must have a Camera component");
			}
			else
			{
				SetCamera3D(component);
			}
		}

		public static void SetCamera3D(Camera camera)
		{
			camTransform = camera.transform;
			cam3D = camera;
			oldPosition = camTransform.position + Vector3.one;
			oldRotation = camTransform.eulerAngles + Vector3.one;
		}

		public static bool CameraHasMoved()
		{
			return oldPosition != camTransform.position || oldRotation != camTransform.eulerAngles;
		}

		public static void UpdateCameraInfo()
		{
			oldPosition = camTransform.position;
			oldRotation = camTransform.eulerAngles;
		}

		public int GetSegmentNumber()
		{
			if (m_lineType == LineType.Points)
			{
				return pointsCount;
			}
			if (m_lineType == LineType.Continuous)
			{
				return (pointsCount != 0) ? (pointsCount - 1) : 0;
			}
			return pointsCount / 2;
		}

		private void SetEndCapColors()
		{
			if (m_lineVertices.Length < 4)
			{
				return;
			}
			if (m_capType <= EndCap.Mirror)
			{
				int num = ((m_lineType != 0) ? (m_drawStart * 2) : (m_drawStart * 4));
				for (int i = 0; i < 4; i++)
				{
					ref Color32 reference = ref m_lineColors[i + m_vertexCount];
					reference = ((!m_useCapColors) ? m_lineColors[i + num] : m_frontColor);
				}
			}
			if (m_capType >= EndCap.Both)
			{
				int num2 = m_drawEnd;
				if (m_lineType == LineType.Continuous)
				{
					if (m_drawEnd == pointsCount)
					{
						num2--;
					}
				}
				else if (num2 < pointsCount)
				{
					num2++;
				}
				int num3 = num2 * ((m_lineType != 0) ? 2 : 4) - 8;
				if (num3 < -4)
				{
					num3 = -4;
				}
				for (int j = 4; j < 8; j++)
				{
					ref Color32 reference2 = ref m_lineColors[j + m_vertexCount];
					reference2 = ((!m_useCapColors) ? m_lineColors[j + num3] : m_backColor);
				}
			}
			if (m_vectorObject != null)
			{
				m_vectorObject.UpdateColors();
			}
		}

		public void SetEndCapColor(Color32 color)
		{
			SetEndCapColor(color, color);
		}

		public void SetEndCapColor(Color32 frontColor, Color32 backColor)
		{
			if (m_capType == EndCap.None)
			{
				Debug.LogError("VectorLine.SetEndCapColor: the line \"" + name + "\" does not have any end caps");
				return;
			}
			m_useCapColors = true;
			m_frontColor = frontColor;
			m_backColor = backColor;
			SetEndCapColors();
		}

		public void SetColor(Color32 color)
		{
			SetColor(color, 0, pointsCount);
		}

		public void SetColor(Color32 color, int index)
		{
			SetColor(color, index, index);
		}

		public void SetColor(Color32 color, int startIndex, int endIndex)
		{
			if (pointsCount != m_pointsCount)
			{
				Resize();
			}
			int segmentNumber = GetSegmentNumber();
			startIndex = Mathf.Clamp(startIndex * 4, 0, segmentNumber * 4);
			endIndex = Mathf.Clamp((endIndex + 1) * 4, 0, segmentNumber * 4);
			if (!m_smoothColor)
			{
				for (int i = startIndex; i < endIndex; i++)
				{
					m_lineColors[i] = color;
				}
			}
			else
			{
				if (startIndex == 0)
				{
					m_lineColors[0] = color;
					m_lineColors[3] = color;
				}
				for (int j = startIndex; j < endIndex; j += 4)
				{
					m_lineColors[j + 1] = color;
					m_lineColors[j + 2] = color;
					if (j + 4 < m_vertexCount)
					{
						m_lineColors[j + 4] = color;
						m_lineColors[j + 7] = color;
					}
				}
			}
			if (m_capType != EndCap.None && (startIndex <= 0 || endIndex >= segmentNumber - 1))
			{
				SetEndCapColors();
			}
			if (m_vectorObject != null)
			{
				m_vectorObject.UpdateColors();
			}
		}

		public void SetColors(List<Color32> lineColors)
		{
			if (lineColors == null)
			{
				Debug.LogError("VectorLine.SetColors: lineColors list must not be null");
				return;
			}
			if (pointsCount != m_pointsCount)
			{
				Resize();
			}
			if (m_lineType != LineType.Points)
			{
				if (WrongArrayLength(lineColors.Count, FunctionName.SetColors))
				{
					return;
				}
			}
			else if (lineColors.Count != pointsCount)
			{
				Debug.LogError("VectorLine.SetColors: Length of lineColors list in \"" + name + "\" must be same length as points list");
				return;
			}
			SetSegmentStartEnd(out var start, out var end);
			if (start == 0 && end == 0)
			{
				return;
			}
			int num = start * 4;
			if (m_lineType == LineType.Points)
			{
				end++;
			}
			if (smoothColor)
			{
				ref Color32 reference = ref m_lineColors[num];
				reference = lineColors[start];
				ref Color32 reference2 = ref m_lineColors[num + 3];
				reference2 = lineColors[start];
				ref Color32 reference3 = ref m_lineColors[num + 2];
				reference3 = lineColors[start];
				ref Color32 reference4 = ref m_lineColors[num + 1];
				reference4 = lineColors[start];
				num += 4;
				for (int i = start + 1; i < end; i++)
				{
					ref Color32 reference5 = ref m_lineColors[num];
					reference5 = lineColors[i - 1];
					ref Color32 reference6 = ref m_lineColors[num + 3];
					reference6 = lineColors[i - 1];
					ref Color32 reference7 = ref m_lineColors[num + 2];
					reference7 = lineColors[i];
					ref Color32 reference8 = ref m_lineColors[num + 1];
					reference8 = lineColors[i];
					num += 4;
				}
			}
			else
			{
				for (int j = start; j < end; j++)
				{
					ref Color32 reference9 = ref m_lineColors[num];
					reference9 = lineColors[j];
					ref Color32 reference10 = ref m_lineColors[num + 1];
					reference10 = lineColors[j];
					ref Color32 reference11 = ref m_lineColors[num + 2];
					reference11 = lineColors[j];
					ref Color32 reference12 = ref m_lineColors[num + 3];
					reference12 = lineColors[j];
					num += 4;
				}
			}
			if (m_capType != EndCap.None)
			{
				SetEndCapColors();
			}
			if (m_vectorObject != null)
			{
				m_vectorObject.UpdateColors();
			}
		}

		private void SetSegmentStartEnd(out int start, out int end)
		{
			start = ((m_lineType == LineType.Discrete) ? (m_drawStart / 2) : m_drawStart);
			end = m_drawEnd;
			if (m_lineType == LineType.Discrete)
			{
				end = m_drawEnd / 2;
				if (m_drawEnd % 2 != 0)
				{
					end++;
				}
			}
		}

		public Color32 GetColor(int index)
		{
			if (pointsCount != m_pointsCount)
			{
				Resize();
			}
			if (m_vertexCount == 0)
			{
				return m_color;
			}
			int num = index * 4 + 2;
			if (num < 0 || num >= m_vertexCount)
			{
				Debug.LogError("VectorLine.GetColor: index " + index + " out of range");
				return Color.clear;
			}
			return m_lineColors[num];
		}

		private void SetupWidths(int max)
		{
			if ((max >= 2 && m_lineWidths.Length == 1) || (max >= 2 && m_lineWidths.Length != max))
			{
				Array.Resize(ref m_lineWidths, max);
				for (int i = 0; i < max; i++)
				{
					m_lineWidths[i] = m_lineWidth * 0.5f;
				}
			}
		}

		public void SetWidth(float width)
		{
			m_lineWidth = width;
			SetWidth(width, 0, pointsCount);
		}

		public void SetWidth(float width, int index)
		{
			SetWidth(width, index, index);
		}

		public void SetWidth(float width, int startIndex, int endIndex)
		{
			if (pointsCount != m_pointsCount)
			{
				Resize();
			}
			int segmentNumber = GetSegmentNumber();
			SetupWidths(segmentNumber);
			startIndex = Mathf.Clamp(startIndex, 0, Mathf.Max(segmentNumber - 1, 0));
			endIndex = Mathf.Clamp(endIndex, 0, Mathf.Max(segmentNumber - 1, 0));
			for (int i = startIndex; i <= endIndex; i++)
			{
				m_lineWidths[i] = width * 0.5f;
			}
		}

		public void SetWidths(List<float> lineWidths)
		{
			SetWidths(lineWidths, null, lineWidths.Count, true);
		}

		public void SetWidths(List<int> lineWidths)
		{
			SetWidths(null, lineWidths, lineWidths.Count, false);
		}

		private void SetWidths(List<float> lineWidthsFloat, List<int> lineWidthsInt, int arrayLength, bool doFloat)
		{
			if ((doFloat && lineWidthsFloat == null) || (!doFloat && lineWidthsInt == null))
			{
				Debug.LogError("VectorLine.SetWidths: line widths list must not be null");
				return;
			}
			if (pointsCount != m_pointsCount)
			{
				Resize();
			}
			if (m_lineType == LineType.Points)
			{
				if (arrayLength != pointsCount)
				{
					Debug.LogError("VectorLine.SetWidths: line widths list must be the same length as the points list for \"" + name + "\"");
					return;
				}
			}
			else if (WrongArrayLength(arrayLength, FunctionName.SetWidths))
			{
				return;
			}
			if (m_lineWidths.Length != arrayLength)
			{
				Array.Resize(ref m_lineWidths, arrayLength);
			}
			if (doFloat)
			{
				for (int i = 0; i < arrayLength; i++)
				{
					m_lineWidths[i] = lineWidthsFloat[i] * 0.5f;
				}
			}
			else
			{
				for (int j = 0; j < arrayLength; j++)
				{
					m_lineWidths[j] = (float)lineWidthsInt[j] * 0.5f;
				}
			}
		}

		public float GetWidth(int index)
		{
			if (pointsCount != m_pointsCount)
			{
				Resize();
			}
			int segmentNumber = GetSegmentNumber();
			if (index < 0 || index >= segmentNumber)
			{
				Debug.LogError("VectorLine.GetWidth: index " + index + " out of range...must be >= 0 and < " + segmentNumber);
				return 0f;
			}
			if (index >= m_lineWidths.Length)
			{
				return m_lineWidth;
			}
			return m_lineWidths[index] * 2f;
		}

		public static VectorLine SetLine(Color color, params Vector2[] points)
		{
			return SetLine(color, 0f, points);
		}

		public static VectorLine SetLine(Color color, float time, params Vector2[] points)
		{
			if (points.Length < 2)
			{
				Debug.LogError("VectorLine.SetLine needs at least two points");
				return null;
			}
			VectorLine vectorLine = new VectorLine("Line", new List<Vector2>(points), null, 1f, LineType.Continuous, Joins.None);
			vectorLine.color = color;
			if (time > 0f)
			{
				lineManager.DisableLine(vectorLine, time);
			}
			vectorLine.Draw();
			return vectorLine;
		}

		public static VectorLine SetLine(Color color, params Vector3[] points)
		{
			return SetLine(color, 0f, points);
		}

		public static VectorLine SetLine(Color color, float time, params Vector3[] points)
		{
			if (points.Length < 2)
			{
				Debug.LogError("VectorLine.SetLine needs at least two points");
				return null;
			}
			VectorLine vectorLine = new VectorLine("SetLine", new List<Vector3>(points), null, 1f, LineType.Continuous, Joins.None);
			vectorLine.color = color;
			if (time > 0f)
			{
				lineManager.DisableLine(vectorLine, time);
			}
			vectorLine.Draw();
			return vectorLine;
		}

		public static VectorLine SetLine3D(Color color, params Vector3[] points)
		{
			return SetLine3D(color, 0f, points);
		}

		public static VectorLine SetLine3D(Color color, float time, params Vector3[] points)
		{
			if (points.Length < 2)
			{
				Debug.LogError("VectorLine.SetLine3D needs at least two points");
				return null;
			}
			VectorLine vectorLine = new VectorLine("SetLine3D", new List<Vector3>(points), null, 1f, LineType.Continuous, Joins.None);
			vectorLine.color = color;
			vectorLine.Draw3DAuto(time);
			return vectorLine;
		}

		public static VectorLine SetRay(Color color, Vector3 origin, Vector3 direction)
		{
			return SetRay(color, 0f, origin, direction);
		}

		public static VectorLine SetRay(Color color, float time, Vector3 origin, Vector3 direction)
		{
			VectorLine vectorLine = new VectorLine("SetRay", new List<Vector3>(new Vector3[2]
			{
				origin,
				new Ray(origin, direction).GetPoint(direction.magnitude)
			}), null, 1f, LineType.Continuous, Joins.None);
			vectorLine.color = color;
			if (time > 0f)
			{
				lineManager.DisableLine(vectorLine, time);
			}
			vectorLine.Draw();
			return vectorLine;
		}

		public static VectorLine SetRay3D(Color color, Vector3 origin, Vector3 direction)
		{
			return SetRay3D(color, 0f, origin, direction);
		}

		public static VectorLine SetRay3D(Color color, float time, Vector3 origin, Vector3 direction)
		{
			VectorLine vectorLine = new VectorLine("SetRay3D", new List<Vector3>(new Vector3[2]
			{
				origin,
				new Ray(origin, direction).GetPoint(direction.magnitude)
			}), null, 1f, LineType.Continuous, Joins.None);
			vectorLine.color = color;
			vectorLine.Draw3DAuto(time);
			return vectorLine;
		}

		private void CheckNormals()
		{
			if (m_useNormals && !m_normalsCalculated)
			{
				m_vectorObject.UpdateNormals();
				m_normalsCalculated = true;
			}
			if (m_useTangents && !m_tangentsCalculated)
			{
				m_vectorObject.UpdateTangents();
				m_tangentsCalculated = true;
			}
		}

		private void CheckLine(bool draw3D)
		{
			if (m_capType != EndCap.None)
			{
				DrawEndCap(draw3D);
			}
			if (m_continuousTexture)
			{
				SetContinuousTexture();
			}
			if (m_joins == Joins.Fill)
			{
				SetLastFillTriangles();
			}
		}

		private void DrawEndCap(bool draw3D)
		{
			if (m_capType <= EndCap.Mirror)
			{
				int num = m_drawStart * 4;
				int num2 = ((m_lineWidths.Length > 1) ? m_drawStart : 0);
				if (m_lineType == LineType.Discrete)
				{
					num2 /= 2;
					num /= 2;
				}
				if (!draw3D)
				{
					Vector3 vector = (m_lineVertices[num] - m_lineVertices[num + 1]).normalized * m_lineWidths[num2] * 2f * capDictionary[m_endCap].ratio1;
					Vector3 vector2 = vector * capDictionary[m_endCap].offset1;
					ref Vector3 reference = ref m_lineVertices[m_vertexCount];
					reference = m_lineVertices[num] + vector + vector2;
					ref Vector3 reference2 = ref m_lineVertices[m_vertexCount + 3];
					reference2 = m_lineVertices[num + 3] + vector + vector2;
					m_lineVertices[num] += vector2;
					m_lineVertices[num + 3] += vector2;
				}
				else
				{
					Vector3 vector3 = (m_screenPoints[num] - m_screenPoints[num + 1]).normalized * m_lineWidths[num2] * 2f * capDictionary[m_endCap].ratio1;
					Vector3 vector4 = vector3 * capDictionary[m_endCap].offset1;
					ref Vector3 reference3 = ref m_lineVertices[m_vertexCount];
					reference3 = cam3D.ScreenToWorldPoint(m_screenPoints[num] + vector3 + vector4);
					ref Vector3 reference4 = ref m_lineVertices[m_vertexCount + 3];
					reference4 = cam3D.ScreenToWorldPoint(m_screenPoints[num + 3] + vector3 + vector4);
					ref Vector3 reference5 = ref m_lineVertices[num];
					reference5 = cam3D.ScreenToWorldPoint(m_screenPoints[num] + vector4);
					ref Vector3 reference6 = ref m_lineVertices[num + 3];
					reference6 = cam3D.ScreenToWorldPoint(m_screenPoints[num + 3] + vector4);
				}
				ref Vector3 reference7 = ref m_lineVertices[m_vertexCount + 2];
				reference7 = m_lineVertices[num + 3];
				ref Vector3 reference8 = ref m_lineVertices[m_vertexCount + 1];
				reference8 = m_lineVertices[num];
				if (capDictionary[m_endCap].scale1 != 1f)
				{
					ScaleCapVertices(m_vertexCount, capDictionary[m_endCap].scale1, (m_lineVertices[m_vertexCount + 1] + m_lineVertices[m_vertexCount + 2]) / 2f);
				}
				m_lineTriangles[0] = m_vertexCount;
				m_lineTriangles[1] = m_vertexCount + 1;
				m_lineTriangles[2] = m_vertexCount + 3;
				m_lineTriangles[3] = m_vertexCount + 1;
				m_lineTriangles[4] = m_vertexCount + 2;
				m_lineTriangles[5] = m_vertexCount + 3;
			}
			if (m_capType >= EndCap.Both)
			{
				int num3 = m_drawEnd;
				if (m_lineType == LineType.Continuous)
				{
					if (m_drawEnd == pointsCount)
					{
						num3--;
					}
				}
				else if (num3 < pointsCount)
				{
					num3++;
				}
				int num4 = num3 * 4;
				int num5 = ((m_lineWidths.Length > 1) ? (num3 - 1) : 0);
				if (num5 < 0)
				{
					num5 = 0;
				}
				if (m_lineType == LineType.Discrete)
				{
					num5 /= 2;
					num4 /= 2;
				}
				if (num4 < 4)
				{
					num4 = 4;
				}
				if (!draw3D)
				{
					Vector3 vector5 = (m_lineVertices[num4 - 2] - m_lineVertices[num4 - 1]).normalized * m_lineWidths[num5] * 2f * capDictionary[m_endCap].ratio2;
					Vector3 vector6 = vector5 * capDictionary[m_endCap].offset2;
					ref Vector3 reference9 = ref m_lineVertices[m_vertexCount + 6];
					reference9 = m_lineVertices[num4 - 2] + vector5 + vector6;
					ref Vector3 reference10 = ref m_lineVertices[m_vertexCount + 5];
					reference10 = m_lineVertices[num4 - 3] + vector5 + vector6;
					m_lineVertices[num4 - 3] += vector6;
					m_lineVertices[num4 - 2] += vector6;
				}
				else
				{
					Vector3 vector7 = (m_screenPoints[num4 - 2] - m_screenPoints[num4 - 1]).normalized * m_lineWidths[num5] * 2f * capDictionary[m_endCap].ratio2;
					Vector3 vector8 = vector7 * capDictionary[m_endCap].offset2;
					ref Vector3 reference11 = ref m_lineVertices[m_vertexCount + 6];
					reference11 = cam3D.ScreenToWorldPoint(m_screenPoints[num4 - 2] + vector7 + vector8);
					ref Vector3 reference12 = ref m_lineVertices[m_vertexCount + 5];
					reference12 = cam3D.ScreenToWorldPoint(m_screenPoints[num4 - 3] + vector7 + vector8);
					ref Vector3 reference13 = ref m_lineVertices[num4 - 3];
					reference13 = cam3D.ScreenToWorldPoint(m_screenPoints[num4 - 3] + vector8);
					ref Vector3 reference14 = ref m_lineVertices[num4 - 2];
					reference14 = cam3D.ScreenToWorldPoint(m_screenPoints[num4 - 2] + vector8);
				}
				ref Vector3 reference15 = ref m_lineVertices[m_vertexCount + 4];
				reference15 = m_lineVertices[num4 - 3];
				ref Vector3 reference16 = ref m_lineVertices[m_vertexCount + 7];
				reference16 = m_lineVertices[num4 - 2];
				if (capDictionary[m_endCap].scale2 != 1f)
				{
					ScaleCapVertices(m_vertexCount + 4, capDictionary[m_endCap].scale2, (m_lineVertices[m_vertexCount + 4] + m_lineVertices[m_vertexCount + 7]) / 2f);
				}
				m_lineTriangles[6] = m_vertexCount + 4;
				m_lineTriangles[7] = m_vertexCount + 5;
				m_lineTriangles[8] = m_vertexCount + 7;
				m_lineTriangles[9] = m_vertexCount + 5;
				m_lineTriangles[10] = m_vertexCount + 6;
				m_lineTriangles[11] = m_vertexCount + 7;
			}
			if (m_drawStart > 0 || m_drawEnd < pointsCount)
			{
				SetEndCapColors();
			}
		}

		private void ScaleCapVertices(int offset, float scale, Vector3 center)
		{
			ref Vector3 reference = ref m_lineVertices[offset];
			reference = (m_lineVertices[offset] - center) * scale + center;
			ref Vector3 reference2 = ref m_lineVertices[offset + 1];
			reference2 = (m_lineVertices[offset + 1] - center) * scale + center;
			ref Vector3 reference3 = ref m_lineVertices[offset + 2];
			reference3 = (m_lineVertices[offset + 2] - center) * scale + center;
			ref Vector3 reference4 = ref m_lineVertices[offset + 3];
			reference4 = (m_lineVertices[offset + 3] - center) * scale + center;
		}

		private void SetContinuousTexture()
		{
			int num = 0;
			float x = 0f;
			SetDistances();
			int num2 = m_distances.Length - 1;
			float num3 = m_distances[num2];
			for (int i = 0; i < num2; i++)
			{
				m_lineUVs[num].x = x;
				m_lineUVs[num + 3].x = x;
				x = 1f / (num3 / m_distances[i + 1]);
				m_lineUVs[num + 1].x = x;
				m_lineUVs[num + 2].x = x;
				num += 4;
			}
			if (m_vectorObject != null)
			{
				m_vectorObject.UpdateUVs();
			}
		}

		private bool UseMatrix(out Matrix4x4 thisMatrix)
		{
			if (m_drawTransform != null)
			{
				thisMatrix = m_drawTransform.localToWorldMatrix;
				return true;
			}
			if (m_useMatrix)
			{
				thisMatrix = m_matrix;
				return true;
			}
			thisMatrix = Matrix4x4.identity;
			return false;
		}

		private bool CheckPointCount()
		{
			if (pointsCount < ((m_lineType == LineType.Points) ? 1 : 2))
			{
				ClearTriangles();
				m_vectorObject.ClearMesh();
				m_pointsCount = pointsCount;
				m_drawEnd = 0;
				return false;
			}
			return true;
		}

		private void ClearTriangles()
		{
			if (m_capType == EndCap.None)
			{
				m_lineTriangles.Clear();
			}
			else
			{
				m_lineTriangles.RemoveRange(12, m_lineTriangles.Count - 12);
			}
		}

		private void SetupDrawStartEnd(out int start, out int end, bool clearVertices)
		{
			start = 0;
			end = m_pointsCount - 1;
			if (m_drawStart > 0)
			{
				start = m_drawStart;
				if (clearVertices)
				{
					ZeroVertices(0, start);
				}
			}
			if (m_drawEnd < m_pointsCount - 1)
			{
				end = m_drawEnd;
				if (end < 0)
				{
					end = 0;
				}
				if (clearVertices)
				{
					ZeroVertices(end, m_pointsCount);
				}
			}
			if (m_endPointsUpdate > 0)
			{
				start = Mathf.Max(0, end - m_endPointsUpdate);
			}
		}

		private void ZeroVertices(int startIndex, int endIndex)
		{
			if (m_lineType != LineType.Discrete)
			{
				startIndex *= 4;
				endIndex *= 4;
				if (endIndex > m_vertexCount)
				{
					endIndex -= 4;
				}
				for (int i = startIndex; i < endIndex; i += 4)
				{
					ref Vector3 reference = ref m_lineVertices[i];
					reference = v3zero;
					ref Vector3 reference2 = ref m_lineVertices[i + 1];
					reference2 = v3zero;
					ref Vector3 reference3 = ref m_lineVertices[i + 2];
					reference3 = v3zero;
					ref Vector3 reference4 = ref m_lineVertices[i + 3];
					reference4 = v3zero;
				}
			}
			else
			{
				startIndex *= 2;
				endIndex *= 2;
				for (int j = startIndex; j < endIndex; j += 2)
				{
					ref Vector3 reference5 = ref m_lineVertices[j];
					reference5 = v3zero;
					ref Vector3 reference6 = ref m_lineVertices[j + 1];
					reference6 = v3zero;
				}
			}
		}

		private void SetupCanvasState(CanvasState wantedState)
		{
			if (wantedState == CanvasState.OnCanvas)
			{
				if (m_go == null)
				{
					return;
				}
				if (m_go.transform.parent == null || m_go.transform.root.GetComponent<Canvas>() == null)
				{
					if (m_canvas == null)
					{
						SetupVectorCanvas();
					}
					m_go.transform.SetParent(m_canvas.transform, true);
				}
				m_canvasState = CanvasState.OnCanvas;
				if (m_go.GetComponent<VectorObject3D>() != null)
				{
					UnityEngine.Object.DestroyImmediate(m_go.GetComponent<VectorObject3D>());
					UnityEngine.Object.DestroyImmediate(m_go.GetComponent<MeshFilter>());
					UnityEngine.Object.DestroyImmediate(m_go.GetComponent<MeshRenderer>());
				}
				if (m_go.GetComponent<VectorObject2D>() == null)
				{
					m_vectorObject = m_go.AddComponent<VectorObject2D>();
				}
				else
				{
					m_vectorObject = m_go.GetComponent<VectorObject2D>();
				}
				m_vectorObject.SetVectorLine(this, m_texture, m_material);
			}
			else
			{
				if (m_go == null)
				{
					return;
				}
				m_go.transform.SetParent(null);
				m_canvasState = CanvasState.OffCanvas;
				if (m_go.GetComponent<VectorObject2D>() != null)
				{
					UnityEngine.Object.DestroyImmediate(m_go.GetComponent<VectorObject2D>());
					UnityEngine.Object.DestroyImmediate(m_go.GetComponent<CanvasRenderer>());
				}
				if (m_go.GetComponent<VectorObject3D>() == null)
				{
					m_vectorObject = m_go.AddComponent<VectorObject3D>();
					if (m_material == null)
					{
						m_material = Resources.Load("DefaultLine3D") as Material;
						if (m_material == null)
						{
							Debug.LogError("No DefaultLine3D material found in Resources");
							return;
						}
					}
				}
				else
				{
					m_vectorObject = m_go.GetComponent<VectorObject3D>();
				}
				m_vectorObject.SetVectorLine(this, m_texture, m_material);
			}
		}

		public void Draw()
		{
			if (!m_active)
			{
				return;
			}
			if (m_canvasState != CanvasState.OnCanvas)
			{
				SetupCanvasState(CanvasState.OnCanvas);
			}
			if (m_vectorObject == null)
			{
				m_vectorObject = m_go.GetComponent<VectorObject2D>();
			}
			if (!CheckPointCount() || m_lineWidths == null)
			{
				return;
			}
			if (pointsCount != m_pointsCount)
			{
				Resize();
			}
			if (m_lineType == LineType.Points)
			{
				DrawPoints();
				return;
			}
			Matrix4x4 thisMatrix;
			bool useTransformMatrix = UseMatrix(out thisMatrix);
			int start = 0;
			int end = 0;
			SetupDrawStartEnd(out start, out end, true);
			if (m_is2D)
			{
				Line2D(start, end, thisMatrix, useTransformMatrix);
			}
			else
			{
				Line3D(start, end, thisMatrix, useTransformMatrix);
			}
			CheckNormals();
			CheckLine(false);
			if (m_useTextureScale)
			{
				SetTextureScale();
			}
			m_vectorObject.UpdateVerts();
		}

		private void Line2D(int start, int end, Matrix4x4 thisMatrix, bool useTransformMatrix)
		{
			Vector3 vector = v3zero;
			Vector3 vector2 = v3zero;
			Vector3 vector3 = v3zero;
			Vector3 vector4 = v3zero;
			Vector2 vector5 = new Vector2(Screen.width, Screen.height);
			int num = 0;
			int num2 = 0;
			int widthIdx = 0;
			int widthIdxAdd = 0;
			if (m_lineWidths.Length > 1)
			{
				widthIdx = start;
				widthIdxAdd = 1;
			}
			if (m_lineType == LineType.Continuous)
			{
				num = 1;
				num2 = start * 4;
			}
			else
			{
				num = 2;
				widthIdx /= 2;
				num2 = start * 2;
			}
			float num3 = 0f;
			for (int i = start; i < end; i += num)
			{
				if (useTransformMatrix)
				{
					vector = thisMatrix.MultiplyPoint3x4(m_points2[i]);
					vector2 = thisMatrix.MultiplyPoint3x4(m_points2[i + 1]);
				}
				else
				{
					vector.x = m_points2[i].x;
					vector.y = m_points2[i].y;
					vector2.x = m_points2[i + 1].x;
					vector2.y = m_points2[i + 1].y;
				}
				if (m_viewportDraw)
				{
					vector.x *= vector5.x;
					vector.y *= vector5.y;
					vector2.x *= vector5.x;
					vector2.y *= vector5.y;
				}
				if (vector.x == vector2.x && vector.y == vector2.y)
				{
					SkipQuad(ref num2, ref widthIdx, ref widthIdxAdd);
					continue;
				}
				if (m_capLength == 0f)
				{
					vector4.x = vector2.y - vector.y;
					vector4.y = vector.x - vector2.x;
					num3 = 1f / (float)Math.Sqrt(vector4.x * vector4.x + vector4.y * vector4.y);
					vector4 *= num3 * m_lineWidths[widthIdx];
					m_lineVertices[num2].x = vector.x - vector4.x;
					m_lineVertices[num2].y = vector.y - vector4.y;
					m_lineVertices[num2 + 3].x = vector.x + vector4.x;
					m_lineVertices[num2 + 3].y = vector.y + vector4.y;
					if (m_smoothWidth && i < end - num)
					{
						vector4.x = vector2.y - vector.y;
						vector4.y = vector.x - vector2.x;
						vector4 *= num3 * m_lineWidths[widthIdx + 1];
					}
				}
				else
				{
					vector4.x = vector2.x - vector.x;
					vector4.y = vector2.y - vector.y;
					vector4 *= 1f / (float)Math.Sqrt(vector4.x * vector4.x + vector4.y * vector4.y);
					vector -= vector4 * m_capLength;
					vector2 += vector4 * m_capLength;
					vector3.x = vector4.y;
					vector3.y = 0f - vector4.x;
					vector4 = vector3 * m_lineWidths[widthIdx];
					m_lineVertices[num2].x = vector.x - vector4.x;
					m_lineVertices[num2].y = vector.y - vector4.y;
					m_lineVertices[num2 + 3].x = vector.x + vector4.x;
					m_lineVertices[num2 + 3].y = vector.y + vector4.y;
					if (m_smoothWidth && i < end - num)
					{
						vector4 = vector3 * m_lineWidths[widthIdx + 1];
					}
				}
				m_lineVertices[num2 + 2].x = vector2.x + vector4.x;
				m_lineVertices[num2 + 2].y = vector2.y + vector4.y;
				m_lineVertices[num2 + 1].x = vector2.x - vector4.x;
				m_lineVertices[num2 + 1].y = vector2.y - vector4.y;
				num2 += 4;
				widthIdx += widthIdxAdd;
			}
			if (m_joins != Joins.Weld)
			{
				return;
			}
			if (m_lineType == LineType.Continuous)
			{
				WeldJoins(start * 4 + ((start == 0) ? 4 : 0), end * 4, Approximately(m_points2[0], m_points2[m_pointsCount - 1]));
				return;
			}
			if ((end & 1) == 0)
			{
				end--;
			}
			WeldJoinsDiscrete(start + 1, end, Approximately(m_points2[0], m_points2[m_pointsCount - 1]));
		}

		private void Line3D(int start, int end, Matrix4x4 thisMatrix, bool useTransformMatrix)
		{
			if (!CheckCamera3D())
			{
				return;
			}
			Vector3 vector = v3zero;
			Vector3 vector2 = v3zero;
			Vector3 vector3 = v3zero;
			Vector3 vector4 = v3zero;
			Vector3 vector5 = v3zero;
			Vector3 vector6 = v3zero;
			float num = 0f;
			int widthIdx = 0;
			int widthIdxAdd = 0;
			if (m_lineWidths.Length > 1)
			{
				widthIdx = start;
				widthIdxAdd = 1;
			}
			int idx = start * 2;
			int num2 = 2;
			if (m_lineType == LineType.Continuous)
			{
				idx = start * 4;
				num2 = 1;
			}
			Plane cameraPlane = new Plane(camTransform.forward, camTransform.position + camTransform.forward * cam3D.nearClipPlane);
			Ray ray = new Ray(v3zero, v3zero);
			float screenHeight = Screen.height;
			for (int i = start; i < end; i += num2)
			{
				if (useTransformMatrix)
				{
					vector5 = thisMatrix.MultiplyPoint3x4(m_points3[i]);
					vector6 = thisMatrix.MultiplyPoint3x4(m_points3[i + 1]);
				}
				else
				{
					vector5 = m_points3[i];
					vector6 = m_points3[i + 1];
				}
				vector = cam3D.WorldToScreenPoint(vector5);
				vector2 = cam3D.WorldToScreenPoint(vector6);
				if ((vector.x == vector2.x && vector.y == vector2.y) || IntersectAndDoSkip(ref vector, ref vector2, ref vector5, ref vector6, ref screenHeight, ref ray, ref cameraPlane))
				{
					SkipQuad(ref idx, ref widthIdx, ref widthIdxAdd);
					continue;
				}
				if (m_capLength == 0f)
				{
					vector4.x = vector2.y - vector.y;
					vector4.y = vector.x - vector2.x;
					num = 1f / (float)Math.Sqrt(vector4.x * vector4.x + vector4.y * vector4.y);
					vector4.x *= num * m_lineWidths[widthIdx];
					vector4.y *= num * m_lineWidths[widthIdx];
					m_lineVertices[idx].x = vector.x - vector4.x;
					m_lineVertices[idx].y = vector.y - vector4.y;
					m_lineVertices[idx + 3].x = vector.x + vector4.x;
					m_lineVertices[idx + 3].y = vector.y + vector4.y;
					if (m_smoothWidth && i < end - num2)
					{
						vector4.x = vector2.y - vector.y;
						vector4.y = vector.x - vector2.x;
						vector4.x *= num * m_lineWidths[widthIdx + 1];
						vector4.y *= num * m_lineWidths[widthIdx + 1];
					}
				}
				else
				{
					vector4.x = vector2.x - vector.x;
					vector4.y = vector2.y - vector.y;
					vector4 *= 1f / (float)Math.Sqrt(vector4.x * vector4.x + vector4.y * vector4.y);
					vector -= vector4 * m_capLength;
					vector2 += vector4 * m_capLength;
					vector3.x = vector4.y;
					vector3.y = 0f - vector4.x;
					vector4 = vector3 * m_lineWidths[widthIdx];
					m_lineVertices[idx].x = vector.x - vector4.x;
					m_lineVertices[idx].y = vector.y - vector4.y;
					m_lineVertices[idx + 3].x = vector.x + vector4.x;
					m_lineVertices[idx + 3].y = vector.y + vector4.y;
					if (m_smoothWidth && i < end - num2)
					{
						vector4 = vector3 * m_lineWidths[widthIdx + 1];
					}
				}
				m_lineVertices[idx + 2].x = vector2.x + vector4.x;
				m_lineVertices[idx + 2].y = vector2.y + vector4.y;
				m_lineVertices[idx + 1].x = vector2.x - vector4.x;
				m_lineVertices[idx + 1].y = vector2.y - vector4.y;
				idx += 4;
				widthIdx += widthIdxAdd;
			}
			if (m_joins != Joins.Weld)
			{
				return;
			}
			if (m_lineType == LineType.Continuous)
			{
				WeldJoins(start * 4 + ((start == 0) ? 4 : 0), end * 4, start == 0 && end == m_pointsCount - 1 && Approximately(m_points3[0], m_points3[m_pointsCount - 1]));
				return;
			}
			if ((end & 1) == 0)
			{
				end--;
			}
			WeldJoinsDiscrete(start + 1, end, start == 0 && end == m_pointsCount - 1 && Approximately(m_points3[0], m_points3[m_pointsCount - 1]));
		}

		public void Draw3D()
		{
			if (!m_active)
			{
				return;
			}
			if (m_is2D)
			{
				Debug.LogError("VectorLine.Draw3D can only be used with a Vector3 array, which \"" + name + "\" doesn't have");
				return;
			}
			if (m_canvasState != CanvasState.OffCanvas)
			{
				SetupCanvasState(CanvasState.OffCanvas);
			}
			if (!CheckPointCount() || m_lineWidths == null)
			{
				return;
			}
			if (pointsCount != m_pointsCount)
			{
				Resize();
			}
			if (!CheckCamera3D())
			{
				return;
			}
			if (m_lineType == LineType.Points)
			{
				DrawPoints3D();
				return;
			}
			int start = 0;
			int end = 0;
			int num = 0;
			int widthIdx = 0;
			SetupDrawStartEnd(out start, out end, true);
			Matrix4x4 thisMatrix;
			bool flag = UseMatrix(out thisMatrix);
			int num2 = 0;
			int widthIdxAdd = 0;
			if (m_lineWidths.Length > 1)
			{
				widthIdx = start;
				widthIdxAdd = 1;
			}
			if (m_lineType == LineType.Continuous)
			{
				num = 1;
				num2 = start * 4;
			}
			else
			{
				widthIdx /= 2;
				num = 2;
				num2 = start * 2;
			}
			Vector3 vector = v3zero;
			Vector3 vector2 = v3zero;
			Vector3 vector3 = v3zero;
			Vector3 vector4 = v3zero;
			Vector3 vector5 = v3zero;
			Vector3 vector6 = v3zero;
			Plane cameraPlane = new Plane(camTransform.forward, camTransform.position + camTransform.forward * cam3D.nearClipPlane);
			Ray ray = new Ray(v3zero, v3zero);
			float screenHeight = Screen.height;
			for (int i = start; i < end; i += num)
			{
				if (flag)
				{
					vector5 = thisMatrix.MultiplyPoint3x4(m_points3[i]);
					vector6 = thisMatrix.MultiplyPoint3x4(m_points3[i + 1]);
				}
				else
				{
					vector5 = m_points3[i];
					vector6 = m_points3[i + 1];
				}
				vector3 = cam3D.WorldToScreenPoint(vector5);
				vector4 = cam3D.WorldToScreenPoint(vector6);
				if ((vector3.x == vector4.x && vector3.y == vector4.y) || IntersectAndDoSkip(ref vector3, ref vector4, ref vector5, ref vector6, ref screenHeight, ref ray, ref cameraPlane))
				{
					SkipQuad3D(ref num2, ref widthIdx, ref widthIdxAdd);
					continue;
				}
				vector2.x = vector4.y - vector3.y;
				vector2.y = vector3.x - vector4.x;
				vector = vector2 / (float)Math.Sqrt(vector2.x * vector2.x + vector2.y * vector2.y);
				vector2.x = vector.x * m_lineWidths[widthIdx];
				vector2.y = vector.y * m_lineWidths[widthIdx];
				m_screenPoints[num2].x = vector3.x - vector2.x;
				m_screenPoints[num2].y = vector3.y - vector2.y;
				m_screenPoints[num2].z = vector3.z - vector2.z;
				m_screenPoints[num2 + 3].x = vector3.x + vector2.x;
				m_screenPoints[num2 + 3].y = vector3.y + vector2.y;
				m_screenPoints[num2 + 3].z = vector3.z + vector2.z;
				ref Vector3 reference = ref m_lineVertices[num2];
				reference = cam3D.ScreenToWorldPoint(m_screenPoints[num2]);
				ref Vector3 reference2 = ref m_lineVertices[num2 + 3];
				reference2 = cam3D.ScreenToWorldPoint(m_screenPoints[num2 + 3]);
				if (smoothWidth && i < end - num)
				{
					vector2.x = vector.x * m_lineWidths[widthIdx + 1];
					vector2.y = vector.y * m_lineWidths[widthIdx + 1];
				}
				m_screenPoints[num2 + 2].x = vector4.x + vector2.x;
				m_screenPoints[num2 + 2].y = vector4.y + vector2.y;
				m_screenPoints[num2 + 2].z = vector4.z + vector2.z;
				m_screenPoints[num2 + 1].x = vector4.x - vector2.x;
				m_screenPoints[num2 + 1].y = vector4.y - vector2.y;
				m_screenPoints[num2 + 1].z = vector4.z - vector2.z;
				ref Vector3 reference3 = ref m_lineVertices[num2 + 2];
				reference3 = cam3D.ScreenToWorldPoint(m_screenPoints[num2 + 2]);
				ref Vector3 reference4 = ref m_lineVertices[num2 + 1];
				reference4 = cam3D.ScreenToWorldPoint(m_screenPoints[num2 + 1]);
				num2 += 4;
				widthIdx += widthIdxAdd;
			}
			if (m_joins == Joins.Weld && end - start > 1)
			{
				if (m_lineType == LineType.Continuous)
				{
					WeldJoins3D(start * 4 + ((start == 0) ? 4 : 0), end * 4, start == 0 && end == m_pointsCount - 1 && Approximately(m_points3[0], m_points3[m_pointsCount - 1]));
				}
				else
				{
					if ((end & 1) == 0)
					{
						end--;
					}
					WeldJoinsDiscrete3D(start + 1, end, start == 0 && end == m_pointsCount - 1 && Approximately(m_points3[0], m_points3[m_pointsCount - 1]));
				}
			}
			CheckLine(true);
			if (m_useTextureScale)
			{
				SetTextureScale();
			}
			m_vectorObject.UpdateVerts();
			CheckNormals();
		}

		private bool IntersectAndDoSkip(ref Vector3 pos1, ref Vector3 pos2, ref Vector3 p1, ref Vector3 p2, ref float screenHeight, ref Ray ray, ref Plane cameraPlane)
		{
			if (pos1.z < 0f)
			{
				if (pos2.z < 0f)
				{
					return true;
				}
				pos1 = cam3D.WorldToScreenPoint(PlaneIntersectionPoint(ref ray, ref cameraPlane, ref p2, ref p1));
				Vector3 vector = camTransform.InverseTransformPoint(p1);
				if ((vector.y < -1f && pos1.y > screenHeight) || (vector.y > 1f && pos1.y < 0f))
				{
					return true;
				}
			}
			if (pos2.z < 0f)
			{
				pos2 = cam3D.WorldToScreenPoint(PlaneIntersectionPoint(ref ray, ref cameraPlane, ref p1, ref p2));
				Vector3 vector2 = camTransform.InverseTransformPoint(p2);
				if ((vector2.y < -1f && pos2.y > screenHeight) || (vector2.y > 1f && pos2.y < 0f))
				{
					return true;
				}
			}
			return false;
		}

		private Vector3 PlaneIntersectionPoint(ref Ray ray, ref Plane plane, ref Vector3 p1, ref Vector3 p2)
		{
			ray.origin = p1;
			ray.direction = p2 - p1;
			float enter = 0f;
			plane.Raycast(ray, out enter);
			return ray.GetPoint(enter);
		}

		private void DrawPoints()
		{
			if (!CheckCamera3D())
			{
				return;
			}
			Matrix4x4 thisMatrix;
			bool flag = UseMatrix(out thisMatrix);
			SetupDrawStartEnd(out var start, out var end, true);
			Vector2 vector = new Vector2(Screen.width, Screen.height);
			int idx = start * 4;
			int widthIdxAdd = ((m_lineWidths.Length > 1) ? 1 : 0);
			int widthIdx = start;
			Vector3 vector2 = new Vector3(m_lineWidths[0], m_lineWidths[0], 0f);
			Vector3 vector3 = new Vector3(0f - m_lineWidths[0], m_lineWidths[0], 0f);
			if (m_is2D)
			{
				for (int i = start; i <= end; i++)
				{
					Vector3 vector4 = ((!flag) ? ((Vector3)m_points2[i]) : thisMatrix.MultiplyPoint3x4(m_points2[i]));
					if (m_viewportDraw)
					{
						vector4.x *= vector.x;
						vector4.y *= vector.y;
					}
					if (widthIdxAdd != 0)
					{
						vector2.x = (vector2.y = (vector3.y = m_lineWidths[widthIdx]));
						vector3.x = 0f - m_lineWidths[widthIdx];
						widthIdx++;
					}
					m_lineVertices[idx].x = vector4.x + vector3.x;
					m_lineVertices[idx].y = vector4.y + vector3.y;
					m_lineVertices[idx + 3].x = vector4.x - vector2.x;
					m_lineVertices[idx + 3].y = vector4.y - vector2.y;
					m_lineVertices[idx + 1].x = vector4.x + vector2.x;
					m_lineVertices[idx + 1].y = vector4.y + vector2.y;
					m_lineVertices[idx + 2].x = vector4.x - vector3.x;
					m_lineVertices[idx + 2].y = vector4.y - vector3.y;
					idx += 4;
				}
			}
			else
			{
				for (int j = start; j <= end; j++)
				{
					Vector3 vector4 = ((!flag) ? cam3D.WorldToScreenPoint(m_points3[j]) : cam3D.WorldToScreenPoint(thisMatrix.MultiplyPoint3x4(m_points3[j])));
					if (vector4.z < 0f)
					{
						SkipQuad(ref idx, ref widthIdx, ref widthIdxAdd);
						continue;
					}
					if (widthIdxAdd != 0)
					{
						vector2.x = (vector2.y = (vector3.y = m_lineWidths[widthIdx]));
						vector3.x = 0f - m_lineWidths[widthIdx];
						widthIdx++;
					}
					m_lineVertices[idx].x = vector4.x + vector3.x;
					m_lineVertices[idx].y = vector4.y + vector3.y;
					m_lineVertices[idx + 3].x = vector4.x - vector2.x;
					m_lineVertices[idx + 3].y = vector4.y - vector2.y;
					m_lineVertices[idx + 1].x = vector4.x + vector2.x;
					m_lineVertices[idx + 1].y = vector4.y + vector2.y;
					m_lineVertices[idx + 2].x = vector4.x - vector3.x;
					m_lineVertices[idx + 2].y = vector4.y - vector3.y;
					idx += 4;
				}
			}
			CheckNormals();
			m_vectorObject.UpdateVerts();
		}

		private void DrawPoints3D()
		{
			if (!m_active)
			{
				return;
			}
			Matrix4x4 thisMatrix;
			bool flag = UseMatrix(out thisMatrix);
			int start = 0;
			int end = 0;
			int widthIdx = 0;
			SetupDrawStartEnd(out start, out end, true);
			int idx = start * 4;
			int widthIdxAdd = 0;
			if (m_lineWidths.Length > 1)
			{
				widthIdx = start;
				widthIdxAdd = 1;
			}
			Vector3 vector = v3zero;
			Vector3 vector2 = v3zero;
			Vector3 vector3 = v3zero;
			for (int i = start; i <= end; i++)
			{
				vector = ((!flag) ? cam3D.WorldToScreenPoint(m_points3[i]) : cam3D.WorldToScreenPoint(thisMatrix.MultiplyPoint3x4(m_points3[i])));
				if (vector.z < 0f)
				{
					SkipQuad(ref idx, ref widthIdx, ref widthIdxAdd);
					continue;
				}
				vector2.x = (vector2.y = (vector3.y = m_lineWidths[widthIdx]));
				vector3.x = 0f - m_lineWidths[widthIdx];
				ref Vector3 reference = ref m_lineVertices[idx];
				reference = cam3D.ScreenToWorldPoint(vector + vector3);
				ref Vector3 reference2 = ref m_lineVertices[idx + 3];
				reference2 = cam3D.ScreenToWorldPoint(vector - vector2);
				ref Vector3 reference3 = ref m_lineVertices[idx + 1];
				reference3 = cam3D.ScreenToWorldPoint(vector + vector2);
				ref Vector3 reference4 = ref m_lineVertices[idx + 2];
				reference4 = cam3D.ScreenToWorldPoint(vector - vector3);
				idx += 4;
				widthIdx += widthIdxAdd;
			}
			CheckNormals();
			m_vectorObject.UpdateVerts();
		}

		private void SkipQuad(ref int idx, ref int widthIdx, ref int widthIdxAdd)
		{
			ref Vector3 reference = ref m_lineVertices[idx];
			reference = v3zero;
			ref Vector3 reference2 = ref m_lineVertices[idx + 1];
			reference2 = v3zero;
			ref Vector3 reference3 = ref m_lineVertices[idx + 2];
			reference3 = v3zero;
			ref Vector3 reference4 = ref m_lineVertices[idx + 3];
			reference4 = v3zero;
			idx += 4;
			widthIdx += widthIdxAdd;
		}

		private void SkipQuad3D(ref int idx, ref int widthIdx, ref int widthIdxAdd)
		{
			ref Vector3 reference = ref m_lineVertices[idx];
			reference = v3zero;
			ref Vector3 reference2 = ref m_lineVertices[idx + 1];
			reference2 = v3zero;
			ref Vector3 reference3 = ref m_lineVertices[idx + 2];
			reference3 = v3zero;
			ref Vector3 reference4 = ref m_lineVertices[idx + 3];
			reference4 = v3zero;
			ref Vector3 reference5 = ref m_screenPoints[idx];
			reference5 = v3zero;
			ref Vector3 reference6 = ref m_screenPoints[idx + 1];
			reference6 = v3zero;
			ref Vector3 reference7 = ref m_screenPoints[idx + 2];
			reference7 = v3zero;
			ref Vector3 reference8 = ref m_screenPoints[idx + 3];
			reference8 = v3zero;
			idx += 4;
			widthIdx += widthIdxAdd;
		}

		private void WeldJoins(int start, int end, bool connectFirstAndLast)
		{
			if (connectFirstAndLast)
			{
				SetIntersectionPoint(m_vertexCount - 4, m_vertexCount - 3, 0, 1);
				SetIntersectionPoint(m_vertexCount - 1, m_vertexCount - 2, 3, 2);
			}
			for (int i = start; i < end; i += 4)
			{
				SetIntersectionPoint(i - 4, i - 3, i, i + 1);
				SetIntersectionPoint(i - 1, i - 2, i + 3, i + 2);
			}
		}

		private void WeldJoinsDiscrete(int start, int end, bool connectFirstAndLast)
		{
			if (connectFirstAndLast)
			{
				SetIntersectionPoint(m_vertexCount - 4, m_vertexCount - 3, 0, 1);
				SetIntersectionPoint(m_vertexCount - 1, m_vertexCount - 2, 3, 2);
			}
			int num = (start + 1) / 2 * 4;
			if (m_is2D)
			{
				for (int i = start; i < end; i += 2)
				{
					if (m_points2[i] == m_points2[i + 1])
					{
						SetIntersectionPoint(num - 4, num - 3, num, num + 1);
						SetIntersectionPoint(num - 1, num - 2, num + 3, num + 2);
					}
					num += 4;
				}
				return;
			}
			for (int j = start; j < end; j += 2)
			{
				if (m_points3[j] == m_points3[j + 1])
				{
					SetIntersectionPoint(num - 4, num - 3, num, num + 1);
					SetIntersectionPoint(num - 1, num - 2, num + 3, num + 2);
				}
				num += 4;
			}
		}

		private void SetIntersectionPoint(int p1, int p2, int p3, int p4)
		{
			Vector3 vector = m_lineVertices[p1];
			Vector3 vector2 = m_lineVertices[p2];
			Vector3 vector3 = m_lineVertices[p3];
			Vector3 vector4 = m_lineVertices[p4];
			if ((vector.x == vector2.x && vector.y == vector2.y) || (vector3.x == vector4.x && vector3.y == vector4.y))
			{
				return;
			}
			float num = (vector4.y - vector3.y) * (vector2.x - vector.x) - (vector4.x - vector3.x) * (vector2.y - vector.y);
			if (num > -0.005f && num < 0.005f)
			{
				if (Mathf.Abs(vector2.x - vector3.x) < 0.005f && Mathf.Abs(vector2.y - vector3.y) < 0.005f)
				{
					ref Vector3 reference = ref m_lineVertices[p2];
					reference = (vector2 + vector3) * 0.5f;
					ref Vector3 reference2 = ref m_lineVertices[p3];
					reference2 = m_lineVertices[p2];
				}
			}
			else
			{
				float num2 = ((vector4.x - vector3.x) * (vector.y - vector3.y) - (vector4.y - vector3.y) * (vector.x - vector3.x)) / num;
				Vector3 vector5 = new Vector3(vector.x + num2 * (vector2.x - vector.x), vector.y + num2 * (vector2.y - vector.y), vector.z);
				if (!((vector5 - vector2).sqrMagnitude > m_maxWeldDistance))
				{
					m_lineVertices[p2] = vector5;
					m_lineVertices[p3] = vector5;
				}
			}
		}

		private void WeldJoins3D(int start, int end, bool connectFirstAndLast)
		{
			if (connectFirstAndLast)
			{
				SetIntersectionPoint3D(m_vertexCount - 4, m_vertexCount - 3, 0, 1);
				SetIntersectionPoint3D(m_vertexCount - 1, m_vertexCount - 2, 3, 2);
			}
			for (int i = start; i < end; i += 4)
			{
				SetIntersectionPoint3D(i - 4, i - 3, i, i + 1);
				SetIntersectionPoint3D(i - 1, i - 2, i + 3, i + 2);
			}
		}

		private void WeldJoinsDiscrete3D(int start, int end, bool connectFirstAndLast)
		{
			if (connectFirstAndLast)
			{
				SetIntersectionPoint3D(m_vertexCount - 4, m_vertexCount - 3, 0, 1);
				SetIntersectionPoint3D(m_vertexCount - 1, m_vertexCount - 2, 3, 2);
			}
			int num = (start + 1) / 2 * 4;
			for (int i = start; i < end; i += 2)
			{
				if (m_points3[i] == m_points3[i + 1])
				{
					SetIntersectionPoint3D(num - 4, num - 3, num, num + 1);
					SetIntersectionPoint3D(num - 1, num - 2, num + 3, num + 2);
				}
				num += 4;
			}
		}

		private void SetIntersectionPoint3D(int p1, int p2, int p3, int p4)
		{
			Vector3 vector = m_screenPoints[p1];
			Vector3 vector2 = m_screenPoints[p2];
			Vector3 vector3 = m_screenPoints[p3];
			Vector3 vector4 = m_screenPoints[p4];
			if ((vector.x == vector2.x && vector.y == vector2.y) || (vector3.x == vector4.x && vector3.y == vector4.y))
			{
				return;
			}
			float num = (vector4.y - vector3.y) * (vector2.x - vector.x) - (vector4.x - vector3.x) * (vector2.y - vector.y);
			if (num > -0.005f && num < 0.005f)
			{
				if (Mathf.Abs(vector2.x - vector3.x) < 0.005f && Mathf.Abs(vector2.y - vector3.y) < 0.005f)
				{
					ref Vector3 reference = ref m_lineVertices[p2];
					reference = cam3D.ScreenToWorldPoint((vector2 + vector3) * 0.5f);
					ref Vector3 reference2 = ref m_lineVertices[p3];
					reference2 = m_lineVertices[p2];
				}
				return;
			}
			float num2 = ((vector4.x - vector3.x) * (vector.y - vector3.y) - (vector4.y - vector3.y) * (vector.x - vector3.x)) / num;
			Vector3 vector5 = new Vector3(vector.x + num2 * (vector2.x - vector.x), vector.y + num2 * (vector2.y - vector.y), vector.z);
			if (!((vector5 - vector2).sqrMagnitude > m_maxWeldDistance))
			{
				ref Vector3 reference3 = ref m_lineVertices[p2];
				reference3 = cam3D.ScreenToWorldPoint(vector5);
				ref Vector3 reference4 = ref m_lineVertices[p3];
				reference4 = m_lineVertices[p2];
			}
		}

		public static void LineManagerCheckDistance()
		{
			lineManager.StartCheckDistance();
		}

		public static void LineManagerDisable()
		{
			lineManager.DisableIfUnused();
		}

		public static void LineManagerEnable()
		{
			lineManager.EnableIfUsed();
		}

		public void Draw3DAuto()
		{
			Draw3DAuto(0f);
		}

		public void Draw3DAuto(float time)
		{
			if (time < 0f)
			{
				time = 0f;
			}
			lineManager.AddLine(this, m_drawTransform, time);
			m_isAutoDrawing = true;
			Draw3D();
		}

		public void StopDrawing3DAuto()
		{
			lineManager.RemoveLine(this);
			m_isAutoDrawing = false;
		}

		private void SetTextureScale()
		{
			if (pointsCount != m_pointsCount)
			{
				Resize();
			}
			SetupDrawStartEnd(out var _, out var end, false);
			int num = ((m_lineType != LineType.Discrete) ? 1 : 2);
			int num2 = 0;
			int num3 = 0;
			int num4 = ((m_lineWidths.Length != 1) ? 1 : 0);
			float num5 = 1f / m_textureScale;
			bool flag = m_drawTransform != null;
			Matrix4x4 matrix4x = ((!flag) ? Matrix4x4.identity : m_drawTransform.localToWorldMatrix);
			Vector2 vector = Vector2.zero;
			Vector2 vector2 = Vector2.zero;
			Vector2 zero = Vector2.zero;
			float num6 = m_textureOffset;
			float num7 = m_capLength * 2f;
			if (m_is2D)
			{
				for (int i = 0; i < end; i += num)
				{
					if (!m_viewportDraw)
					{
						if (flag)
						{
							vector = matrix4x.MultiplyPoint3x4(m_points2[i]);
							vector2 = matrix4x.MultiplyPoint3x4(m_points2[i + 1]);
						}
						else
						{
							vector.x = m_points2[i].x;
							vector.y = m_points2[i].y;
							vector2.x = m_points2[i + 1].x;
							vector2.y = m_points2[i + 1].y;
						}
					}
					else if (flag)
					{
						vector = matrix4x.MultiplyPoint3x4(new Vector2(m_points2[i].x * (float)Screen.width, m_points2[i].y * (float)Screen.height));
						vector2 = matrix4x.MultiplyPoint3x4(new Vector2(m_points2[i + 1].x * (float)Screen.width, m_points2[i + 1].y * (float)Screen.height));
					}
					else
					{
						vector = new Vector2(m_points2[i].x * (float)Screen.width, m_points2[i].y * (float)Screen.height);
						vector2 = new Vector2(m_points2[i + 1].x * (float)Screen.width, m_points2[i + 1].y * (float)Screen.height);
					}
					zero.x = vector2.x - vector.x;
					zero.y = vector2.y - vector.y;
					float num8 = num5 / (m_lineWidths[num3] * 2f / ((float)Math.Sqrt(zero.x * zero.x + zero.y * zero.y) + num7));
					m_lineUVs[num2].x = num6;
					m_lineUVs[num2 + 3].x = num6;
					m_lineUVs[num2 + 2].x = num8 + num6;
					m_lineUVs[num2 + 1].x = num8 + num6;
					num2 += 4;
					num6 = (num6 + num8) % 1f;
					num3 += num4;
				}
			}
			else
			{
				if (!CheckCamera3D())
				{
					return;
				}
				for (int j = 0; j < end; j += num)
				{
					if (flag)
					{
						vector = cam3D.WorldToScreenPoint(matrix4x.MultiplyPoint3x4(m_points3[j]));
						vector2 = cam3D.WorldToScreenPoint(matrix4x.MultiplyPoint3x4(m_points3[j + 1]));
					}
					else
					{
						vector = cam3D.WorldToScreenPoint(m_points3[j]);
						vector2 = cam3D.WorldToScreenPoint(m_points3[j + 1]);
					}
					zero.x = vector.x - vector2.x;
					zero.y = vector.y - vector2.y;
					float num9 = num5 / (m_lineWidths[num3] * 2f / (float)Math.Sqrt(zero.x * zero.x + zero.y * zero.y));
					m_lineUVs[num2].x = num6;
					m_lineUVs[num2 + 3].x = num6;
					m_lineUVs[num2 + 2].x = num9 + num6;
					m_lineUVs[num2 + 1].x = num9 + num6;
					num2 += 4;
					num6 = (num6 + num9) % 1f;
					num3 += num4;
				}
			}
			if (m_vectorObject != null)
			{
				m_vectorObject.UpdateUVs();
			}
		}

		private void ResetTextureScale()
		{
			for (int i = 0; i < m_vertexCount; i += 4)
			{
				m_lineUVs[i].x = 0f;
				m_lineUVs[i + 3].x = 0f;
				m_lineUVs[i + 2].x = 1f;
				m_lineUVs[i + 1].x = 1f;
			}
			if (m_vectorObject != null)
			{
				m_vectorObject.UpdateUVs();
			}
		}

		private void SetPathVerticesContinuous(ref int i, ref int startIdx, ref int endIdx, Vector2[] path)
		{
			path[startIdx].x = m_lineVertices[i].x;
			path[startIdx].y = m_lineVertices[i].y;
			path[startIdx + 1].x = m_lineVertices[i + 1].x;
			path[startIdx + 1].y = m_lineVertices[i + 1].y;
			path[endIdx].x = m_lineVertices[i + 3].x;
			path[endIdx].y = m_lineVertices[i + 3].y;
			path[endIdx - 1].x = m_lineVertices[i + 2].x;
			path[endIdx - 1].y = m_lineVertices[i + 2].y;
			startIdx += 2;
			endIdx -= 2;
		}

		private void SetPathWorldVerticesContinuous(ref int i, ref Vector3 v3, ref int startIdx, ref int endIdx, Vector2[] path)
		{
			v3.x = m_lineVertices[i].x;
			v3.y = m_lineVertices[i].y;
			ref Vector2 reference = ref path[startIdx];
			reference = cam3D.ScreenToWorldPoint(v3);
			v3.x = m_lineVertices[i + 1].x;
			v3.y = m_lineVertices[i + 1].y;
			ref Vector2 reference2 = ref path[startIdx + 1];
			reference2 = cam3D.ScreenToWorldPoint(v3);
			v3.x = m_lineVertices[i + 3].x;
			v3.y = m_lineVertices[i + 3].y;
			ref Vector2 reference3 = ref path[endIdx];
			reference3 = cam3D.ScreenToWorldPoint(v3);
			v3.x = m_lineVertices[i + 2].x;
			v3.y = m_lineVertices[i + 2].y;
			ref Vector2 reference4 = ref path[endIdx - 1];
			reference4 = cam3D.ScreenToWorldPoint(v3);
			startIdx += 2;
			endIdx -= 2;
		}

		private void SetPathVerticesDiscrete(ref int i, ref int pIdx, Vector2[] path)
		{
			path[0].x = m_lineVertices[i].x;
			path[0].y = m_lineVertices[i].y;
			path[1].x = m_lineVertices[i + 3].x;
			path[1].y = m_lineVertices[i + 3].y;
			path[2].x = m_lineVertices[i + 2].x;
			path[2].y = m_lineVertices[i + 2].y;
			path[3].x = m_lineVertices[i + 1].x;
			path[3].y = m_lineVertices[i + 1].y;
		}

		private void SetPathWorldVerticesDiscrete(ref int i, ref Vector3 v3, ref int pIdx, Vector2[] path)
		{
			v3.x = m_lineVertices[i].x;
			v3.y = m_lineVertices[i].y;
			ref Vector2 reference = ref path[0];
			reference = cam3D.ScreenToWorldPoint(v3);
			v3.x = m_lineVertices[i + 3].x;
			v3.y = m_lineVertices[i + 3].y;
			ref Vector2 reference2 = ref path[1];
			reference2 = cam3D.ScreenToWorldPoint(v3);
			v3.x = m_lineVertices[i + 2].x;
			v3.y = m_lineVertices[i + 2].y;
			ref Vector2 reference3 = ref path[2];
			reference3 = cam3D.ScreenToWorldPoint(v3);
			v3.x = m_lineVertices[i + 1].x;
			v3.y = m_lineVertices[i + 1].y;
			ref Vector2 reference4 = ref path[3];
			reference4 = cam3D.ScreenToWorldPoint(v3);
		}

		public static List<Vector3> BytesToVector3List(byte[] lineBytes)
		{
			if (lineBytes.Length % 12 != 0)
			{
				Debug.LogError("VectorLine.BytesToVector3Array: Incorrect input byte length...must be a multiple of 12");
				return null;
			}
			SetupByteBlock();
			List<Vector3> list = new List<Vector3>(lineBytes.Length / 12);
			for (int i = 0; i < lineBytes.Length; i += 12)
			{
				list.Add(new Vector3(ConvertToFloat(lineBytes, i), ConvertToFloat(lineBytes, i + 4), ConvertToFloat(lineBytes, i + 8)));
			}
			return list;
		}

		public static List<Vector2> BytesToVector2List(byte[] lineBytes)
		{
			if (lineBytes.Length % 8 != 0)
			{
				Debug.LogError("VectorLine.BytesToVector2Array: Incorrect input byte length...must be a multiple of 8");
				return null;
			}
			SetupByteBlock();
			List<Vector2> list = new List<Vector2>(lineBytes.Length / 8);
			for (int i = 0; i < lineBytes.Length; i += 8)
			{
				list.Add(new Vector2(ConvertToFloat(lineBytes, i), ConvertToFloat(lineBytes, i + 4)));
			}
			return list;
		}

		private static void SetupByteBlock()
		{
			if (byteBlock == null)
			{
				byteBlock = new byte[4];
			}
			if (BitConverter.IsLittleEndian)
			{
				endianDiff1 = 0;
				endianDiff2 = 0;
			}
			else
			{
				endianDiff1 = 3;
				endianDiff2 = 1;
			}
		}

		private static float ConvertToFloat(byte[] bytes, int i)
		{
			byteBlock[endianDiff1] = bytes[i];
			byteBlock[1 + endianDiff2] = bytes[i + 1];
			byteBlock[2 - endianDiff2] = bytes[i + 2];
			byteBlock[3 - endianDiff1] = bytes[i + 3];
			return BitConverter.ToSingle(byteBlock, 0);
		}

		public static void Destroy(ref VectorLine line)
		{
			DestroyLine(ref line);
		}

		public static void Destroy(VectorLine[] lines)
		{
			for (int i = 0; i < lines.Length; i++)
			{
				DestroyLine(ref lines[i]);
			}
		}

		public static void Destroy(List<VectorLine> lines)
		{
			for (int i = 0; i < lines.Count; i++)
			{
				VectorLine line = lines[i];
				DestroyLine(ref line);
			}
		}

		private static void DestroyLine(ref VectorLine line)
		{
			if (line != null)
			{
				UnityEngine.Object.Destroy(line.m_go);
				if (line.m_vectorObject != null)
				{
					line.m_vectorObject.Destroy();
				}
				if (line.isAutoDrawing)
				{
					line.StopDrawing3DAuto();
				}
				line = null;
			}
		}

		public static void Destroy(ref VectorLine line, GameObject go)
		{
			Destroy(ref line);
			if (go != null)
			{
				UnityEngine.Object.Destroy(go);
			}
		}

		public void SetDistances()
		{
			if (m_lineType == LineType.Points)
			{
				return;
			}
			if (m_distances == null || m_distances.Length != ((m_lineType == LineType.Discrete) ? (m_pointsCount / 2 + 1) : m_pointsCount))
			{
				m_distances = new float[(m_lineType == LineType.Discrete) ? (m_pointsCount / 2 + 1) : m_pointsCount];
			}
			double num = 0.0;
			int num2 = pointsCount - 1;
			if (is2D)
			{
				if (m_lineType != LineType.Discrete)
				{
					for (int i = 0; i < num2; i++)
					{
						Vector2 vector = m_points2[i] - m_points2[i + 1];
						num += Math.Sqrt(vector.x * vector.x + vector.y * vector.y);
						m_distances[i + 1] = (float)num;
					}
					return;
				}
				int num3 = 1;
				for (int j = 0; j < num2; j += 2)
				{
					Vector2 vector2 = m_points2[j] - m_points2[j + 1];
					num += Math.Sqrt(vector2.x * vector2.x + vector2.y * vector2.y);
					m_distances[num3++] = (float)num;
				}
			}
			else if (m_lineType != LineType.Discrete)
			{
				for (int k = 0; k < num2; k++)
				{
					Vector3 vector3 = m_points3[k] - m_points3[k + 1];
					num += Math.Sqrt(vector3.x * vector3.x + vector3.y * vector3.y + vector3.z * vector3.z);
					m_distances[k + 1] = (float)num;
				}
			}
			else
			{
				int num4 = 1;
				for (int l = 0; l < num2; l += 2)
				{
					Vector3 vector4 = m_points3[l] - m_points3[l + 1];
					num += Math.Sqrt(vector4.x * vector4.x + vector4.y * vector4.y + vector4.z * vector4.z);
					m_distances[num4++] = (float)num;
				}
			}
		}

		public float GetLength()
		{
			if (m_distances == null || m_distances.Length != ((m_lineType == LineType.Discrete) ? (pointsCount / 2 + 1) : pointsCount))
			{
				SetDistances();
			}
			return m_distances[m_distances.Length - 1];
		}

		public Vector2 GetPoint01(float distance)
		{
			int index;
			return GetPoint(Mathf.Lerp(0f, GetLength(), distance), out index);
		}

		public Vector2 GetPoint01(float distance, out int index)
		{
			return GetPoint(Mathf.Lerp(0f, GetLength(), distance), out index);
		}

		public Vector2 GetPoint(float distance)
		{
			int index;
			return GetPoint(distance, out index);
		}

		public Vector2 GetPoint(float distance, out int index)
		{
			if (!m_is2D)
			{
				Debug.LogError("VectorLine.GetPoint only works with Vector2 points");
				index = 0;
				return Vector2.zero;
			}
			SetDistanceIndex(out index, distance);
			Vector2 result = ((m_lineType == LineType.Discrete) ? Vector2.Lerp(m_points2[(index - 1) * 2], m_points2[(index - 1) * 2 + 1], Mathf.InverseLerp(m_distances[index - 1], m_distances[index], distance)) : Vector2.Lerp(m_points2[index - 1], m_points2[index], Mathf.InverseLerp(m_distances[index - 1], m_distances[index], distance)));
			if ((bool)m_drawTransform)
			{
				result += new Vector2(m_drawTransform.position.x, m_drawTransform.position.y);
			}
			index--;
			return result;
		}

		public Vector3 GetPoint3D01(float distance)
		{
			int index;
			return GetPoint3D(Mathf.Lerp(0f, GetLength(), distance), out index);
		}

		public Vector3 GetPoint3D01(float distance, out int index)
		{
			return GetPoint3D(Mathf.Lerp(0f, GetLength(), distance), out index);
		}

		public Vector3 GetPoint3D(float distance)
		{
			int index;
			return GetPoint3D(distance, out index);
		}

		public Vector3 GetPoint3D(float distance, out int index)
		{
			if (m_is2D)
			{
				Debug.LogError("VectorLine.GetPoint3D only works with Vector3 points");
				index = 0;
				return Vector3.zero;
			}
			SetDistanceIndex(out index, distance);
			Vector3 result = ((m_lineType == LineType.Discrete) ? Vector3.Lerp(m_points3[(index - 1) * 2], m_points3[(index - 1) * 2 + 1], Mathf.InverseLerp(m_distances[index - 1], m_distances[index], distance)) : Vector3.Lerp(m_points3[index - 1], m_points3[index], Mathf.InverseLerp(m_distances[index - 1], m_distances[index], distance)));
			if ((bool)m_drawTransform)
			{
				result += m_drawTransform.position;
			}
			index--;
			return result;
		}

		private void SetDistanceIndex(out int i, float distance)
		{
			if (m_distances == null)
			{
				SetDistances();
			}
			i = m_drawStart + 1;
			if (m_lineType == LineType.Discrete)
			{
				i = (i + 1) / 2;
			}
			if (i >= m_distances.Length)
			{
				i = m_distances.Length - 1;
			}
			int num = m_drawEnd;
			if (m_lineType == LineType.Discrete)
			{
				num = (num + 1) / 2;
			}
			while (distance > m_distances[i] && i < num)
			{
				i++;
			}
		}

		public static void SetEndCap(string name, EndCap capType)
		{
			SetEndCap(name, capType, 0f, 0f, 1f, 1f, (Texture2D[])null);
		}

		public static void SetEndCap(string name, EndCap capType, params Texture2D[] textures)
		{
			SetEndCap(name, capType, 0f, 0f, 1f, 1f, textures);
		}

		public static void SetEndCap(string name, EndCap capType, float offset, params Texture2D[] textures)
		{
			SetEndCap(name, capType, offset, offset, 1f, 1f, textures);
		}

		public static void SetEndCap(string name, EndCap capType, float offsetFront, float offsetBack, params Texture2D[] textures)
		{
			SetEndCap(name, capType, offsetFront, offsetBack, 1f, 1f, textures);
		}

		public static void SetEndCap(string name, EndCap capType, float offsetFront, float offsetBack, float scaleFront, float scaleBack, params Texture2D[] textures)
		{
			if (capDictionary == null)
			{
				capDictionary = new Dictionary<string, CapInfo>();
			}
			if (name == null || name == "")
			{
				Debug.LogError("VectorLine.SetEndCap: must supply a name");
				return;
			}
			if (capDictionary.ContainsKey(name) && capType != EndCap.None)
			{
				Debug.LogError("VectorLine.SetEndCap: end cap \"" + name + "\" has already been set up");
				return;
			}
			switch (capType)
			{
			case EndCap.None:
				RemoveEndCap(name);
				return;
			case EndCap.Front:
			case EndCap.Mirror:
			case EndCap.Back:
				if (textures.Length < 2)
				{
					Debug.LogError("VectorLine.SetEndCap (\"" + name + "\"): must supply two textures when using SetEndCap with EndCap.Front, EndCap.Back, or EndCap.Mirror");
					return;
				}
				break;
			}
			if (textures[0] == null || textures[1] == null)
			{
				Debug.LogError("VectorLine.SetEndCap (\"" + name + "\"): end cap textures must not be null");
				return;
			}
			if (textures[0].width != textures[0].height)
			{
				Debug.LogError("VectorLine.SetEndCap (\"" + name + "\"): the line texture must be square");
				return;
			}
			if (textures[1].height != textures[0].height)
			{
				Debug.LogError("VectorLine.SetEndCap (\"" + name + "\"): all textures must be the same height");
				return;
			}
			if (capType == EndCap.Both)
			{
				if (textures.Length < 3)
				{
					Debug.LogError("VectorLine.SetEndCap (\"" + name + "\"): must supply three textures when using SetEndCap with EndCap.Both");
					return;
				}
				if (textures[2] == null)
				{
					Debug.LogError("VectorLine.SetEndCap (\"" + name + "\"): end cap textures must not be null");
					return;
				}
				if (textures[2].height != textures[0].height)
				{
					Debug.LogError("VectorLine.SetEndCap (\"" + name + "\"): all textures must be the same height");
					return;
				}
			}
			Texture2D texture2D = textures[0];
			Texture2D texture2D2 = textures[1];
			Texture2D texture2D3 = ((textures.Length != 3) ? null : textures[2]);
			int num = 4;
			int width = texture2D.width;
			float num2 = 0f;
			float ratio = 0f;
			int num3 = 0;
			int num4 = 0;
			Color32[] array = null;
			Color32[] array2 = null;
			switch (capType)
			{
			case EndCap.Front:
				array = GetRotatedPixels(texture2D2);
				num3 = texture2D2.width;
				array2 = GetRowPixels(array, num, 0, width);
				num4 = num;
				num2 = (float)texture2D2.width / (float)texture2D2.height;
				break;
			case EndCap.Back:
				array2 = GetRotatedPixels(texture2D2);
				num4 = texture2D2.width;
				array = GetRowPixels(array2, num, num4 - 1, width);
				num3 = num;
				ratio = (float)texture2D2.width / (float)texture2D2.height;
				break;
			case EndCap.Both:
				array = GetRotatedPixels(texture2D2);
				num3 = texture2D2.width;
				array2 = GetRotatedPixels(texture2D3);
				num4 = texture2D3.width;
				num2 = (float)texture2D2.width / (float)texture2D2.height;
				ratio = (float)texture2D3.width / (float)texture2D3.height;
				break;
			case EndCap.Mirror:
				array = GetRotatedPixels(texture2D2);
				num3 = texture2D2.width;
				array2 = GetRowPixels(array, num, 0, width);
				num4 = num;
				num2 = (float)texture2D2.width / (float)texture2D2.height;
				ratio = num2;
				break;
			}
			int num5 = texture2D.height + num3 + num4 + num * 4;
			Color32[] pixels = texture2D.GetPixels32();
			Color32[] array3 = new Color32[num * width];
			Color32 color = Color.clear;
			for (int i = 0; i < num * width; i++)
			{
				array3[i] = color;
			}
			Color32[] rowPixels = GetRowPixels(array2, num, num4 - 1, width);
			Color32[] rowPixels2 = GetRowPixels(array, num, 0, width);
			bool flag = texture2D.mipmapCount > 1;
			Texture2D texture2D4 = new Texture2D(width, num5, TextureFormat.ARGB32, flag);
			texture2D4.name = texture2D.name + " end cap";
			texture2D4.wrapMode = texture2D.wrapMode;
			texture2D4.filterMode = texture2D.filterMode;
			float num6 = 1f / (float)num5;
			float[] array4 = new float[6];
			int num7 = 0;
			texture2D4.SetPixels32(0, 0, width, num, array3);
			num7 += num;
			array4[0] = num6 * (float)num7;
			texture2D4.SetPixels32(0, num7, width, texture2D.height, pixels);
			num7 += texture2D.height;
			array4[1] = num6 * (float)num7;
			texture2D4.SetPixels32(0, num7, width, num, array3);
			num7 += num;
			array4[2] = num6 * (float)num7;
			texture2D4.SetPixels32(0, num7, width, num4, array2);
			num7 += num4;
			array4[3] = num6 * (float)num7;
			texture2D4.SetPixels32(0, num7, width, num, rowPixels);
			num7 += num;
			texture2D4.SetPixels32(0, num7, width, num, rowPixels2);
			num7 += num;
			array4[4] = num6 * (float)num7;
			texture2D4.SetPixels32(0, num7, width, num3, array);
			array4[5] = num6 * (float)(num7 + num3);
			texture2D4.Apply(flag, true);
			capDictionary.Add(name, new CapInfo(capType, texture2D4, num2, ratio, offsetFront, offsetBack, scaleFront, scaleBack, array4));
		}

		private static Color32[] GetRowPixels(Color32[] texPixels, int numberOfRows, int row, int w)
		{
			Color32[] array = new Color32[w * numberOfRows];
			for (int i = 0; i < numberOfRows; i++)
			{
				Array.Copy(texPixels, row * w, array, i * w, w);
			}
			return array;
		}

		private static Color32[] GetRotatedPixels(Texture2D tex)
		{
			Color32[] pixels = tex.GetPixels32();
			Color32[] array = new Color32[pixels.Length];
			int width = tex.width;
			int height = tex.height;
			int num = 0;
			for (int i = 0; i < height; i++)
			{
				int num2 = tex.width - 1;
				for (int j = 0; j < width; j++)
				{
					ref Color32 reference = ref array[num2 * height + num];
					reference = pixels[i * width + j];
					num2--;
				}
				num++;
			}
			return array;
		}

		public static void RemoveEndCap(string name)
		{
			if (!capDictionary.ContainsKey(name))
			{
				Debug.LogError("VectorLine: RemoveEndCap: \"" + name + "\" has not been set up");
				return;
			}
			UnityEngine.Object.Destroy(capDictionary[name].texture);
			capDictionary.Remove(name);
		}

		public bool Selected(Vector2 p)
		{
			int index;
			return Selected(p, 0, 0, out index, cam3D);
		}

		public bool Selected(Vector2 p, out int index)
		{
			return Selected(p, 0, 0, out index, cam3D);
		}

		public bool Selected(Vector2 p, int extraDistance, out int index)
		{
			return Selected(p, extraDistance, 0, out index, cam3D);
		}

		public bool Selected(Vector2 p, int extraDistance, int extraLength, out int index)
		{
			return Selected(p, extraDistance, extraLength, out index, cam3D);
		}

		public bool Selected(Vector2 p, Camera cam)
		{
			int index;
			return Selected(p, 0, 0, out index, cam);
		}

		public bool Selected(Vector2 p, out int index, Camera cam)
		{
			return Selected(p, 0, 0, out index, cam);
		}

		public bool Selected(Vector2 p, int extraDistance, out int index, Camera cam)
		{
			return Selected(p, extraDistance, 0, out index, cam);
		}

		public bool Selected(Vector2 p, int extraDistance, int extraLength, out int index, Camera cam)
		{
			if (cam == null)
			{
				SetCamera3D();
				if (!cam3D)
				{
					Debug.LogError("VectorLine.Selected: camera cannot be null. If there is no camera tagged \"MainCamera\", supply one manually");
					index = 0;
					return false;
				}
				cam = cam3D;
			}
			int num = ((m_lineWidths.Length != 1) ? 1 : 0);
			int num2 = ((m_lineType == LineType.Discrete) ? (m_drawStart / 2 - num) : (m_drawStart - num));
			if (m_lineWidths.Length == 1)
			{
				num = 0;
				num2 = 0;
			}
			else
			{
				num = 1;
			}
			int num3 = m_drawEnd;
			bool flag = m_drawTransform != null;
			Matrix4x4 matrix4x = ((!flag) ? Matrix4x4.identity : m_drawTransform.localToWorldMatrix);
			Vector2 vector = new Vector2(Screen.width, Screen.height);
			if (m_lineType == LineType.Points)
			{
				if (num3 == pointsCount)
				{
					num3--;
				}
				if (m_is2D)
				{
					for (int i = m_drawStart; i <= num3; i++)
					{
						num2 += num;
						float num4 = m_lineWidths[num2] + (float)extraDistance;
						Vector2 vector2 = ((!flag) ? m_points2[i] : ((Vector2)matrix4x.MultiplyPoint3x4(m_points2[i])));
						if (m_viewportDraw)
						{
							vector2.x *= vector.x;
							vector2.y *= vector.y;
						}
						if (p.x >= vector2.x - num4 && p.x <= vector2.x + num4 && p.y >= vector2.y - num4 && p.y <= vector2.y + num4)
						{
							index = i;
							return true;
						}
					}
					index = -1;
					return false;
				}
				for (int j = m_drawStart; j <= num3; j++)
				{
					num2 += num;
					float num5 = m_lineWidths[num2] + (float)extraDistance;
					Vector2 vector2 = ((!flag) ? cam.WorldToScreenPoint(m_points3[j]) : cam.WorldToScreenPoint(matrix4x.MultiplyPoint3x4(m_points3[j])));
					if (p.x >= vector2.x - num5 && p.x <= vector2.x + num5 && p.y >= vector2.y - num5 && p.y <= vector2.y + num5)
					{
						index = j;
						return true;
					}
				}
				index = -1;
				return false;
			}
			float num6 = 0f;
			int num7 = ((m_lineType != LineType.Discrete) ? 1 : 2);
			Vector2 zero = Vector2.zero;
			if (m_lineType != LineType.Discrete && m_drawEnd == pointsCount)
			{
				num3--;
			}
			Vector2 vector3 = default(Vector2);
			Vector2 vector4 = default(Vector2);
			if (m_is2D)
			{
				for (int k = m_drawStart; k < num3; k += num7)
				{
					num2 += num;
					if (flag)
					{
						vector3 = matrix4x.MultiplyPoint3x4(m_points2[k]);
						vector4 = matrix4x.MultiplyPoint3x4(m_points2[k + 1]);
					}
					else
					{
						vector3.x = m_points2[k].x;
						vector3.y = m_points2[k].y;
						vector4.x = m_points2[k + 1].x;
						vector4.y = m_points2[k + 1].y;
					}
					if (m_viewportDraw)
					{
						vector3.x *= vector.x;
						vector3.y *= vector.y;
						vector4.x *= vector.x;
						vector4.y *= vector.y;
					}
					if (extraLength > 0)
					{
						zero = (vector3 - vector4).normalized * extraLength;
						vector3.x += zero.x;
						vector3.y += zero.y;
						vector4.x -= zero.x;
						vector4.y -= zero.y;
					}
					num6 = Vector2.Dot(p - vector3, vector4 - vector3) / (vector4 - vector3).sqrMagnitude;
					if (!(num6 < 0f) && !(num6 > 1f) && (p - (vector3 + num6 * (vector4 - vector3))).sqrMagnitude <= (m_lineWidths[num2] + (float)extraDistance) * (m_lineWidths[num2] + (float)extraDistance))
					{
						index = ((m_lineType == LineType.Discrete) ? (k / 2) : k);
						return true;
					}
				}
				index = -1;
				return false;
			}
			Vector3 vector5 = v3zero;
			for (int l = m_drawStart; l < num3; l += num7)
			{
				num2 += num;
				Vector3 vector6;
				if (flag)
				{
					vector6 = cam.WorldToScreenPoint(matrix4x.MultiplyPoint3x4(m_points3[l]));
					vector5 = cam.WorldToScreenPoint(matrix4x.MultiplyPoint3x4(m_points3[l + 1]));
				}
				else
				{
					vector6 = cam.WorldToScreenPoint(m_points3[l]);
					vector5 = cam.WorldToScreenPoint(m_points3[l + 1]);
				}
				if (vector6.z < 0f || vector5.z < 0f)
				{
					continue;
				}
				vector3.x = (int)vector6.x;
				vector4.x = (int)vector5.x;
				vector3.y = (int)vector6.y;
				vector4.y = (int)vector5.y;
				if (vector3.x != vector4.x || vector3.y != vector4.y)
				{
					if (extraLength > 0)
					{
						zero = (vector3 - vector4).normalized * extraLength;
						vector3.x += zero.x;
						vector3.y += zero.y;
						vector4.x -= zero.x;
						vector4.y -= zero.y;
					}
					num6 = Vector2.Dot(p - vector3, vector4 - vector3) / (vector4 - vector3).sqrMagnitude;
					if (!(num6 < 0f) && !(num6 > 1f) && (p - (vector3 + num6 * (vector4 - vector3))).sqrMagnitude <= (m_lineWidths[num2] + (float)extraDistance) * (m_lineWidths[num2] + (float)extraDistance))
					{
						index = ((m_lineType == LineType.Discrete) ? (l / 2) : l);
						return true;
					}
				}
			}
			index = -1;
			return false;
		}

		private bool Approximately(Vector2 p1, Vector2 p2)
		{
			return Approximately(p1.x, p2.x) && Approximately(p1.y, p2.y);
		}

		private bool Approximately(Vector3 p1, Vector3 p2)
		{
			return Approximately(p1.x, p2.x) && Approximately(p1.y, p2.y) && Approximately(p1.z, p2.z);
		}

		private bool Approximately(float a, float b)
		{
			return Mathf.Round(a * 100f) / 100f == Mathf.Round(b * 100f) / 100f;
		}

		private bool WrongArrayLength(int arrayLength, FunctionName functionName)
		{
			if (m_lineType == LineType.Continuous)
			{
				if (arrayLength != pointsCount - 1)
				{
					Debug.LogError(functionNames[(int)functionName] + " array for \"" + name + "\" must be length of points array minus one for a continuous line (one entry per line segment)");
					return true;
				}
			}
			else if (arrayLength != pointsCount / 2)
			{
				Debug.LogError(functionNames[(int)functionName] + " array in \"" + name + "\" must be exactly half the length of points array for a discrete line (one entry per line segment)");
				return true;
			}
			return false;
		}

		private bool CheckArrayLength(FunctionName functionName, int segments, int index)
		{
			if (segments < 1)
			{
				Debug.LogError("VectorLine." + functionNames[(int)functionName] + " needs at least 1 segment");
				return false;
			}
			if (index < 0)
			{
				Debug.LogError("VectorLine." + functionNames[(int)functionName] + ": The index value for \"" + name + "\" must be >= 0");
				return false;
			}
			if (m_lineType == LineType.Points)
			{
				if (index + segments > m_pointsCount)
				{
					if (index == 0)
					{
						Debug.LogError("VectorLine." + functionNames[(int)functionName] + ": The number of segments cannot exceed the number of points in the array for \"" + name + "\"");
						return false;
					}
					Debug.LogError("VectorLine: Calling " + functionNames[(int)functionName] + " with an index of " + index + " would exceed the length of the Vector array for \"" + name + "\"");
					return false;
				}
				return true;
			}
			if (m_lineType == LineType.Continuous)
			{
				if (index + (segments + 1) > m_pointsCount)
				{
					if (index == 0)
					{
						Debug.LogError("VectorLine." + functionNames[(int)functionName] + ": The length of the array for continuous lines needs to be at least the number of segments plus one for \"" + name + "\"");
						return false;
					}
					Debug.LogError("VectorLine: Calling " + functionNames[(int)functionName] + " with an index of " + index + " would exceed the length of the Vector array for \"" + name + "\"");
					return false;
				}
			}
			else if (index + segments * 2 > m_pointsCount)
			{
				if (index == 0)
				{
					Debug.LogError("VectorLine." + functionNames[(int)functionName] + ": The length of the array for discrete lines needs to be at least twice the number of segments for \"" + name + "\"");
					return false;
				}
				Debug.LogError("VectorLine: Calling " + functionNames[(int)functionName] + " with an index of " + index + " would exceed the length of the Vector array for \"" + name + "\"");
				return false;
			}
			return true;
		}

		public void MakeRect(Rect rect)
		{
			MakeRect(new Vector2(rect.x, rect.y), new Vector2(rect.x + rect.width, rect.y + rect.height), 0);
		}

		public void MakeRect(Rect rect, int index)
		{
			MakeRect(new Vector2(rect.x, rect.y), new Vector2(rect.x + rect.width, rect.y + rect.height), index);
		}

		public void MakeRect(Vector3 bottomLeft, Vector3 topRight)
		{
			MakeRect(bottomLeft, topRight, 0);
		}

		public void MakeRect(Vector3 bottomLeft, Vector3 topRight, int index)
		{
			if (m_lineType != LineType.Discrete)
			{
				if (index + 5 > pointsCount)
				{
					if (index == 0)
					{
						Debug.LogError("VectorLine.MakeRect: The length of the array for continuous lines needs to be at least 5 for \"" + name + "\"");
						return;
					}
					Debug.LogError("Calling VectorLine.MakeRect with an index of " + index + " would exceed the length of the Vector2 array for \"" + name + "\"");
				}
				else if (m_is2D)
				{
					m_points2[index] = new Vector2(bottomLeft.x, bottomLeft.y);
					m_points2[index + 1] = new Vector2(topRight.x, bottomLeft.y);
					m_points2[index + 2] = new Vector2(topRight.x, topRight.y);
					m_points2[index + 3] = new Vector2(bottomLeft.x, topRight.y);
					m_points2[index + 4] = new Vector2(bottomLeft.x, bottomLeft.y);
				}
				else
				{
					m_points3[index] = new Vector3(bottomLeft.x, bottomLeft.y, bottomLeft.z);
					m_points3[index + 1] = new Vector3(topRight.x, bottomLeft.y, bottomLeft.z);
					m_points3[index + 2] = new Vector3(topRight.x, topRight.y, topRight.z);
					m_points3[index + 3] = new Vector3(bottomLeft.x, topRight.y, topRight.z);
					m_points3[index + 4] = new Vector3(bottomLeft.x, bottomLeft.y, bottomLeft.z);
				}
			}
			else if (index + 8 > pointsCount)
			{
				if (index == 0)
				{
					Debug.LogError("VectorLine.MakeRect: The length of the array for discrete lines needs to be at least 8 for \"" + name + "\"");
					return;
				}
				Debug.LogError("Calling VectorLine.MakeRect with an index of " + index + " would exceed the length of the Vector2 array for \"" + name + "\"");
			}
			else if (m_is2D)
			{
				m_points2[index] = new Vector2(bottomLeft.x, bottomLeft.y);
				m_points2[index + 1] = new Vector2(topRight.x, bottomLeft.y);
				m_points2[index + 2] = new Vector2(topRight.x, bottomLeft.y);
				m_points2[index + 3] = new Vector2(topRight.x, topRight.y);
				m_points2[index + 4] = new Vector2(topRight.x, topRight.y);
				m_points2[index + 5] = new Vector2(bottomLeft.x, topRight.y);
				m_points2[index + 6] = new Vector2(bottomLeft.x, topRight.y);
				m_points2[index + 7] = new Vector2(bottomLeft.x, bottomLeft.y);
			}
			else
			{
				m_points3[index] = new Vector3(bottomLeft.x, bottomLeft.y, bottomLeft.z);
				m_points3[index + 1] = new Vector3(topRight.x, bottomLeft.y, bottomLeft.z);
				m_points3[index + 2] = new Vector3(topRight.x, bottomLeft.y, bottomLeft.z);
				m_points3[index + 3] = new Vector3(topRight.x, topRight.y, topRight.z);
				m_points3[index + 4] = new Vector3(topRight.x, topRight.y, topRight.z);
				m_points3[index + 5] = new Vector3(bottomLeft.x, topRight.y, topRight.z);
				m_points3[index + 6] = new Vector3(bottomLeft.x, topRight.y, topRight.z);
				m_points3[index + 7] = new Vector3(bottomLeft.x, bottomLeft.y, bottomLeft.z);
			}
		}

		public void MakeRoundedRect(Rect rect, float cornerRadius, int cornerSegments)
		{
			MakeRoundedRect(new Vector2(rect.x, rect.y), new Vector2(rect.x + rect.width, rect.y + rect.height), cornerRadius, cornerSegments, 0);
		}

		public void MakeRoundedRect(Rect rect, float cornerRadius, int cornerSegments, int index)
		{
			MakeRoundedRect(new Vector2(rect.x, rect.y), new Vector2(rect.x + rect.width, rect.y + rect.height), cornerRadius, cornerSegments, index);
		}

		public void MakeRoundedRect(Vector3 bottomLeft, Vector3 topRight, float cornerRadius, int cornerSegments)
		{
			MakeRoundedRect(bottomLeft, topRight, cornerRadius, cornerSegments, 0);
		}

		public void MakeRoundedRect(Vector3 bottomLeft, Vector3 topRight, float cornerRadius, int cornerSegments, int index)
		{
			if (cornerSegments < 1)
			{
				Debug.LogError("VectorLine.MakeRoundedRect: cornerSegments value must be >= 1");
				return;
			}
			if (index < 0)
			{
				Debug.LogError("VectorLine.MakeRoundedRect: index value must be >= 0");
				return;
			}
			int num = ((m_lineType == LineType.Discrete) ? (cornerSegments * 8 + 8 + index) : (cornerSegments * 4 + 5 + index));
			if (pointsCount < num)
			{
				Resize(num);
			}
			if (bottomLeft.x > topRight.x)
			{
				Exchange(ref bottomLeft, ref topRight, 0);
			}
			if (bottomLeft.y > topRight.y)
			{
				Exchange(ref bottomLeft, ref topRight, 1);
			}
			bottomLeft += new Vector3(cornerRadius, cornerRadius);
			topRight -= new Vector3(cornerRadius, cornerRadius);
			MakeCircle(bottomLeft, cornerRadius, 4 * cornerSegments, index);
			int num2 = ((m_lineType == LineType.Discrete) ? (cornerSegments * 2) : (cornerSegments + 1));
			int originalCount = ((m_lineType == LineType.Discrete) ? (cornerSegments * 2) : cornerSegments);
			if (m_is2D)
			{
				CopyAndAddPoints(num2, originalCount, 3, new Vector2(0f, topRight.y - bottomLeft.y), index);
				CopyAndAddPoints(num2, originalCount, 2, Vector2.zero, index);
				CopyAndAddPoints(num2, originalCount, 1, new Vector2(topRight.x - bottomLeft.x, 0f), index);
				CopyAndAddPoints(num2, originalCount, 0, new Vector2(topRight.x - bottomLeft.x, topRight.y - bottomLeft.y), index);
				if (m_lineType != LineType.Discrete)
				{
					m_points2[num2 * 4 + index] = m_points2[index];
					return;
				}
				m_points2[num2 * 4 + 7 + index] = m_points2[index];
				m_points2[num2 * 3 + 5 + index] = m_points2[num2 * 3 + 6 + index];
				m_points2[num2 * 2 + 3 + index] = m_points2[num2 * 2 + 4 + index];
				m_points2[num2 + 1 + index] = m_points2[num2 + 2 + index];
			}
			else
			{
				CopyAndAddPoints(num2, originalCount, 3, Vector2.zero, index);
				CopyAndAddPoints(num2, originalCount, 2, new Vector2(0f, topRight.y - bottomLeft.y), index);
				CopyAndAddPoints(num2, originalCount, 1, new Vector2(topRight.x - bottomLeft.x, topRight.y - bottomLeft.y), index);
				CopyAndAddPoints(num2, originalCount, 0, new Vector2(topRight.x - bottomLeft.x, 0f), index);
				if (m_lineType != LineType.Discrete)
				{
					m_points3[num2 * 4 + index] = m_points3[index];
					return;
				}
				m_points3[num2 * 4 + 7 + index] = m_points3[index];
				m_points3[num2 * 3 + 5 + index] = m_points3[num2 * 3 + 6 + index];
				m_points3[num2 * 2 + 3 + index] = m_points3[num2 * 2 + 4 + index];
				m_points3[num2 + 1 + index] = m_points3[num2 + 2 + index];
			}
		}

		private void CopyAndAddPoints(int cornerPointCount, int originalCount, int sectionNumber, Vector2 add, int index)
		{
			Vector3 vector = add;
			for (int num = cornerPointCount - 1; num >= 0; num--)
			{
				if (m_lineType != LineType.Discrete)
				{
					if (m_is2D)
					{
						m_points2[cornerPointCount * sectionNumber + num + index] = m_points2[originalCount * sectionNumber + num + index] + add;
					}
					else
					{
						m_points3[cornerPointCount * sectionNumber + num + index] = m_points3[originalCount * sectionNumber + num + index] + vector;
					}
				}
				else if (m_is2D)
				{
					m_points2[cornerPointCount * sectionNumber + sectionNumber * 2 + num + index] = m_points2[originalCount * sectionNumber + num + index] + add;
				}
				else
				{
					m_points3[cornerPointCount * sectionNumber + sectionNumber * 2 + num + index] = m_points3[originalCount * sectionNumber + num + index] + vector;
				}
			}
			if (m_lineType == LineType.Discrete)
			{
				int num2 = cornerPointCount * (sectionNumber + 1) + sectionNumber * 2 + index;
				if (m_is2D)
				{
					m_points2[num2] = m_points2[num2 - 1];
				}
				else
				{
					m_points3[num2] = m_points3[num2 - 1];
				}
			}
		}

		private void Exchange(ref Vector3 v1, ref Vector3 v2, int i)
		{
			float value = v1[i];
			v1[i] = v2[i];
			v2[i] = value;
		}

		public void MakeCircle(Vector3 origin, float radius)
		{
			MakeEllipse(origin, Vector3.forward, radius, radius, 0f, 0f, GetSegmentNumber(), 0f, 0);
		}

		public void MakeCircle(Vector3 origin, float radius, int segments)
		{
			MakeEllipse(origin, Vector3.forward, radius, radius, 0f, 0f, segments, 0f, 0);
		}

		public void MakeCircle(Vector3 origin, float radius, int segments, float pointRotation)
		{
			MakeEllipse(origin, Vector3.forward, radius, radius, 0f, 0f, segments, pointRotation, 0);
		}

		public void MakeCircle(Vector3 origin, float radius, int segments, int index)
		{
			MakeEllipse(origin, Vector3.forward, radius, radius, 0f, 0f, segments, 0f, index);
		}

		public void MakeCircle(Vector3 origin, float radius, int segments, float pointRotation, int index)
		{
			MakeEllipse(origin, Vector3.forward, radius, radius, 0f, 0f, segments, pointRotation, index);
		}

		public void MakeCircle(Vector3 origin, Vector3 upVector, float radius)
		{
			MakeEllipse(origin, upVector, radius, radius, 0f, 0f, GetSegmentNumber(), 0f, 0);
		}

		public void MakeCircle(Vector3 origin, Vector3 upVector, float radius, int segments)
		{
			MakeEllipse(origin, upVector, radius, radius, 0f, 0f, segments, 0f, 0);
		}

		public void MakeCircle(Vector3 origin, Vector3 upVector, float radius, int segments, float pointRotation)
		{
			MakeEllipse(origin, upVector, radius, radius, 0f, 0f, segments, pointRotation, 0);
		}

		public void MakeCircle(Vector3 origin, Vector3 upVector, float radius, int segments, int index)
		{
			MakeEllipse(origin, upVector, radius, radius, 0f, 0f, segments, 0f, index);
		}

		public void MakeCircle(Vector3 origin, Vector3 upVector, float radius, int segments, float pointRotation, int index)
		{
			MakeEllipse(origin, upVector, radius, radius, 0f, 0f, segments, pointRotation, index);
		}

		public void MakeEllipse(Vector3 origin, float xRadius, float yRadius)
		{
			MakeEllipse(origin, Vector3.forward, xRadius, yRadius, 0f, 0f, GetSegmentNumber(), 0f, 0);
		}

		public void MakeEllipse(Vector3 origin, float xRadius, float yRadius, int segments)
		{
			MakeEllipse(origin, Vector3.forward, xRadius, yRadius, 0f, 0f, segments, 0f, 0);
		}

		public void MakeEllipse(Vector3 origin, float xRadius, float yRadius, int segments, int index)
		{
			MakeEllipse(origin, Vector3.forward, xRadius, yRadius, 0f, 0f, segments, 0f, index);
		}

		public void MakeEllipse(Vector3 origin, float xRadius, float yRadius, int segments, float pointRotation)
		{
			MakeEllipse(origin, Vector3.forward, xRadius, yRadius, 0f, 0f, segments, pointRotation, 0);
		}

		public void MakeEllipse(Vector3 origin, Vector3 upVector, float xRadius, float yRadius)
		{
			MakeEllipse(origin, upVector, xRadius, yRadius, 0f, 0f, GetSegmentNumber(), 0f, 0);
		}

		public void MakeEllipse(Vector3 origin, Vector3 upVector, float xRadius, float yRadius, int segments)
		{
			MakeEllipse(origin, upVector, xRadius, yRadius, 0f, 0f, segments, 0f, 0);
		}

		public void MakeEllipse(Vector3 origin, Vector3 upVector, float xRadius, float yRadius, int segments, int index)
		{
			MakeEllipse(origin, upVector, xRadius, yRadius, 0f, 0f, segments, 0f, index);
		}

		public void MakeEllipse(Vector3 origin, Vector3 upVector, float xRadius, float yRadius, int segments, float pointRotation)
		{
			MakeEllipse(origin, upVector, xRadius, yRadius, 0f, 0f, segments, pointRotation, 0);
		}

		public void MakeEllipse(Vector3 origin, Vector3 upVector, float xRadius, float yRadius, int segments, float pointRotation, int index)
		{
			MakeEllipse(origin, upVector, xRadius, yRadius, 0f, 0f, segments, pointRotation, index);
		}

		public void MakeArc(Vector3 origin, float xRadius, float yRadius, float startDegrees, float endDegrees)
		{
			MakeEllipse(origin, Vector3.forward, xRadius, yRadius, startDegrees, endDegrees, GetSegmentNumber(), 0f, 0);
		}

		public void MakeArc(Vector3 origin, float xRadius, float yRadius, float startDegrees, float endDegrees, int segments)
		{
			MakeEllipse(origin, Vector3.forward, xRadius, yRadius, startDegrees, endDegrees, segments, 0f, 0);
		}

		public void MakeArc(Vector3 origin, float xRadius, float yRadius, float startDegrees, float endDegrees, int segments, int index)
		{
			MakeEllipse(origin, Vector3.forward, xRadius, yRadius, startDegrees, endDegrees, segments, 0f, index);
		}

		public void MakeArc(Vector3 origin, Vector3 upVector, float xRadius, float yRadius, float startDegrees, float endDegrees)
		{
			MakeEllipse(origin, upVector, xRadius, yRadius, startDegrees, endDegrees, GetSegmentNumber(), 0f, 0);
		}

		public void MakeArc(Vector3 origin, Vector3 upVector, float xRadius, float yRadius, float startDegrees, float endDegrees, int segments)
		{
			MakeEllipse(origin, upVector, xRadius, yRadius, startDegrees, endDegrees, segments, 0f, 0);
		}

		public void MakeArc(Vector3 origin, Vector3 upVector, float xRadius, float yRadius, float startDegrees, float endDegrees, int segments, int index)
		{
			MakeEllipse(origin, upVector, xRadius, yRadius, startDegrees, endDegrees, segments, 0f, index);
		}

		private void MakeEllipse(Vector3 origin, Vector3 upVector, float xRadius, float yRadius, float startDegrees, float endDegrees, int segments, float pointRotation, int index)
		{
			if (segments < 3)
			{
				Debug.LogError("VectorLine.MakeEllipse needs at least 3 segments");
			}
			else
			{
				if (!CheckArrayLength(FunctionName.MakeEllipse, segments, index))
				{
					return;
				}
				startDegrees = Mathf.Repeat(startDegrees, 360f);
				endDegrees = Mathf.Repeat(endDegrees, 360f);
				float num;
				float num2;
				if (startDegrees == endDegrees)
				{
					num = 360f;
					num2 = (0f - pointRotation) * ((float)Math.PI / 180f);
				}
				else
				{
					num = ((!(endDegrees > startDegrees)) ? (360f - startDegrees + endDegrees) : (endDegrees - startDegrees));
					num2 = startDegrees * ((float)Math.PI / 180f);
				}
				float num3 = num / (float)segments * ((float)Math.PI / 180f);
				if (m_lineType != LineType.Discrete)
				{
					if (startDegrees != endDegrees)
					{
						segments++;
					}
					int num4 = 0;
					if (m_is2D)
					{
						Vector2 vector = origin;
						for (num4 = 0; num4 < segments; num4++)
						{
							m_points2[index + num4] = vector + new Vector2(0.5f + Mathf.Sin(num2) * xRadius, 0.5f + Mathf.Cos(num2) * yRadius);
							num2 += num3;
						}
						if (m_lineType != LineType.Points && startDegrees == endDegrees)
						{
							m_points2[index + num4] = m_points2[index + (num4 - segments)];
						}
					}
					else
					{
						Matrix4x4 matrix4x = Matrix4x4.TRS(Vector3.zero, Quaternion.LookRotation(-upVector, upVector), Vector3.one);
						for (num4 = 0; num4 < segments; num4++)
						{
							m_points3[index + num4] = origin + matrix4x.MultiplyPoint3x4(new Vector3(Mathf.Sin(num2) * xRadius, Mathf.Cos(num2) * yRadius, 0f));
							num2 += num3;
						}
						if (m_lineType != LineType.Points && startDegrees == endDegrees)
						{
							m_points3[index + num4] = m_points3[index + (num4 - segments)];
						}
					}
				}
				else if (m_is2D)
				{
					Vector2 vector2 = origin;
					int num5;
					for (num5 = 0; num5 < segments * 2; num5++)
					{
						m_points2[index + num5] = vector2 + new Vector2(0.5f + Mathf.Sin(num2) * xRadius, 0.5f + Mathf.Cos(num2) * yRadius);
						num2 += num3;
						num5++;
						m_points2[index + num5] = vector2 + new Vector2(0.5f + Mathf.Sin(num2) * xRadius, 0.5f + Mathf.Cos(num2) * yRadius);
					}
				}
				else
				{
					Matrix4x4 matrix4x2 = Matrix4x4.TRS(Vector3.zero, Quaternion.LookRotation(-upVector, upVector), Vector3.one);
					int num6;
					for (num6 = 0; num6 < segments * 2; num6++)
					{
						m_points3[index + num6] = origin + matrix4x2.MultiplyPoint3x4(new Vector3(Mathf.Sin(num2) * xRadius, Mathf.Cos(num2) * yRadius, 0f));
						num2 += num3;
						num6++;
						m_points3[index + num6] = origin + matrix4x2.MultiplyPoint3x4(new Vector3(Mathf.Sin(num2) * xRadius, Mathf.Cos(num2) * yRadius, 0f));
					}
				}
			}
		}

		public void MakeCurve(Vector2[] curvePoints)
		{
			MakeCurve(curvePoints, GetSegmentNumber(), 0);
		}

		public void MakeCurve(Vector2[] curvePoints, int segments)
		{
			MakeCurve(curvePoints, segments, 0);
		}

		public void MakeCurve(Vector2[] curvePoints, int segments, int index)
		{
			if (curvePoints.Length != 4)
			{
				Debug.LogError("VectorLine.MakeCurve needs exactly 4 points in the curve points array");
			}
			else
			{
				MakeCurve(curvePoints[0], curvePoints[1], curvePoints[2], curvePoints[3], segments, index);
			}
		}

		public void MakeCurve(Vector3[] curvePoints)
		{
			MakeCurve(curvePoints, GetSegmentNumber(), 0);
		}

		public void MakeCurve(Vector3[] curvePoints, int segments)
		{
			MakeCurve(curvePoints, segments, 0);
		}

		public void MakeCurve(Vector3[] curvePoints, int segments, int index)
		{
			if (curvePoints.Length != 4)
			{
				Debug.LogError("VectorLine.MakeCurve needs exactly 4 points in the curve points array");
			}
			else
			{
				MakeCurve(curvePoints[0], curvePoints[1], curvePoints[2], curvePoints[3], segments, index);
			}
		}

		public void MakeCurve(Vector3 anchor1, Vector3 control1, Vector3 anchor2, Vector3 control2)
		{
			MakeCurve(anchor1, control1, anchor2, control2, GetSegmentNumber(), 0);
		}

		public void MakeCurve(Vector3 anchor1, Vector3 control1, Vector3 anchor2, Vector3 control2, int segments)
		{
			MakeCurve(anchor1, control1, anchor2, control2, segments, 0);
		}

		public void MakeCurve(Vector3 anchor1, Vector3 control1, Vector3 anchor2, Vector3 control2, int segments, int index)
		{
			if (!CheckArrayLength(FunctionName.MakeCurve, segments, index))
			{
				return;
			}
			if (m_lineType != LineType.Discrete)
			{
				int num = ((m_lineType != LineType.Points) ? (segments + 1) : segments);
				if (m_is2D)
				{
					Vector2 anchor3 = anchor1;
					Vector2 anchor4 = anchor2;
					Vector2 control3 = control1;
					Vector2 control4 = control2;
					for (int i = 0; i < num; i++)
					{
						m_points2[index + i] = GetBezierPoint(ref anchor3, ref control3, ref anchor4, ref control4, (float)i / (float)segments);
					}
				}
				else
				{
					for (int j = 0; j < num; j++)
					{
						m_points3[index + j] = GetBezierPoint3D(ref anchor1, ref control1, ref anchor2, ref control2, (float)j / (float)segments);
					}
				}
				return;
			}
			int num2 = 0;
			if (m_is2D)
			{
				Vector2 anchor5 = anchor1;
				Vector2 anchor6 = anchor2;
				Vector2 control5 = control1;
				Vector2 control6 = control2;
				for (int k = 0; k < segments; k++)
				{
					m_points2[index + num2++] = GetBezierPoint(ref anchor5, ref control5, ref anchor6, ref control6, (float)k / (float)segments);
					m_points2[index + num2++] = GetBezierPoint(ref anchor5, ref control5, ref anchor6, ref control6, (float)(k + 1) / (float)segments);
				}
			}
			else
			{
				for (int l = 0; l < segments; l++)
				{
					m_points3[index + num2++] = GetBezierPoint3D(ref anchor1, ref control1, ref anchor2, ref control2, (float)l / (float)segments);
					m_points3[index + num2++] = GetBezierPoint3D(ref anchor1, ref control1, ref anchor2, ref control2, (float)(l + 1) / (float)segments);
				}
			}
		}

		private static Vector2 GetBezierPoint(ref Vector2 anchor1, ref Vector2 control1, ref Vector2 anchor2, ref Vector2 control2, float t)
		{
			float num = 3f * (control1.x - anchor1.x);
			float num2 = 3f * (control2.x - control1.x) - num;
			float num3 = anchor2.x - anchor1.x - num - num2;
			float num4 = 3f * (control1.y - anchor1.y);
			float num5 = 3f * (control2.y - control1.y) - num4;
			float num6 = anchor2.y - anchor1.y - num4 - num5;
			return new Vector2(num3 * (t * t * t) + num2 * (t * t) + num * t + anchor1.x, num6 * (t * t * t) + num5 * (t * t) + num4 * t + anchor1.y);
		}

		private static Vector3 GetBezierPoint3D(ref Vector3 anchor1, ref Vector3 control1, ref Vector3 anchor2, ref Vector3 control2, float t)
		{
			float num = 3f * (control1.x - anchor1.x);
			float num2 = 3f * (control2.x - control1.x) - num;
			float num3 = anchor2.x - anchor1.x - num - num2;
			float num4 = 3f * (control1.y - anchor1.y);
			float num5 = 3f * (control2.y - control1.y) - num4;
			float num6 = anchor2.y - anchor1.y - num4 - num5;
			float num7 = 3f * (control1.z - anchor1.z);
			float num8 = 3f * (control2.z - control1.z) - num7;
			float num9 = anchor2.z - anchor1.z - num7 - num8;
			return new Vector3(num3 * (t * t * t) + num2 * (t * t) + num * t + anchor1.x, num6 * (t * t * t) + num5 * (t * t) + num4 * t + anchor1.y, num9 * (t * t * t) + num8 * (t * t) + num7 * t + anchor1.z);
		}

		public void MakeSpline(Vector2[] splinePoints)
		{
			MakeSpline(splinePoints, null, GetSegmentNumber(), 0, false);
		}

		public void MakeSpline(Vector2[] splinePoints, bool loop)
		{
			MakeSpline(splinePoints, null, GetSegmentNumber(), 0, loop);
		}

		public void MakeSpline(Vector2[] splinePoints, int segments)
		{
			MakeSpline(splinePoints, null, segments, 0, false);
		}

		public void MakeSpline(Vector2[] splinePoints, int segments, bool loop)
		{
			MakeSpline(splinePoints, null, segments, 0, loop);
		}

		public void MakeSpline(Vector2[] splinePoints, int segments, int index)
		{
			MakeSpline(splinePoints, null, segments, index, false);
		}

		public void MakeSpline(Vector2[] splinePoints, int segments, int index, bool loop)
		{
			MakeSpline(splinePoints, null, segments, index, loop);
		}

		public void MakeSpline(Vector3[] splinePoints)
		{
			MakeSpline(null, splinePoints, GetSegmentNumber(), 0, false);
		}

		public void MakeSpline(Vector3[] splinePoints, bool loop)
		{
			MakeSpline(null, splinePoints, GetSegmentNumber(), 0, loop);
		}

		public void MakeSpline(Vector3[] splinePoints, int segments)
		{
			MakeSpline(null, splinePoints, segments, 0, false);
		}

		public void MakeSpline(Vector3[] splinePoints, int segments, bool loop)
		{
			MakeSpline(null, splinePoints, segments, 0, loop);
		}

		public void MakeSpline(Vector3[] splinePoints, int segments, int index)
		{
			MakeSpline(null, splinePoints, segments, index, false);
		}

		public void MakeSpline(Vector3[] splinePoints, int segments, int index, bool loop)
		{
			MakeSpline(null, splinePoints, segments, index, loop);
		}

		private void MakeSpline(Vector2[] splinePoints2, Vector3[] splinePoints3, int segments, int index, bool loop)
		{
			int num = ((splinePoints2 == null) ? splinePoints3.Length : splinePoints2.Length);
			if (num < 2)
			{
				Debug.LogError("VectorLine.MakeSpline needs at least 2 spline points");
			}
			else if (splinePoints2 != null && !m_is2D)
			{
				Debug.LogError("VectorLine.MakeSpline was called with a Vector2 spline points array, but the line uses Vector3 points");
			}
			else if (splinePoints3 != null && m_is2D)
			{
				Debug.LogError("VectorLine.MakeSpline was called with a Vector3 spline points array, but the line uses Vector2 points");
			}
			else
			{
				if (!CheckArrayLength(FunctionName.MakeSpline, segments, index))
				{
					return;
				}
				int num2 = index;
				int num3 = ((!loop) ? (num - 1) : num);
				float num4 = 1f / (float)segments * (float)num3;
				float num5 = 0f;
				int num6 = 0;
				int num7 = 0;
				int num8 = 0;
				int i;
				for (i = 0; i < num3; i++)
				{
					num6 = i - 1;
					num7 = i + 1;
					num8 = i + 2;
					if (num6 < 0)
					{
						num6 = (loop ? (num3 - 1) : 0);
					}
					if (loop && num7 > num3 - 1)
					{
						num7 -= num3;
					}
					if (num8 > num3 - 1)
					{
						num8 = ((!loop) ? num3 : (num8 - num3));
					}
					float num9;
					if (m_lineType != LineType.Discrete)
					{
						if (m_is2D)
						{
							for (num9 = num5; num9 <= 1f; num9 += num4)
							{
								m_points2[num2++] = GetSplinePoint(ref splinePoints2[num6], ref splinePoints2[i], ref splinePoints2[num7], ref splinePoints2[num8], num9);
							}
						}
						else
						{
							for (num9 = num5; num9 <= 1f; num9 += num4)
							{
								m_points3[num2++] = GetSplinePoint3D(ref splinePoints3[num6], ref splinePoints3[i], ref splinePoints3[num7], ref splinePoints3[num8], num9);
							}
						}
					}
					else if (m_is2D)
					{
						for (num9 = num5; num9 <= 1f; num9 += num4)
						{
							m_points2[num2++] = GetSplinePoint(ref splinePoints2[num6], ref splinePoints2[i], ref splinePoints2[num7], ref splinePoints2[num8], num9);
							if (num2 > index + 1 && num2 < index + segments * 2)
							{
								m_points2[num2++] = m_points2[num2 - 2];
							}
						}
					}
					else
					{
						for (num9 = num5; num9 <= 1f; num9 += num4)
						{
							m_points3[num2++] = GetSplinePoint3D(ref splinePoints3[num6], ref splinePoints3[i], ref splinePoints3[num7], ref splinePoints3[num8], num9);
							if (num2 > index + 1 && num2 < index + segments * 2)
							{
								m_points3[num2++] = m_points3[num2 - 2];
							}
						}
					}
					num5 = num9 - 1f;
				}
				if ((m_lineType != LineType.Discrete && num2 < index + (segments + 1)) || (m_lineType == LineType.Discrete && num2 < index + segments * 2))
				{
					if (m_is2D)
					{
						m_points2[num2] = GetSplinePoint(ref splinePoints2[num6], ref splinePoints2[i - 1], ref splinePoints2[num7], ref splinePoints2[num8], 1f);
					}
					else
					{
						m_points3[num2] = GetSplinePoint3D(ref splinePoints3[num6], ref splinePoints3[i - 1], ref splinePoints3[num7], ref splinePoints3[num8], 1f);
					}
				}
			}
		}

		private static Vector2 GetSplinePoint(ref Vector2 p0, ref Vector2 p1, ref Vector2 p2, ref Vector2 p3, float t)
		{
			Vector4 p4 = Vector4.zero;
			Vector4 p5 = Vector4.zero;
			float num = Mathf.Pow(VectorDistanceSquared(ref p0, ref p1), 0.25f);
			float num2 = Mathf.Pow(VectorDistanceSquared(ref p1, ref p2), 0.25f);
			float num3 = Mathf.Pow(VectorDistanceSquared(ref p2, ref p3), 0.25f);
			if (num2 < 0.0001f)
			{
				num2 = 1f;
			}
			if (num < 0.0001f)
			{
				num = num2;
			}
			if (num3 < 0.0001f)
			{
				num3 = num2;
			}
			InitNonuniformCatmullRom(p0.x, p1.x, p2.x, p3.x, num, num2, num3, ref p4);
			InitNonuniformCatmullRom(p0.y, p1.y, p2.y, p3.y, num, num2, num3, ref p5);
			return new Vector2(EvalCubicPoly(ref p4, t), EvalCubicPoly(ref p5, t));
		}

		private static Vector3 GetSplinePoint3D(ref Vector3 p0, ref Vector3 p1, ref Vector3 p2, ref Vector3 p3, float t)
		{
			Vector4 p4 = Vector4.zero;
			Vector4 p5 = Vector4.zero;
			Vector4 p6 = Vector4.zero;
			float num = Mathf.Pow(VectorDistanceSquared(ref p0, ref p1), 0.25f);
			float num2 = Mathf.Pow(VectorDistanceSquared(ref p1, ref p2), 0.25f);
			float num3 = Mathf.Pow(VectorDistanceSquared(ref p2, ref p3), 0.25f);
			if (num2 < 0.0001f)
			{
				num2 = 1f;
			}
			if (num < 0.0001f)
			{
				num = num2;
			}
			if (num3 < 0.0001f)
			{
				num3 = num2;
			}
			InitNonuniformCatmullRom(p0.x, p1.x, p2.x, p3.x, num, num2, num3, ref p4);
			InitNonuniformCatmullRom(p0.y, p1.y, p2.y, p3.y, num, num2, num3, ref p5);
			InitNonuniformCatmullRom(p0.z, p1.z, p2.z, p3.z, num, num2, num3, ref p6);
			return new Vector3(EvalCubicPoly(ref p4, t), EvalCubicPoly(ref p5, t), EvalCubicPoly(ref p6, t));
		}

		private static float VectorDistanceSquared(ref Vector2 p, ref Vector2 q)
		{
			float num = q.x - p.x;
			float num2 = q.y - p.y;
			return num * num + num2 * num2;
		}

		private static float VectorDistanceSquared(ref Vector3 p, ref Vector3 q)
		{
			float num = q.x - p.x;
			float num2 = q.y - p.y;
			float num3 = q.z - p.z;
			return num * num + num2 * num2 + num3 * num3;
		}

		private static void InitNonuniformCatmullRom(float x0, float x1, float x2, float x3, float dt0, float dt1, float dt2, ref Vector4 p)
		{
			float num = ((x1 - x0) / dt0 - (x2 - x0) / (dt0 + dt1) + (x2 - x1) / dt1) * dt1;
			float num2 = ((x2 - x1) / dt1 - (x3 - x1) / (dt1 + dt2) + (x3 - x2) / dt2) * dt1;
			p.x = x1;
			p.y = num;
			p.z = -3f * x1 + 3f * x2 - 2f * num - num2;
			p.w = 2f * x1 - 2f * x2 + num + num2;
		}

		private static float EvalCubicPoly(ref Vector4 p, float t)
		{
			return p.x + p.y * t + p.z * (t * t) + p.w * (t * t * t);
		}

		public void MakeText(string text, Vector3 startPos, float size)
		{
			MakeText(text, startPos, size, 1f, 1.5f, true);
		}

		public void MakeText(string text, Vector3 startPos, float size, bool uppercaseOnly)
		{
			MakeText(text, startPos, size, 1f, 1.5f, uppercaseOnly);
		}

		public void MakeText(string text, Vector3 startPos, float size, float charSpacing, float lineSpacing)
		{
			MakeText(text, startPos, size, charSpacing, lineSpacing, true);
		}

		public void MakeText(string text, Vector3 startPos, float size, float charSpacing, float lineSpacing, bool uppercaseOnly)
		{
			if (m_lineType != LineType.Discrete)
			{
				Debug.LogError("VectorLine.MakeText only works with a discrete line");
				return;
			}
			int num = 0;
			for (int i = 0; i < text.Length; i++)
			{
				int num2 = Convert.ToInt32(text[i]);
				if (num2 < 0 || num2 > 256)
				{
					Debug.LogError("VectorLine.MakeText: Character '" + text[i] + "' is not valid");
					return;
				}
				if (uppercaseOnly && num2 >= 97 && num2 <= 122)
				{
					num2 -= 32;
				}
				if (VectorChar.data[num2] != null)
				{
					num += VectorChar.data[num2].Length;
				}
			}
			if (num != pointsCount)
			{
				Resize(num);
			}
			float num3 = 0f;
			float num4 = 0f;
			int num5 = 0;
			Vector2 vector = new Vector2(size, size);
			for (int j = 0; j < text.Length; j++)
			{
				int num6 = Convert.ToInt32(text[j]);
				switch (num6)
				{
				case 10:
					num4 -= lineSpacing;
					num3 = 0f;
					continue;
				case 32:
					num3 += charSpacing;
					continue;
				}
				if (uppercaseOnly && num6 >= 97 && num6 <= 122)
				{
					num6 -= 32;
				}
				int num7 = 0;
				if (VectorChar.data[num6] != null)
				{
					num7 = VectorChar.data[num6].Length;
					if (m_is2D)
					{
						for (int k = 0; k < num7; k++)
						{
							m_points2[num5++] = Vector2.Scale(VectorChar.data[num6][k] + new Vector2(num3, num4), vector) + (Vector2)startPos;
						}
					}
					else
					{
						for (int l = 0; l < num7; l++)
						{
							m_points3[num5++] = Vector3.Scale((Vector3)VectorChar.data[num6][l] + new Vector3(num3, num4, 0f), vector) + startPos;
						}
					}
					num3 += charSpacing;
				}
				else
				{
					num3 += charSpacing;
				}
			}
		}

		public void MakeWireframe(Mesh mesh)
		{
			if (m_lineType != LineType.Discrete)
			{
				Debug.LogError("VectorLine.MakeWireframe only works with a discrete line");
				return;
			}
			if (m_is2D)
			{
				Debug.LogError("VectorLine.MakeWireframe can only be used with Vector3 points, which \"" + name + "\" doesn't have");
				return;
			}
			if (mesh == null)
			{
				Debug.LogError("VectorLine.MakeWireframe can't use a null mesh");
				return;
			}
			int[] triangles = mesh.triangles;
			Vector3[] vertices = mesh.vertices;
			Dictionary<Vector3Pair, bool> pairs = new Dictionary<Vector3Pair, bool>();
			List<Vector3> list = new List<Vector3>();
			for (int i = 0; i < triangles.Length; i += 3)
			{
				CheckPairPoints(pairs, vertices[triangles[i]], vertices[triangles[i + 1]], list);
				CheckPairPoints(pairs, vertices[triangles[i + 1]], vertices[triangles[i + 2]], list);
				CheckPairPoints(pairs, vertices[triangles[i + 2]], vertices[triangles[i]], list);
			}
			if (list.Count != m_pointsCount)
			{
				Resize(list.Count);
			}
			for (int j = 0; j < m_pointsCount; j++)
			{
				m_points3[j] = list[j];
			}
		}

		private static void CheckPairPoints(Dictionary<Vector3Pair, bool> pairs, Vector3 p1, Vector3 p2, List<Vector3> linePoints)
		{
			Vector3Pair key = new Vector3Pair(p1, p2);
			Vector3Pair key2 = new Vector3Pair(p2, p1);
			if (!pairs.ContainsKey(key) && !pairs.ContainsKey(key2))
			{
				pairs[key] = true;
				pairs[key2] = true;
				linePoints.Add(p1);
				linePoints.Add(p2);
			}
		}

		public void MakeCube(Vector3 position, float xSize, float ySize, float zSize)
		{
			MakeCube(position, xSize, ySize, zSize, 0);
		}

		public void MakeCube(Vector3 position, float xSize, float ySize, float zSize, int index)
		{
			if (m_lineType != LineType.Discrete)
			{
				Debug.LogError("VectorLine.MakeCube only works with a discrete line");
				return;
			}
			if (m_is2D)
			{
				Debug.LogError("VectorLine.MakeCube can only be used with Vector3 points, which \"" + name + "\" doesn't have");
				return;
			}
			if (index + 24 > m_pointsCount)
			{
				if (index == 0)
				{
					Debug.LogError("VectorLine.MakeCube: The number of Vector3 points needs to be at least 24 for \"" + name + "\"");
					return;
				}
				Debug.LogError("Calling VectorLine.MakeCube with an index of " + index + " would exceed the length of the Vector3 points for \"" + name + "\"");
				return;
			}
			xSize /= 2f;
			ySize /= 2f;
			zSize /= 2f;
			m_points3[index] = position + new Vector3(0f - xSize, ySize, 0f - zSize);
			m_points3[index + 1] = position + new Vector3(xSize, ySize, 0f - zSize);
			m_points3[index + 2] = position + new Vector3(xSize, ySize, 0f - zSize);
			m_points3[index + 3] = position + new Vector3(xSize, ySize, zSize);
			m_points3[index + 4] = position + new Vector3(xSize, ySize, zSize);
			m_points3[index + 5] = position + new Vector3(0f - xSize, ySize, zSize);
			m_points3[index + 6] = position + new Vector3(0f - xSize, ySize, zSize);
			m_points3[index + 7] = position + new Vector3(0f - xSize, ySize, 0f - zSize);
			m_points3[index + 8] = position + new Vector3(0f - xSize, 0f - ySize, 0f - zSize);
			m_points3[index + 9] = position + new Vector3(0f - xSize, ySize, 0f - zSize);
			m_points3[index + 10] = position + new Vector3(xSize, 0f - ySize, 0f - zSize);
			m_points3[index + 11] = position + new Vector3(xSize, ySize, 0f - zSize);
			m_points3[index + 12] = position + new Vector3(0f - xSize, 0f - ySize, zSize);
			m_points3[index + 13] = position + new Vector3(0f - xSize, ySize, zSize);
			m_points3[index + 14] = position + new Vector3(xSize, 0f - ySize, zSize);
			m_points3[index + 15] = position + new Vector3(xSize, ySize, zSize);
			m_points3[index + 16] = position + new Vector3(0f - xSize, 0f - ySize, 0f - zSize);
			m_points3[index + 17] = position + new Vector3(xSize, 0f - ySize, 0f - zSize);
			m_points3[index + 18] = position + new Vector3(xSize, 0f - ySize, 0f - zSize);
			m_points3[index + 19] = position + new Vector3(xSize, 0f - ySize, zSize);
			m_points3[index + 20] = position + new Vector3(xSize, 0f - ySize, zSize);
			m_points3[index + 21] = position + new Vector3(0f - xSize, 0f - ySize, zSize);
			m_points3[index + 22] = position + new Vector3(0f - xSize, 0f - ySize, zSize);
			m_points3[index + 23] = position + new Vector3(0f - xSize, 0f - ySize, 0f - zSize);
		}
	}
}
