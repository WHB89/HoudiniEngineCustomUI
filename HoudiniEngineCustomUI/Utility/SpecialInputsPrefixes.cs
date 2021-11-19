using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using HoudiniEngineUnity;
using System;
using System.Collections.Generic;


namespace HoudiniEngineCustomUI
{
    public struct SpecialInputsPrefixes
    {
        public string MaterialPrefix;
        public string StringAsObjectPrefix;
        public string HdaPrefix;
        public string HoudiniOnlyPrefix;
        public string RampPrefix;

        public SpecialInputsPrefixes(VisualElement mainSettingsParent)
        {

            MaterialPrefix = mainSettingsParent.Q<TextField>("Material_Prefix").text;
            StringAsObjectPrefix = mainSettingsParent.Q<TextField>("StringAsObj_Prefix").text;
            HdaPrefix = mainSettingsParent.Q<TextField>("HDA_Prefix").text;
            HoudiniOnlyPrefix = mainSettingsParent.Q<TextField>("HoudiniOnly_Prefix").text;
            RampPrefix = mainSettingsParent.Q<TextField>("Ramp_Prefix").text;

            mainSettingsParent.Q<TextField>("Material_Prefix").SetEnabled(false);
            mainSettingsParent.Q<TextField>("StringAsObj_Prefix").SetEnabled(false);
            mainSettingsParent.Q<TextField>("HDA_Prefix").SetEnabled(false);
            mainSettingsParent.Q<TextField>("HoudiniOnly_Prefix").SetEnabled(false);
            mainSettingsParent.Q<TextField>("Ramp_Prefix").SetEnabled(false);

        }
    }
}
