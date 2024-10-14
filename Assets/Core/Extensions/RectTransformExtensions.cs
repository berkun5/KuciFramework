using UnityEngine;

namespace Kuci.UI
{
    public static class RectTransformExtensions
    {
        public static Vector2 GetInScreenRectPosition(Vector2 receivedPosition, RectTransform rectTransform)
        {
            var rect = rectTransform.rect;
            var pivot = rectTransform.pivot;
            
            //trying anchor it off of the finger so it's visible
            var visibleXPos = receivedPosition.x - pivot.x * rect.width;
            var visibleYPos = receivedPosition.y + pivot.y * rect.height;

            var clampedX = Mathf.Clamp(visibleXPos, rect.width / 2, Screen.width - rect.width / 2);
            var clampedY = Mathf.Clamp(visibleYPos, rect.height / 2,  Screen.height - rect.height / 2);

            return new Vector2(clampedX, clampedY);
        }
        
        public static void CenterAndExpandToParent(this RectTransform rectTransform)
        {
            rectTransform.anchorMin = Vector2.zero;
            rectTransform.anchorMax = Vector2.one;
            rectTransform.pivot = Vector2.one / 2;
            rectTransform.sizeDelta = Vector2.zero;
            rectTransform.anchoredPosition = Vector3.zero;
        }
    }
}