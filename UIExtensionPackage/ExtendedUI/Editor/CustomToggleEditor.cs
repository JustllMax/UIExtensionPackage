using UIExtensionPackage.ExtendedUI.CustomUIElements;
using UnityEditor;
using UnityEngine;
using UnityEditor.UI;

namespace UIExtensionPackage.ExtendedUI.Editor
{
    
    /// <summary>
    /// Represents custom editor for <see cref="CustomToggle"/>
    /// </summary>
    [CustomEditor(typeof(CustomToggle))]
    public class CustomToggleEditor : ToggleEditor
    {
        private SerializedProperty targetGraphics;
        private SerializedProperty displayData;
        protected override void OnEnable()
        {
            base.OnEnable();
            targetGraphics = serializedObject.FindProperty(nameof(targetGraphics));
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            
            CustomToggle customToggle = (CustomToggle)target;
            
            if (GUILayout.Button("Set default transition"))
            {
                customToggle.SetDefaultTransition();
                customToggle.HandleVisuals();
            }
            if (GUILayout.Button("Set default settings"))
            {
                customToggle.SetDefaultSettings();
                customToggle.HandleVisuals();
            }
            
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Custom Toggle Properties", EditorStyles.boldLabel);
            
            //Draw target graphics list
            EditorGUILayout.PropertyField(targetGraphics, new GUIContent("Target Graphics"), true);

            EditorGUILayout.Space();


            
            

            serializedObject.ApplyModifiedProperties();

            //Call base inspector 
            base.OnInspectorGUI();
        }
    }
}