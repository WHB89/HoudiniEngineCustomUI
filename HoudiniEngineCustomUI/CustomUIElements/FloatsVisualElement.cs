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
    public class FloatsVisualElement : CustomUIElement
    {
        public override string ElementContainerClassName => "floatContainer";
        public override string LabelClassName => "attributeName";
        public override string ValueFieldClassName => "none";

        private string floatSubContainerClassName = "floatSubContainer";

        private string numberFieldClassName = "numberField";

        private string sliderClassName = "slider";

        private FloatField floatNumberField_x;
        private FloatField floatNumberField_y;
        private FloatField floatNumberField_z;
        private FloatField floatNumberField_w;

        private Slider floatSlider_x;
        private Slider floatSlider_y;
        private Slider floatSlider_z;
        private Slider floatSlider_w;

        private VisualElement floatSubContainer_x;
        private VisualElement floatSubContainer_y;
        private VisualElement floatSubContainer_z;
        private VisualElement floatSubContainer_w;

        // Set min/max
        float minValue = 0.0f;
        float maxValue = 1.0f;
        int tupleSize = 0;
        public FloatsVisualElement(HEU_ParameterData parmData, int folderID, VisualElement parentContainer, HEU_HoudiniAsset houdiniAsset)
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
            
            //Setup floats or vectors
            tupleSize = parmData._parmInfo.size;


            // Create UI  for each value
            for (int tuple = 0; tuple < tupleSize; tuple++)
            {
                int index = 0;
                string namePostFix = "";
                if (tuple == 0 && tupleSize > 1)
                {
                    namePostFix = "_x";
                    index = 0;
                }
                if (tuple == 1)
                {
                    namePostFix = "_y";
                    index = 1;
                }
                if (tuple == 2)
                {
                    namePostFix = "_z";
                    index = 2;
                }
                if (tuple == 3)
                {
                    namePostFix = "_w";
                    index = 3;
                }


                elementLabel = new Label()
                {
                    name = parmData._labelName.ToString()
                };
                elementLabel.text = parmData._labelName.ToString() + namePostFix;
                elementLabel.AddToClassList(LabelClassName);


                if (index == 0 )//&& tupleSize > 1)
                {
                    //floatNumberField_x = new FloatField();
                    SetupFloatContainer(tuple, index, namePostFix, out floatNumberField_x, out floatSlider_x, out floatSubContainer_x);
                }
                if (index == 1)
                {
                    SetupFloatContainer(tuple, index, namePostFix, out floatNumberField_y, out floatSlider_y, out floatSubContainer_y);
                }
                if (index == 2)
                {
                    SetupFloatContainer(tuple, index, namePostFix, out floatNumberField_z, out floatSlider_z, out floatSubContainer_z);
                }
                if (index == 3)
                {
                    SetupFloatContainer(tuple, index, namePostFix, out floatNumberField_w, out floatSlider_w, out floatSubContainer_w);
                }
            }
        }

        public override void SetChangeEvent()
        {
            if (tupleSize >= 1)
            {
                // X
                //Change events - numberField and  slider react to each other
                floatNumberField_x.RegisterCallback<ChangeEvent<float>>((evt) =>
                {
                    UpdateValue(evt, floatSlider_x, floatNumberField_x);
                });
                floatSlider_x.RegisterCallback<ChangeEvent<float>>((evt) =>
                {
                    floatNumberField_x.value = evt.newValue;
                });
            }
            if (tupleSize >= 2)
            {
                // Y
                //Change events - numberField and  slider react to each other
                floatNumberField_y.RegisterCallback<ChangeEvent<float>>((evt) =>
                {
                    UpdateValue(evt, floatSlider_y, floatNumberField_y);
                });

                floatSlider_y.RegisterCallback<ChangeEvent<float>>((evt) =>
                {
                    floatNumberField_y.value = evt.newValue;
                });
            }
            if (tupleSize >= 3)
            {
                // Z
                //Change events - numberField and  slider react to each other
                floatNumberField_z.RegisterCallback<ChangeEvent<float>>((evt) =>
                {
                    UpdateValue(evt, floatSlider_z, floatNumberField_z);
                });

                floatSlider_z.RegisterCallback<ChangeEvent<float>>((evt) =>
                {
                    floatNumberField_z.value = evt.newValue;
                });
            }
            if (tupleSize >= 4)
            {
                // W
                //Change events - numberField and  slider react to each other
                floatNumberField_w.RegisterCallback<ChangeEvent<float>>((evt) =>
                {
                    UpdateValue(evt, floatSlider_w, floatNumberField_w);
                });

                floatSlider_w.RegisterCallback<ChangeEvent<float>>((evt) =>
                {
                    floatNumberField_w.value = evt.newValue;
                });
            }

        }

        private void SetupFloatContainer(int tuple, int index, string namePostFix, out FloatField floatNumberField, out Slider floatSlider, out VisualElement floatContainer)
        {
            floatNumberField = new FloatField();
            floatNumberField.name = "FloatNumberField_" + index;
            floatNumberField.AddToClassList(numberFieldClassName);
            floatNumberField.value = parmData._floatValues[tuple];
            if (parmData._parmInfo.disabled)
            {
                floatNumberField.SetEnabled(false);
            }

            // Set min/max

            if (parmData._parmInfo.hasUIMax)
            {
                maxValue = parmData._parmInfo.max;
            }
            if (parmData._parmInfo.hasUIMin)
            {
                minValue = parmData._parmInfo.min;
            }


            floatSlider = new Slider(parmData._parmInfo.UIMin, parmData._parmInfo.UIMax);
            floatSlider.name = "FloatSlider_" + namePostFix;
            floatSlider.value = parmData._floatValues[tuple];
            if (parmData._parmInfo.disabled)
            {
                floatSlider.SetEnabled(false);
            }
            floatSlider.AddToClassList(sliderClassName);
            floatSlider.highValue = maxValue;
            floatSlider.lowValue = minValue;

            floatContainer = new VisualElement();
            floatContainer.AddToClassList(floatSubContainerClassName);

            floatContainer.Add(elementLabel);
            floatContainer.Add(floatNumberField);
            floatContainer.Add(floatSlider);
        }

        private void UpdateValue(ChangeEvent<float> evt, Slider floatSlider, FloatField floatNumberField)
        {
            if (evt.newValue > maxValue && parmData._parmInfo.hasMax)
                floatSlider.highValue = evt.newValue;
            if (evt.newValue < minValue && parmData._parmInfo.hasMin)
                floatSlider.lowValue = evt.newValue;
            floatSlider.value = evt.newValue;
            HEU_ParameterUtility.SetFloat(houdiniAsset, parmData._name, floatNumberField.value);
            houdiniAsset.RequestCook(true, false, true, true);
            AssetDatabase.Refresh();
            HoudiniEngineCustomUI_Main.SliderSettingsChanged = true;
        }

        public override void AddFieldToContainer()
        {
            if(tupleSize == 1)
            {
                elementContainer.Add(floatSubContainer_x);
            }
            if (tupleSize == 2)
            {
                elementContainer.Add(floatSubContainer_x);
                elementContainer.Add(floatSubContainer_y);
            }
            if (tupleSize == 3)
            {
                elementContainer.Add(floatSubContainer_x);
                elementContainer.Add(floatSubContainer_y);
                elementContainer.Add(floatSubContainer_z);
            }
            if (tupleSize == 4)
            {
                elementContainer.Add(floatSubContainer_x);
                elementContainer.Add(floatSubContainer_y);
                elementContainer.Add(floatSubContainer_z);
                elementContainer.Add(floatSubContainer_w);
            }

            

        }
    }
}
