
namespace UIExtensionPackage.UISystem.Core.Interfaces
{
    /// <summary>
    /// Interface for object that require initialization at the start of their existance.
    /// </summary>
    public interface IInitializable
    {
        /// <summary>
        /// Flag for initialization
        /// </summary>
        public bool IsInitialized { get; set; }
        /// <summary>
        /// Method for handling initialization
        /// </summary>
        public void Initialize();
    }
}

