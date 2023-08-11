﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace UniverseLib.UI.Models
{
    /// <summary>
    /// An abstract UI object which does not exist as an actual UI Component, but which may be a reference to one.
    /// </summary>
    public abstract class UIModel
    {
        public abstract GameObject UIRoot { get; }

        public bool Enabled
        {
            get => UIRoot && UIRoot.activeInHierarchy;
            set
            {
                if (!UIRoot || Enabled == value)
                    return;
                UIRoot.SetActive(value);
                OnToggleEnabled?.Invoke(value);
            }
        }

        public event Action<bool> OnToggleEnabled;

        public abstract void ConstructUI(GameObject parent);

        public virtual void Toggle() => SetActive(!Enabled);

        public virtual void SetActive(bool active)
        {
            UIRoot?.SetActive(active);
        }

        public virtual void Destroy()
        {
            if (UIRoot)
                GameObject.Destroy(UIRoot);
        }
    }
}
