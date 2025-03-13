using System;
using System.Collections.Generic;
using System.Linq;
using NaughtyAttributes;
using UIExtensionPackage.Core.Interfaces;
using UIExtensionPackage.ExtendedUI.Extensions;
using UIExtensionPackage.UISystem.Core.Generics;
using UnityEngine;

namespace UIExtensionPackage.UISystem.UI.Windows
{
    /// <summary>
    /// Class for managing windows and window stacks
    /// </summary>
    [RequireComponent(typeof(CanvasGroup))]
    public class UIWindowManager : SingletonMonobehaviour<UIWindowManager>, IShowHide
    {
        [Foldout("Config"),SerializeField] private List<UIWindow> allWindowsPrefabs;
        [Foldout("Debug"), SerializeField, ReadOnly] private Transform windowsContainer;
        [Foldout("Debug"), SerializeField, ReadOnly] private CanvasGroup canvasGroup;

        /// <summary>
        /// All opened windows that are not in stack (root windows)
        /// Last index is the top windows stack
        /// </summary>
        [Foldout("Debug"),SerializeField, ReadOnly] private List<WindowStack> windowStacks = new();
        
        private WindowStack TopStack => windowStacks.Count > 0 ? windowStacks[^1] : null;
        private readonly Dictionary<Type, UIWindow> _availableWindows = new();

        public bool IsAnyMovableWindowOpen => windowStacks.Any(stack => stack.IsAnyMovableWindowOpen);

        protected override void Awake()
        {
            base.Awake();
            canvasGroup = GetComponent<CanvasGroup>();
            BuildAvailableWindows();
            windowsContainer = transform;
        }
    
        
        /// <summary>
        /// Method, on start, validates assigned windows to manager in the Inspector.
        /// </summary>
        private void BuildAvailableWindows()
        {
            _availableWindows.Clear();
            // Loop through assigned windows
            foreach (UIWindow window in allWindowsPrefabs)
            {
                if (!window)
                {
                    // Log warning if window is null
                    Debug.LogWarning($"[{this}] Window prefab is null");
                    continue;
                }
                
                // If window type already exists, skip it, othwerwise add to available windows
                Type type = window.GetType();
                if (_availableWindows.ContainsKey(type))
                    Debug.LogWarning(
                        $"[{this}] Window with type {type} is added more than once to List of available prefabs");
                else
                    _availableWindows.Add(type, window);
            }
        }
        
        /// <summary>
        /// Method tries to open window from available windows in the manager.
        /// </summary>
        /// <param name="windowRef">The window that will be above in window stack</param>
        /// <returns>The opened window</returns>
        public TWindowType OpenWindow<TWindowType>(UIWindow windowRef) where TWindowType : UIWindow
        {
            return OpenWindow<TWindowType>(windowRef.Stack);
        }
        
        /// <summary>
        /// Method tries to open window from available windows in the manager.
        /// </summary>
        /// <param name="stackRef">The stack reference to add the window to</param>
        /// <returns>The opened window</returns>
        public TWindowType OpenWindow<TWindowType>(WindowStack stackRef = null) where TWindowType : UIWindow
        {
            // Get window type
            Type windowType = typeof(TWindowType);

            // Check if window is available
            if (!_availableWindows.ContainsKey(windowType))
            {
                Debug.LogError(
                    $"[{this}] Couldn't open window with type {typeof(TWindowType).Name}, there is no added window with that type");
                return null;
            }

            if (TryGetWindowFromStacks<TWindowType>(out UIWindow windowRef))
            {
                // Close window if it is already opened and no repetitions are allowed
                if (!windowRef.AllowMultiple)
                {
                    CloseWindow<TWindowType>();
                }
            }

            // Create window
            UIWindow window = Instantiate(_availableWindows[windowType], windowsContainer);

            // Check if stack ref is not set
            if (stackRef == null)
            {
                // Create new stack and add it to the list
                stackRef = new WindowStack();
                windowStacks.Add(stackRef);
            }
            /*
            NOTE: In case of a need to have windows without canvas on it
            uncomment this line, so the window can still be managed on display hierarchy
            Remember to delete RequireComponent on UIWindow
            else if (stackRef.Count > 0)
            {
                // Get last window
                UIWindow lastWindow = stackRef[^1];

                // Check if window is canvas based, if not, move it to the last window
                if (!window.GetComponent<Canvas>())
                {
                    // Move window to be a child of the root window
                    window.transform.SetParent(lastWindow.transform);
                }
            }*/

            // Add window to stack and set stack reference
            stackRef.AddWindow(window);
            window.Stack = stackRef;

            // Show window and move it to the top
            window.OpenWindow();
            stackRef.MoveToTop();
            return window as TWindowType;
        }

        /// <summary>
        /// Method loops through all stacks, searching for opened window of given type to close it.
        /// </summary>
        /// <remarks>Closes window no matter the type</remarks>
        public void CloseWindow<T>() where T : UIWindow
        {
            // Loop through all stacks
            for (int stackSlot = 0; stackSlot < windowStacks.Count; stackSlot++)
            {
                // Get window stack
                WindowStack stack = windowStacks[stackSlot];
                // Try to close window in the stack
                stack.CloseWindowByType<T>();
                if (stack.IsEmpty)
                {
                    // If stack is empty after closing the window, remove it from list
                    RemoveWindowStackFromList(stack);
                    stackSlot--;
                }
            }
        }
        
