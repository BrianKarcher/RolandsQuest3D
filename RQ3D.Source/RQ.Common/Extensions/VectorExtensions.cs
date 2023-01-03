using UnityEngine;

namespace RQ.Base.Extensions
{
    public static class VectorExtensions
    {
        public static Vector3 xz(this Vector2 vector)
        {
            return new Vector3(vector.x, 0f, vector.y);
        }

        public static Vector2 xz(this Vector3 vector)
        {
            return new Vector2(vector.x, vector.z);
        }
    }
}
