using System.Collections;
using System.Collections.Generic;
using Il2CppInterop.Runtime.Injection;
using UnityEngine;
using UniverseLib.Runtime.Il2Cpp;
using Object = UnityEngine.Object;

namespace Vectrosity
{
	public class LineManager : MonoBehaviour
	{
		static LineManager()
		{
			ClassInjector.RegisterTypeInIl2Cpp<LineManager>();
		}
		
		private static List<VectorLine> lines;

		private static List<Transform> transforms;

		private static int lineCount = 0;

		private bool destroyed = false;

		private void Awake()
		{
			Initialize();
			Object.DontDestroyOnLoad(this);
		}

		private void Initialize()
		{
			lines = new List<VectorLine>();
			transforms = new List<Transform>();
			lineCount = 0;
			base.enabled = false;
		}

		public void AddLine(VectorLine vectorLine, Transform thisTransform, float time)
		{
			if (time > 0f)
			{
				StartCoroutine(DisableLine(vectorLine, time, false).WrapToIl2Cpp());
			}
			for (int i = 0; i < lineCount; i++)
			{
				if (vectorLine == lines[i])
				{
					return;
				}
			}
			lines.Add(vectorLine);
			transforms.Add(thisTransform);
			if (++lineCount == 1)
			{
				base.enabled = true;
			}
		}

		public void DisableLine(VectorLine vectorLine, float time)
		{
			StartCoroutine(DisableLine(vectorLine, time, false).WrapToIl2Cpp());
		}

		private IEnumerator DisableLine(VectorLine vectorLine, float time, bool remove)
		{
			yield return new WaitForSeconds(time);
			if (remove)
			{
				RemoveLine(vectorLine);
			}
			else
			{
				RemoveLine(vectorLine);
				VectorLine.Destroy(ref vectorLine);
			}
			vectorLine = null;
		}

		private void LateUpdate()
		{
			if (!VectorLine.camTransformExists)
			{
				return;
			}
			for (int i = 0; i < lineCount; i++)
			{
				if (lines[i].rectTransform != null)
				{
					lines[i].Draw3D();
				}
				else
				{
					RemoveLine(i--);
				}
			}
			if (VectorLine.CameraHasMoved())
			{
				VectorManager.DrawArrayLines();
			}
			VectorLine.UpdateCameraInfo();
			VectorManager.DrawArrayLines2();
		}

		private void RemoveLine(int i)
		{
			lines.RemoveAt(i);
			transforms.RemoveAt(i);
			lineCount--;
			DisableIfUnused();
		}

		public void RemoveLine(VectorLine vectorLine)
		{
			for (int i = 0; i < lineCount; i++)
			{
				if (vectorLine == lines[i])
				{
					RemoveLine(i);
					break;
				}
			}
		}

		public void DisableIfUnused()
		{
			if (!destroyed && lineCount == 0 && VectorManager.arrayCount == 0 && VectorManager.arrayCount2 == 0)
			{
				base.enabled = false;
			}
		}

		public void EnableIfUsed()
		{
			if (VectorManager.arrayCount == 1 || VectorManager.arrayCount2 == 1)
			{
				base.enabled = true;
			}
		}

		public void StartCheckDistance()
		{
			InvokeRepeating("CheckDistance", 0.01f, VectorManager.distanceCheckFrequency);
		}

		private void CheckDistance()
		{
			VectorManager.CheckDistance();
		}

		private void OnDestroy()
		{
			destroyed = true;
		}

		private void OnLevelWasLoaded()
		{
			Initialize();
		}
	}
}
