using UnityEngine.EventSystems;

namespace UIExtensionPackage.UISystem.Core.Interfaces
{
    /// <summary>
    /// Interface for draggable objects.
    /// </summary>
    public interface IDraggable : IBeginDragHandler, IDragHandler, IEndDragHandler 
    {
        bool CanBeDragged { get; }
        
    }
}

