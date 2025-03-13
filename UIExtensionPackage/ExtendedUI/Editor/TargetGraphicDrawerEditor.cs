using UIExtensionPackage.ExtendedUI.Base;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace UIExtensionPackage.ExtendedUI.Editor
{
    /// <summary>
    /// Represents a custom property drawer for <see cref="TargetGraphicData"/>
    /// </summary>
    [CustomPropertyDrawer(typeof(TargetGraphicData))]
    public class TargetGraphicDataDrawer : PropertyDrawer
    {
        private static readonly string ColorsFoldoutKey = "TargetGraphicDataDrawer_showColors_";
        private static readonly string SpritesFoldoutKey = "TargetGraphicDataDrawer_showSprites_";
        private static readonly string AnimationsFoldoutKey = "TargetGraphicDataDrawer_showAnimations_";

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);
            
            //Set basic spacing for properties
            float lineHeight = EditorGUIUtility.singleLineHeight;
            float spacing = 4f;

            // Reserve space for each line
            Rect currentRect = new Rect(position.x, position.y, position.width, lineHeight);
            
            // Target Graphic
            SerializedProperty targetGraphicProp = property.FindPropertyRelative("targetGraphic");
            EditorGUI.LabelField(currentRect, new GUIContent("Target Graphic"), EditorStyles.boldLabel);
            EditorGUI.PropertyField(currentRect, targetGraphicProp, new GUIContent(" "));
            currentRect.y += lineHeight + spacing;

            // Transition
            SerializedProperty transitionProp = property.FindPropertyRelative("transition");
            EditorGUI.PropertyField(currentRect, transitionProp, new GUIContent("Transition"));
            currentRect.y += lineHeight + spacing;

            // Get current transition type
            Selectable.Transition transitionType = (Selectable.Transition)transitionProp.enumValueIndex;

            
            // Animation transition
            if (transitionType == Selectable.Transition.Animation)
            {
                string propertyPath = property.propertyPath;
                bool showAnimations = SessionState.GetBool(AnimationsFoldoutKey + propertyPath, true);
                
                showAnimations = EditorGUI.Foldout(currentRect, showAnimations, "Colors", true);
                SessionState.SetBool(AnimationsFoldoutKey + propertyPath, showAnimations);

                currentRect.y += lineHeight + spacing;

                if (showAnimations)
                {
                    DrawProperty(ref currentRect, property, "normalTrigger", "Normal Trigger");
                    DrawProperty(ref currentRect, property, "highlightedTrigger", "Highlight Trigger");
                    DrawProperty(ref currentRect, property, "pressedTrigger", "Pressed Trigger");
                    DrawProperty(ref currentRect, property, "selectedTrigger", "Selected Trigger");
                    DrawProperty(ref currentRect, property, "disabledTrigger", "Disabled Trigger");
                }
            }

            // Color transition
            if (transitionType == Selectable.Transition.ColorTint)
            {
                string propertyPath = property.propertyPath;
                bool showColors = SessionState.GetBool(ColorsFoldoutKey + propertyPath, true);

                showColors = EditorGUI.Foldout(currentRect, showColors, "Colors", true);
                SessionState.SetBool(ColorsFoldoutKey + propertyPath, showColors);

                currentRect.y += lineHeight + spacing;

                if (showColors)
                {
                    DrawProperty(ref currentRect, property, "normalColor", "Normal Color");
                    DrawProperty(ref currentRect, property, "highlightedColor", "Highlight Color");
                    DrawProperty(ref currentRect, property, "pressedColor", "Pressed Color");
                    DrawProperty(ref currentRect, property, "selectedColor", "Selected Color");
                    DrawProperty(ref currentRect, property, "disabledColor", "Disabled Color");
                }
            }
            
            
            // Sprite swap transition
            if (transitionType == Selectable.Transition.SpriteSwap )
            {
                // Draw sprite references
                if (targetGraphicProp.objectReferenceValue is Image)
                {
                    string propertyPath = property.propertyPath;
                    bool showSprites = SessionState.GetBool(SpritesFoldoutKey + propertyPath, true);

                    showSprites = EditorGUI.Foldout(currentRect, showSprites, "Sprites", true);
                    SessionState.SetBool(SpritesFoldoutKey + propertyPath, showSprites);

                    currentRect.y += lineHeight + spacing;

                    if (showSprites)
                    {
                        DrawProperty(ref currentRect, property, "normalSprite", "Normal Sprite");
                        DrawProperty(ref currentRect, property, "highlightedSprite", "Highlighted Sprite");
                        DrawProperty(ref currentRect, property, "pressedSprite", "Pressed Sprite");
                        DrawProperty(ref currentRect, property, "selectedSprite", "Selected Sprite");
                        DrawProperty(ref currentRect, property, "disabledSprite", "Disabled Sprite");
                    }
                }
                // Draw info warning
                else
                {   
                    GUIStyle bigBoxStyle = new GUIStyle(EditorStyles.helpBox)
                    {
                        fontSize = 14, 
                        fontStyle = FontStyle.Bold, 
                        padding = new RectOffset(10, 10, 10, 10) // Add extra padding for a bigger look
                    };

                    Rect boxRect = new Rect(currentRect.x, currentRect.y, currentRect.width, lineHeight * 3); // Bigger box
                    EditorGUI.LabelField(boxRect, "⚠ Sprite Swap transition requires an Image!", bigBoxStyle);
                    //EditorGUI.HelpBox(currentRect, "Sprite Swap transition requires an Image.", MessageType.Warning);
                }
            }

            //currentRect.y += (lineHeight + spacing) * 2;
            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            float lineHeight = EditorGUIUtility.singleLineHeight;
            float spacing = 4f;
            float bottomMargin = 20f;

            float height = 2 * (lineHeight + spacing); // TargetGraphic and Transition

            SerializedProperty transitionProp = property.FindPropertyRelative("transition");
            Selectable.Transition transitionType = (Selectable.Transition)transitionProp.enumValueIndex;
            
            SerializedProperty targetGraphicProp = property.FindPropertyRelative("targetGraphic");

            string propertyPath = property.propertyPath;
            // Whether the foldouts are shown
            bool showColors = SessionState.GetBool(ColorsFoldoutKey + propertyPath, true);
            bool showSprites = SessionState.GetBool(SpritesFoldoutKey + propertyPath, true);
            bool showAnimations = SessionState.GetBool(AnimationsFoldoutKey + propertyPath, true);

            if (transitionType == Selectable.Transition.ColorTint && showColors)
            {
                height += (5 * (lineHeight + spacing)); // Five color fields
            }

            if (transitionType == Selectable.Transition.Animation && showAnimations)
            {
                height += (5 * (lineHeight + spacing)); // Five string fields
            }
            
            if (transitionType == Selectable.Transition.SpriteSwap)
            {
                if (targetGraphicProp.objectReferenceValue is Image)
                {
                    if (showSprites)
                    {
                        height += (5 * (lineHeight + spacing)); // Five sprite fields
                    }
                }
                else
                {
                    height += lineHeight *2; // Just enough space for the warning message
                }
            }
            return height + bottomMargin;
        }

        private void DrawProperty(ref Rect position, SerializedProperty property, string fieldName, string label)
        {
            SerializedProperty colorProp = property.FindPropertyRelative(fieldName);
            EditorGUI.PropertyField(position, colorProp, new GUIContent(label));
            position.y += EditorGUIUtility.singleLineHeight + 4f;
        }
    }
}