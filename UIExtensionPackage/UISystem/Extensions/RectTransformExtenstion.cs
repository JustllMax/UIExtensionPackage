using UnityEngine;
using Unity.Mathematics;
using JetBrains.Annotations;
using UIExtensionPackage.UISystem.Core.Base;

namespace UIExtensionPackage.UISystem.Extensions
{
    /// <summary>
    /// Extends RectTransform and UIObject class
    /// </summary>
    /// <remarks>Credits to H1M4W4R1</remarks>
    public static class RectTransformExtenstion
    {

        /// <summary>
        /// Gets the distance between the specified UI object and the specified point.
        /// </summary>
        /// <param name="fromPoint">Point to get the distance from.</param>
        /// <returns>Distance between the UI object and the specified point.</returns>
        public static float GetDistanceTo([NotNull] this UIObject uiObject, Vector2 fromPoint)
        {
            // Get nearest point
            Vector2 nearestPoint = uiObject.GetNearestPoint(fromPoint);

            // Return the distance between the point and the nearest point
            return math.distance(nearestPoint, fromPoint);
        }

        /// <summary>
        /// Gets the nearest point on the element to the specified point.
        /// </summary>
        /// <param name="uiObject">UI object.</param>
        /// <param name="toPoint">Point to get the nearest point to.</param>
        /// <returns>Nearest point on the element to the specified point.</returns>
        public static Vector2 GetNearestPoint([NotNull] this UIObject uiObject, Vector2 toPoint)
        {
            // Get rect transform
            RectTransform rectTransform = uiObject.GetComponent<RectTransform>();

            // Get bounds
            Rect rect = rectTransform.rect;

            Vector2 position = rectTransform.position;
            Vector2 pivotOffset = (float2)rectTransform.pivot * (float2)rect.size;

            // Get left bottom corner of the element
            position -= pivotOffset;

            // Shift position to centre of rect
            position += rect.size / 2;

            Bounds bounds = new(position, rect.size);

            // Return the nearest point (absolute position)
            return bounds.ClosestPoint(toPoint);
        }

        /// <summary>
        /// Checks if specified UI object is within the bounds of its parent.
        /// </summary>
        public static bool IsWithinParentBounds([NotNull] this UIObject uiObject)
        {
            // Get rect transform
            RectTransform rectTransform = uiObject.GetComponent<RectTransform>();
            return rectTransform.IsWithinParentBounds();
        }

        /// <summary>
        /// Checks if specified UI object is within the bounds of its parent.
        /// </summary>
        public static bool IsWithinParentBounds([NotNull] this RectTransform rectTransform)
        {
            // Get parent
            Transform parent = rectTransform.parent;

            return IsWithinRectTransformBounds(rectTransform, parent.GetComponent<RectTransform>());
        }

        /// <summary>
        /// Checks if the specified UI object is within the bounds of specified rect transform.
        /// </summary>
        public static bool IsWithinRectTransformBounds(
            [NotNull] this RectTransform rectTransform,
            [NotNull] RectTransform otherRectTransform)
        {
            Rect targetRect = rectTransform.rect;
            Rect otherRect = otherRectTransform.rect;

            Vector2 pivotOffset = (float2)rectTransform.pivot * (float2)targetRect.size;

            // Get left bottom corner of the element
            Vector2 targetPosition = rectTransform.position;
            targetPosition -= pivotOffset;

            // Get left bottom corner of the parent
            Vector2 parentPosition = rectTransform.parent.position;
            parentPosition -= otherRect.size / 2;

            // Check if the element is within the bounds of the parent,
            // if it is not, then return false.
            if (targetPosition.x < parentPosition.x) return false;
            if (targetPosition.y < parentPosition.y) return false;
            if (targetPosition.x + targetRect.width > parentPosition.x + otherRect.width) return false;
            if (targetPosition.y + targetRect.height > parentPosition.y + otherRect.height) return false;

            return true;
        }

        /// <summary>
        /// Fits the element into the bounds of the parent.
        /// </summary>
        public static void FitIntoParent([NotNull] this UIObject uiObject)
        {
            // Get rect transform
            RectTransform rectTransform = uiObject.GetComponent<RectTransform>();
            rectTransform.FitIntoParent();
        }

        /// <summary>
        /// Fits the element into the bounds of the parent.
        /// </summary>
        public static void FitIntoParent([NotNull] this RectTransform rectTransform)
        {
            // Get parent
            Transform parent = rectTransform.parent;

            // Fit into parent
            rectTransform.FitIntoRectTransform(parent.GetComponent<RectTransform>());
        }

