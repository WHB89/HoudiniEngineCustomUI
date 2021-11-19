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
    public class IntVisualElement : CustomUIElement
    {
        public override string ElementContainerClassName => "intContainer";
        public override string LabelClassName => "attributeName";
        public override string ValueFieldClassName => "none";

        private string numberFieldClassName = "numberField";
        private string sliderClassName = "slider";

        private IntegerField intNumberField;
        private SliderInt intSlider;
        // Set min/max
        private int maxValue = 10;
        private int minValue = 0;

        public IntVisualElement(HEU_ParameterData parmData, int folderID, VisualElement parentContainer, HEU_HoudiniAsset houdiniAsset)
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

            intNumberField = new IntegerField();
            intNumberField.name = "IntNumberField";
            intNumberField.AddToClassList(numberFieldClassName);
            intNumberField.value = parmData._intValues[0];
            if (parmData._parmInfo.disabled)
            {
                intNumberField.SetEnabled(false);
            }


            if (parmData._parmInfo.hasUIMax)
            {
                maxValue = (int)parmData._parmInfo.max;
            }
            if (parmData._parmInfo.hasUIMin)
            {
                minValue = (int)parmData._parmInfo.min;
            }
            intSlider = new SliderInt((int)parmData._parmInfo.UIMin, (int)parmData._parmInfo.UIMax);
            intSlider.name = "IntSlider";
            intSlider.value = parmData._intValues[0];
            if (parmData._parmInfo.disabled)
            {
                intSlider.SetEnabled(false);
            }
            intSlider.AddToClassList(sliderClassName);
            intSlider.highValue = (int)parmData._parmInfo.UIMax;
            intSlider.lowValue = minValue;
            
        }
        public override void SetChangeEvent()
        {
            intNumberField.RegisterCallback<ChangeEvent<int>>((evt) =>
            {
                if (evt.newValue > maxValue && parmData._parmInfo.hasMax)
                    intSlider.highValue = evt.newValue;
                if (evt.newValue < minValue && parmData._parmInfo.hasMin)
                    intSlider.lowValue = evt.newValue;
                intSlider.value = evt.newValue;
                string paramName = parmData._name.ToString();

                HEU_ParameterUtility.SetInt(houdiniAsset, paramName, intNumberField.value);
                houdiniAsset.RequestCook(true, false, true, true);
                AssetDatabase.Refresh();
                HoudiniEngineCustomUI_Main.SliderSettingsChanged = true;
            });
            intSlider.RegisterCallback<ChangeEvent<int>>((evt) =>
            {
                intNumberField.value = evt.newValue;
            });
        }

        public override void AddFieldToContainer()
        {
            elementContainer.Add(elementLabel);
            elementContainer.Add(intNumberField);
            elementContainer.Add(intSlider);
        }

        


    }
}
