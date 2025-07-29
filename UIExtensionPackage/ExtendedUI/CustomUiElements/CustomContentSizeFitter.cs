using UnityEngine;
using UnityEngine.UI;

namespace UIExtensionPackage.ExtendedUI.CustomUIElements
{
    [RequireComponent(typeof(RectTransform))]
    public class CustomContentSizeFitter : ContentSizeFitter
    {

        [SerializeField] private bool _useHorizontalConstraints;
        [SerializeField] private Vector2 _horizontalConstraints;
        [Space]
        [SerializeField] private bool _useVerticalConstraints;
        [SerializeField] private Vector2 _verticalConstraints;


        private RectTransform _rectTransform;

        protected override void OnEnable()
        {
            base.OnEnable();
            _rectTransform = GetComponent<RectTransform>();
        }

        public override void SetLayoutHorizontal()
        {
            base.SetLayoutHorizontal();
            if(!_useHorizontalConstraints) return;
        
            Vector2 sizeDelta = _rectTransform.sizeDelta;
            float clampedWidth = Mathf.Clamp(_rectTransform.rect.width, _horizontalConstraints.x, _horizontalConstraints.y);
            sizeDelta.x = clampedWidth - (_rectTransform.rect.width - _rectTransform.sizeDelta.x);
            _rectTransform.sizeDelta = sizeDelta;
        }

        public override void SetLayoutVertical()
        {
            base.SetLayoutVertical();
        
            if(!_useVerticalConstraints) return;
            Vector2 sizeDelta = _rectTransform.sizeDelta;
            float clampedHeight = Mathf.Clamp(_rectTransform.rect.height, _verticalConstraints.x, _verticalConstraints.y);
            sizeDelta.y = clampedHeight - (_rectTransform.rect.height - _rectTransform.sizeDelta.y);
            _rectTransform.sizeDelta = sizeDelta;
        }

        public void SetNewHorizontalConstraints(Vector2 newHorizontalConstraints)
        {
            if (newHorizontalConstraints.x < 1 || newHorizontalConstraints.y < 1)
            {
                Debug.LogWarning($"{gameObject.name} {nameof(CustomContentSizeFitter)} horizontal constraints cannot be smaller than 1.");
                return;
            }
            _horizontalConstraints = newHorizontalConstraints;
            SetLayoutHorizontal();
        }

        public void SetNewVerticalConstraints(Vector2 newVerticalConstraints)
        {
            if (newVerticalConstraints.x < 1 || newVerticalConstraints.y < 1)
            {
                Debug.LogWarning($"{gameObject.name} {nameof(CustomContentSizeFitter)} vertical constraints cannot be smaller than 1.");
                return;
            }
            _verticalConstraints = newVerticalConstraints;
            SetLayoutVertical();
        }
        
        protected override void OnValidate()
        {
            base.OnValidate();
            if(_rectTransform == null) _rectTransform = GetComponent<RectTransform>();
        }

        public void SetLayouts()
        {
            SetLayoutHorizontal();
            SetLayoutVertical();
        }
    }
}
