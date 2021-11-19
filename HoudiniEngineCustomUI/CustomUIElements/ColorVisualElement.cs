
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
    public class ColorVisualElement : CustomUIElement
    {
        public override string ElementContainerClassName => "colorContainer";
        public override string LabelClassName => "attributeName";
        public override string ValueFieldClassName => "colorField";

        private ColorField colorField;

        public ColorVisualElement(HEU_ParameterData parmData, int folderID, VisualElement parentContainer, HEU_HoudiniAsset houdiniAsset)
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

            colorField = new ColorField();
            colorField.showAlpha = false;
            if (parmData._parmInfo.size == 4)
            {
                colorField.showAlpha = true;
            }
            colorField.name = "colorField";
            colorField.AddToClassList(ValueFieldClassName);
            colorField.value = parmData._color;
            if (parmData._parmInfo.disabled)
            {
                colorField.SetEnabled(false);
            }
        }
        public override void SetChangeEvent()
        {
            colorField.RegisterCallback<ChangeEvent<Color>>((evt) =>
            {
                string paramName = parmData._name.ToString();
                HEU_ParameterUtility.SetColor(houdiniAsset, paramName, colorField.value);
                houdiniAsset.RequestCook(true, false, true, true);
                AssetDatabase.Refresh();

                HoudiniEngineCustomUI_Main.SettingsChanged = true;
            });
        }

        public override void AddFieldToContainer()
        {
            elementContainer.Add(elementLabel);
            elementContainer.Add(colorField);
        }
    }
}

