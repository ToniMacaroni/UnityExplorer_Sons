﻿using UniverseLib.UI;
using UniverseLib.UI.Models;

namespace UnityExplorer.CacheObject.IValues
{
    public class InteractiveValueStruct : InteractiveValue
    {
        #region Struct cache / wrapper

        public class StructInfo
        {
            public bool IsSupported;
            public FieldInfo[] Fields;

            public StructInfo(bool isSupported, FieldInfo[] fields)
            {
                IsSupported = isSupported;
                Fields = fields;
            }

            public void SetValue(object instance, string input, int fieldIndex)
            {
                FieldInfo field = Fields[fieldIndex];

                object val;
                if (field.FieldType == typeof(string))
                    val = input;
                else
                {
                    if (!ParseUtility.TryParse(input, field.FieldType, out val, out Exception ex))
                    {
                        ExplorerCore.LogWarning("Unable to parse input!");
                        if (ex != null) ExplorerCore.Log(ex.ReflectionExToString());
                        return;
                    }
                }

                field.SetValue(instance, val);
            }

            public string GetValue(object instance, int fieldIndex)
            {
                FieldInfo field = Fields[fieldIndex];
                object value = field.GetValue(instance);
                return ParseUtility.ToStringForInput(value, field.FieldType);
            }
        }

        private static readonly Dictionary<string, StructInfo> typeSupportCache = new();

        private const BindingFlags INSTANCE_FLAGS = BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public;
        private const string SYSTEM_VOID = "System.Void";

        public static bool SupportsType(Type type)
        {
            if (!type.IsValueType || string.IsNullOrEmpty(type.AssemblyQualifiedName) || type.FullName == SYSTEM_VOID)
                return false;

            if (typeSupportCache.TryGetValue(type.AssemblyQualifiedName, out StructInfo info))
                return info.IsSupported;

            bool supported = false;

            FieldInfo[] fields = type.GetFields(INSTANCE_FLAGS);
            if (fields.Length > 0)
            {
                if (fields.Any(it => !ParseUtility.CanParse(it.FieldType)))
                {
                    supported = false;
                    info = new StructInfo(supported, null);
                }
                else
                {
                    supported = true;
                    info = new StructInfo(supported, fields);
                }
            }

            typeSupportCache.Add(type.AssemblyQualifiedName, info);

            return supported;
        }

        #endregion

        public object RefInstance;

        public StructInfo CurrentInfo;
        private Type lastStructType;

        private ButtonRef applyButton;
        private readonly List<GameObject> fieldRows = new();
        private readonly List<InputFieldRef> inputFields = new();
        private readonly List<Text> labels = new();

        public override void OnBorrowed(CacheObjectBase owner)
        {
            base.OnBorrowed(owner);

            applyButton.Component.gameObject.SetActive(owner.CanWrite);
        }

        // Setting value from owner to this

        public override void SetValue(object value)
        {
            RefInstance = value;

            Type type = RefInstance.GetType();

            if (type != lastStructType)
            {
                CurrentInfo = typeSupportCache[type.AssemblyQualifiedName];
                SetupUIForType();
                lastStructType = type;
            }

            for (int i = 0; i < CurrentInfo.Fields.Length; i++)
            {
                inputFields[i].Text = CurrentInfo.GetValue(RefInstance, i);
            }
        }

        private void OnApplyClicked()
        {
            try
            {
                for (int i = 0; i < CurrentInfo.Fields.Length; i++)
                {
                    CurrentInfo.SetValue(RefInstance, inputFields[i].Text, i);
                }

                CurrentOwner.SetUserValue(RefInstance);
            }
            catch (Exception ex)
            {
                ExplorerCore.LogWarning("Exception setting value: " + ex);
            }
        }

        // UI Setup for type

        private void SetupUIForType()
        {
            for (int i = 0; i < CurrentInfo.Fields.Length || i <= inputFields.Count; i++)
            {
                if (i >= CurrentInfo.Fields.Length)
                {
                    if (i >= inputFields.Count)
                        break;

                    fieldRows[i].SetActive(false);
                    continue;
                }

                if (i >= inputFields.Count)
                    AddEditorRow();

                fieldRows[i].SetActive(true);

                string label = SignatureHighlighter.Parse(CurrentInfo.Fields[i].FieldType, false);
                label += $" <color={SignatureHighlighter.FIELD_INSTANCE}>{CurrentInfo.Fields[i].Name}</color>:";
                labels[i].text = label;
            }
        }

        private void AddEditorRow()
        {
            GameObject row = UIFactory.CreateUIObject("HoriGroup", UIRoot);
            //row.AddComponent<ContentSizeFitter>().horizontalFit = ContentSizeFitter.FitMode.PreferredSize;
            UIFactory.SetLayoutElement(row, minHeight: 25, flexibleWidth: 9999);
            UIFactory.SetLayoutGroup<HorizontalLayoutGroup>(row, false, false, true, true, 8, childAlignment: TextAnchor.MiddleLeft);

            fieldRows.Add(row);

            Text label = UIFactory.CreateLabel(row, "Label", "notset", TextAnchor.MiddleLeft);
            UIFactory.SetLayoutElement(label.gameObject, minHeight: 25, minWidth: 50, flexibleWidth: 0);
            label.horizontalOverflow = HorizontalWrapMode.Wrap;
            labels.Add(label);

            InputFieldRef input = UIFactory.CreateInputField(row, "InputField", "...");
            UIFactory.SetLayoutElement(input.UIRoot, minHeight: 25, minWidth: 200);
            ContentSizeFitter fitter = input.UIRoot.AddComponent<ContentSizeFitter>();
            fitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
            fitter.horizontalFit = ContentSizeFitter.FitMode.PreferredSize;
            input.Component.lineType = InputField.LineType.MultiLineNewline;
            inputFields.Add(input);
        }

        // UI Construction

        public override GameObject CreateContent(GameObject parent)
        {
            UIRoot = UIFactory.CreateVerticalGroup(parent, "InteractiveValueStruct", false, false, true, true, 3, new Vector4(4, 4, 4, 4),
                new Color(0.06f, 0.06f, 0.06f), TextAnchor.MiddleLeft);
            UIFactory.SetLayoutElement(UIRoot, minHeight: 25, flexibleWidth: 9999);

            applyButton = UIFactory.CreateButton(UIRoot, "ApplyButton", "Apply", new Color(0.2f, 0.27f, 0.2f));
            UIFactory.SetLayoutElement(applyButton.Component.gameObject, minHeight: 25, minWidth: 175);
            applyButton.OnClick += OnApplyClicked;

            return UIRoot;
        }
    }
}
