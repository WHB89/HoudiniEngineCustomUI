using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using HoudiniEngineUnity;
using System;
using System.Collections.Generic;
// Typedefs (copy these from HEU_Common.cs)



namespace HoudiniEngineCustomUI
{

    public static class CustomUI_DataManager
    {
        public static void ReloadSettings(VisualElement assetSettings)
        {
            assetSettings.Clear();
            if (HoudiniEngineCustomUI_Main.FoldersGroups.Count > 0)
            {
                HoudiniEngineCustomUI_Main.FoldersGroups.Clear();
            }
            HoudiniEngineCustomUI_Main.FoldersGroups.Add(-1, assetSettings);
            VisualElement assetSettingsHeadline = new Label("Asset settings");
            assetSettingsHeadline.name = "MainHeadline";
            assetSettings.Add(assetSettingsHeadline);
            HoudiniEngineCustomUI_Main.AssetUI.GetSettings(HoudiniEngineCustomUI_Main.HoudiniAsset);
        }


    }
}
