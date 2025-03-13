using System;
using System.Collections.Generic;
using NaughtyAttributes;
using UIExtensionPackage.UISystem.Core.Base;
using UIExtensionPackage.UISystem.UI.Components;
using UIExtensionPackage.UISystem.Core.Interfaces;
using UIExtensionPackage.UISystem.UI.Windows.Interfaces;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace UIExtensionPackage.UISystem.UI.Windows
{
    /// <summary>
    /// Class represents an instance of UI Window.
    /// </summary>
    [RequireComponent(typeof(Canvas), typeof(CanvasGroup), typeof(GraphicRaycaster))]
    public abstract class UIWindow : UIObject, IClickable, IWindow, IWithSetup
    {
        [Foldout("Config")][SerializeField] private RenderMode canvasRenderMode = RenderMode.ScreenSpaceOverlay;
        [Foldout("Config")][SerializeField] private Canvas canvas;
        [Foldout("Config")][SerializeField] private CanvasGroup canvasGroup;
        [Foldout("Config")][SerializeField] private bool isDraggable = false;
        [Foldout("Config")][SerializeField, ShowIf(nameof(isDraggable))] 
            private bool resetPositionOnDragEnd = true;
        
        [Foldout("Debug")][SerializeField, ReadOnly, ShowIf(nameof(isDraggable))] 
            DraggableUIComponent draggableUIComponent;
        [Foldout("Debug")][SerializeField, ReadOnly] 
            List<UIPanel> uiPanels = new();
        
        protected CanvasGroup CanvasGroup => canvasGroup;
        public WindowStack Stack { get; set; }
        public DraggableUIComponent DraggableUIComponent => draggableUIComponent;
        public bool CanBeDragged => draggableUIComponent.CanBeDragged;
        
        /// <summary>
        /// Get the root window in the stack.
        /// </summary>
        public UIWindow RootWindow => Stack[0];
        public bool IsDraggable => isDraggable;
        public bool CanBeInteractedWith { get; set; } = true;
        
        /// <summary>
        /// Can there be multiple instances of this class
        /// </summary>
        public abstract bool AllowMultiple { get; }
        
        // NOTE:Windows are being mostly opened through UIWindowManager and when done so,
        // the event has already been invoked, even before reference has been passed
        // (it would be a null reference if it wasn't static and other objects would be just trying to access null.OnWindowOpened)
        // Hence static method that passes its type.
        public static event Action<UIWindow> OnWindowOpened;
        
        public event Action OnOpened;
        public event Action OnRefreshed;
        public event Action OnClosing;
        public event Action OnDestroyed;
        
        /// <summary>
        /// Handles changing draggable state
        /// </summary>
        public void SetCanBeDragged(bool value) => draggableUIComponent.SetCanBeDragged(value);
        
        public virtual void SetUp()
        {
            AttachEvents();
            uiPanels.AddRange(GetComponentsInChildren<UIPanel>());
            if (IsDraggable)
            {
                draggableUIComponent = transform.GetChild(0).gameObject.AddComponent<DraggableUIComponent>();
                draggableUIComponent.Init(transform, resetPositionOnDragEnd);
            }
            
        }

        /// <summary>
        /// Method makes canvas visible and interactable
        /// </summary>
        public void Show()
        {
            canvasGroup.alpha = 1;
            canvasGroup.interactable = true;
            canvasGroup.blocksRaycasts = true;
        }
        /// <summary>
        /// Method makes canvas invisible and not interactable
        /// </summary>
        public void Hide()
        {
            canvasGroup.alpha = 0;
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;
        }


        public void OnClicked(PointerEventData eventData) => OnClickedHandle();

        /// <summary>
        /// Handles logic when the window was clicked
        /// </summary>
        protected virtual void OnClickedHandle()
        {
            Stack?.MoveToTop();
        }

        /// <summary>
        /// Method sets sorting order value to canvas, used by <see cref="Stack"/>
        /// </summary>
        public void BringToTopAt(int sortingOrder)
        {
            if (!canvas) return;

            // Set sorting order
            canvas.sortingOrder = sortingOrder;
        }
        
        /// <summary>
        /// Method for opening window
        /// </summary>
        public virtual void OpenWindow()
        {
            Show();
            OnOpened?.Invoke();
            OnWindowOpened?.Invoke(this);
        }
        
        /// <summary>
        /// Method for refreshing open window and it's panels
        /// </summary>
        public void RefreshWindow()
        {
            Show();
            HandleRefreshing();
            foreach (UIPanel uiPanel in uiPanels)
            {
                uiPanel.Refresh();
            }
            OnRefreshed?.Invoke();
        }
        
        /// <summary>
        /// Handles refreshing of the window content
        /// </summary>
        protected virtual void HandleRefreshing(){}
        

        /// <summary>
        /// Method to call for when trying to close window
        /// </summary>
        public void Close() => CloseWindow();

        /// <summary>
        /// Handler for closing window
        /// </summary>
        protected virtual void CloseWindow()
        {
            OnClosing?.Invoke();
            
            // Close the window via stack
            if (Stack != null) Stack.CloseWindow(this);
            else DestroyWindow();
        }

        /// <summary> Call to destroy window </summary>
        /// <remarks>Shouldn't be called from outside, except the <see cref="WindowStack"/></remarks>
        public void DestroyWindow()
        {
            DeleteWindowObject();
        }

        /// <summary>
        /// Handles destroy window object
        /// </summary>
        private void DeleteWindowObject()
        {
            if (this != null) Destroy(gameObject);
        }
        
        public virtual void TearDown()
        {
            OnDestroyed?.Invoke();
            OnClosing = null;
        }

        protected virtual void OnValidate()
        {
            canvasGroup = GetComponent<CanvasGroup>();
            canvas = GetComponent<Canvas>();
            canvas.renderMode = canvasRenderMode;
        }


    }
}