
using UnityEngine;

namespace Kuci.Core.Extensions
{
    public static class QuaternionExtensions
    {
        public static Quaternion Unity2Ros(this Quaternion quaternion)
        {
            return new Quaternion(-quaternion.z, quaternion.x, -quaternion.y, quaternion.w);
        }
    }
}
