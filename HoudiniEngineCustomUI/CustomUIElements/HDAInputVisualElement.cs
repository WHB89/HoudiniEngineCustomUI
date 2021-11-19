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
    public class HDAInputVisualElement : CustomUIElement
    {
        public override string ElementContainerClassName => "allInputContainer";
        public override string LabelClassName => "attributeName";
        public override string ValueFieldClassName => "inputField";

        private ObjectField inputField;

        public HDAInputVisualElement(HEU_ParameterData parmData, int folderID, VisualElement parentContainer, HEU_HoudiniAsset houdiniAsset)
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

            inputField = new ObjectField();
            inputField.objectType = typeof(HEU_HoudiniAssetRoot);
            inputField.name = "inputField";

            if (parmData._paramInputNode.GetConnectedInputCount() == 1)
            {
                HEU_ParameterUtility.GetInputNode(houdiniAsset, parmData._name.ToString(), 0, out GameObject currentInput);
                inputField.value = currentInput;
            }

            inputField.AddToClassList(ValueFieldClassName);

            if (parmData._parmInfo.disabled)
            {
                inputField.SetEnabled(false);
            }
        }
        public override void SetChangeEvent()
        {
            inputField.RegisterCallback<ChangeEvent<UnityEngine.Object>>((evt) =>
            {
                string paramName = parmData._name.ToString();

                parmData._paramInputNode.PendingInputObjectType = HEU_InputNode.InputObjectType.HDA;
                parmData._paramInputNode.RemoveAllInputEntries();

                HEU_ParameterUtility.SetInputNode(houdiniAsset, paramName, GameObject.Find(inputField.value.name), 0);
                houdiniAsset.RequestCook(true, false, true, true);
                AssetDatabase.Refresh();
                HoudiniEngineCustomUI_Main.SettingsChanged = true;
            });
        }

        public override void AddFieldToContainer()
        {
            elementContainer.Add(elementLabel);
            elementContainer.Add(inputField);
        }

    }
}
