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
    public class FolderVisualElement : CustomUIElement
    {
        public override string ElementContainerClassName => "none";
        public override string LabelClassName => "none";
        public override string ValueFieldClassName => "none";

        private string firstFolderClassName = "FirstFolders";

        private string secondFolderClassName = "SecondFolders";

        private string houdiniOnlyPrefix;
        private bool folderState;

        private Foldout newContainer;
        public FolderVisualElement(HEU_ParameterData parmData, int folderID, VisualElement parentContainer, HEU_HoudiniAsset houdiniAsset, string houdiniOnlyPrefix, bool folderState)
        {
            this.parmData = parmData;
            this.folderID = folderID;
            this.parentContainer = parentContainer;
            this.houdiniAsset = houdiniAsset;
            this.houdiniOnlyPrefix = houdiniOnlyPrefix;
            this.folderState = folderState;
        }
        public override void SetUpElementContainer()
        {
            newContainer = new Foldout();
            newContainer.text = parmData._labelName.ToString();

            newContainer.value = folderState;
            // Choose folder class
            if (parentContainer.ClassListContains(firstFolderClassName))
            {
                newContainer.AddToClassList(secondFolderClassName);
            }
            else
            {
                newContainer.AddToClassList(firstFolderClassName);
            }

            string folderName = parmData._labelName.Replace(" ", "");
            newContainer.name = folderName;
            if (HoudiniEngineCustomUI_Main.FoldersGroups.ContainsKey(folderID) == false)
                HoudiniEngineCustomUI_Main.FoldersGroups.Add(folderID, newContainer);
            if (parmData._name.StartsWith(houdiniOnlyPrefix) == false)
            {
                parentContainer.Add(newContainer);
            }
        }

        public override void SetUpElementField()
        {

        }
        public override void SetChangeEvent()
        {
            newContainer.RegisterValueChangedCallback((evt) =>
            {
                

                if( evt.target.GetType()== newContainer.GetType())
                {

                    ValueChanged();
                }
                
            }
           
            
            ); 

        }

        public override void AddFieldToContainer()
        {

        }

        private void ValueChanged()
        {
            for (int i = 0; i < HoudiniEngineCustomUI_Main.FolderNameList.Count; i++)
            {
                if (HoudiniEngineCustomUI_Main.FolderNameList.ElementAt(i).ToString() == parmData._name)
                {
                    if (newContainer.value == true)
                    {
                        folderState = true;
                        HoudiniEngineCustomUI_Main.FolderValueList[i] = true;
                    }
                    else
                    {
                        folderState = false;
                        HoudiniEngineCustomUI_Main.FolderValueList[i] = false;
                    }

                }
            }


        }



    }
}
