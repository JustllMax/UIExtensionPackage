using UIExtensionPackage.ExtendedUI.CustomUIElements;
using UnityEditor;
using UnityEditor.UI;
using UnityEngine;

[CustomEditor(typeof(CustomContentSizeFitter))]
[CanEditMultipleObjects]
public class CustomContentSizeFitterEditor : ContentSizeFitterEditor
{
    private SerializedProperty _useHorizontalConstraints;
    private SerializedProperty _horizontalConstraints;
    private SerializedProperty _useVerticalConstraints;
    private SerializedProperty _verticalConstraints;

    protected override void OnEnable()
    {
        base.OnEnable();
        _useHorizontalConstraints = serializedObject.FindProperty("_useHorizontalConstraints");
        _horizontalConstraints = serializedObject.FindProperty("_horizontalConstraints");
        _useVerticalConstraints = serializedObject.FindProperty("_useVerticalConstraints");
        _verticalConstraints = serializedObject.FindProperty("_verticalConstraints");
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        
        serializedObject.Update();

        EditorGUI.BeginChangeCheck(); 

        EditorGUILayout.PropertyField(_useHorizontalConstraints, new GUIContent("Use Horizontal Constraints"));
        if (_useHorizontalConstraints.boolValue)
        {
            EditorGUILayout.PropertyField(_horizontalConstraints, new GUIContent("Horizontal Constraints"));
        }

        EditorGUILayout.Space();

        EditorGUILayout.PropertyField(_useVerticalConstraints, new GUIContent("Use Vertical Constraints"));
        if (_useVerticalConstraints.boolValue)
        {
            EditorGUILayout.PropertyField(_verticalConstraints, new GUIContent("Vertical Constraints"));
        }

        serializedObject.ApplyModifiedProperties();

        if (EditorGUI.EndChangeCheck())
        {
            foreach (var t in targets)
            {
                var fitter = t as CustomContentSizeFitter;
                fitter?.SetLayouts();
            }
        }
    }
}
