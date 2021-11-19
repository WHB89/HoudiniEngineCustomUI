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
    public class SeparatorVisualElement : CustomUIElement
    {
        public override string ElementContainerClassName => "separatorContainer";
        public override string LabelClassName => "none";
        public override string ValueFieldClassName => "none";

        public SeparatorVisualElement(HEU_ParameterData parmData, int folderID, VisualElement parentContainer, HEU_HoudiniAsset houdiniAsset)
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
            parentContainer.Add(elementContainer);
        }

        public override void SetUpElementField()
        {

        }
        public override void SetChangeEvent()
        {

        }

        public override void AddFieldToContainer()
        {

        }

        
    }
}
