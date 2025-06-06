using System;
using System.Collections.Generic;
using System.Linq;
using UIExtensionPackage.ExtendedUI.Extensions;
using UIExtensionPackage.UISystem.Utils;
using Unity.Collections;
using UnityEngine;

namespace UIExtensionPackage.UISystem.UI.Windows
{
    
    /// <summary>
    /// Represents a virtual window that can be used to stack multiple windows on top of each other
    /// </summary>
    [Serializable]
    public class WindowStack
    {
        
        
        
        [SerializeField, ReadOnly] private List<UIWindow> _windows = new();

        /// <summary>
        /// Base value for each individual window stack
        /// </summary>
        private static int _sortingOrderValue = DefaultConstants.DEFAULT_WINDOW_SORTING_ORDER;
        private int _sortingOrderLimit = DefaultConstants.SORTING_ORDER_LIMIT;
        
        /// <summary>
        /// Amount of possible windows that can be opened in single stack at once without having same sorting order as others
        /// </summary>
        int _amountOfPossibleWindows = 20;
        private static WindowStack _topStack;
        public int StackIndex { get; set; }
        public int Count => _windows.Count;
        public bool IsTopStack => _topStack == this;

        public bool IsEmpty => _windows.IsNullOrEmpty();
        public bool IsAnyMovableWindowOpen => _windows.Any(window => window && window.IsDraggable);

        public UIWindow this[int index] => _windows[index];
        
        
        /// <summary>
        /// Move stack to the top of display and UIWindowManager list hierarchy
        /// </summary>
        public void MoveToTop()
        {
            _topStack = this;
            UIWindowManager.Instance.MoveWindowStackToLastIndex(this);
            
            if(CheckForSortingOrderLimit()) return;
            
            UpdateStackSortingOrder();
        }

        
        /// <summary>
        /// Shows all windows in Stack
        /// </summary>
        public void ShowAllWindows()
        {
            for (int i = 0; i < _windows.Count; i++)
            {
                _windows[i].Show();
            }
        }
                
        /// <summary>
        /// Hides all windows in Stack
        /// </summary>
        public void HideAllWindows()
        {
            for (int i = 0; i < _windows.Count; i++)
            {
                _windows[i].Hide();
            }
        }

        /// <summary>
        /// Method checks whether the sorting order cap has been reached.
        /// If it was, it will call <see cref="UIWindowManager.UpdateWindowStacksSortingOrders"/>
        /// </summary>
        private bool CheckForSortingOrderLimit()
        {
            if (_sortingOrderValue >= _sortingOrderLimit)
            {
                SetDefaultSortingOrderValue();
                UIWindowManager.Instance.UpdateWindowStacksSortingOrders();
                return true;
            }
            return false;
        }
        /// <summary>
        /// Method updates sorting order of all windows opened in single <see cref="WindowStack"/>
        /// </summary>
        public void UpdateStackSortingOrder()
        {
            // Loop through all windows in the stack and bring them to top while
            // increasing the sorting order for each window
            for (int windowIndex = 0; windowIndex < _windows.Count; windowIndex++)
            {
                _windows[windowIndex].BringToTopAt(_sortingOrderValue + StackIndex*_amountOfPossibleWindows);
                _sortingOrderValue++;
            }
        }

        /// <summary>
        /// Add window to the stack
        /// </summary>
        /// <typeparam name="T">Type of window to add</typeparam>
        /// <param name="window">Window to add</param>
        public T AddWindow<T>(T window) where T : UIWindow
        {
            _windows.Add(window);
            return window;
        }

        /// <summary>
        /// Close window from the stack
        /// </summary>
        /// <typeparam name="TWindowType">Type of window to close</typeparam>
        /// <returns>True if window was closed, false otherwise</returns>
        public bool CloseWindowByType<TWindowType>() where TWindowType : UIWindow
        {
            // Loop through all windows in the stack
            for (int i = _windows.Count - 1; i >= 0; i--)
            {
                // If window is not of the type we are looking for, skip it
                if (_windows[i] is not TWindowType window) continue;

                // Close window and return true as we found the window
                // we don't need to scan the rest of the stack as
                // all windows under this one will be closed as well
                CloseWindow(window);
                return true;
            }

            // If we didn't find the window, return false
            return false;
        }

