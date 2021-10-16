using System;
using Unity.Assertions;
using Unity.Collections;
using Unity.Mathematics;
using UnityEngine;

namespace LineBurst
{
    public static partial class Draw
    {
        public struct Graph
        {
            readonly float2 _min;
            readonly float2 _max;
            readonly float2 _grid;
            readonly int2 _markingInterval;
            readonly float2 _range;
            readonly float4x4 _tr;
            readonly float2 _scale;
            readonly quaternion _rot;
            const float Epsilon = 1e-4f;

            public Graph(float2 pos, float2 size, float2 min, float2 max, float2 grid)
                : this(pos, size, min, max, grid, 1, DefaulGraphSettings) { }

            public Graph(float2 pos, float2 size, float2 min, float2 max, float2 grid, int2 markingInterval)
                : this(pos, size, min, max, grid, markingInterval, DefaulGraphSettings) { }

            public Graph(float2 pos, float2 size, float2 min, float2 max, float2 grid, int2 markingInterval, GraphSettings settings)
            {
                Assert.IsTrue(math.all(size > 0));
                Assert.IsTrue(math.all(grid >= 0));
                Assert.IsTrue(math.all(max > min));

                _min = min;
                _max = max;
                _grid = grid;
                _markingInterval = markingInterval;
                _range = max - min;
                _scale = size / _range;
                _rot = quaternion.identity;
                _tr = float4x4.TRS(new float3(pos + -min * _scale, 0), _rot, new float3(_scale, 1));

                DrawGrid(settings.GridColor, settings.GridAltColor);
                DrawAxes(settings.AxisColor, settings.MarkingColor);
            }

            void DrawGrid(Color color, Color altColor)
            {
                if (_grid.y > 0)
                {
                    var y = _min.y - ModEpsilon(_min.y, _grid.y);
                    while (y < _max.y + Epsilon)
                    {
                        if (math.abs(y) > Epsilon)
                        {
                            var c = _markingInterval.y == 1 || math.abs(ModEpsilon(y, _markingInterval.y * _grid.y)) < Epsilon ? color : altColor;
                            DrawLine(new float2(_min.x, y), new float2(_max.x, y), c);
                        }

                        y += _grid.y;
                    }
                }

                if (_grid.x > 0)
                {
                    var x = _min.x - ModEpsilon(_min.x, _grid.x);
                    while (x < _max.x + Epsilon)
                    {
                        if (math.abs(x) > Epsilon)
                        {
                            var c = _markingInterval.x == 1 || math.abs(ModEpsilon(x, _markingInterval.x * _grid.x)) < Epsilon ? color : altColor;
                            DrawLine(new float2(x, _min.y), new float2(x, _max.y), c);
                        }

                        x += _grid.x;
                    }
                }
            }

            void DrawMarkingsX(float y, Color color)
            {
                var step = _grid.x * _markingInterval.x;
                var x = _min.x - ModEpsilon(_min.x, step);
                while (x < _max.x + Epsilon)
                {
                    FixedString512 s = $"{System.Math.Round(x, 3)}";
                    var l = math.abs(x) < Epsilon ? s.Length + 1.4f : s.Length;
                    var pos = math.transform(_tr, new float3(x, y, 0));
                    var tr = float4x4.TRS(pos, _rot, new float3(TextScale, TextScale, 1));
                    tr = math.mul(tr, float4x4.Translate(new float3(-l * FontWidth / 2, 0, 0)));
                    Text(s, tr, color);
                    x += step;
                }
            }

            void DrawMarkingsY(float x, Color color)
            {
                var step = _grid.y * _markingInterval.y;
                var y = _min.y - ModEpsilon(_min.y, step);
                while (y < _max.y + Epsilon)
                {
                    if (math.abs(y) > Epsilon)
                    {
                        FixedString512 s = $"{System.Math.Round(y, 3)}";
                        var pos = math.transform(_tr, new float3(x, y, 0));
                        var tr = float4x4.TRS(pos, _rot, new float3(TextScale, TextScale, 1));
                        tr = math.mul(tr, float4x4.Translate(new float3(-(s.Length + .2f) * FontWidth, .5f, 0)));
                        Text(s, tr, color);
                    }

                    y += step;
                }
            }

            float TextScale
            {
                get
                {
                    var f = _grid.x < _grid.y
                        ? (_markingInterval.x > 1 ? 1.4f : 1) * _grid.x * _scale.y
                        : (_markingInterval.y > 1 ? 1.4f : 1) * _grid.y * _scale.x;

                    return DefaulGraphSettings.MarkingScale * f;
                }
            }

            static float ModEpsilon(float a, float b)
            {
                var t = math.abs(a);
                var s = math.sign(a);
                while (t + Epsilon > b)
                    t -= b;
                if (t <= 0)
                    return 0;
                return s * t;
            }

            void DrawAxes(Color color, Color markingColor)
            {
                if (_min.y <= 0 && _max.y >= 0)
                {
                    DrawLine(new float2(_min.x, 0), new float2(_max.x, 0), color);
                    DrawMarkingsX(0, markingColor);
                }
                else
                {
                    DrawMarkingsX(_min.y, markingColor);
                }

                if (_min.x <= 0 && _max.x >= 0)
                {
                    DrawLine(new float2(0, _min.y), new float2(0, _max.y), color);
                    DrawMarkingsY(0, markingColor);
                }
                else
                {
                    DrawMarkingsY(_min.x, markingColor);
                }
            }

            void DrawLine(float2 o, float2 d, Color color) => Line(math.transform(_tr, new float3(o, 0)), math.transform(_tr, new float3(d, 0)), color);

            public void Plot(Func<float, float> f, int samples, Color color) => Plot(new FuncWrapper(f), samples, color);

            struct FuncWrapper : IFunction
            {
                readonly Func<float, float> _func;

                public FuncWrapper(Func<float, float> func)
                {
                    _func = func;
                }

                public float F(float x) => _func(x);
            }

            public void Plot<T>(T f, int samples, Color color) where T : IFunction
            {
                var step = _range.x / (samples - 1);
                var x = _min.x;
                var prev = new float2(x, f.F(x));

                for (int i = 1; i < samples; i++)
                {
                    x += step;
                    var y = f.F(x);
                    var p = new float2(x, y);

                    var o = prev;
                    var d = p;

                    var min = _min.y;
                    var max = _max.y;

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

                    prev = p;
                }
            }

            static float2 Intercept(float2 p0, float2 p1, float y)
            {
                var v = p0 - p1;
                var s = v.y / v.x;
                return new float2(p0.x + (y - p0.y) / s, y);
            }
        }
    }
}