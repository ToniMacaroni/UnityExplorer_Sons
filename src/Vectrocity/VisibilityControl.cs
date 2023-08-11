using Il2CppInterop.Runtime.Injection;
using UnityEngine;
using Vectrosity;

public class VisibilityControl : MonoBehaviour
{
	static VisibilityControl()
	{
		ClassInjector.RegisterTypeInIl2Cpp<VisibilityControl>();
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
		VectorManager.VisibilitySetup(base.transform, line, out m_objectNumber);
		m_vectorLine = line;
	}

	private void OnBecameVisible()
	{
		m_vectorLine.active = true;
		VectorManager.DrawArrayLine2(m_objectNumber.i);
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
			VectorManager.VisibilityRemove(m_objectNumber.i);
			VectorLine.Destroy(ref m_vectorLine);
		}
	}
}