        /// <summary>
        /// Close window from the stack
        /// </summary>
        /// <param name="window">Window to close</param>
        /// <returns>True if window was closed, false otherwise</returns>
        public bool CloseWindow<T>(T window) where T : UIWindow
        {
            // Get index of window
            int index = _windows.IndexOf(window);

            // If window is not in the stack, return false
            if (index == -1) return false;

            // Get all elements from stack
            List<UIWindow> windowsToClose = _windows.GetRange(index, _windows.Count - index);

            // Remove window and all children from the stack
            _windows.RemoveRange(index, _windows.Count - index);

            // Close all windows
            for (int windowIndex = windowsToClose.Count - 1; windowIndex >= 0; windowIndex--)
            {
                UIWindow windowToClose = windowsToClose[windowIndex];
                windowToClose.DestroyWindow();
            }

            if (this.IsEmpty)
            {
                UIWindowManager.Instance.RemoveWindowStackFromList(this);
            }
            return true;
        }

        /// <summary>
        /// Refresh window of the specified type
        /// </summary>
        /// <typeparam name="TWindowType">Type of window to refresh</typeparam>
        public void RefreshWindow<TWindowType>() where TWindowType : UIWindow
        {
            // Loop through all windows in the stack
            for (int i = _windows.Count - 1; i >= 0; i--)
            {
                // If window is not of the type we are looking for, skip it
                if (_windows[i] is not TWindowType window) continue;

                // Refresh window
                window.RefreshWindow();
                return;
            }
        }

        /// <summary>
        /// Method closes last opened window in the stack, if the stack is empty, it removes itself from list of stacks
        /// </summary>
        public bool CloseLastOpenedWindow()
        {
            if (IsEmpty) return false;
            CloseWindow(_windows[^1]);
            return true;    
        }
        
        /// <summary>
        /// Checks if there is an opened window in the stack of given type. 
        /// </summary>
        public bool IsWindowOfTypeOpen<TWindowType>()
        {
            // Loop through all windows in the stack
            for (int i = _windows.Count - 1; i >= 0; i--)
            {
                // If window is of the type we are looking for, return true
                if (_windows[i] is TWindowType) return true;
            }

            // If we didn't find the window, return false
            return false;
        }

        /// <summary>
        /// Close all windows in the stack
        /// </summary>
        public void CloseAllWindows()
        {
            // Close all windows
            for (int i = _windows.Count - 1; i >= 0; i--)
            {
                UIWindow window = _windows[i];
                window.DestroyWindow();
            }

            // Clear stack - remove all null windows
            _windows.RemoveAll(window => !window);
            
            // Delete stack if empty
            if (_windows.IsNullOrEmpty()) 
                UIWindowManager.Instance.RemoveWindowStackFromList(this);
        }
        
        
        /// <summary>
        /// Close all windows in the stack, except the given type
        /// </summary>
        public void CloseAllExcept<TWindowType>()
        where TWindowType : UIWindow
        {
            // Close all windows
            for (int i = _windows.Count - 1; i >= 0; i--)
            {
                // Get window
                UIWindow window = _windows[i];
                // If window should be ignored, skip
                if(window is TWindowType) continue;
                // Destroy window
                window.DestroyWindow();
            }

            // Clear stack - remove all null windows
            _windows.RemoveAll(window => !window);
            
            // Delete stack if empty
            if (_windows.IsNullOrEmpty()) 
                UIWindowManager.Instance.RemoveWindowStackFromList(this);
        }
        
        /// <summary>
        /// Try to get opened window of the specified type
        /// </summary>
        /// <param name="window">Opened window</param>
        /// <typeparam name="TWindowType">Type of window to get</typeparam>
        /// <returns>True if window was found, false otherwise</returns>
        public bool TryGetOpenedWindow<TWindowType>(out TWindowType window) where TWindowType : UIWindow
        {
            // Loop through all windows in the stack
            for (int i = _windows.Count - 1; i >= 0; i--)
            {
                // If window is not of the type we are looking for, skip it
                if (_windows[i] is not TWindowType foundWindow) continue;

                // Return window and true as we found the window
                window = foundWindow;
                return true;
            }

            // If we didn't find the window, return false
            window = null;
            return false;
        }
        
        /// <summary>
        /// Used when all window stacks has been removed.
        /// Delays costly live update of sorting all WindowStacks through <see cref="UIWindowManager.UpdateWindowStacksSortingOrders()"/>
        /// </summary>
        public static void SetDefaultSortingOrderValue()
        {
            _sortingOrderValue = DefaultConstants.DEFAULT_WINDOW_SORTING_ORDER;
        }
    }
}