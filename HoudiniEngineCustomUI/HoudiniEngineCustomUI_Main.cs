using HoudiniEngineUnity;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;


// Typedefs (copy these from HEU_Common.cs)

namespace HoudiniEngineCustomUI
{
    public class HoudiniEngineCustomUI_Main : EditorWindow
    {

        public static ObjectField assetInputField;

        public static AssetUI AssetUI;
        public static bool SettingsChanged = false;
        public static bool SliderSettingsChanged = false;
        public static HEU_HoudiniAsset HoudiniAsset;

        ////Dictonary for folders in asset
        public static Dictionary<int, VisualElement> FoldersGroups = new Dictionary<int, VisualElement>();

        public static VisualElement AssetSettingsParent;

        public static List<string> FolderNameList = new List<string>();
        public static List<bool> FolderValueList = new List<bool>();


        [MenuItem("Window/HoudiniEngineCustomUI")]
        public static void ShowUI()
        {
            HoudiniEngineCustomUI_Main wnd = GetWindow<HoudiniEngineCustomUI_Main>();
            wnd.titleContent = new GUIContent("HoudiniEngineCustomUI");
        }

        public void OnEnable()
        {
            // Each editor window contains a root VisualElement object
            VisualElement root = rootVisualElement;

            // Import UXML
            VisualTreeAsset visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/Editor/HoudiniEngineCustomUI/HoudiniEngineCustomUI_Main.uxml");
            VisualElement uxmlRoot = visualTree.CloneTree();
            root.Add(uxmlRoot);

            StyleSheet styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/Editor/HoudiniEngineCustomUI/HoudiniEngineCustomUI_Main.uss");
            root.styleSheets.Add(styleSheet);

            VisualElement mainSettingsParent = rootVisualElement.Q<VisualElement>("MainSettings");
            AssetSettingsParent = rootVisualElement.Q<VisualElement>("AssetSettings");

            SpecialInputsPrefixes specialPrefixes = new SpecialInputsPrefixes(mainSettingsParent);

            assetInputField = root.Query<ObjectField>("AssetInput").First();
            assetInputField.objectType = typeof(GameObject);
            CustomUI_StandardEvents.SetupAssetFieldEvent(assetInputField, AssetSettingsParent, specialPrefixes);

            mainSettingsParent.Add(assetInputField);


            //Reload settings
            Button reloadBtn = rootVisualElement.Q<Button>("RecookBtn");
            CustomUI_StandardEvents.SetupReloadButtonEvent(reloadBtn, assetInputField, AssetSettingsParent, AssetUI);

            //LoadPreset
            Button LoadPresetBtn = rootVisualElement.Q<Button>("LoadPresetBtn");
            CustomUI_StandardEvents.SetupLoadPresetButtonEvent(LoadPresetBtn, AssetSettingsParent, AssetUI);

            //Save  Preset
            Button SavePresetBtn = rootVisualElement.Q<Button>("SavePresetBtn");
            CustomUI_StandardEvents.SetupSavePresetButtonEvent(SavePresetBtn, AssetSettingsParent, AssetUI);

            //Rebuild asset
            Button rebuildBtn = rootVisualElement.Q<Button>("RebuildBtn");
            CustomUI_StandardEvents.SetupRebuildButtonEvent(rebuildBtn, assetInputField, AssetSettingsParent, AssetUI);

            //Switch to edit mode if available
            Button editModeBtn = rootVisualElement.Q<Button>("EditModeBtn");
            CustomUI_StandardEvents.SetupEditModeSwitchEvent(editModeBtn);

            Scroller uxmlField = root.Q<Scroller>("the-uxml-scroller");
            SetupScroller(root, uxmlField);

            AssetSettingsParent.RegisterCallback<MouseMoveEvent>(OnMouseMove);


        }

        private void SetupScroller(VisualElement root, Scroller uxmlField)
        {
            if (uxmlField != null)
            {
                uxmlField.valueChanged += (v) => { };
                uxmlField.value = 42;

                // Create a new scroller, disable it, and give it a style class.
                Scroller csharpField = new Scroller(0, 100, (v) => { }, SliderDirection.Vertical);

                csharpField.AddToClassList("some-styled-scroller");
                csharpField.value = uxmlField.value;
                root.Add(csharpField);

                // Mirror value of uxml scroller into the C# field.
                CustomUI_StandardEvents.SetupScrollerEvent(uxmlField, csharpField);

                ScrollView view = root.Q<ScrollView>("ScrollView");
                view.style.marginRight = -55;
                view.verticalScroller.slider.style.paddingLeft = 0;
                view.verticalScroller.slider.style.unitySliceLeft = 14;
            }
        }

        private void OnMouseMove(MouseMoveEvent e)
        {
            //Reload settings when Mouse is not down anymore(not the best solution but it works)
            if (SliderSettingsChanged)
            {
                CustomUI_DataManager.ReloadSettings(AssetSettingsParent);
                SliderSettingsChanged = false;
            }
        }
        void Update()
        {
            CheckForChanges();
        }

        public void CheckForChanges()
        {
            if (SettingsChanged)
            {
                SettingsChanged = false;
                CustomUI_DataManager.ReloadSettings(AssetSettingsParent);
            }
        }
    }

}

