using NaughtyAttributes;
using UIExtensionPackage.UISystem.UI.Components;
using UIExtensionPackage.UISystem.Core.Interfaces;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace UIExtensionPackage.UISystem.UI.Elements
{
    /// <summary>
    /// Class represents UI elements that follow the pointer.
    /// </summary>
    [RequireComponent(typeof(FollowPointerUIComponent))]
    public abstract class FollowPointerUIElement : UIElement, IWithSetup
    {

        [Foldout("Config")] [SerializeField] private bool _destroyComponentOnClick = true;

        [Foldout("Debug")] [SerializeField, ReadOnly]
        FollowPointerUIComponent _followPointerUIComponent;

        public UnityEvent<PointerEventData> onClicked;
        public UnityEvent<PointerEventData> onPointerMoved;
        public UnityEvent onStartFollow;
        public UnityEvent onStopFollow;
        
        public FollowPointerUIComponent FollowPointerUIComponent => _followPointerUIComponent;

        public bool CanFollow
        {
            get => _followPointerUIComponent.CanFollow;
            set => _followPointerUIComponent.CanFollow = value;
        }

        public bool CanBeInteractedWith
        {
            get => _followPointerUIComponent.CanBeInteractedWith;
            set => _followPointerUIComponent.CanBeInteractedWith = value;
        }


        public virtual void SetUp()
        {
            _followPointerUIComponent.Init(_destroyComponentOnClick);
            _followPointerUIComponent.OnPointerClicked += OnClickedProxy;
            _followPointerUIComponent.OnPointerMoved += OnPointerMovedProxy;
            _followPointerUIComponent.OnStartFollow += OnStartFollowProxy;
            _followPointerUIComponent.OnStopFollow += OnStopFollowProxy;

        }

        public virtual void TearDown()
        {
            if(!_followPointerUIComponent) return;
            _followPointerUIComponent.OnPointerClicked -= OnClickedProxy;
            _followPointerUIComponent.OnPointerMoved -= OnPointerMovedProxy;
            _followPointerUIComponent.OnStartFollow -= OnStartFollowProxy;
            _followPointerUIComponent.OnStopFollow -= OnStopFollowProxy;
        }
        
        public void StartFollow() => _followPointerUIComponent?.StartFollow();
        public void StopFollow() => _followPointerUIComponent?.StopFollow();
        
        private void OnStartFollowProxy() => onStartFollow?.Invoke();
        private void OnStopFollowProxy() => onStopFollow?.Invoke();
        private void OnPointerMovedProxy(PointerEventData eventData) => onPointerMoved?.Invoke(eventData);
        private void OnClickedProxy(PointerEventData eventData) => onClicked?.Invoke(eventData);
        
        public virtual void OnValidate()
        {
            if(_followPointerUIComponent == null) _followPointerUIComponent = GetComponent<FollowPointerUIComponent>();
        }

    }
}