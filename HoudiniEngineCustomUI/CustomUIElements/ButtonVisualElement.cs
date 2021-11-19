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
    public class ButtonVisualElement : CustomUIElement
    {
        public override string ElementContainerClassName => "bContainer";
        public override string LabelClassName => "none";
        public override string ValueFieldClassName => "customButtonField";

        private Button actionButton;

        public ButtonVisualElement(HEU_ParameterData parmData, int folderID, VisualElement parentContainer, HEU_HoudiniAsset houdiniAsset)
        {
            this.parmData = parmData;
            this.folderID = folderID;
            this.parentContainer = parentContainer;
            this.houdiniAsset = houdiniAsset;
        }
        public override void SetUpElementContainer()
        {
            elementContainer = new VisualElement();
            elementContainer.AddToClassList(ElementContainerClassName);
            string folderName = parmData._labelName.Replace(" ", "");
            elementContainer.name = folderName;
            if (HoudiniEngineCustomUI_Main.FoldersGroups.ContainsKey(folderID) == false)
                HoudiniEngineCustomUI_Main.FoldersGroups.Add(folderID, elementContainer);
            parentContainer.Add(elementContainer);
        }

        public override void SetUpElementField()
        {
            actionButton = new Button();
            actionButton.text = parmData._labelName.ToString();
            actionButton.name = "buttonField";

            parmData._intValues[0] = 0;

            actionButton.AddToClassList(ValueFieldClassName);
            if (parmData._parmInfo.disabled)
            {
                actionButton.SetEnabled(false);
            }
        }
        public override void SetChangeEvent()
        {

            actionButton.clickable.clicked += () =>
            {

                string paramName = parmData._name.ToString();



                parmData._intValues[0] = 1;
               
                houdiniAsset.RequestCook(true, false, true, true);
                
                AssetDatabase.Refresh();

                HoudiniEngineCustomUI_Main.SettingsChanged = true;




            };

        }

        public override void AddFieldToContainer()
        {
            elementContainer.Add(actionButton);
        }

    }
}
