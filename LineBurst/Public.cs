using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Mathematics;
using UnityEngine;

namespace LineBurst
{
    // struct Color
    // {
    //     public static ColorIndex Quantize(float4 rgba)
    //     {
    //         var oldi = 0;
    //         var oldd = math.lengthsq(rgba - Unmanaged.Instance.Data.ColorData[0]);
    //         for (var i = 1; i < Unmanaged.KMaxColors; ++i)
    //         {
    //             var newd = math.lengthsq(rgba - Unmanaged.Instance.Data.ColorData[0]);
    //             if (newd < oldd)
    //             {
    //                 oldi = i;
    //                 oldd = newd;
    //             }
    //         }
    //         return new ColorIndex {Value = oldi};
    //     }
    // }

    public struct Arrows
    {
        Lines _lines;
        public Arrows(int count)
        {
            _lines = new Lines(count * 5);
        }

        public void Draw(float3 x, float3 v, Color color)
        {
            var x0 = x;
            var x1 = x + v;

            _lines.Draw(x0, x1, color);

            var length = Math.NormalizeWithLength(v, out var dir);
            Math.CalculatePerpendicularNormalized(dir, out var perp, out var perp2);
            float3 scale = length * 0.2f;

            _lines.Draw(x1, x1 + (perp - dir) * scale, color);
            _lines.Draw(x1, x1 - (perp + dir) * scale, color);
            _lines.Draw(x1, x1 + (perp2 - dir) * scale, color);
            _lines.Draw(x1, x1 - (perp2 + dir) * scale, color);
        }
    }

    public struct Arrow
    {
        public static void Draw(float3 x, float3 v, Color color)
        {
            new Arrows(1).Draw(x, v, color);
        }
    }

    public struct Planes
    {
        Lines _lines;
        public Planes(int count)
        {
            _lines = new Lines(count * 9);
        }

        public void Draw(float3 x, float3 v, Color color)
        {
            var x0 = x;
            var x1 = x + v;

            _lines.Draw(x0, x1, color);

            var length = Math.NormalizeWithLength(v, out var dir);
            Math.CalculatePerpendicularNormalized(dir, out var perp, out var perp2);
            float3 scale = length * 0.2f;

            _lines.Draw(x1, x1 + (perp - dir) * scale, color);
            _lines.Draw(x1, x1 - (perp + dir) * scale, color);
            _lines.Draw(x1, x1 + (perp2 - dir) * scale, color);
            _lines.Draw(x1, x1 - (perp2 + dir) * scale, color);

            perp *= length;
            perp2 *= length;

            _lines.Draw(x0 + perp + perp2, x0 + perp - perp2, color);
            _lines.Draw(x0 + perp - perp2, x0 - perp - perp2, color);
            _lines.Draw(x0 - perp - perp2, x0 - perp + perp2, color);
            _lines.Draw(x0 - perp + perp2, x0 + perp + perp2, color);
        }
    }

    public struct Plane
    {
        public static void Draw(float3 x, float3 v, Color color)
        {
            new Planes(1).Draw(x, v, color);
        }
    }

    public struct Arcs
    {
        Lines _lines;
        readonly int _res;

        public Arcs(int count, int resolution = 16)
        {
            _lines = new Lines(count * (2 + resolution));
            _res = resolution;
        }

        public void Draw(float3 center, float3 normal, float3 arm, float angle, Color color, bool delimit = false)
        {
            delimit &= angle < 2 * math.PI;
            var q = quaternion.AxisAngle(normal, angle / _res);
            var currentArm = arm;
            if (delimit)
                _lines.Draw(center, center + currentArm, color);
            for (var i = 0; i < _res; i++)
            {
                var nextArm = math.mul(q, currentArm);
                _lines.Draw(center + currentArm, center + nextArm, color);
                currentArm = nextArm;
            }
            if (delimit)
                _lines.Draw(center, center + currentArm, color);
            else
            {
                _lines.Draw(0, 0, color);
                _lines.Draw(0, 0, color);
            }
        }
    }

    public struct Arc
    {
        public static void Draw(float3 center, float3 normal, float3 arm, float angle, Color color, bool delimit = false, int resolution = 16)
        {
            new Arcs(1, resolution).Draw(center, normal, arm, angle, color, delimit);
        }
    }

    public struct Boxes
    {
        Lines _lines;

        public Boxes(int count)
        {
            _lines = new Lines(count * 12);
        }

