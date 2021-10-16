using Unity.Mathematics;

namespace LineBurst
{
    static class FloatExtensions
    {
        public static bool IsReal(this float f) => !float.IsNaN(f) && !float.IsInfinity(f);
    }
    
    static class Float2Extensions
    {
        public static float3 ToXYf(this float2 f2, float f = 0) => new float3(f2.x, f2.y, f);
        public static float3 ToXfY(this float2 f2, float f = 0) => new float3(f2.x, f, f2.y);
        public static bool2 IsReal(this float2 f) => new bool2(f.x.IsReal(), f.y.IsReal());
        public static bool2 IsNan(this float2 f) => new bool2(float.IsNaN(f.x), float.IsNaN(f.y));
        public static bool2 IsInfitiy(this float2 f) => new bool2(float.IsInfinity(f.x), float.IsInfinity(f.y));
        public static bool2 IsNegativeInfinity(this float2 f) => new bool2(float.IsNegativeInfinity(f.x), float.IsNegativeInfinity(f.y));
        public static bool2 IsPositiveInfinity(this float2 f) => new bool2(float.IsPositiveInfinity(f.x), float.IsPositiveInfinity(f.y));
    }
}