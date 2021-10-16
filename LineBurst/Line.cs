using Unity.Mathematics;
using UnityEngine;

namespace LineBurst
{
    public struct Line
    {
        public float4 Begin;
        public float4 End;

        internal Line(float3 begin, float3 end, Color color)
        {
            var packedColor = ((int) (color.r * 63) << 18) | ((int) (color.g * 63) << 12) | ((int) (color.b * 63) << 6) | (int) (color.a * 63);
            Begin = new float4(begin, packedColor);
            End = new float4(end, packedColor);
        }
    }
}