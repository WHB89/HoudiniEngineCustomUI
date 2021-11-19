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
    public class MultiparmVisualElement : CustomUIElement
    {
        public override string ElementContainerClassName => "Multiparms";
        public override string LabelClassName => "attributeName";
        public override string ValueFieldClassName => "toggleNumberField";

        private string folderHeadlineClassName = "FolderHeadline";
        private string mainMultiparmInstancesContainerClassName = "mainMultiparmInstancesContainer";
        private string firstMultiparmInstanceClassName = "FirstMultiparmInstance";
        private string secondMultiparmInstanceClassName = "SecondMultiparmInstance";
        private Button addButton;
        private Button clearButton;
        private Button multiAddButton;
        private Button sortButton;
        private Button removeUnusedButton;

        private VisualElement mainMultiparmInstancesContainer;
        private List<HEU_ParameterData> parms;
        private AssetUI assetUI;
        public MultiparmVisualElement(AssetUI assetUI,List<HEU_ParameterData> parms, HEU_ParameterData parmData, int folderID, VisualElement parentContainer, HEU_HoudiniAsset houdiniAsset)
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

            //remove all multiparm instances
            clearButton = new Button();
            clearButton.text = "Clear button";

            //Add multiparm instances by wallID count
            multiAddButton = new Button();
            multiAddButton.text = "Auto IDs";


            //Sort wall IDs
            sortButton = new Button();
            sortButton.text = "Sort IDs";

            //Remove unused wallIDs
            removeUnusedButton = new Button();
            removeUnusedButton.text = "Remove unused IDs";


            mainMultiparmInstancesContainer = new VisualElement();
            mainMultiparmInstancesContainer.name = "mainMultiparmContainer";
            mainMultiparmInstancesContainer.AddToClassList(mainMultiparmInstancesContainerClassName);

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

                mainMultiparmInstancesContainer.Add(multiparmInstanceContainer);

            }
            //multiparm parameters
            List<int> childParmList = parmData._childParameterIDs;
            // Create paramater UI
            for (int multParmChild = 0; multParmChild < childParmList.Count; multParmChild++)
            {
                VisualElement multParmContainer = mainMultiparmInstancesContainer.ElementAt(parms[childParmList[multParmChild]]._parmInfo.instanceNum - 1);
                int multparmFolderID = parms[childParmList[multParmChild]].ParmID;
                
                if (parms[childParmList[multParmChild]]._parmInfo.type == HAPI_ParmType.HAPI_PARMTYPE_FOLDER || parms[childParmList[multParmChild]]._parmInfo.type == HAPI_ParmType.HAPI_PARMTYPE_FOLDERLIST)
                {
                    assetUI.SetupParameters(parms[childParmList[multParmChild]], multparmFolderID+1, multParmContainer);

                }
                else
                {
                    assetUI.SetupParameters(parms[childParmList[multParmChild]], multparmFolderID, multParmContainer);

                }

            }

        }
        public override void SetChangeEvent()
        {
            addButton.clickable.clicked += () =>
            {
                houdiniAsset.Parameters.InsertInstanceToMultiParm(parmData._unityIndex, parmData._parmInfo.instanceStartOffset, 1);
                houdiniAsset.RequestCook(true, false, true, true);
                AssetDatabase.Refresh();
                HoudiniEngineCustomUI_Main.SettingsChanged = true;
            };

            clearButton.clickable.clicked += () =>
            {
                houdiniAsset.Parameters.ClearInstancesFromMultiParm(parmData._unityIndex);
                houdiniAsset.RequestCook(true, false, true, true);
                AssetDatabase.Refresh();
                HoudiniEngineCustomUI_Main.SettingsChanged = true;
            };


        }

        public override void AddFieldToContainer()
        {
            elementContainer.Add(elementLabel);
            elementContainer.Add(addButton);
            elementContainer.Add(clearButton);
            elementContainer.Add(mainMultiparmInstancesContainer);
        }






    }
}
