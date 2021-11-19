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
    public class TextureInputVisualElement : CustomUIElement
    {
        public override string ElementContainerClassName => "allInputContainer";
        public override string LabelClassName => "attributeName";
        public override string ValueFieldClassName => "textureField";

        private ObjectField textureField;

        public TextureInputVisualElement(HEU_ParameterData parmData, int folderID, VisualElement parentContainer, HEU_HoudiniAsset houdiniAsset)
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

            textureField = new ObjectField();
            textureField.objectType = typeof(Texture2D);
            textureField.name = "textureInput";
            textureField.AddToClassList(ValueFieldClassName);

            Texture2D currentTexture = (Texture2D)AssetDatabase.LoadAssetAtPath(parmData._stringValues[0], typeof(Texture2D));
            if (currentTexture != null)
            {
                textureField.value = currentTexture;
            }

            if (parmData._parmInfo.disabled)
            {
                textureField.SetEnabled(false);
            }
        }

        public override void SetChangeEvent()
        {
            textureField.RegisterCallback<ChangeEvent<UnityEngine.Object>>((evt) =>
            {
                string paramName = parmData._name.ToString();
                Texture2D texture = (Texture2D)textureField.value;
                string texturePath = AssetDatabase.GetAssetPath(texture);
                HEU_ParameterUtility.SetString(houdiniAsset, paramName, texturePath);

                houdiniAsset.RequestCook(true, false, true, true);
                AssetDatabase.Refresh();
                HoudiniEngineCustomUI_Main.SettingsChanged = true;
            });
        }

        public override void AddFieldToContainer()
        {
            elementContainer.Add(elementLabel);
            elementContainer.Add(textureField);
        }

    }
}
