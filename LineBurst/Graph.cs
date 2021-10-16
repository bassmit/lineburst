using System;
using Unity.Collections;
using Unity.Mathematics;
using UnityEngine;

namespace LineBurst
{
    public static partial class Draw
    {
        public struct Graph
        {
            readonly GraphSettings _settings;
            const float Epsilon = 1e-4f;

            float2 Pos => _settings.Pos;
            float2 Size => _settings.Size;
            float2 Min => _settings.Min;
            float2 Max => _settings.Max;
            float2 Grid => _settings.Grid;
            float2 Range => _settings.Range;
            float2 Scale => _settings.Scale;
            quaternion Rot => _settings.Rot;
            float4x4 Tr => _settings.Tr;

            float MarkingScale => _settings.MarkingScale;
            int2 MarkingInterval => _settings.MarkingInterval;
            Color GridColor => _settings.GridColor;
            Color GridAltColor => _settings.GridAltColor;
            Color AxisColor => _settings.AxisColor;
            Color MarkingColor => _settings.MarkingColor;

            public Graph(float2 pos, float2 size, float2 min, float2 max, float2 grid) : this(new GraphSettings(pos, size, min, max, grid)) { }

            public Graph(GraphSettings settings)
            {
                _settings = settings;
                DrawGrid(GridColor, GridAltColor);
                DrawAxes(AxisColor, MarkingColor);
            }

            public void Plot(Func<float, float> f, int samples, Color color) => Plot(new FuncWrapper(f), samples, color);

            public void Plot<T>(T f, int samples, Color color) where T : IFunction
            {
                var step = Range.x / (samples - 1);
                var x = Min.x;
                var prev = new float2(x, f.F(x));

                for (int i = 1; i < samples; i++)
                {
                    x += step;
                    var p = new float2(x, f.F(x));
                    DrawClamped<T>(prev, p, color);
                    prev = p;
                }
            }

            void DrawGrid(Color color, Color altColor)
            {
                if (Grid.y > 0)
                {
                    var y = Min.y - ModEpsilon(Min.y, Grid.y);
                    while (y < Max.y + Epsilon)
                    {
                        if (math.abs(y) > Epsilon)
                        {
                            var c = MarkingInterval.y == 1 || math.abs(ModEpsilon(y, MarkingInterval.y * Grid.y)) < Epsilon ? color : altColor;
                            DrawLine(new float2(Min.x, y), new float2(Max.x, y), c);
                        }

                        y += Grid.y;
                    }
                }

                if (Grid.x > 0)
                {
                    var x = Min.x - ModEpsilon(Min.x, Grid.x);
                    while (x < Max.x + Epsilon)
                    {
                        if (math.abs(x) > Epsilon)
                        {
                            var c = MarkingInterval.x == 1 || math.abs(ModEpsilon(x, MarkingInterval.x * Grid.x)) < Epsilon ? color : altColor;
                            DrawLine(new float2(x, Min.y), new float2(x, Max.y), c);
                        }

                        x += Grid.x;
                    }
                }
            }

            void DrawMarkingsX(float y, Color color)
            {
                var step = Grid.x * MarkingInterval.x;
                var x = Min.x - ModEpsilon(Min.x, step);
                while (x < Max.x + Epsilon)
                {
                    FixedString512 s = $"{System.Math.Round(x, 3)}";
                    var l = math.abs(x) < Epsilon ? s.Length + 1.4f : s.Length;
                    var pos = math.transform(Tr, new float3(x, y, 0));
                    var tr = float4x4.TRS(pos, Rot, new float3(TextScale, TextScale, 1));
                    tr = math.mul(tr, float4x4.Translate(new float3(-l * FontWidth / 2, 0, 0)));
                    Text(s, tr, color);
                    x += step;
                }
            }

            void DrawMarkingsY(float x, Color color)
            {
                var step = Grid.y * MarkingInterval.y;
                var y = Min.y - ModEpsilon(Min.y, step);
                while (y < Max.y + Epsilon)
                {
                    if (math.abs(y) > Epsilon)
                    {
                        FixedString512 s = $"{System.Math.Round(y, 3)}";
                        var pos = math.transform(Tr, new float3(x, y, 0));
                        var tr = float4x4.TRS(pos, Rot, new float3(TextScale, TextScale, 1));
                        tr = math.mul(tr, float4x4.Translate(new float3(-(s.Length + .2f) * FontWidth, .5f, 0)));
                        Text(s, tr, color);
                    }

                    y += step;
                }
            }

            float TextScale => MarkingScale * (Grid.x < Grid.y
                ? (MarkingInterval.x > 1 ? 1.4f : 1) * Grid.x * Scale.y
                : (MarkingInterval.y > 1 ? 1.4f : 1) * Grid.y * Scale.x);

            static float ModEpsilon(float a, float b)
            {
                var t = math.abs(a);
                var s = math.sign(a);
                while (t + Epsilon > b)
                    t -= b;
                return t <= 0 ? 0 : s * t;
            }

            void DrawAxes(Color color, Color markingColor)
            {
                if (Min.y <= 0 && Max.y >= 0)
                {
                    DrawLine(new float2(Min.x, 0), new float2(Max.x, 0), color);
                    DrawMarkingsX(0, markingColor);
                }
                else
                {
                    DrawMarkingsX(Min.y, markingColor);
                }

                if (Min.x <= 0 && Max.x >= 0)
                {
                    DrawLine(new float2(0, Min.y), new float2(0, Max.y), color);
                    DrawMarkingsY(0, markingColor);
                }
                else
                {
                    DrawMarkingsY(Min.x, markingColor);
                }
            }

            void DrawLine(float2 o, float2 d, Color color) => Line(math.transform(Tr, new float3(o, 0)), math.transform(Tr, new float3(d, 0)), color);

            void DrawClamped<T>(float2 o, float2 d, Color color) where T : IFunction
            {
                var min = Min.y;
                var max = Max.y;

                if (o.y < min)
                {
                    if (d.y > min)
                    {
                        o = Intercept(o, d, min);

                        if (d.y > max)
                            d = Intercept(o, d, max);

                        DrawLine(o, d, color);
                    }
                }
                else if (o.y > max)
                {
                    if (d.y < max)
                    {
                        o = Intercept(o, d, max);

                        if (d.y < min)
                            d = Intercept(o, d, min);

                        DrawLine(o, d, color);
                    }
                }
                else
                {
                    if (d.y > max)
                    {
                        d = Intercept(o, d, max);
                        DrawLine(o, d, color);
                    }
                    else if (d.y < min)
                    {
                        d = Intercept(o, d, min);
                        DrawLine(o, d, color);
                    }
                    else
                    {
                        DrawLine(o, d, color);
                    }
                }
            }

            static float2 Intercept(float2 p0, float2 p1, float y) => new float2(p0.x + (y - p0.y) * ((p0.x - p1.x) / (p0.y - p1.y)), y);

            struct FuncWrapper : IFunction
            {
                readonly Func<float, float> _func;

                public FuncWrapper(Func<float, float> func)
                {
                    _func = func;
                }

                public float F(float x) => _func(x);
            }
        }
    }
}