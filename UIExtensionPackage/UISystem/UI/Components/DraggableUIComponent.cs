using System.Collections.Generic;
using NaughtyAttributes;
using UIExtensionPackage.UISystem.Core.Components;
using UIExtensionPackage.UISystem.Extensions;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace UIExtensionPackage.UISystem.UI.Components
{
    /// <summary>
    /// Class extends <see cref="DraggableComponentBase"/> for UI elements.
    /// </summary>
    /// <remarks>Shouldn't be added to Object by hand</remarks>
    public sealed class DraggableUIComponent : DraggableComponentBase
    {
        
        [Foldout("Debug")] [SerializeField, ReadOnly]
        private RectTransform _rectTransform;

        [Foldout("Debug")] [SerializeField, ReadOnly]
        private List<Graphic> _graphics = new List<Graphic>();

        [Foldout("Debug")] [SerializeField, ReadOnly]
        private Vector2 _offset;

        [Foldout("Debug")] [SerializeField, ReadOnly]
        private Vector2 _startPosition;

        private void Awake()
        {
            _graphics.AddRange(GetComponentsInChildren<Graphic>());
            _rectTransform = _graphics[0].transform as RectTransform;
        }

        protected override void HandleRegisterStartPosition()
        {
            _startPosition = _rectTransform.anchoredPosition;
        }

        protected override void HandleDragBegin(PointerEventData eventData)
        {
            for (int i = 0; i < _graphics.Count; i++)
            { 
                _graphics[i].raycastTarget = false;
            }
             
            // Get RT position from left bottom corner point of view and compute offset
            Vector2 rtPos = _rectTransform.position;
            _offset = rtPos - eventData.position;
        }

        protected override void HandleOnDrag(PointerEventData eventData)
        {
            // Move to new position and add offset
            _rectTransform.position = eventData.position + _offset;

            // Fit into viewport
            _rectTransform.FitIntoViewport();
        }

        protected override void HandleOnDragEnd(PointerEventData eventData)
        {
            for (int i = 0; i < _graphics.Count; i++)
            {
                _graphics[i].raycastTarget = true;
            }
        }

        protected override void HandleResetPosition()
        {
            _rectTransform.anchoredPosition = _startPosition;
        }


    }
}