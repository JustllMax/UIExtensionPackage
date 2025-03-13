using NaughtyAttributes;
using UIExtensionPackage.ExtendedUI.Enums;
using UIExtensionPackage.UISystem.Core.Interfaces;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace UIExtensionPackage.UISystem.UI.Elements
{
    /// <summary>
    /// Represents UI element that can be clicked
    /// </summary>  
    public abstract class SelectableUIElement<T> : HoverableUIElement, IPressable
    where T : SelectableUIElement<T>
    {
        
        [Foldout("General")][SerializeField, ShowIf(nameof(ShouldShowUnselectOnPointerUp))] 
        protected bool unselectOnPointerUp = true;
        [Foldout("Debug")][SerializeField, ReadOnly] protected bool isSelected = false;
        [Header("Select Events")]
        [Foldout("Events")][SerializeField, ShowIf(nameof(ShouldShowSelectEvents))] public UnityEvent<T> OnSelected;
        [Foldout("Events")][SerializeField, ShowIf(nameof(ShouldShowSelectEvents))] public UnityEvent<T> OnDeselected;
        public void OnClicked(PointerEventData eventData)
        {
            HandleOnClickLogic();
        }
        
        /// <summary>
        /// Handle logic when element is clicked
        /// </summary>
        protected virtual void HandleOnClickLogic(){}
        public virtual void OnPointerDown(PointerEventData eventData)
        {
            if(!IsActive || IsInteractionDisabled) return;
            
            // Set state to Selected
            if (unselectOnPointerUp) ChangeSelectState(true);
            
            // NOTE: In case of unselectOnPointerUp, clicking on the window will ALWAYS select it
            // In other case, select state will be changed when clicking on window depending on current state
            // i.e: unselected -> selected OR selected -> unselected
            
            // Change select state on click
            else ChangeSelectState(!isSelected);
            
            //Set to pressed visual state
            SetInteractionState(InteractionState.Pressed);
        }

        public virtual void OnPointerUp(PointerEventData eventData)
        {
            // Always set unselected flag
            isSelected = false;
            
            if (!IsActive || IsInteractionDisabled) return;
            
            //Unselect object and handle visuals
            if (unselectOnPointerUp)
            {
                // Check whether to fire select events
                ChangeSelectState(false);
                
                if(IsHovered) SetInteractionState(InteractionState.Hovered);
                else SetInteractionState(InteractionState.None);
            }
            // Set to selected visual state
            else
            {
                SetInteractionState(InteractionState.Selected);
            }
        }
        
        protected override void HandleInteractionStateOnHoverExit()
        {
            //If is not selected, perform OnHoverVisuals exit
            if(!isSelected)
                base.HandleInteractionStateOnHoverExit();
        }

        /// <summary>
        /// Method used for changing select state and firing select events
        /// </summary>
        private void ChangeSelectState(bool value)
        {
            // Check whether to fire select events
            if(!ShouldShowSelectEvents()) return;
            
            if (value)
            {
                isSelected = true;
                OnSelected?.Invoke(this as T);
            }
            else
            {
                isSelected = false;
                OnDeselected?.Invoke(this as T);
            }
        }

        /// <summary>
        /// Method used for showing or hiding <see cref="unselectOnPointerUp"/> checkmark
        /// </summary>
        /// <remarks>To not show, simply override and return false</remarks>
        protected virtual bool ShouldShowUnselectOnPointerUp()
        {
            return true;
        }
        
        /// <summary>
        /// Method used for showing or hiding <see cref="OnSelected"/> and <see cref="OnDeselected"/> events
        /// </summary>
        /// <remarks>To not show, simply override and return false</remarks>
        protected virtual bool ShouldShowSelectEvents()
        {
            return true;
        }
    }
}