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
    public class MenuVisualElement : CustomUIElement
    {
        public override string ElementContainerClassName => "menuContainer";
        public override string LabelClassName => "attributeName";
        public override string ValueFieldClassName => "MenuField";

        private PopupField<string> menuField;

        public MenuVisualElement(HEU_ParameterData parmData, int folderID, VisualElement parentContainer, HEU_HoudiniAsset houdiniAsset)
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

            //List for menu options
            var choices = new List<string> { };

            //Fill menu list
            for (int choice = 0; choice < parmData._parmInfo.choiceCount; choice++)
            {
                choices.Add(parmData._choiceLabels[choice].text);
            }

            // Menu UI
            menuField = new PopupField<string>("", choices, parmData._choiceValue);
            menuField.AddToClassList(ValueFieldClassName);
            if (parmData._parmInfo.disabled)
            {
                menuField.SetEnabled(false);
            }
        }

        public override void SetChangeEvent()
        {
            // Change event
            menuField.RegisterCallback<ChangeEvent<string>>((evt) =>
            {
                string paramName = parmData._name.ToString();
                HEU_ParameterUtility.SetInt(houdiniAsset, paramName, menuField.index);
                houdiniAsset.RequestCook(true, false, true, true);
                AssetDatabase.Refresh();
                HoudiniEngineCustomUI_Main.SettingsChanged = true;

            });
        }

        public override void AddFieldToContainer()
        {
            elementContainer.Add(elementLabel);
            elementContainer.Add(menuField);
        }



     



    }
}
