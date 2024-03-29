using Unity.Assertions;
using Unity.Collections;
using Unity.Mathematics;
using UnityEngine;

namespace LineBurst
{
    public struct GraphSettings
    {
        public readonly float2 Pos;
        public readonly float2 Size;
        public readonly float2 Min;
        public readonly float2 Max;
        public readonly float2 Grid;
        public readonly float2 Range;
        public readonly float2 Scale;
        public readonly quaternion Rot;
        public readonly float4x4 Tr;

        public int2 MarkingInterval;
        public Color AxisColor;
        public Color GridColor;
        public Color GridAltColor;
        public Color MarkingColor;
        public float UiBaseScale;
        public float MarkingScale;
        public float AxisNameScale;
        public float LegendScale;
        public float TitleScale;
        public FixedString64Bytes Title;
        public FixedString64Bytes HorizontalAxisName;
        public FixedString64Bytes VerticalAxisName;

        public GraphSettings(float2 pos, float2 size, float2 min, float2 max, float2 grid)
        {
            Assert.IsTrue(math.all(size > 0));
            Assert.IsTrue(math.all(grid >= 0));
            Assert.IsTrue(math.all(max > min));

            Pos = pos;
            Size = size;
            Min = min;
            Max = max;
            Grid = grid;
            Range = max - min;
            Scale = size / Range;
            Rot = quaternion.identity;
            Tr = float4x4.TRS(new float3(pos + -min * Scale, 0), Rot, new float3(Scale, 1));

            MarkingInterval = new int2(1);
            AxisColor = Color.black;
            GridColor = new Color(.25f, .25f, .25f, 1);
            GridAltColor = new Color(.11f, .11f, .11f, 1);
            MarkingColor = Color.white;
            UiBaseScale = 1;
            MarkingScale = 1;
            AxisNameScale = 1;
            LegendScale = 1;
            TitleScale = 1;
            Title = "";
            HorizontalAxisName = "";
            VerticalAxisName = "";
        }
    }
}