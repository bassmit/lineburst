using System.Runtime.CompilerServices;
using Unity.Mathematics;

namespace LineBurst
{
    static class Math
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float NormalizeWithLength(float3 v, out float3 n)
        {
            float lengthSq = math.lengthsq(v);
            float invLength = math.rsqrt(lengthSq);
            n = v * invLength;
            return lengthSq * invLength;
        }

        // Return two normals perpendicular to the input vector
        public static void CalculatePerpendicularNormalized(float3 v, out float3 p, out float3 q)
        {
            float3 vSquared = v * v;
            float3 lengthsSquared = vSquared + vSquared.xxx; // y = ||j x v||^2, z = ||k x v||^2
            float3 invLengths = math.rsqrt(lengthsSquared);

            // select first direction, j x v or k x v, whichever has greater magnitude
            float3 dir0 = new float3(-v.y, v.x, 0.0f);
            float3 dir1 = new float3(-v.z, 0.0f, v.x);
            bool cmp = (lengthsSquared.y > lengthsSquared.z);
            float3 dir = math.select(dir1, dir0, cmp);

            // normalize and get the other direction
            float invLength = math.select(invLengths.z, invLengths.y, cmp);
            p = dir * invLength;
            float3 cross = math.cross(v, dir);
            q = cross * invLength;
        }

        public static float3 GetPerpendicular(float3 v) => math.abs(v.y) > 1 - 1e-6f ? new float3(0, 0, 1) : math.normalize(math.cross(new float3(0, 1, 0), v));
    }
}