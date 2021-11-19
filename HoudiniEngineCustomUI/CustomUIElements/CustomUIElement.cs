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
    public abstract class CustomUIElement 
    {
        public abstract string ElementContainerClassName  { get; }
        public abstract string LabelClassName { get;  }
        public abstract string ValueFieldClassName { get;  }

        protected HEU_ParameterData parmData;
        protected int folderID;
        protected VisualElement parentContainer;
        protected HEU_HoudiniAsset houdiniAsset;

        protected VisualElement elementContainer;
        protected Label elementLabel;

        public virtual void GenerateElement()
        {
            SetUpElementContainer();
            SetUpElementField();
            SetChangeEvent();
            AddFieldToContainer();
        }
        public abstract void SetUpElementContainer();

        public abstract void SetUpElementField();

        public abstract void SetChangeEvent();

        public abstract void AddFieldToContainer();
    }

}
