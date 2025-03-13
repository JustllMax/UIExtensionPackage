using UnityEngine.EventSystems;

namespace UIExtensionPackage.UISystem.Core.Interfaces
{
    public interface IClickable: IPointerClickHandler
    {
        /// <summary>
        /// Handles base logic when clicked
        /// </summary>
        public void OnClicked(PointerEventData eventData);
        public bool CanBeInteractedWith { get; }
        
        void IPointerClickHandler.OnPointerClick(PointerEventData eventData)
        {
           OnClicked(eventData); 
        }
    }
}
