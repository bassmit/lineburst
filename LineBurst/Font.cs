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

        public unsafe void Draw(FixedString32 text, Matrix4x4 transform, Color color) => Draw(text.GetUnsafePtr(), text.Length, transform, color);
        public unsafe void Draw(FixedString64 text, Matrix4x4 transform, Color color) => Draw(text.GetUnsafePtr(), text.Length, transform, color);
        public unsafe void Draw(FixedString128 text, Matrix4x4 transform, Color color) => Draw(text.GetUnsafePtr(), text.Length, transform, color);
        public unsafe void Draw(FixedString512 text, Matrix4x4 transform, Color color) => Draw(text.GetUnsafePtr(), text.Length, transform, color);
        public unsafe void Draw(FixedString4096 text, Matrix4x4 transform, Color color) => Draw(text.GetUnsafePtr(), text.Length, transform, color);

        unsafe void Draw(byte* text, int length, Matrix4x4 transform, Color color)
        {
            var offset = float2.zero;
            
            for (int i = 0; i < length; i++)
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
                        var o = math.transform(transform, new float3(offset + line.Org, 0));
                        var d = math.transform(transform, new float3(offset + line.Dest, 0));
                        lines.Draw(o, d, color);
                    }
                }

                offset.x += Width;
            }
        }
    }
}