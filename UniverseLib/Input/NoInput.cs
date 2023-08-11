﻿using UnityEngine;
using UnityEngine.EventSystems;

namespace UniverseLib.Input
{
    // Just a stub for games where no Input module was able to load at all.

    public class NoInput : IHandleInput
    {
        public Vector2 MousePosition => Vector2.zero;
        public Vector2 MouseScrollDelta => Vector2.zero;

        public bool GetKey(KeyCode key) => false;
        public bool GetKeyDown(KeyCode key) => false;
        public bool GetKeyUp(KeyCode key) => false;

        public bool GetMouseButton(int btn) => false;
        public bool GetMouseButtonDown(int btn) => false;
        public bool GetMouseButtonUp(int btn) => false;

        public void ResetInputAxes() { }

        public BaseInputModule UIInputModule => null;
        public void ActivateModule() { }
        public void AddUIInputModule() { }
    }
}