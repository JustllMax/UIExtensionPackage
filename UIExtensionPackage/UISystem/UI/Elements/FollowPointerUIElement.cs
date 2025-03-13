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

        [Foldout("Config")] [SerializeField] private bool destroyComponentOnClick = true;

        [Foldout("Debug")] [SerializeField, ReadOnly]
        FollowPointerUIComponent followPointerUIComponent;

        public UnityEvent<PointerEventData> OnClicked;
        public UnityEvent<PointerEventData> OnPointerMoved;
        public UnityEvent OnStartFollow;
        public UnityEvent OnStopFollow;

        
        public FollowPointerUIComponent FollowPointerUIComponent
        {
            get => followPointerUIComponent;
            private set => followPointerUIComponent = value;
        }

        public bool CanFollow
        {
            get => followPointerUIComponent.CanFollow;
            set => followPointerUIComponent.CanFollow = value;
        }

        public bool CanBeInteractedWith
        {
            get => followPointerUIComponent.CanBeInteractedWith;
            set => followPointerUIComponent.CanBeInteractedWith = value;
        }


        public virtual void SetUp()
        {
            followPointerUIComponent.Init(destroyComponentOnClick);
            followPointerUIComponent.OnPointerClicked += OnClickedProxy;
            followPointerUIComponent.OnPointerMoved += OnPointerMovedProxy;
            followPointerUIComponent.OnStartFollow += OnStartFollowProxy;
            followPointerUIComponent.OnStopFollow += OnStopFollowProxy;

        }

        public virtual void TearDown()
        {
            if(!followPointerUIComponent) return;
            followPointerUIComponent.OnPointerClicked -= OnClickedProxy;
            followPointerUIComponent.OnPointerMoved -= OnPointerMovedProxy;
            followPointerUIComponent.OnStartFollow -= OnStartFollowProxy;
            followPointerUIComponent.OnStopFollow -= OnStopFollowProxy;
        }
        
        public void StartFollow() => followPointerUIComponent?.StartFollow();
        public void StopFollow() => followPointerUIComponent?.StopFollow();
        
        private void OnStartFollowProxy() => OnStartFollow?.Invoke();
        private void OnStopFollowProxy() => OnStopFollow?.Invoke();
        private void OnPointerMovedProxy(PointerEventData eventData) => OnPointerMoved?.Invoke(eventData);
        private void OnClickedProxy(PointerEventData eventData) => OnClicked?.Invoke(eventData);
        
        public virtual void OnValidate()
        {
            if(followPointerUIComponent == null) followPointerUIComponent = GetComponent<FollowPointerUIComponent>();
        }

    }
}