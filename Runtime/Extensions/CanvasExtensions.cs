using UnityEngine;

namespace Kuci.Core.Extensions
{
    public static class CanvasExtensions
    {
        public static void ConfigureColliderSizes(this Canvas canvas, Vector2 canvasSizeDelta, BoxCollider[] colliders, out float overlap)
        {
            if (colliders.Length < 4)
            {
                overlap = 0;
                return;
            }

            const float canvasDepth = 0.01f;

            var averageThickness = (canvasSizeDelta.x + canvasSizeDelta.y) / 8f;
            var clampedThickness = Mathf.Clamp(averageThickness, averageThickness / 10f, averageThickness / 2f);
            overlap = clampedThickness * 2;

            var sizes = new Vector3[]
            {
                new(canvasSizeDelta.x + overlap, clampedThickness, canvasDepth),
                new(canvasSizeDelta.x + overlap, clampedThickness, canvasDepth),
                new(clampedThickness, canvasSizeDelta.y + overlap, canvasDepth),
                new(clampedThickness, canvasSizeDelta.y + overlap, canvasDepth)
            };

            var positions = new Vector3[]
            {
                new(0, (canvasSizeDelta.y / 2) + (clampedThickness / 2), 0),
                new(0, (-canvasSizeDelta.y / 2) - (clampedThickness / 2), 0),
                new((-canvasSizeDelta.x / 2) - (clampedThickness / 2), 0, 0),
                new((canvasSizeDelta.x / 2) + (clampedThickness / 2), 0, 0)
            };

            for (var i = 0; i < 4; i++)
            {
                colliders[i].size = sizes[i];
                colliders[i].center = positions[i];
            }
        }
    }
}