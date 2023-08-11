﻿using UnityExplorer.Config;
using UniverseLib.UI;
using UniverseLib.UI.Models;

namespace UnityExplorer.CacheObject.IValues
{
    public class InteractiveString : InteractiveValue
    {
        private string RealValue;
        public string EditedValue = "";

        public InputFieldRef inputField;
        public ButtonRef ApplyButton;

        public GameObject SaveFileRow;
        public InputFieldRef SaveFilePath;

        public override void OnBorrowed(CacheObjectBase owner)
        {
            base.OnBorrowed(owner);

            bool canWrite = owner.CanWrite && owner.State != ValueState.Exception;
            inputField.Component.readOnly = !canWrite;
            ApplyButton.Component.gameObject.SetActive(canWrite);

            SaveFilePath.Text = Path.Combine(ConfigManager.Default_Output_Path.Value, "untitled.txt");
        }

        private bool IsStringTooLong(string s)
        {
            if (s == null)
                return false;

            return s.Length >= UniversalUI.MAX_INPUTFIELD_CHARS;
        }

        public override void SetValue(object value)
        {
            if (CurrentOwner.State == ValueState.Exception)
                value = CurrentOwner.LastException.ToString();

            RealValue = value as string;
            SaveFileRow.SetActive(IsStringTooLong(RealValue));

            if (value == null)
            {
                inputField.Text = "";
                EditedValue = "";
            }
            else
            {
                EditedValue = (string)value;
                inputField.Text = EditedValue;
            }
        }

        private void OnApplyClicked()
        {
            CurrentOwner.SetUserValue(EditedValue);
        }

        private void OnInputChanged(string input)
        {
            EditedValue = input;
            SaveFileRow.SetActive(IsStringTooLong(EditedValue));
        }

        private void OnSaveFileClicked()
        {
            if (RealValue == null)
                return;

            if (string.IsNullOrEmpty(SaveFilePath.Text))
            {
                ExplorerCore.LogWarning("Cannot save an empty file path!");
                return;
            }

            string path = IOUtility.EnsureValidFilePath(SaveFilePath.Text);

            if (File.Exists(path))
                File.Delete(path);

            File.WriteAllText(path, RealValue);
        }

        public override GameObject CreateContent(GameObject parent)
        {
            UIRoot = UIFactory.CreateVerticalGroup(parent, "InteractiveString", false, false, true, true, 3, new Vector4(4, 4, 4, 4),
                new Color(0.06f, 0.06f, 0.06f));

            // Save to file helper

            SaveFileRow = UIFactory.CreateUIObject("SaveFileRow", UIRoot);
            UIFactory.SetLayoutElement(SaveFileRow, flexibleWidth: 9999);
            UIFactory.SetLayoutGroup<VerticalLayoutGroup>(SaveFileRow, false, true, true, true, 3);

            UIFactory.CreateLabel(SaveFileRow, "Info", "<color=red>String is too long! Save to file if you want to see the full string.</color>",
                TextAnchor.MiddleLeft);

            GameObject horizRow = UIFactory.CreateUIObject("Horiz", SaveFileRow);
            UIFactory.SetLayoutGroup<HorizontalLayoutGroup>(horizRow, false, false, true, true, 4);

            ButtonRef saveButton = UIFactory.CreateButton(horizRow, "SaveButton", "Save file");
            UIFactory.SetLayoutElement(saveButton.Component.gameObject, minHeight: 25, minWidth: 100, flexibleWidth: 0);
            saveButton.OnClick += OnSaveFileClicked;

            SaveFilePath = UIFactory.CreateInputField(horizRow, "SaveInput", "...");
            UIFactory.SetLayoutElement(SaveFilePath.UIRoot, minHeight: 25, flexibleWidth: 9999);

            // Main Input / apply

            ApplyButton = UIFactory.CreateButton(UIRoot, "ApplyButton", "Apply", new Color(0.2f, 0.27f, 0.2f));
            UIFactory.SetLayoutElement(ApplyButton.Component.gameObject, minHeight: 25, minWidth: 100, flexibleWidth: 0);
            ApplyButton.OnClick += OnApplyClicked;

            inputField = UIFactory.CreateInputField(UIRoot, "InputField", "empty");
            inputField.UIRoot.AddComponent<ContentSizeFitter>().verticalFit = ContentSizeFitter.FitMode.PreferredSize;
            UIFactory.SetLayoutElement(inputField.UIRoot, minHeight: 25, flexibleHeight: 500, flexibleWidth: 9999);
            inputField.Component.lineType = InputField.LineType.MultiLineNewline;
            inputField.OnValueChanged += OnInputChanged;

            return UIRoot;
        }

    }
}
