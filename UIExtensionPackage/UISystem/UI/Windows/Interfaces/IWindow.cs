using UIExtensionPackage.Core.Interfaces;


namespace UIExtensionPackage.UISystem.UI.Windows.Interfaces
{
    /// <summary>
    /// Interfaces for marking objects as windows
    /// </summary>
    public interface IWindow : IShowHide
    {
        /// <summary>
        /// Can there be multiple instances of the same window type
        /// </summary>
        public bool AllowMultiple { get; }
    }
}

