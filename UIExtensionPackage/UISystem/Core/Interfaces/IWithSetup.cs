
namespace UIExtensionPackage.UISystem.Core.Interfaces
{
    /// <summary>
    /// Interface for objects that require additional setup during initialization phase
    /// </summary>
    public interface IWithSetup 
    {
        /// <summary>
        /// Method used during initializing phase in place of the Start method
        /// </summary>
        public void SetUp();
    
        /// <summary>
        /// Method for cleaning up data set in SetUp, called when destroying object
        /// </summary>
        public void TearDown();
    }
}
