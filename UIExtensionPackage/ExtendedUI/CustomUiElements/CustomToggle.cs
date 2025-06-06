using UIExtensionPackage.ExtendedUI.Base;
using UIExtensionPackage.ExtendedUI.Enums;
using UnityEngine;
using UnityEngine.UI;

namespace UIExtensionPackage.ExtendedUI.CustomUIElements
{
    
    /// <summary>
    /// Custom class for UIToggles
    /// </summary>
    public class CustomToggle : Toggle
    {
        
        [SerializeField] TargetGraphicData[] _targetGraphics; 
        
        // Cast interactable bool to ActiveState
        private ActiveState ActiveState => interactable ? ActiveState.Enabled : ActiveState.Disabled;
        
        // Cast default SelectionState to InteractionState
        private InteractionState InteractionState => (InteractionState)currentSelectionState;
        protected override void DoStateTransition(SelectionState state, bool instant)
        {
            // Do base transition
            base.DoStateTransition(state, instant);
            // Handle custom visuals
            HandleVisuals(ActiveState, InteractionState);
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
            for (int i = 0; i < _targetGraphics.Length; i++)
                _targetGraphics[i].HandleVisuals(activeState, interactionState);
        }
        
        
        /// <summary>
        /// Sets transition for all <see cref="_targetGraphics"/> driven from default button
        /// </summary>
        public void SetDefaultTransition()
        {
            for (int i = 0; i < _targetGraphics.Length; i++)
                _targetGraphics[i].SetTransition(transition);
        }
        
        /// <summary>
        /// Sets values driven from default button for <see cref="_targetGraphics"/> by selected transition
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
            for (int i = 0; i < _targetGraphics.Length; i++)
                _targetGraphics[i].SetColors(colors);
        }

        /// <summary>
        /// Sets sprites from default button
        /// </summary>
        private void SetDefaultSprites() 
        {
            for (int i = 0; i < _targetGraphics.Length; i++) 
                _targetGraphics[i].SetSprites(spriteState);
        }

        /// <summary>
        /// Sets names of animation triggers from default button
        /// </summary>
        private void SetDefaultAnimationTriggers()
        {
            for (int i = 0; i < _targetGraphics.Length; i++)
                _targetGraphics[i].SetAnimations(animationTriggers);
        }
    }
}

