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
    public class ToggleVisualElement : CustomUIElement
    {
        public override string ElementContainerClassName => "toggleContainer";
        public override string LabelClassName => "attributeName";
        public override string ValueFieldClassName => "toggleNumberField";

        private Toggle toggleField;

        public ToggleVisualElement(HEU_ParameterData parmData, int folderID, VisualElement parentContainer, HEU_HoudiniAsset houdiniAsset)
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
            elementLabel = new Label()
            {
                name = parmData._labelName.ToString()
            };
            elementLabel.text = parmData._labelName.ToString();
            elementLabel.AddToClassList(LabelClassName);

            toggleField = new Toggle();
            toggleField.name = "toggleNumberField";
            toggleField.AddToClassList(ValueFieldClassName);
            toggleField.value = parmData._toggle;
            if (parmData._parmInfo.disabled)
            {
                toggleField.SetEnabled(false);
            }
        }
        public override void SetChangeEvent()
        {
            toggleField.RegisterCallback<ChangeEvent<bool>>((evt) =>
            {
                string paramName = parmData._name.ToString();
                HEU_ParameterUtility.SetToggle(houdiniAsset, paramName, toggleField.value);
                houdiniAsset.RequestCook(true, false, true, true);
                AssetDatabase.Refresh();
                HoudiniEngineCustomUI_Main.SettingsChanged = true;

            });

        }

        public override void AddFieldToContainer()
        {
            elementContainer.Add(elementLabel);
            elementContainer.Add(toggleField);
        }

    }
}
