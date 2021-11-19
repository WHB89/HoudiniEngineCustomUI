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
using UnityEditor.VersionControl;

namespace HoudiniEngineCustomUI
{

    public class AssetUI 
    {
        
        SpecialInputsPrefixes specialPrefixes;
        public AssetUI(SpecialInputsPrefixes specialPrefixes)
        {

            this.specialPrefixes = specialPrefixes;
        }
        public void GetSettings(HEU_HoudiniAsset hAsset)
        {
            if(hAsset != null)
            {
                List<HEU_ParameterData> parms = hAsset.Parameters.GetParameters();
                int index = 0;
                Dictionary<int, int> parmIndexList = new Dictionary<int, int>();
                foreach (HEU_ParameterData parmData in parms)
                {
                    index += 1;
                    int parentID = parmData.ParentID;
                    int folderID = parmData.ParmID;
                    parmIndexList.Add(parmData.ParmID, index);

                    if (HoudiniEngineCustomUI_Main.FoldersGroups.ContainsKey(parentID))
                    {
                        
                        VisualElement targetContainer = HoudiniEngineCustomUI_Main.FoldersGroups[parentID];


                        if (parmData._parmInfo.type == HAPI_ParmType.HAPI_PARMTYPE_MULTIPARMLIST)
                        {

                            if (parmData._name.StartsWith(specialPrefixes.RampPrefix))
                            {
                                CreateUIElement(new RampVisualElement(this, parms, parmData, folderID, targetContainer, hAsset));
                            }
                            else
                            {
                                
                                CreateUIElement(new MultiparmVisualElement(this, parms, parmData, folderID, targetContainer, hAsset));
                            }
                        }
                        else
                        {
                            
                            if (!parmData._parmInfo.isChildOfMultiParm || parms[parmIndexList[parentID]-1]._parmInfo.isChildOfMultiParm)//check if parentfolder exists
                            {                                    
                                SetupParameters(parmData, folderID, targetContainer);
                            }
                        }
                    }





                }
            }
            
        }

