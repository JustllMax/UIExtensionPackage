using System.Collections.Generic;
using NaughtyAttributes;
using UIExtensionPackage.ExtendedUI.Base;
using UIExtensionPackage.ExtendedUI.Enums;
using UIExtensionPackage.UISystem.Core.Interfaces;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace UIExtensionPackage.UISystem.UI.Elements
{
    /// <summary>
    /// Class represents base class for hoverable ui elements
    /// </summary>
    [RequireComponent(typeof(Graphic))]
    public abstract class HoverableUIElement : UIElement, IHoverable
    {
        [Foldout("General")] [SerializeField] private TargetGraphicData[] targetGraphics;
        [Space] 
        [Header("Hover Events")] 
        [Foldout("Events")] [SerializeField] public UnityEvent onHoverEntered;
        [Foldout("Events")] [SerializeField] public UnityEvent onHoverExited;

        [Foldout("Debug")] [SerializeField, ReadOnly] protected bool _canBeInteractedWith = true;
        
        public bool IsHovered { get; private set; } = false;
        public bool CanBeInteractedWith 
        { 
            get => _canBeInteractedWith && !IsInteractionDisabled; 
            private set => _canBeInteractedWith = value;
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            // Check object state flags
            if (!IsActive || !CanBeInteractedWith) return;

            // Only change IsHovered flag when object is active and interactable
            IsHovered = true;
            
            // Change interaction state
            if (InteractionState == InteractionState.None)
                SetInteractionState(InteractionState.Hovered);
            
            // Change visuals
            HandleVisuals();
            // Handle on hover logic and invoke events
            HandleOnHoverEnter();
        }
        
        public void OnPointerExit(PointerEventData eventData)
        {
            // Always change the IsHovered flag
            IsHovered = false;
            
            // Check  object state flags
            if (!IsActive || !CanBeInteractedWith) return;
            
            // Change interaction state
            HandleInteractionStateOnHoverExit();
            // Change visuals
            HandleVisuals();
            
            // Handle on hover logic and invoke events
            HandleOnHoverExit();
        }

        /// <summary>
        /// Method that handles changing interaction state on hover exit
        /// </summary>
        /// <remarks>See <see cref="SelectableUIElement{T}"/> for override example </remarks>
        protected virtual void HandleInteractionStateOnHoverExit()
        {
            if(!IsInteractionDisabled) SetInteractionState(InteractionState.None);
        }
        public virtual void HandleOnHoverEnter()
        {
            onHoverEntered?.Invoke();
        }

        public virtual void HandleOnHoverExit()
        {
            onHoverExited?.Invoke();
        }


        protected override void HandleEnable()
        {
            base.HandleEnable();
            HandleVisuals();
        }

        protected override void HandleDisable()
        {
            base.HandleDisable();
            SetCanBeInteractedWith(false);
            HandleVisuals();
        }

        protected override void SetInteractionState(InteractionState state)
        {
            base.SetInteractionState(state);
            HandleVisuals();
        }
    
        /// <summary>
        /// Method handles visuals based on <see cref="ActiveState"/> and <see cref="InteractionState"/>
        /// </summary>
        private void HandleVisuals()
        {
            for (int i = 0; i < targetGraphics.Length; i++)
                targetGraphics[i].HandleVisuals(ActiveState, InteractionState);
            
        }
        /// <summary>
        /// Method handles changing <see cref="InteractionState"/>
        /// </summary>
        protected void SetCanBeInteractedWith(bool value)
        {
            CanBeInteractedWith = value;
            if (value)
            {
                if(IsHovered) SetInteractionState(InteractionState.Hovered);
                else SetInteractionState(InteractionState.None);
            }
            else
            {
                SetInteractionState(InteractionState.Disabled);
            }
            HandleVisuals();
        }
        

        protected virtual void OnValidate()
        {
            HandleVisuals();
        }
    }
}
