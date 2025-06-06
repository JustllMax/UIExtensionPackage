using UnityEditor;
using UnityEngine;
using UnityEditor.UI;
using UIExtensionPackage.ExtendedUI.CustomUIElements;



// ReSharper disable CheckNamespace
namespace UIExtensionPackage.ExtendedUI.Editor
{

    /// <summary>
    /// Represents custom editor for <see cref="CustomDropdown"/>
    /// </summary>
    [CustomEditor(typeof(CustomDropdown))]
    public class CustomDropdownEditor : DropdownEditor
    {

        private SerializedProperty _unselectAfterPressed;
        private SerializedProperty _targetGraphics;

        protected override void OnEnable()
        {
            base.OnEnable();
            _unselectAfterPressed = serializedObject.FindProperty(nameof(_unselectAfterPressed));
            _targetGraphics = serializedObject.FindProperty(nameof(_targetGraphics));
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            CustomDropdown customDropdown = (CustomDropdown)target;

            if (GUILayout.Button("Set default transition"))
            {
                customDropdown.SetDefaultTransition();
                customDropdown.HandleVisuals();
            }

            if (GUILayout.Button("Set default settings"))
            {
                customDropdown.SetDefaultSettings();
                customDropdown.HandleVisuals();
            }

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Custom Button Properties", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(_unselectAfterPressed, new GUIContent("Unselect after pressed"));

            //Draw target graphics list
            EditorGUILayout.PropertyField(_targetGraphics, new GUIContent("Target Graphics"), true);

            EditorGUILayout.Space();

            serializedObject.ApplyModifiedProperties();

            //Call base inspector 
            base.OnInspectorGUI();


        }
    }
}