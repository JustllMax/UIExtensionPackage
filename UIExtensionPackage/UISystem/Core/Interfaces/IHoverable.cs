using UnityEngine.EventSystems;

namespace UIExtensionPackage.UISystem.Core.Interfaces
{
    /// <summary>
    /// Interface for handling hovers
    /// </summary>
    public interface IHoverable : IPointerEnterHandler, IPointerExitHandler
    {
         public bool IsHovered { get;}
         /// <summary>
         /// Handles on hover enter logic
         /// </summary>
         public void HandleOnHoverEnter();
       
         /// <summary>
         /// Handles on hover exit logic
         /// </summary>
         public void HandleOnHoverExit();
       
    }
}
