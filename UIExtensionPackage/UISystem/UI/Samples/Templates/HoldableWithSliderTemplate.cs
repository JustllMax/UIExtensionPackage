using NaughtyAttributes;
using UIExtensionPackage.UISystem.UI.Elements;
using UnityEngine;
using UnityEngine.UI;


namespace UIExtensionPackage.UISystem.UI.Samples.Templates
{
    /// <summary>
    /// Class for basic template of holdable ui element with slider.
    /// All logic is supposed to be done through the Inspector.
    /// </summary>
    /// <remarks>For more advanced logic, implement new class that inherits from <see cref="HoldableElementUI"/>.</remarks>
    internal sealed class HoldableWithSliderTemplate : HoldableElementUI
    {
        [Foldout("General")] [SerializeField] private Image _sliderFillImage;

        protected override void AttachEvents()
        {
            base.AttachEvents();
            onHoldProgressedChanged.AddListener(HoldProgressedChanged);
        }

        protected override void DetachEvents()
        {
            base.DetachEvents();
            onHoldProgressedChanged.RemoveListener(HoldProgressedChanged);
        }
        
        private void HoldProgressedChanged(float progress)
        {
            _sliderFillImage.fillAmount = progress;
        }
        
    }
}