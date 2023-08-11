﻿using UnityExplorer.UI.Panels;
using UniverseLib.UI.ObjectPool;

namespace UnityExplorer.Inspectors
{
    public abstract class InspectorBase : IPooledObject
    {
        public bool IsActive { get; internal set; }
        public object Target { get; set; }
        public Type TargetType { get; protected set; }

        public InspectorTab Tab { get; internal set; }

        public GameObject UIRoot { get; set; }

        public float DefaultHeight => -1f;
        public abstract GameObject CreateContent(GameObject parent);

        public abstract void Update();

        public abstract void CloseInspector();

        public virtual void OnBorrowedFromPool(object target)
        {
            this.Target = target;
            this.TargetType = target is Type type ? type : target.GetActualType();

            Tab = Pool<InspectorTab>.Borrow();
            Tab.UIRoot.transform.SetParent(InspectorPanel.Instance.NavbarHolder.transform, false);

            Tab.TabButton.OnClick += OnTabButtonClicked;
            Tab.CloseButton.OnClick += CloseInspector;
        }

        public virtual void OnReturnToPool()
        {
            Pool<InspectorTab>.Return(Tab);

            this.Target = null;

            Tab.TabButton.OnClick -= OnTabButtonClicked;
            Tab.CloseButton.OnClick -= CloseInspector;
        }

        public virtual void OnSetActive()
        {
            Tab.SetTabColor(true);
            UIRoot.SetActive(true);
            IsActive = true;
            LayoutRebuilder.ForceRebuildLayoutImmediate(UIRoot.GetComponent<RectTransform>());
        }

        public virtual void OnSetInactive()
        {
            Tab.SetTabColor(false);
            UIRoot.SetActive(false);
            IsActive = false;
        }

        private void OnTabButtonClicked()
        {
            InspectorManager.SetInspectorActive(this);
        }
    }
}
