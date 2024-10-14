using UnityEngine;

namespace Kuci.Core.Extensions
{
    public static class BoxColliderExtensions
    {
        public static Vector3 GetRandomPositionInBounds(this BoxCollider boxCollider)
        {
            var center = boxCollider.center;
            var size = boxCollider.size;

            var x = Random.Range(center.x - size.x / 2, center.x + size.x / 2);
            var y = Random.Range(center.y - size.y / 2, center.y + size.y / 2);
            var z = Random.Range(center.z - size.z / 2, center.z + size.z / 2);

            // Convert local position to world position
            var randomPosition = boxCollider.transform.TransformPoint(new Vector3(x, y, z));
            return randomPosition;
        }
        
        public static Vector3 GetRandomPositionOnLowerSurface(this BoxCollider boxCollider)
        {
            var center = boxCollider.center;
            var size = boxCollider.size;

            var x = Random.Range(center.x - size.x / 2, center.x + size.x / 2);
            var z = Random.Range(center.z - size.z / 2, center.z + size.z / 2);

            // The y-coordinate for the lower surface is at the bottom of the box collider
            var y = center.y - size.y / 2;

            // Convert local position to world position
            var randomPosition = boxCollider.transform.TransformPoint(new Vector3(x, y, z));
            return randomPosition;
        }
    }
}