using UniverseLib.UI;
using UniverseLib.UI.Panels;

namespace UnityExplorer.UI
{
    internal class ExplorerUIBase : UIBase
    {
        
        public ExplorerUIBase(string id, Action updateMethod) : base(id, updateMethod) { }

        protected override PanelManager CreatePanelManager()
        {
            return new UEPanelManager(this);
        }
    }
    
    internal class ExplorerOverlayUIBase : UIBase
    {
        public ExplorerOverlayUIBase(string id, Action updateMethod) : base(id, updateMethod) { }

        protected override PanelManager CreatePanelManager()
        {
            return new UEPanelManager(this);
        }
    }

    internal class OverlayPanelManager : PanelManager
    {
        public OverlayPanelManager(UIBase owner) : base(owner)
        { }
    }
}
