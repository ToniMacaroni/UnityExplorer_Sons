using RedLoader;
using UnityExplorer.Config;

namespace UnityExplorer.Inspectors.MouseInspectors
{
    public class WorldInspector : MouseInspectorBase
    {
        private static Camera MainCamera;
        private static GameObject lastHitObject;
        private static int _layerMask;

        public override void OnBeginMouseInspect()
        {
            MainCamera = Camera.main;
            
            _layerMask = ~0;
            
            if (!string.IsNullOrEmpty(ConfigManager.Inspector_Ignore_Layers.Value))
            {
                var splitted = ConfigManager.Inspector_Ignore_Layers.Value.Split(',');
                foreach (var layerName in splitted)
                {
                    var layer = LayerMask.NameToLayer(layerName);
                    _layerMask &= ~(1 << layer);
                    RLog.Msg($"Added layer {layerName} to ignore mask");
                }
                
                RLog.Msg($"World Inspector ignoring layers: {ConfigManager.Inspector_Ignore_Layers.Value}");
                RLog.Msg($"Resulting Layer mask is: {_layerMask}");
            }

            if (!MainCamera)
            {
                ExplorerCore.LogWarning("No MainCamera found! Cannot inspect world!");
                return;
            }
        }

        public override void ClearHitData()
        {
            lastHitObject = null;
        }

        public override void OnSelectMouseInspect()
        {
            InspectorManager.Inspect(lastHitObject);
        }

        public override void UpdateMouseInspect(Vector2 mousePos)
        {
            if (!MainCamera)
                MainCamera = Camera.main;
            if (!MainCamera)
            {
                ExplorerCore.LogWarning("No Main Camera was found, unable to inspect world!");
                MouseInspector.Instance.StopInspect();
                return;
            }

            Ray ray = MainCamera.ScreenPointToRay(mousePos);
            Physics.Raycast(ray, out RaycastHit hit, 1000f, _layerMask);

            if (hit.transform)
                OnHitGameObject(hit.transform.gameObject);
            else if (lastHitObject)
                MouseInspector.Instance.ClearHitData();
        }

        internal void OnHitGameObject(GameObject obj)
        {
            if (obj != lastHitObject)
            {
                lastHitObject = obj;
                MouseInspector.Instance.objNameLabel.text = $"<b>Click to Inspect:</b> <color=cyan>{obj.name}</color>";
                MouseInspector.Instance.objPathLabel.text = $"Path: {obj.transform.GetTransformPath(true)}";
            }
        }

        public override void OnEndInspect()
        {
            // not needed
        }
    }
}
