using Il2CppInterop.Runtime.Injection;
using UnityEngine;
using UnityExplorer;
using Object = UnityEngine.Object;

namespace Vectrosity
{
	public class VectorObject3D : MonoBehaviour, IVectorObject
	{
		static VectorObject3D()
		{
			ClassInjector.RegisterTypeInIl2Cpp<VectorObject3D>();
		}
		
		private bool m_updateVerts = true;

		private bool m_updateUVs = true;

		private bool m_updateColors = true;

		private bool m_updateNormals = false;

		private bool m_updateTangents = false;

		private bool m_updateTris = true;

		private Mesh m_mesh;

		private VectorLine m_vectorLine;

		private Material m_material;

		private bool useCustomMaterial = false;

		public void SetVectorLine(VectorLine vectorLine, Texture tex, Material mat)
		{
			base.gameObject.AddComponent<MeshRenderer>();
			base.gameObject.AddComponent<MeshFilter>();
			m_vectorLine = vectorLine;
			m_material = new Material(mat);
			m_material.mainTexture = tex;
			GetComponent<MeshRenderer>().sharedMaterial = m_material;
			SetupMesh();
		}

		public void Destroy()
		{
			Object.Destroy(m_mesh);
			if (!useCustomMaterial)
			{
				Object.Destroy(m_material);
			}
		}

		public void Enable(bool enable)
		{
			if (!(this == null))
			{
				GetComponent<MeshRenderer>().enabled = enable;
			}
		}

		public void SetTexture(Texture tex)
		{
			GetComponent<MeshRenderer>().sharedMaterial.mainTexture = tex;
		}

		public void SetMaterial(Material mat)
		{
			m_material = mat;
			useCustomMaterial = true;
			GetComponent<MeshRenderer>().sharedMaterial = mat;
			GetComponent<MeshRenderer>().sharedMaterial.mainTexture = m_vectorLine.texture;
		}

		private void SetupMesh()
		{
			ExplorerCore.Log("Setup Mesh");
			if(m_vectorLine == null)
			{
				ExplorerCore.Log("VectorLine is null");
				return;
			}

			if (!GetComponent<MeshFilter>())
			{
				gameObject.AddComponent<MeshFilter>();
			}
			
			m_mesh = new Mesh();
			m_mesh.name = m_vectorLine.name;
			m_mesh.hideFlags = HideFlags.HideAndDontSave;
			GetComponent<MeshFilter>().mesh = m_mesh;
		}

		private void LateUpdate()
		{
			if (m_updateVerts)
			{
				SetVerts();
			}
			if (m_updateUVs)
			{
				if (m_vectorLine.lineUVs.Length == m_mesh.vertexCount)
				{
					m_mesh.uv = m_vectorLine.lineUVs;
				}
				m_updateUVs = false;
			}
			if (m_updateColors)
			{
				if (m_vectorLine.lineColors.Length == m_mesh.vertexCount)
				{
					m_mesh.colors32 = m_vectorLine.lineColors;
				}
				m_updateColors = false;
			}
			if (m_updateTris)
			{
				var l = new Il2CppSystem.Collections.Generic.List<int>();
				for (var i = 0; i < m_vectorLine.lineTriangles.Count; i++)
				{
					l.Add(m_vectorLine.lineTriangles[i]);
				}
				m_mesh.SetTriangles(l, 0);
				m_updateTris = false;
			}
			if (m_updateNormals)
			{
				m_mesh.RecalculateNormals();
				m_updateNormals = false;
			}
			if (m_updateTangents)
			{
				m_mesh.tangents = m_vectorLine.CalculateTangents(m_mesh.normals);
				m_updateTangents = false;
			}
		}

		private void SetVerts()
		{
			if (!m_mesh)
			{
				ExplorerCore.Log("Mesh is null!");
				return;
			}

			if (m_vectorLine?.lineVertices == null)
			{
				ExplorerCore.Log("VectorLine is null!");
			}
			
			m_mesh.vertices = m_vectorLine.lineVertices;
			m_updateVerts = false;
			m_mesh.RecalculateBounds();
		}

		public void SetName(string name)
		{
			if (!(m_mesh == null))
			{
				m_mesh.name = name;
			}
		}

		public void UpdateVerts()
		{
			m_updateVerts = true;
		}

		public void UpdateUVs()
		{
			m_updateUVs = true;
		}

		public void UpdateColors()
		{
			m_updateColors = true;
		}

		public void UpdateNormals()
		{
			m_updateNormals = true;
		}

		public void UpdateTangents()
		{
			m_updateTangents = true;
		}

		public void UpdateTris()
		{
			m_updateTris = true;
		}

		public void UpdateMeshAttributes()
		{
			m_mesh.Clear();
			m_updateVerts = true;
			m_updateUVs = true;
			m_updateColors = true;
			m_updateTris = true;
		}

		public void ClearMesh()
		{
			if (!(m_mesh == null))
			{
				m_mesh.Clear();
			}
		}

		public int VertexCount()
		{
			return m_mesh.vertexCount;
		}
	}
}
