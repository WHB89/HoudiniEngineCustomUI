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
    public class RampVisualElement : CustomUIElement
    {
        public override string ElementContainerClassName => "Multiparms";
        public override string LabelClassName => "attributeName";
        public override string ValueFieldClassName => "toggleNumberField";
        private string folderHeadlineClassName = "FolderHeadline";

        private string rampInstancesContainerClassName = "mainMultiparmInstancesContainer";
        private string firstMultiparmInstanceClassName = "FirstMultiparmInstance";
        private string secondMultiparmInstanceClassName = "SecondMultiparmInstance";
        private string curveElementClassName = "CurveField";

        private Button addButton;
        private CurveField rampCurve;
        private VisualElement rampContainer;
        private List<HEU_ParameterData> parms;
        private AssetUI assetUI;
        private Keyframe[] keys;
        private List<int> childParmList; 

        public RampVisualElement(AssetUI assetUI,List<HEU_ParameterData> parms, HEU_ParameterData parmData, int folderID, VisualElement parentContainer, HEU_HoudiniAsset houdiniAsset)
        {
            this.parmData = parmData;
            this.folderID = folderID;
            this.parentContainer = parentContainer;
            this.houdiniAsset = houdiniAsset;
            this.parms = parms;
            this.assetUI = assetUI;
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
            elementLabel.AddToClassList(folderHeadlineClassName);


            //Add single multiparm instance
            addButton = new Button();
            addButton.text = "Add button";

           
            rampContainer = new VisualElement();
            rampContainer.name = "mainRampContainer";
            rampContainer.AddToClassList(rampInstancesContainerClassName);

            int multParmInstanceCount = parmData._parmInfo.instanceCount;

            //Create container and remove button for each multiparm instance
            for (int instance = 0; instance < multParmInstanceCount; instance++)
            {
                VisualElement multiparmInstanceContainer = new VisualElement();
                if (instance % 2 == 0)
                {
                    multiparmInstanceContainer.AddToClassList(firstMultiparmInstanceClassName);
                }
                else
                {
                    multiparmInstanceContainer.AddToClassList(secondMultiparmInstanceClassName);
                }

                string multParmInstanceName = parmData._labelName.Replace(" ", "");

                //remove button for multiparm instance
                Button removeButton = new Button();
                int numberExtension = instance + 1;
                removeButton.name = "removeButton_" + numberExtension;
                removeButton.text = "Remove button";
                removeButton.clickable.clicked += () =>
                {
                    int index = Int32.Parse(removeButton.name.Replace("removeButton_", ""));
                    houdiniAsset.Parameters.RemoveInstancesFromMultiParm(parmData._unityIndex, index, 1);
                    houdiniAsset.RequestCook(true, false, true, true);
                    AssetDatabase.Refresh();
                    HoudiniEngineCustomUI_Main.SettingsChanged = true;
                };
                multiparmInstanceContainer.Add(removeButton);
                rampContainer.Add(multiparmInstanceContainer);

            }
            //multiparm parameters
            childParmList = parmData._childParameterIDs;
            keys = new Keyframe[childParmList.Count];

            List<float> positionList = new List<float>();
            List<float> valueList = new List<float>();
            List<int> interpolatiionList = new List<int>();
            // Create parameter UI
            for (int multParmChild = 0; multParmChild < childParmList.Count; multParmChild++)
            {
                if(parms[childParmList[multParmChild]]._labelName == "Position")
                {
                   positionList.Add(parms[childParmList[multParmChild]]._floatValues[0]);
                }

                if (parms[childParmList[multParmChild]]._labelName == "Value")
                {
                    valueList.Add(parms[childParmList[multParmChild]]._floatValues[0]);
                }

                if (parms[childParmList[multParmChild]]._labelName == "Interpolation")
                {
                    interpolatiionList.Add(parms[childParmList[multParmChild]]._intValues[0]);
                }


                if (parms[childParmList[multParmChild]]._labelName != "Interpolation")
                {
                    VisualElement multParmContainer = rampContainer.ElementAt(parms[childParmList[multParmChild]]._parmInfo.instanceNum - 1);
                    int multparmFolderID = parms[childParmList[multParmChild]].ParmID;
                    assetUI.SetupParameters(parms[childParmList[multParmChild]], multparmFolderID, multParmContainer);
                }
                
            }

            for (int keyIndex = 0; keyIndex < positionList.Count; keyIndex++)
            {
                    keys[keyIndex] = new Keyframe(positionList[keyIndex], valueList[keyIndex], 1.0f, 1.0f, 0.0f, 0.0f);// interpolatiionList[keyIndex], interpolatiionList[keyIndex],1,1);
            }
            // Create a new field, disable it, and give it a style class.
            rampCurve = new CurveField();
            rampCurve.ranges = new Rect(0, 0, 1, 1);
            rampCurve.AddToClassList(curveElementClassName);
            rampCurve.value = new AnimationCurve(keys);
            rampCurve.SetEnabled(false);


        }
        public override void SetChangeEvent()
        {
            addButton.clickable.clicked += () =>
            {
                houdiniAsset.Parameters.InsertInstanceToMultiParm(parmData._unityIndex, parmData._parmInfo.instanceCount+1, 1);
                houdiniAsset.RequestCook(true, false, true, true);
                AssetDatabase.Refresh();
                HoudiniEngineCustomUI_Main.SettingsChanged = true;
            };

            // Mirror value of uxml field into the C# field.
            rampCurve.RegisterCallback<ChangeEvent<AnimationCurve>>((evt) =>
            {
                
            });

        }

        public override void AddFieldToContainer()
        {
            elementContainer.Add(elementLabel);
            elementContainer.Add(addButton);
            elementContainer.Add(rampCurve);
            elementContainer.Add(rampContainer);
        }






    }
}
