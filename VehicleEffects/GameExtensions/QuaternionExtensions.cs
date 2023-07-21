using UnityEngine;

namespace VehicleEffects.GameExtensions
{
    internal static class QuaternionExtensions
    {
        public static Quaternion LookRotationWithoutComplaining(Vector3 vector3)
        {
            if (vector3.sqrMagnitude < Mathf.Epsilon)
                return Quaternion.identity;
            return Quaternion.LookRotation(vector3);
        }
    }
}
