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
        [Foldout("Config")] [SerializeField] private bool _resetPositionOnDragEnd = true;
        [Foldout("Config")] [SerializeField] private Transform _parentDuringDrag;

        [Foldout("Debug")] [SerializeField, ReadOnly]
        DraggableUIComponent _draggableUIComponent;


        [Space] [Header("Drag Events")]
        [Foldout("Events")] public UnityEvent<PointerEventData> onDragBegin;
        [Foldout("Events")] public UnityEvent<PointerEventData> onDragging;
        [Foldout("Events")] public UnityEvent<PointerEventData> onDragEnd;
        public DraggableUIComponent DraggableUIComponent => _draggableUIComponent;
        public bool CanBeDragged => _draggableUIComponent.CanBeDragged;
        public void SetCanBeDragged(bool value) => _draggableUIComponent.SetCanBeDragged(value);
        public virtual void SetUp()
        {
            _draggableUIComponent.Init(_parentDuringDrag, _resetPositionOnDragEnd);
            _draggableUIComponent.OnDragBegin += OnBeginDragProxy;
            _draggableUIComponent.OnDragging += OnDraggingProxy; 
            _draggableUIComponent.OnDragEnd += OnDragEndProxy;
        }
        
        public virtual void TearDown()
        {
            _draggableUIComponent.OnDragBegin -= OnBeginDragProxy;
            _draggableUIComponent.OnDragging -= OnDraggingProxy; 
            _draggableUIComponent.OnDragEnd -= OnDragEndProxy;
        }
        
        private void OnBeginDragProxy(PointerEventData eventData) => onDragBegin?.Invoke(eventData);
        private void OnDraggingProxy(PointerEventData eventData) => onDragging?.Invoke(eventData);
        private void OnDragEndProxy(PointerEventData eventData) => onDragEnd?.Invoke(eventData);


        protected override bool ShouldShowUnselectOnPointerUp() => false; 
        protected override void OnValidate()
        {
            base.OnValidate();
            if (_parentDuringDrag == null) _parentDuringDrag = transform.parent;
            if (_draggableUIComponent == null) _draggableUIComponent = GetComponent<DraggableUIComponent>();
        }
    }
}