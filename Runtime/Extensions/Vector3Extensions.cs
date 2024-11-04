
using UnityEngine;

namespace Kuci.Core.Extensions
{
    public static class Vector3Extensions
    {
        public static Vector3 Unity2Ros(this Vector3 vector3)
        {
            return new Vector3(vector3.z, -vector3.x, vector3.y);
        }
    }
}
