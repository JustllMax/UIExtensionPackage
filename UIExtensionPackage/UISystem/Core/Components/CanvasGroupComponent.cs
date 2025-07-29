using UnityEngine;

namespace UIExtensionPackage.UISystem.Core.Components
{
    [RequireComponent(typeof(CanvasGroup))]
    public class CanvasGroupComponent : MonoBehaviour
    {

        private CanvasGroup _canvasGroup;
        
        /// <summary>
        /// Method makes canvas group visible and interactable
        /// </summary>
        public void Show()
        {
            _canvasGroup.alpha = 1;
            _canvasGroup.interactable = true;
            _canvasGroup.blocksRaycasts = true;
        }

        /// <summary>
        /// Method makes canvas group invisible and noninteractable
        /// </summary>
        public void Hide()
        {
            _canvasGroup.alpha = 0;
            _canvasGroup.interactable = false;
            _canvasGroup.blocksRaycasts = false;
        }

        public void SetIgnoreParentGroup(bool ignoreParentGroup)
        {
            _canvasGroup.ignoreParentGroups = ignoreParentGroup;
        }

#if UNITY_EDITOR

        private void OnValidate()
        {
            if(!_canvasGroup) _canvasGroup = GetComponent<CanvasGroup>();
        }
#endif
    }
}
