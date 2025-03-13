using System;
using System.Collections.Generic;
using TMPro;
using UIExtensionPackage.ExtendedUI.Base;
using UIExtensionPackage.ExtendedUI.Enums;
using UnityEngine;
using UnityEngine.EventSystems;


namespace UIExtensionPackage.ExtendedUI.CustomUIElements
{
    /// <summary>
    /// Represents a dropdown UI component.
    /// </summary>
    [Serializable]
    public class CustomDropdown : TMP_Dropdown
    {
        [SerializeField] private bool unselectAfterPressed = true;
        [SerializeField] TargetGraphicData[] targetGraphics;
        
        private ActiveState ActiveState => interactable ? ActiveState.Enabled : ActiveState.Disabled;
        private InteractionState InteractionState => (InteractionState)currentSelectionState;
        protected override void DoStateTransition(SelectionState state, bool instant)
        {
            // Do base transition
            base.DoStateTransition(state, instant);
            // Handle custom visuals
            HandleVisuals(ActiveState, InteractionState);
        }

        public override void OnPointerExit(PointerEventData eventData)
        {
            base.OnPointerExit(eventData);

            // Check if the button is pressed
            if (IsPressed())
            {
                // If pressed stay in the Pressed state
                DoStateTransition(SelectionState.Pressed, false);
            }
            else
            {
                // Otherwise transition to Normal or Highlighted state
                DoStateTransition(currentSelectionState, false);
            }
        }

        public override void OnPointerUp(PointerEventData eventData)
        {
            base.OnPointerUp(eventData);
            if (unselectAfterPressed)
            {
                OnDeselect(eventData);
            }
        }
    
       /// <summary>
        /// Public wrapper for <see cref="HandleVisuals"/>
        /// </summary>
        public void HandleVisuals() => HandleVisuals(ActiveState, InteractionState);
        /// <summary>
        /// Perform visuals transition based of ActiveState and InteractionState
        /// </summary>
       private void HandleVisuals(ActiveState activeState, InteractionState interactionState)
        {
            for (int i = 0; i < targetGraphics.Length; i++)
                targetGraphics[i].HandleVisuals(activeState, interactionState);
        }
        
        
        /// <summary>
        /// Sets transition for all <see cref="targetGraphics"/> driven from default button
        /// </summary>
        public void SetDefaultTransition()
        {
            for (int i = 0; i < targetGraphics.Length; i++)
                targetGraphics[i].SetTransition(transition);
        }
        
        /// <summary>
        /// Sets values driven from default button for <see cref="targetGraphics"/> by default button selected transition
        /// </summary>
        public void SetDefaultSettings()
        {
            switch (transition)
            {
                case Transition.ColorTint:
                    SetDefaultColors();
                    return;
                case Transition.SpriteSwap:
                    SetDefaultSprites();
                    return;
                case Transition.Animation:
                    SetDefaultAnimationTriggers();
                    return;
                default:
                    return;
            }
        }

        /// <summary>
        /// Sets colors from default button
        /// </summary>
        private void SetDefaultColors() 
        {
            for (int i = 0; i < targetGraphics.Length; i++)
                targetGraphics[i].SetColors(colors);
        }

        /// <summary>
        /// Sets sprites from default button
        /// </summary>
        private void SetDefaultSprites() 
        {
            for (int i = 0; i < targetGraphics.Length; i++) 
                targetGraphics[i].SetSprites(spriteState);
        }

        /// <summary>
        /// Sets names of animation triggers from default button
        /// </summary>
        private void SetDefaultAnimationTriggers()
        {
            for (int i = 0; i < targetGraphics.Length; i++)
                targetGraphics[i].SetAnimations(animationTriggers);
        }
    }
}
