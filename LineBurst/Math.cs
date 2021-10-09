using System.Runtime.CompilerServices;
using Unity.Mathematics;

namespace LineBurst
{
    static class Math
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float NormalizeWithLength(float3 v, out float3 n)
        {
            var lengthSq = math.lengthsq(v);
            var invLength = math.rsqrt(lengthSq);
            n = v * invLength;
            return lengthSq * invLength;
        }

        // Return two normals perpendicular to the input vector
        public static void CalculatePerpendicularNormalized(float3 v, out float3 p, out float3 q)
        {
            var vSquared = v * v;
            var lengthsSquared = vSquared + vSquared.xxx; // y = ||j x v||^2, z = ||k x v||^2
            var invLengths = math.rsqrt(lengthsSquared);

            // select first direction, j x v or k x v, whichever has greater magnitude
            var dir0 = new float3(-v.y, v.x, 0.0f);
            var dir1 = new float3(-v.z, 0.0f, v.x);
            var cmp = (lengthsSquared.y > lengthsSquared.z);
            var dir = math.select(dir1, dir0, cmp);

            // normalize and get the other direction
            var invLength = math.select(invLengths.z, invLengths.y, cmp);
            p = dir * invLength;
            var cross = math.cross(v, dir);
            q = cross * invLength;
        }

        public static float3 GetPerpendicular(float3 v) => math.abs(v.y) > 1 - 1e-6f ? new float3(0, 0, 1) : math.normalize(math.cross(new float3(0, 1, 0), v));
    }
}