        public void Draw(float3 size, float3 center, quaternion orientation, Color color)
        {
            var mat = math.float3x3(orientation);
            var x = mat.c0 * size.x * 0.5f;
            var y = mat.c1 * size.y * 0.5f;
            var z = mat.c2 * size.z * 0.5f;
            var c0 = center - x - y - z;
            var c1 = center - x - y + z;
            var c2 = center - x + y - z;
            var c3 = center - x + y + z;
            var c4 = center + x - y - z;
            var c5 = center + x - y + z;
            var c6 = center + x + y - z;
            var c7 = center + x + y + z;

            _lines.Draw(c0, c1, color); // ring 0
            _lines.Draw(c1, c3, color);
            _lines.Draw(c3, c2, color);
            _lines.Draw(c2, c0, color);

            _lines.Draw(c4, c5, color); // ring 1
            _lines.Draw(c5, c7, color);
            _lines.Draw(c7, c6, color);
            _lines.Draw(c6, c4, color);

            _lines.Draw(c0, c4, color); // between rings
            _lines.Draw(c1, c5, color);
            _lines.Draw(c2, c6, color);
            _lines.Draw(c3, c7, color);
        }
    }

    public struct Box
    {
        public static void Draw(float3 size, float3 center, quaternion orientation, Color color)
        {
            new Boxes(1).Draw(size, center, orientation, color);
        }
    }

    public struct Cones
    {
        Lines _lines;
        const int Res = 16;

        public Cones(int count)
        {
            _lines = new Lines(count * Res * 2);
        }

        public void Draw(float3 point, float3 axis, float angle, Color color)
        {
            var scale = Math.NormalizeWithLength(axis, out var dir);
            float3 arm;
            {
                Math.CalculatePerpendicularNormalized(dir, out var perp1, out _);
                arm = math.mul(quaternion.AxisAngle(perp1, angle), dir) * scale;
            }
            var q = quaternion.AxisAngle(dir, 2.0f * math.PI / Res);

            for (var i = 0; i < Res; i++)
            {
                var nextArm = math.mul(q, arm);
                _lines.Draw(point, point + arm, color);
                _lines.Draw(point + arm, point + nextArm, color);
                arm = nextArm;
            }
        }
    }

    public struct Cone
    {
        public static void Draw(float3 point, float3 axis, float angle, Color color)
        {
            new Cones(1).Draw(point, axis, angle, color);
        }
    }

    public struct Lines
    {
        Unit _unit;

        public Lines(int count)
        {
            _unit = Unmanaged.Instance.Data.LineBufferAllocations.AllocateAtomic(count);
        }

        public static unsafe void Draw(NativeArray<Line> lines)
        {
            var l = new Lines(lines.Length);
            var linesToCopy = math.min(lines.Length, l._unit.End - l._unit.Next);
            Unmanaged.Instance.Data.LineBuffer.CopyFrom(lines.GetUnsafeReadOnlyPtr(), linesToCopy, l._unit.Next);
            l._unit.Next += linesToCopy;
        }

        public void Draw(float3 begin, float3 end, Color color)
        {
            if (_unit.Next < _unit.End)
                Unmanaged.Instance.Data.LineBuffer.SetLine(new Line(begin, end, color), _unit.Next++);
        }
    }

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

        public static void Draw(float3 begin, float3 end, Color color)
        {
            new Lines(1).Draw(begin, end, color);
        }
    }

    public struct Sphere
    {
        public static void Draw(float3 point, float radius, Color color, int resolution = 16) => Draw(point, radius, color, new float3(0, 1, 0), resolution);

        public static void Draw(float3 point, float radius, Color color, float3 viewDir, int resolution = 16)
        {
            Arc.Draw(point, new float3(0, 0, 1), new float3(radius, 0, 0), 2 * math.PI, color, false, resolution);
            Arc.Draw(point, new float3(1, 0, 0), new float3(0, 0, radius), 2 * math.PI, color, false, resolution);
            Arc.Draw(point, new float3(0, 1, 0), new float3(radius, 0, 0), 2 * math.PI, color, false, resolution);

            if (math.any(viewDir != new float3(0, 1, 0)))
            {
                float3 arm;
                if (viewDir.x == 0)
                    arm = new float3(radius, 0, 0);
                else if (viewDir.y == 0)
                    arm = new float3(0, radius, 0);
                else if (viewDir.z == 0)
                    arm = new float3(0, 0, radius);
                else
                    arm = math.normalize(new float3(1, 0, -(1 / viewDir.z) * viewDir.x)) * radius;

                Arc.Draw(point, viewDir, arm, 2 * math.PI, color, false, resolution);
            }
        }
    }

    public struct Transform
    {
        public static void Draw(float3 pos, quaternion rot, float size = 1) => Draw(pos, rot, 1, size);

        public static void Draw(float3 pos, quaternion rot, float3 scale, float size = 1)
        {
            var lines = new Lines(3);
            lines.Draw(pos, pos + math.mul(rot, new float3(scale.x * size, 0, 0)), Color.red);
            lines.Draw(pos, pos + math.mul(rot, new float3(0, scale.y * size, 0)), Color.green);
            lines.Draw(pos, pos + math.mul(rot, new float3(0, 0, scale.z * size)), Color.blue);
        }
    }
}