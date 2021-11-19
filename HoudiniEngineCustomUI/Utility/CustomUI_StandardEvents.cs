using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using HoudiniEngineUnity;
using System;
using System.Collections.Generic;


// Typedefs (copy these from HEU_Common.cs)
using HAPI_NodeId = System.Int32;
using HAPI_PartId = System.Int32;
using PointerType = UnityEngine.UIElements.PointerType;
using UnityEditor.PackageManager.Requests;
using System.Linq;



namespace HoudiniEngineCustomUI
{


    public static class CustomUI_StandardEvents 
    {
        public static void SetupAssetFieldEvent(ObjectField assetField, VisualElement assetSettingsUI, SpecialInputsPrefixes specialPrefixes)
        {
            assetField.RegisterCallback<ChangeEvent<UnityEngine.Object>>(e =>
            {
                assetSettingsUI.Clear();
                if (HoudiniEngineCustomUI_Main.FoldersGroups.Count > 0)
                {
                    HoudiniEngineCustomUI_Main.FoldersGroups.Clear();
                }
                HoudiniEngineCustomUI_Main.FoldersGroups.Add(-1, assetSettingsUI);

                VisualElement assetSettingsHeadline = new Label("Asset settings");
                assetSettingsHeadline.name = "MainHeadline";
                assetSettingsUI.Add(assetSettingsHeadline);


                HoudiniEngineCustomUI_Main.HoudiniAsset = ((GameObject)assetField.value).GetComponent<HEU_HoudiniAssetRoot>() != null ? ((GameObject)assetField.value).GetComponent<HEU_HoudiniAssetRoot>()._houdiniAsset : null;
                HoudiniEngineCustomUI_Main.AssetUI = new AssetUI(specialPrefixes);
                HoudiniEngineCustomUI_Main.AssetUI.GetSettings(HoudiniEngineCustomUI_Main.HoudiniAsset);
            });
        }

       

        public static void SetupReloadButtonEvent(Button reloadBtn, ObjectField assetField, VisualElement assetSettingsUI , AssetUI assetUI)
        {
            reloadBtn.clickable.clicked += () =>
            {
                if (HoudiniEngineCustomUI_Main.HoudiniAsset != null)
                {
                    HoudiniEngineCustomUI_Main.HoudiniAsset.RequestCook(true, false, true, true);
                    AssetDatabase.Refresh();
                    //HouCustomUI.houdiniAsset = ((GameObject)assetField.value).GetComponent<HEU_HoudiniAssetRoot>() != null ? ((GameObject)assetField.value).GetComponent<HEU_HoudiniAssetRoot>()._houdiniAsset : null;
                    CustomUI_DataManager.ReloadSettings(assetSettingsUI);
                }
            };
        }
        public static void SetupRebuildButtonEvent(Button rebuildBtn, ObjectField assetField, VisualElement assetSettingsUI, AssetUI assetUI)
        {
            rebuildBtn.clickable.clicked += () =>
            {
                if (HoudiniEngineCustomUI_Main.HoudiniAsset != null)
                {
                    HoudiniEngineCustomUI_Main.HoudiniAsset.RequestReload(false);
                    AssetDatabase.Refresh();
                    CustomUI_DataManager.ReloadSettings(assetSettingsUI);

                }
            };
        }

        public static void SetupSavePresetButtonEvent(Button recookBtn, VisualElement assetSettingsUI, AssetUI assetUI)
        {
            recookBtn.clickable.clicked += () =>
            {
                if (HoudiniEngineCustomUI_Main.HoudiniAsset != null)
                {

                    string fileName = HoudiniEngineCustomUI_Main.HoudiniAsset.AssetName;
                    string filePattern = "preset";
                    string newPath = EditorUtility.SaveFilePanel("Save HDA preset", "", fileName + "." + filePattern, filePattern);
                    if (newPath != null && !string.IsNullOrEmpty(newPath))
                    {
                        HEU_AssetPresetUtility.SaveAssetPresetToFile(HoudiniEngineCustomUI_Main.HoudiniAsset, newPath);
                    }

                    HoudiniEngineCustomUI_Main.HoudiniAsset.RequestCook(true, false, true, true);
                    AssetDatabase.Refresh();
                    CustomUI_DataManager.ReloadSettings(assetSettingsUI);
                }
            };
        }


        public static void SetupLoadPresetButtonEvent(Button recookBtn, VisualElement assetSettingsUI, AssetUI assetUI)
        {
            recookBtn.clickable.clicked += () =>
            {
                if (HoudiniEngineCustomUI_Main.HoudiniAsset != null)
                {

                    string fileName = HoudiniEngineCustomUI_Main.HoudiniAsset.AssetName;
                    string filePattern = "preset";
                    string newPath = EditorUtility.OpenFilePanel("Load HDA preset", "", filePattern);
                    if (newPath != null && !string.IsNullOrEmpty(newPath))
                    {
                        HEU_AssetPresetUtility.LoadPresetFileIntoAssetAndCook(HoudiniEngineCustomUI_Main.HoudiniAsset, newPath);
                    }

                    HoudiniEngineCustomUI_Main.HoudiniAsset.RequestCook(true, false, true, true);
                    AssetDatabase.Refresh();
                    CustomUI_DataManager.ReloadSettings(assetSettingsUI);
                }
            };
        }

        

        public static void SetupEditModeSwitchEvent(Button editModeBtn)
        {
            
            editModeBtn.clickable.clicked += () =>
            {
                if (HoudiniEngineCustomUI_Main.HoudiniAsset != null)
                {
                    if (HoudiniEngineCustomUI_Main.HoudiniAsset.EditableNodesToolsEnabled == false)
                    {
                        Selection.activeGameObject = HoudiniEngineCustomUI_Main.HoudiniAsset.gameObject.transform.parent.gameObject;
                        HoudiniEngineCustomUI_Main.HoudiniAsset.EditableNodesToolsEnabled = true;
                    }
                    else
                    {
                        Selection.activeGameObject = null;
                        HoudiniEngineCustomUI_Main.HoudiniAsset.EditableNodesToolsEnabled = false;

                    }
                    HoudiniEngineCustomUI_Main.HoudiniAsset.RequestCook(false, false, false, false);
                    AssetDatabase.Refresh();
                }
            };
        }

        public static void SetupScrollerEvent(Scroller uxmlField, Scroller csharpField)
        {
            uxmlField.RegisterCallback<ChangeEvent<float>>((evt) =>
            {
                csharpField.value = evt.newValue;
            });
        }

    }
}
