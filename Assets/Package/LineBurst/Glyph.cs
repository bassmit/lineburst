using System;
using Unity.Mathematics;
using UnityEngine;

namespace LineBurst.Authoring
{
    [Serializable]
    public class Glyph
    {
        public Line[] Lines = new Line[0];

        [Serializable]
        public struct Line
        {
            public float2 Org;
            public float2 Dest;

            public Line(float2 org, float2 dest)
            {
                Org = org;
                Dest = dest;
            }
        }
    }
}