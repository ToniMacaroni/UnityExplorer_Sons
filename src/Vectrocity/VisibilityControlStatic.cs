using System.Collections;
using System.Collections.Generic;
using Il2CppInterop.Runtime.Injection;
using UnityEngine;
using UniverseLib.Runtime.Il2Cpp;
using Vectrosity;

public class VisibilityControlStatic : MonoBehaviour
{
	static VisibilityControlStatic()
	{
		ClassInjector.RegisterTypeInIl2Cpp<VisibilityControlStatic>();
	}
	
	private RefInt m_objectNumber;

	private VectorLine m_vectorLine;

	private bool m_destroyed = false;

	public RefInt objectNumber => m_objectNumber;

	public void Setup(VectorLine line, bool makeBounds)
	{
		if (makeBounds)
		{
			VectorManager.SetupBoundsMesh(base.gameObject, line);
		}
		Matrix4x4 localToWorldMatrix = base.transform.localToWorldMatrix;
		List<Vector3> list = new List<Vector3>(line.points3);
		for (int i = 0; i < list.Count; i++)
		{
			list[i] = localToWorldMatrix.MultiplyPoint3x4(list[i]);
		}
		line.points3 = list;
		m_vectorLine = line;
		VectorManager.VisibilityStaticSetup(line, out m_objectNumber);
		StartCoroutine(WaitCheck().WrapToIl2Cpp());
	}

	private IEnumerator WaitCheck()
	{
		VectorManager.DrawArrayLine(m_objectNumber.i);
		yield return null;
		if (!GetComponent<Renderer>().isVisible)
		{
			m_vectorLine.active = false;
		}
	}

	private void OnBecameVisible()
	{
		m_vectorLine.active = true;
		VectorManager.DrawArrayLine(m_objectNumber.i);
	}

	private void OnBecameInvisible()
	{
		m_vectorLine.active = false;
	}

	private void OnDestroy()
	{
		if (!m_destroyed)
		{
			m_destroyed = true;
			VectorManager.VisibilityStaticRemove(m_objectNumber.i);
			VectorLine.Destroy(ref m_vectorLine);
		}
	}
}
