using NaughtyAttributes;
using UIExtensionPackage.UISystem.Core.Components;
using UIExtensionPackage.UISystem.Extensions;
using UnityEngine;
using UnityEngine.EventSystems;

namespace UIExtensionPackage.UISystem.UI.Components
{
    /// <summary>
    /// Class extends <see cref="FollowPointerComponentBase"/>  for UI elements.
    /// </summary>
    public class FollowPointerUIComponent : FollowPointerComponentBase
    {

        [Foldout("Debug")] [SerializeField, ReadOnly]
        private RectTransform _rectTransform;
        
        [Foldout("Debug")] [SerializeField, ReadOnly]
        private Vector2 _offset;


        public override void Init(bool destroyComponentOnClick = true)
        {
            base.Init(destroyComponentOnClick);
            // Get Rect Transform
            _rectTransform = GetComponent<RectTransform>();
        }
        
        protected override void HandleOnPointerMove(PointerEventData eventData)
        {
            // Get RT position from left bottom corner point of view and compute offset
            Vector2 rtPos = _rectTransform.position;
            // Calculate offset
            _offset = rtPos - eventData.position;
            
            // Move to new position and add offset
            _rectTransform.position = eventData.position + _offset;

            // Fit into viewport
            _rectTransform.FitIntoViewport();
        }
        
    }
}