        /// <summary>
        /// This method fits the element into the bounds of the specified rect transform.
        /// </summary>
        public static void FitIntoRectTransform(
            [NotNull] this RectTransform rectTransform,
            [NotNull] RectTransform otherRectTransform)
        {
            // Get rect
            Rect targetRect = rectTransform.rect;

            // Get other rect
            Rect otherRect = otherRectTransform.rect;

            // Check if rect is smaller or same size as viewport rect
            // if not, then return
            if (targetRect.width > otherRect.width ||
                targetRect.height > otherRect.height)
            {
                // Log a warning
                Debug.LogWarning("Draggable element is larger than parent.");
                return;
            }

            Vector2 pivotOffset = (float2)rectTransform.pivot * (float2)targetRect.size;

            // Get left bottom corner of the element
            Vector2 targetPosition = rectTransform.position;
            targetPosition -= pivotOffset;

            // Get left bottom corner of the parent
            Vector2 parentPosition = rectTransform.parent.position;
            parentPosition -= otherRect.size / 2;

            // If absolute position is lower than zero (outside the bounds of the parent)
            if (targetPosition.x < parentPosition.x) targetPosition.x = parentPosition.x;
            if (targetPosition.y < parentPosition.y) targetPosition.y = parentPosition.y;

            // If absolute position is further than the parent width (outside the bounds of the parent)
            if (targetPosition.x + targetRect.width > parentPosition.x + otherRect.width)
                targetPosition.x = parentPosition.x + otherRect.width - targetRect.width;

            // If absolute position is higher than the parent height (outside the bounds of the parent)
            if (targetPosition.y + targetRect.height > parentPosition.y + otherRect.height)
                targetPosition.y = parentPosition.y + otherRect.height - targetRect.height;

            // Set absolute position
            rectTransform.position = targetPosition + pivotOffset;
        }

        /// <summary>
        /// Fits the element into the bounds of the viewport.
        /// </summary>
        public static void FitIntoViewport([NotNull] this UIObject uiObjectBase)
        {
            // Get rect transform
            RectTransform rectTransform = uiObjectBase.GetComponent<RectTransform>();
            rectTransform.FitIntoViewport();
        }

        /// <summary>
        /// Fits the element into the bounds of the viewport.
        /// </summary>
        public static void FitIntoViewport([NotNull] this RectTransform rectTransform)
        {
            // Get rect
            Rect targetRect = rectTransform.rect;

            // Check if rect is smaller or same size as viewport rect
            // if not, then return
            if (targetRect.width > Screen.width ||
                targetRect.height > Screen.height)
            {
                // Log a warning
                Debug.LogWarning("Draggable element is larger than viewport.");
                return;
            }

            Vector2 pivotOffset = (float2)rectTransform.pivot * (float2)targetRect.size;

            // Get left bottom corner of the element
            Vector2 targetPosition = rectTransform.position;
            targetPosition -= pivotOffset;

            // Check if the element is within the bounds of the viewport,
            // if it is not, then move it to the nearest point within the bounds.

            // If absolute position is lower than zero (outside the bounds of the viewport)
            if (targetPosition.x < 0) targetPosition.x = 0;
            if (targetPosition.y < 0) targetPosition.y = 0;

            // If absolute position is higher than the viewport width (outside the bounds of the viewport)
            if (targetPosition.x + targetRect.width > Screen.width)
                targetPosition.x = Screen.width - targetRect.width;

            // If absolute position is higher than the viewport height (outside the bounds of the viewport)
            if (targetPosition.y + targetRect.height > Screen.height)
                targetPosition.y = Screen.height - targetRect.height;

            // Set absolute position
            rectTransform.position = targetPosition + pivotOffset;
        }

        /// <summary>
        /// Checks if the specified UI object is within the bounds of the viewport.
        /// </summary>
        public static bool IsWithinViewportBounds([NotNull] this UIObject uiObject)
        {
            // Get the rect transform
            RectTransform rectTransform = uiObject.GetComponent<RectTransform>();
            return rectTransform.IsWithinViewportBounds();
        }

        public static bool IsWithinViewportBounds([NotNull] this RectTransform rectTransform)
        {
            Rect targetRect = rectTransform.rect;

            Vector2 pivotOffset = (float2)rectTransform.pivot * (float2)targetRect.size;

            // Get left bottom corner of the element
            Vector2 targetPosition = rectTransform.position;
            targetPosition -= pivotOffset;

            // Check if the draggable element is within the bounds of the viewport
            return targetPosition is { x: >= 0, y: >= 0 } &&
                   targetPosition.x + targetRect.width <= Screen.width &&
                   targetPosition.y + targetRect.height <= Screen.height;
        }
        
        /// <summary>
        /// Sets left offset bounds
        /// </summary>
        /// <param name="left">Amount to set offset to</param>
        public static void SetLeft(this RectTransform rt, float left)
        {
            rt.offsetMin = new Vector2(left, rt.offsetMin.y);
        }
        /// <summary>
        /// Sets right offset bounds
        /// </summary>
        /// <param name="right">Amount to set offset to</param>
        public static void SetRight(this RectTransform rt, float right)
        {
            rt.offsetMax = new Vector2(-right, rt.offsetMax.y);
        }
        /// <summary>
        /// Sets top offset bounds
        /// </summary>
        /// <param name="top">Amount to set offset to</param>
        public static void SetTop(this RectTransform rt, float top)
        {
            rt.offsetMax = new Vector2(rt.offsetMax.x, -top);
        }
        /// <summary>
        /// Sets bottom offset bounds
        /// </summary>
        /// <param name="bottom">Amount to set offset to</param>
        public static void SetBottom(this RectTransform rt, float bottom)
        {
            rt.offsetMin = new Vector2(rt.offsetMin.x, bottom);
        }
    }
}