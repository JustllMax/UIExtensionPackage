using UnityEditor;
using UnityEngine;
using UnityEditor.UI;
using UIExtensionPackage.ExtendedUI.CustomUIElements;

namespace UIExtensionPackage.ExtendedUI.Editor
{

    /// <summary>
    /// Represents custom editor for <see cref="CustomButton"/>
    /// </summary>
    [CustomEditor(typeof(CustomButton))]
    public class CustomButtonEditor : ButtonEditor
    {
        private SerializedProperty unselectAfterPressed;
        private SerializedProperty targetGraphics;
        protected override void OnEnable()
        {
            base.OnEnable();
            unselectAfterPressed = serializedObject.FindProperty(nameof(unselectAfterPressed));
            targetGraphics = serializedObject.FindProperty(nameof(targetGraphics));
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            CustomButton customButton = (CustomButton)target;

            if (GUILayout.Button("Set default transition"))
            {
                customButton.SetDefaultTransition();
                customButton.HandleVisuals();
            }
            
            if (GUILayout.Button("Set default settings"))
            {
                customButton.SetDefaultSettings();
                customButton.HandleVisuals();
            }
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Custom Button Properties", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(unselectAfterPressed, new GUIContent("Unselect after pressed"));
            
            //Draw target graphics list
            EditorGUILayout.PropertyField(targetGraphics, new GUIContent("Target Graphics"), true);

            EditorGUILayout.Space();

            serializedObject.ApplyModifiedProperties();

            //Call base inspector 
            base.OnInspectorGUI();
        }
    }
}