        public void SetupParameters(HEU_ParameterData parmData, int folderID, VisualElement parentContainer)
        {
            
            if (parmData._parmInfo.type == HAPI_ParmType.HAPI_PARMTYPE_FOLDER  || parmData._parmInfo.type == HAPI_ParmType.HAPI_PARMTYPE_FOLDERLIST )
            {
                //Debug.Log("_labelName: " + parmData._labelName);
                int folderIndex = -1;
                bool folderOpen = false;
                for (int i = 0; i < HoudiniEngineCustomUI_Main.FolderNameList.Count; i++)
                {
                    if (HoudiniEngineCustomUI_Main.FolderNameList.ElementAt(i) == parmData._name)
                    {
                       // Debug.Log("_labelName: " + parmData._name);
                        folderIndex = i;
                    }
                }

                if (folderIndex == -1)
                {
                    HoudiniEngineCustomUI_Main.FolderNameList.Add(parmData._name);
                    HoudiniEngineCustomUI_Main.FolderValueList.Add(false);
                }
                else 
                {
                    
                    folderOpen = HoudiniEngineCustomUI_Main.FolderValueList[folderIndex];
                }

                //save folder name and state
                // get state if available
                //Debug.Log(parmData._labelName);
                if(parmData._parmInfo.isChildOfMultiParm ||parmData._labelName != "")
                {
                    CreateUIElement(new FolderVisualElement(parmData, folderID, parentContainer, HoudiniEngineCustomUI_Main.HoudiniAsset, specialPrefixes.HoudiniOnlyPrefix, folderOpen));
                }
            }
            if (parmData._name.StartsWith(specialPrefixes.HoudiniOnlyPrefix) == false)
            {
                if (parmData._parmInfo.type == HAPI_ParmType.HAPI_PARMTYPE_INT)
                {
                    //Setup menu
                    if (parmData._parmInfo.choiceCount > 0)
                    {
                        CreateUIElement(new MenuVisualElement(parmData, folderID, parentContainer, HoudiniEngineCustomUI_Main.HoudiniAsset));
                    }
                    else
                    {
                        CreateUIElement(new IntVisualElement(parmData, folderID, parentContainer, HoudiniEngineCustomUI_Main.HoudiniAsset));
                    }
                }
                else if (parmData._parmInfo.type == HAPI_ParmType.HAPI_PARMTYPE_BUTTON)
                {
                    CreateUIElement(new ButtonVisualElement(parmData, folderID, parentContainer, HoudiniEngineCustomUI_Main.HoudiniAsset));
                }
                else if (parmData._parmInfo.type == HAPI_ParmType.HAPI_PARMTYPE_FLOAT)
                {
                    CreateUIElement(new FloatsVisualElement(parmData, folderID, parentContainer, HoudiniEngineCustomUI_Main.HoudiniAsset));
                }
                else if (parmData._parmInfo.type == HAPI_ParmType.HAPI_PARMTYPE_SEPARATOR)
                {
                    CreateUIElement(new SeparatorVisualElement(parmData, folderID, parentContainer, HoudiniEngineCustomUI_Main.HoudiniAsset));
                }
                else if (parmData._parmInfo.type == HAPI_ParmType.HAPI_PARMTYPE_STRING)
                {
                    //Create Material input if parameter name contains the prefix
                    if (parmData._name.StartsWith(specialPrefixes.MaterialPrefix))
                    {
                        CreateUIElement(new MaterialVisualElement(parmData, folderID, parentContainer, HoudiniEngineCustomUI_Main.HoudiniAsset));
                    }
                    else if (parmData._name.StartsWith(specialPrefixes.StringAsObjectPrefix))
                    {
                        CreateUIElement(new StringAsObjectInputVisualElement(parmData, folderID, parentContainer, HoudiniEngineCustomUI_Main.HoudiniAsset));
                    }
                    else
                    {
                        CreateUIElement(new StringVisualElement(parmData, folderID, parentContainer, HoudiniEngineCustomUI_Main.HoudiniAsset));
                    }
                }
                else if (parmData._parmInfo.type == HAPI_ParmType.HAPI_PARMTYPE_TOGGLE)
                {
                    CreateUIElement(new ToggleVisualElement(parmData, folderID, parentContainer, HoudiniEngineCustomUI_Main.HoudiniAsset));
                }
                else if (parmData._parmInfo.type == HAPI_ParmType.HAPI_PARMTYPE_COLOR)
                {
                    CreateUIElement(new ColorVisualElement(parmData, folderID, parentContainer, HoudiniEngineCustomUI_Main.HoudiniAsset));
                }
                else if (parmData._parmInfo.type == HAPI_ParmType.HAPI_PARMTYPE_NODE)
                {
                    //Create HDA input if parameter name contains the prefix
                    if (parmData._name.StartsWith(specialPrefixes.HdaPrefix))
                    {
                        CreateUIElement(new HDAInputVisualElement(parmData, folderID, parentContainer, HoudiniEngineCustomUI_Main.HoudiniAsset));
                    }
                    else
                    {
                        CreateUIElement(new GeometryInputVisualElement(parmData, folderID, parentContainer, HoudiniEngineCustomUI_Main.HoudiniAsset));
                    }
                }
                else if (parmData._parmInfo.type == HAPI_ParmType.HAPI_PARMTYPE_PATH_FILE_IMAGE)
                {
                    CreateUIElement(new TextureInputVisualElement(parmData, folderID, parentContainer, HoudiniEngineCustomUI_Main.HoudiniAsset));
                }
                else if (parmData._parmInfo.type == HAPI_ParmType.HAPI_PARMTYPE_LABEL)
                {
                    CreateUIElement(new LabelVisualElement(parmData, folderID, parentContainer, HoudiniEngineCustomUI_Main.HoudiniAsset));
                }

            }
        }

        public void CreateUIElement(CustomUIElement customElement)
        {
            customElement.GenerateElement();
        }


    }
}