        /// <summary>
        /// Loops through all stacks, searching for window of given type to refresh it
        /// </summary>
        public void RefreshWindow<TWindowType>() where TWindowType : UIWindow
        {
            // Loop through all stacks
            for (int i = 0; i < windowStacks.Count; i++)
            {
                WindowStack stack = windowStacks[i];
                stack.RefreshWindow<TWindowType>();
            }
        }

        /// <summary>
        /// Method loops through all stacks, searching for opened window of given type.
        /// </summary>
        /// <param name="window">Opened window</param>
        /// <typeparam name="TWindwoType">Type of searched window</typeparam>
        public bool TryGetWindowFromStacks<TWindwoType>(out UIWindow window) 
            where TWindwoType : UIWindow
        {
            window = null;
            for (int stackSlot = 0; stackSlot < windowStacks.Count; stackSlot++)
            {
                WindowStack stack = windowStacks[stackSlot];
                if(stack.IsEmpty) continue;
                if (stack.TryGetOpenedWindow(out TWindwoType win))
                {
                    window = win;
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Method closes all windows, except <see cref="IgnoreCloseWindow"/>
        /// </summary>
        /// <param name="forceClose">Can be overriden to close all windows, no matter the type</param>
        public void CloseAllWindows(bool forceClose = false)
        {
            for (int stackSlot = 0; stackSlot < windowStacks.Count; stackSlot++)
            {
                // Get window stack
                WindowStack stack = windowStacks[stackSlot];
                
                // Close all windows, no matter the type
                if(forceClose) stack.CloseAllWindows();
                // Close all windows, except IgnoreCloseWindow classes
                else stack.CloseAllExcept<IgnoreCloseWindow>();
                if (stack.IsEmpty)
                {
                    // If stack is empty after closing the window, remove it from list
                    RemoveWindowStackFromList(stack);
                    stackSlot--;
                }
            }
        }
        
        /// <summary>
        /// Moves window stack to last index in list
        /// </summary>
        /// <remarks>Last index is a top stack</remarks>
        public void MoveWindowStackToLastIndex(WindowStack stack)
        {
            windowStacks.Move(stack, windowStacks.Count - 1);
            UpdateWindowStackIndexes();
        }

        /// <summary>
        /// Updates all window stacks' indexes
        /// </summary>
        private void UpdateWindowStackIndexes()
        {
            for (int i = 0; i < windowStacks.Count; i++)
            {
                windowStacks[i].StackIndex = i;
            }
        }

        /// <summary>
        /// Method loops through all windows in all window stacks and updates their sorting values.
        /// </summary>
        /// <remarks>This method is very costly, it is called only when sorting cap has been reached.
        /// This should never happen, it is only a precaution.</remarks>
        public void UpdateWindowStacksSortingOrders()
        {
            for (int i = 0; i < windowStacks.Count; i++)
            {
                windowStacks[i].UpdateStackSortingOrder();
            }
        }

        /// <summary>
        /// Return index in list of given window stack
        /// </summary>
        public int GetWindowStackIndex(WindowStack windowStack)
        {
            return windowStacks.IndexOf(windowStack);
        }

        /// <summary>
        /// Method tries to close window from current top stack
        /// </summary>
        /// <returns>True if window has been closed or the window cannot be closed</returns>
        private bool TryClosingWindowFromTopStack()
        {
            // Return if there are no windows stacks
            if (windowStacks.Count <= 0) return false;

            // Get top stack (last in the list)
            WindowStack topStack = TopStack;
            
            while (topStack.IsEmpty && windowStacks.Count > 1)
            {
                // Remove stack from list 
                windowStacks.Remove(topStack);
                // Set new top stack
                topStack = windowStacks[^1];
                // Move it to top in render queue
                topStack.MoveToTop();
            }

            // in case stack is for ignore close window, do not close it and return
            if (topStack.IsWindowOfTypeOpen<IgnoreCloseWindow>()) return true;

            return topStack.CloseLastOpenedWindow();
        }

        /// <summary>
        /// Removes stack from the list, when the list is empty, reset sorting order values for windows.
        /// </summary>
        public void RemoveWindowStackFromList(WindowStack stack)
        {
            windowStacks.Remove(stack);
            if (windowStacks.IsNullOrEmpty()) WindowStack.SetDefaultSortingOrderValue();
        }

        /// <summary>
        /// Show all opened windows in all window stacks, no matter the type.
        /// </summary>
        public void ShowAllWindows()
        {
            for (int stackSlot = 0; stackSlot < windowStacks.Count; stackSlot++)
            {
                WindowStack stack = windowStacks[stackSlot];
                stack.ShowAllWindows();
            }
        }
        
        /// <summary>
        /// Hide all opened windows in all window stacks, no matter the type.
        /// </summary>
        public void HideAllWindows()
        {
            for (int stackSlot = 0; stackSlot < windowStacks.Count; stackSlot++)
            {
                WindowStack stack = windowStacks[stackSlot];
                stack.HideAllWindows();
            }
        }
        
        /// <summary>
        /// Show opened windows
        /// </summary>
        /// <remarks>Ignores <see cref="IgnoreGroupWindow"/> windows</remarks>
        public void Show()
        {
            canvasGroup.alpha = 1;
            canvasGroup.blocksRaycasts = true;
            canvasGroup.interactable = true;
        }
        
        /// <summary>
        /// Hide opened windows
        /// </summary>
        /// <remarks>Ignores <see cref="IgnoreGroupWindow"/> windows</remarks>
        public void Hide()
        {
            canvasGroup.alpha = 0;
            canvasGroup.blocksRaycasts = false;
            canvasGroup.interactable = false;
        }
    }
}