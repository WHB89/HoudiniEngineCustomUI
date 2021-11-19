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
    public class MaterialVisualElement : CustomUIElement
    {
        public override string ElementContainerClassName => "allInputContainer";
        public override string LabelClassName => "attributeName";
        public override string ValueFieldClassName => "inputField";

        private ObjectField materialField;

        public MaterialVisualElement(HEU_ParameterData parmData, int folderID, VisualElement parentContainer, HEU_HoudiniAsset houdiniAsset)
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

            materialField = new ObjectField();
            materialField.objectType = typeof(UnityEngine.Material);
            materialField.name = "materialField";
            materialField.AddToClassList(ValueFieldClassName);
            Material currentMaterial = (Material)AssetDatabase.LoadAssetAtPath(parmData._stringValues[0], typeof(Material));
            if (currentMaterial != null)
            {
                materialField.value = currentMaterial;
            }

            if (parmData._parmInfo.disabled)
            {
                materialField.SetEnabled(false);
            }

        }
        public override void SetChangeEvent()
        {
            materialField.RegisterCallback<ChangeEvent<UnityEngine.Object>>((evt) =>
            {
                string paramName = parmData._name.ToString();
                Material material = (Material)materialField.value;
                string materialPath = AssetDatabase.GetAssetPath(material);
                HEU_ParameterUtility.SetString(houdiniAsset, paramName, materialPath);

                houdiniAsset.RequestCook(true, false, true, true);
                AssetDatabase.Refresh();
                HoudiniEngineCustomUI_Main.SettingsChanged = true;
            });
        }
        public override void AddFieldToContainer()
        {
            elementContainer.Add(elementLabel);
            elementContainer.Add(materialField);
        }

    }
}
