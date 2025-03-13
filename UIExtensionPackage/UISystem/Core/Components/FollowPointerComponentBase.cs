using System;
using NaughtyAttributes;
using UIExtensionPackage.UISystem.Core.Interfaces;
using UnityEngine;
using UnityEngine.EventSystems;

namespace UIExtensionPackage.UISystem.Core.Components
{
    /// <summary>
    /// Class represents base for FollowPointerComponents
    /// </summary>
    public abstract class FollowPointerComponentBase : MonoBehaviour, IPointerMoveHandler, IClickable
    {
        [Foldout("Debug")] [SerializeField, ReadOnly]
        protected bool destroyComponentOnClick = true; 
        [Foldout("Debug")] [SerializeField, ReadOnly]
        protected bool canFollow = true; 
        [Foldout("Debug")] [SerializeField, ReadOnly]
        private bool initialized = false;
        public bool CanBeInteractedWith { get; set; } = true;
        public bool CanFollow 
        { 
            get => canFollow; 
            set => canFollow = value; 
        }
        public bool DestroyComponentOnClick => destroyComponentOnClick;
  
        
        public event Action<PointerEventData> OnPointerClicked;
        public event Action<PointerEventData> OnPointerMoved;
        public event Action OnStartFollow;
        public event Action OnStopFollow;

        /// <summary>
        /// Handles initialization, called by outside classes when instantiating.
        /// </summary>
        public virtual void Init(bool destroyComponentOnClick = true)
        {
            if(initialized) return;
            initialized = true;
            this.destroyComponentOnClick = destroyComponentOnClick;
        }
        
        public void OnPointerMove(PointerEventData eventData)
        {
            if(!CanFollow) return;
            HandleOnPointerMove(eventData);
            OnPointerMoved?.Invoke(eventData);
        }
        
        public void OnClicked(PointerEventData eventData)
        {
            if(!CanBeInteractedWith) return;
            OnPointerClicked?.Invoke(eventData);
            if(DestroyComponentOnClick) Destroy(this);
        }
        
        /// <summary>
        /// Handles logic when moving pointer.
        /// </summary>
        protected abstract void HandleOnPointerMove(PointerEventData eventData);

        /// <summary>
        /// Stops follow
        /// </summary>
        public void StopFollow()
        {
            CanFollow = false;
            OnStopFollow?.Invoke();
        }
        /// <summary>
        /// Starts follow
        /// </summary>
        public void StartFollow()
        {
            CanFollow = true;
            OnStartFollow?.Invoke();
        }
        
    }
}

