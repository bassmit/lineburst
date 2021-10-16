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
            readonly GraphSettings _s;
            const float Epsilon = 1e-4f;

            public Graph(float2 pos, float2 size, float2 min, float2 max, float2 grid) : this(new GraphSettings(pos, size, min, max, grid)) { }

            public Graph(GraphSettings settings)
            {
                _s = settings;
                DrawGrid(_s.GridColor, _s.GridAltColor);
                DrawAxes(_s.AxisColor, _s.MarkingColor);
                DrawAxisNames();
            }

            public void Plot(Func<float, float> f, int samples, Color color, NativeArray<float> explicitSamples = default, float maxAmplitude = 1e+6f)
                => Plot(new FuncWrapper(f), samples, color, explicitSamples, maxAmplitude);

            public void Plot<T>(T f, int samples, Color color, NativeArray<float> explicitSamples = default, float maxAmplitude = 1e+6f) where T : IFunction
            {
                var l = new NativeList<float>(samples + (explicitSamples.IsCreated ? explicitSamples.Length : 0), Allocator.Temp);
                l.Add(_s.Min.x);

                var step = _s.Range.x / (samples - 1);
                var x = _s.Min.x;
                for (int i = 1; i < samples - 1; i++)
                {
                    x += step;
                    l.Add(x);
                }
                
                l.Add(_s.Max.x);
                
                if (explicitSamples.IsCreated)
                    l.AddRange(explicitSamples);

                l.Sort();

                var prev = new float2(l[0], f.F(l[0]));
                for (int i = 1; i < l.Length; i++)
                {
                    var p = new float2(l[i], f.F(l[i]));
                    DrawClamped<T>(prev, p, color, maxAmplitude);
                    prev = p;
                }
            }

            void DrawGrid(Color color, Color altColor)
            {
                if (_s.Grid.y > 0)
                {
                    var y = _s.Min.y - ModEpsilon(_s.Min.y, _s.Grid.y);
                    while (y < _s.Max.y + Epsilon)
                    {
                        if (math.abs(y) > Epsilon)
                        {
                            var c = _s.MarkingInterval.y == 1 || math.abs(ModEpsilon(y, _s.MarkingInterval.y * _s.Grid.y)) < Epsilon ? color : altColor;
                            DrawLine(new float2(_s.Min.x, y), new float2(_s.Max.x, y), c);
                        }

                        y += _s.Grid.y;
                    }
                }

                if (_s.Grid.x > 0)
                {
                    var x = _s.Min.x - ModEpsilon(_s.Min.x, _s.Grid.x);
                    while (x < _s.Max.x + Epsilon)
                    {
                        if (math.abs(x) > Epsilon)
                        {
                            var c = _s.MarkingInterval.x == 1 || math.abs(ModEpsilon(x, _s.MarkingInterval.x * _s.Grid.x)) < Epsilon ? color : altColor;
                            DrawLine(new float2(x, _s.Min.y), new float2(x, _s.Max.y), c);
                        }

                        x += _s.Grid.x;
                    }
                }
            }

            void DrawMarkingsX(float y, Color color)
            {
                var step = _s.Grid.x * _s.MarkingInterval.x;
                var x = _s.Min.x - ModEpsilon(_s.Min.x, step);
                while (x < _s.Max.x + Epsilon)
                {
                    FixedString32 s = $"{System.Math.Round(x, 3)}";
                    var l = math.abs(x) < Epsilon ? s.Length + 1.4f : s.Length;
                    var pos = math.transform(_s.Tr, new float3(x, y, 0));
                    var tr = float4x4.TRS(pos, _s.Rot, new float3(TextScale, TextScale, 1));
                    tr = math.mul(tr, float4x4.Translate(new float3(-l * FontWidth / 2, 0, 0)));
                    Text(s, tr, color);
                    x += step;
                }
            }

            void DrawMarkingsY(float x, Color color)
            {
                var step = _s.Grid.y * _s.MarkingInterval.y;
                var y = _s.Min.y - ModEpsilon(_s.Min.y, step);
                while (y < _s.Max.y + Epsilon)
                {
                    if (math.abs(y) > Epsilon)
                    {
                        FixedString32 s = $"{System.Math.Round(y, 3)}";
                        var pos = math.transform(_s.Tr, new float3(x, y, 0));
                        var tr = float4x4.TRS(pos, _s.Rot, new float3(TextScale, TextScale, 1));
                        tr = math.mul(tr, float4x4.Translate(new float3(-(s.Length + .2f) * FontWidth, .5f, 0)));
                        Text(s, tr, color);
                    }

                    y += step;
                }
            }

            float TextScale => _s.MarkingScale * (_s.Grid.x < _s.Grid.y
                                   ? (_s.MarkingInterval.x > 1 ? 1.4f : 1) * _s.Grid.x * _s.Scale.y
                                   : (_s.MarkingInterval.y > 1 ? 1.4f : 1) * _s.Grid.y * _s.Scale.x);

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
                if (_s.Min.y <= 0 && _s.Max.y >= 0)
                {
                    DrawLine(new float2(_s.Min.x, 0), new float2(_s.Max.x, 0), color);
                    DrawMarkingsX(0, markingColor);
                }
                else
                {
                    DrawMarkingsX(_s.Min.y, markingColor);
                }

                if (_s.Min.x <= 0 && _s.Max.x >= 0)
                {
                    DrawLine(new float2(0, _s.Min.y), new float2(0, _s.Max.y), color);
                    DrawMarkingsY(0, markingColor);
                }
                else
                {
                    DrawMarkingsY(_s.Min.x, markingColor);
                }
            }

            void DrawAxisNames()
            {
                var shortestAxisLength = math.min(_s.Size.x, _s.Size.y);
                var smallestScale = math.min(_s.Scale.x, _s.Scale.y);
                var longestAxisNameLength = math.max(_s.HorizontalAxisName.Length, _s.VerticalAxisName.Length);
                var scaleFromMarkingScale = 2.5f * _s.MarkingScale * smallestScale * FontWidth;
                var scaleFromAxisLength = .8f * shortestAxisLength / (longestAxisNameLength * FontWidth);
                var scale = math.min(scaleFromMarkingScale, scaleFromAxisLength);

                if (_s.HorizontalAxisName.Length > 0)
                {
                    var pos = _s.Pos.ToXYf();
                    pos.x += _s.Size.x / 2 - _s.HorizontalAxisName.Length * FontWidth / 2 * scale;
                    pos.y -= .3f * scale;
                    var tr = float4x4.TRS(pos, quaternion.identity, new float3(scale, scale, 1));
                    Text(_s.HorizontalAxisName, tr, _s.MarkingColor);
                }
                
                if (_s.VerticalAxisName.Length > 0)
                {
                    var pos = _s.Pos.ToXYf();
                    pos.x -= 1.3f * scale;
                    pos.y += _s.Size.y / 2 - _s.VerticalAxisName.Length * FontWidth / 2 * scale;
                    var tr = float4x4.TRS(pos, quaternion.RotateZ(math.PI / 2), new float3(scale, scale, 1));
                    Text(_s.VerticalAxisName, tr, _s.MarkingColor);
                }
            }

            void DrawLine(float2 o, float2 d, Color color) => Line(math.transform(_s.Tr, new float3(o, 0)), math.transform(_s.Tr, new float3(d, 0)), color);

            void DrawClamped<T>(float2 o, float2 d, Color color, float maxAmplitude) where T : IFunction
            {
                if (!IsValid(o.y, maxAmplitude) || !IsValid(d.y, maxAmplitude))
                    return;

                var min = _s.Min.y;
                var max = _s.Max.y;

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

            static bool IsValid(float f, float maxAmplitude) => f.IsReal() && math.abs(f) <= maxAmplitude;

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