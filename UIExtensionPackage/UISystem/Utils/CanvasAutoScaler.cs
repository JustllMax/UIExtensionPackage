using NaughtyAttributes;
using UnityEngine;
using UnityEngine.UI;

namespace UIExtensionPackage.UISystem.Utils
{
    /// <summary>
    /// Class represents component for canvas, that sets canvas scaler depending on screen size.
    /// </summary>
    /// <remarks>Credits to H1M4W4R1</remarks>
    [RequireComponent(typeof(CanvasScaler))]
    public class CanvasAutoScaler : MonoBehaviour
    {
        [Foldout("Debug"), SerializeField, ReadOnly]
        CanvasScaler canvasScaler;
        [Foldout("Debug"), SerializeField, ReadOnly]
        private float resolutionRatio;
        private void Awake()
        {
            canvasScaler = GetComponent<CanvasScaler>();
        }

        /// <summary>
        /// Should be fixed update as resolution may change during runtime.
        /// </summary>
        private void FixedUpdate()
        {
            // Compute resolution ratio
            resolutionRatio = (float)Screen.width / Screen.height;
        
            // If resolution ratio is greater than base resolution ratio aka.
            // the screen is wider than the base resolution, then match height.
            // Otherwise, match width.
            canvasScaler.matchWidthOrHeight = resolutionRatio > DefaultConstants.BASE_RESOLUTION_RATIO ? 1 : 0;
        }
    }
}