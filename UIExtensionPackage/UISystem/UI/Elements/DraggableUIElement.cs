using NaughtyAttributes;
using UIExtensionPackage.UISystem.UI.Components;
using UIExtensionPackage.UISystem.Core.Interfaces;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace UIExtensionPackage.UISystem.UI.Elements
{

    /// <summary>
    /// Represents base class for draggable UI elements.
    /// </summary>
    [RequireComponent(typeof(DraggableUIComponent))]
    public abstract class DraggableUIElement : SelectableUIElement<DraggableUIElement>, IWithSetup
    {

        [Foldout("Config")] [SerializeField] private bool resetPositionOnDragEnd = true;
        [Foldout("Config")] [SerializeField] private Transform parentDuringDrag;

        [Foldout("Debug")] [SerializeField, ReadOnly]
        DraggableUIComponent draggableUIComponent;

        [Space] [Header("Drag Events")]
        [Foldout("Events")] public UnityEvent<PointerEventData> OnDragBegin;
        [Foldout("Events")] public UnityEvent<PointerEventData> OnDragging;
        [Foldout("Events")] public UnityEvent<PointerEventData> OnDragEnd;
        public DraggableUIComponent DraggableUIComponent => draggableUIComponent;

        public bool CanBeDragged => draggableUIComponent.CanBeDragged;
        public void SetCanBeDragged(bool value) => draggableUIComponent.SetCanBeDragged(value);
        public virtual void SetUp()
        {
            draggableUIComponent.Init(parentDuringDrag, resetPositionOnDragEnd);
            draggableUIComponent.OnDragBegin += OnBeginDragProxy;
            draggableUIComponent.OnDragging += OnDraggingProxy; 
            draggableUIComponent.OnDragEnd += OnDragEndProxy;
        }
        
        public virtual void TearDown()
        {
            draggableUIComponent.OnDragBegin -= OnBeginDragProxy;
            draggableUIComponent.OnDragging -= OnDraggingProxy; 
            draggableUIComponent.OnDragEnd -= OnDragEndProxy;
        }
        
        private void OnBeginDragProxy(PointerEventData eventData) => OnDragBegin?.Invoke(eventData);
        private void OnDraggingProxy(PointerEventData eventData) => OnDragging?.Invoke(eventData);
        private void OnDragEndProxy(PointerEventData eventData) => OnDragEnd?.Invoke(eventData);


        protected override bool ShouldShowUnselectOnPointerUp() => false; 
        protected override void OnValidate()
        {
            base.OnValidate();
            if (parentDuringDrag == null) parentDuringDrag = transform.parent;
            if (draggableUIComponent == null) draggableUIComponent = GetComponent<DraggableUIComponent>();
        }
    }
}