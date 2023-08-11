using Il2CppInterop.Runtime.Injection;
using UnityEngine;
using Vectrosity;

public class VisibilityControlAlways : MonoBehaviour
{
	static VisibilityControlAlways()
	{
		ClassInjector.RegisterTypeInIl2Cpp<VisibilityControlAlways>();
	}
	
	private RefInt m_objectNumber;

	private VectorLine m_vectorLine;

	private bool m_destroyed = false;

	public RefInt objectNumber => m_objectNumber;

	public void Setup(VectorLine line)
	{
		VectorManager.VisibilitySetup(base.transform, line, out m_objectNumber);
		VectorManager.DrawArrayLine2(m_objectNumber.i);
		m_vectorLine = line;
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
