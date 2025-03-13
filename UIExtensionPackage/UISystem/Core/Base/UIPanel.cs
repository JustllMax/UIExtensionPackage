using System;

namespace UIExtensionPackage.UISystem.Core.Base
{
    /// <summary>
    /// Class represents a fragment of the UIWindow.
    /// </summary>
    public abstract class UIPanel : UIObject
    {
        public event Action OnRefreshed;
        /// <summary>
        /// Method refreshes panel
        /// </summary>
        public void Refresh()
        {
            HandleRefreshing();
            OnRefreshed?.Invoke();
        }

        /// <summary>
        /// Handles refreshing of the panel content
        /// </summary>
        protected virtual void HandleRefreshing() { } 
    }
}

