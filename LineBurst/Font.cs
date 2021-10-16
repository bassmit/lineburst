using LineBurst.Authoring;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace LineBurst
{
    struct Font
    {
        internal const int FirstAscii = 33;
        internal const int FinalAscii = 126;

        internal float Width;
        internal BlobArray<int2> Indices;
        internal BlobArray<Glyph.Line> Lines;

        public void Draw(FixedString512 text, Matrix4x4 transform, Color color)
        {
            var offset = float2.zero;
            
            for (int i = 0; i < text.Length; i++)
            {
                var c = (int) text[i];

                if (c == '\n')
                {
                    offset.x = 0;
                    --offset.y;
                    continue;
                }

                if (c >= FirstAscii && c <= FinalAscii)
                {
                    var ind = Indices[c - FirstAscii];
                    var amount = ind.y - ind.x;
                    var lines = new Draw.Lines(amount);
                    for (int j = ind.x; j < ind.y; j++)
                    {
                        var line = Lines[j];
                        var o = math.mul(transform, new float4(offset + line.Org, 0, 1)).xyz;
                        var d = math.mul(transform, new float4(offset + line.Dest, 0, 1)).xyz;
                        lines.Draw(o, d, color);
                    }
                }

                offset.x += Width;
            }
        }
    }
}