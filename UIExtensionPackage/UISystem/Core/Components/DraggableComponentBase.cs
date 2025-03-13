using System;
using NaughtyAttributes;
using UIExtensionPackage.UISystem.Core.Interfaces;
using UnityEngine;
using UnityEngine.EventSystems;

namespace UIExtensionPackage.UISystem.Core.Components
{
    /// <summary>
    /// Class represents base for all Draggable Components.
    /// </summary>
    /// <remarks>Shouldn't be added to Object by hand</remarks>
    public abstract class DraggableComponentBase : MonoBehaviour, IDraggable
    {
        [InfoBox( "Component shouldn't be attached to the object alone, it requires other classes to initialize it first.", EInfoBoxType.Warning), 
         ShowIf(nameof(ShowInfoBox))]
        [Foldout("Debug")] [SerializeField, ReadOnly] private bool initialized = false;
        [Foldout("Debug")] [SerializeField, ReadOnly] protected Transform parentDuringDrag;
        [Foldout("Debug")] [SerializeField, ReadOnly] protected bool m_resetPositionOnEnd = true;
        [Foldout("Debug")] [SerializeField, ReadOnly] protected Vector3 startPosition;
        [Foldout("Debug")] [SerializeField, ReadOnly] private Transform startParentTransform;
        [Foldout("Debug")] [SerializeField, ReadOnly] private bool canBeDragged = true;
        [Foldout("Debug")] [SerializeField, ReadOnly] private bool isDragged;
        
        public virtual bool CanBeDragged => canBeDragged;

        public bool IsDragged => isDragged;

        public event Action<PointerEventData> OnDragBegin;
        public event Action<PointerEventData> OnDragging;
        public event Action<PointerEventData> OnDragEnd;

        protected virtual void Start()
        {
            if(parentDuringDrag == null)
                parentDuringDrag = transform.parent;
        }

        public virtual void Init(Transform dragParent = null, bool resetPositionOnEnd = true)
        {
            if(initialized) return;
            initialized = true;
            if(dragParent) SetNewDragParent(parentDuringDrag);
            m_resetPositionOnEnd = resetPositionOnEnd;
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            if(!CanBeDragged) return;
            isDragged = true;
            HandleRegisterStartPosition();
            startParentTransform = transform.parent;
            transform.SetParent(parentDuringDrag);
            HandleDragBegin(eventData);
            OnDragBegin?.Invoke(eventData);
        }

        public void OnDrag(PointerEventData eventData)
        {
            if(!CanBeDragged) return;
            HandleOnDrag(eventData);
            OnDragging?.Invoke(eventData);
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            if(!CanBeDragged) return;
            OnDragEnd?.Invoke(eventData);
            HandleOnDragEnd(eventData);
            if(m_resetPositionOnEnd) ResetPosition();
        }
        
        /// <summary>
        /// Method sets parent for object during dragging.
        /// </summary>
        public void SetNewDragParent(Transform newParentTransform) => parentDuringDrag = newParentTransform;
        
        
        /// <summary>
        /// Method resets object position to cached one
        /// </summary>
        protected void ResetPosition()
        {
            isDragged = false;
            transform.SetParent(startParentTransform, false);
            HandleResetPosition();
        }

        /// <summary>
        /// Handles changing draggable state
        /// </summary>
        /// <param name="value"></param>
        public void SetCanBeDragged(bool value)
        {
            if(canBeDragged && !value)
                ResetPosition();
            canBeDragged = value;
        }

        /// <summary>
        /// Handles caching start position
        /// </summary>
        protected abstract void HandleRegisterStartPosition();
        /// <summary>
        /// Handles drag begin logic
        /// </summary>
        protected abstract void HandleDragBegin(PointerEventData eventData);
        /// <summary>
        /// Handles drag logic
        /// </summary>
        protected abstract void HandleOnDrag(PointerEventData eventData);
        /// <summary>
        /// Handles drag end logic
        /// </summary>
        protected abstract void HandleOnDragEnd(PointerEventData eventData);
        /// <summary>
        /// Handles reset position
        /// </summary>
        protected abstract void HandleResetPosition();
        
        protected virtual void OnValidate()
        {
            ShowInfoBox();
        }

        private bool ShowInfoBox()
        {
            MonoBehaviour[] components = GetComponents<MonoBehaviour>();
            if(components == null || components.Length == 1) return true;
            return false;
        }
    }
}