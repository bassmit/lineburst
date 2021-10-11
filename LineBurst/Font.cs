using System.Linq;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace LineBurst.Authoring
{
    public class Font : MonoBehaviour
    {
        const int GlyphAmount = LineBurst.Font.FinalAscii - LineBurst.Font.FirstAscii + 1;
        public float2 Size;
        public int TotalLines;
        public float MarginBottom = .25f;
        public float MarginTop = .75f;
        public float MarginSide = .15f;
        public Glyph[] Glyphs;

        void OnValidate()
        {
            if (Size.x <= 0)
                Size.x = .1f;
            if (Size.y <= 0)
                Size.y = .1f;

            MarginBottom = math.clamp(MarginBottom, 0, .5f);
            MarginTop = math.clamp(MarginTop, 0, .5f);
            MarginSide = math.clamp(MarginSide, 0, .5f);

            if (Glyphs == null || Glyphs.Length != GlyphAmount)
            {
                Glyphs = new Glyph[GlyphAmount];

                for (int i = 0; i < Glyphs.Length; i++) 
                    Glyphs[i] = new Glyph();
            }

            TotalLines = Glyphs.Sum(g => g.Lines.Length);
        }

        internal BlobAssetReference<LineBurst.Font> Convert()
        {
            using (var blobBuilder = new BlobBuilder(Allocator.Temp))
            {
                ref var a = ref blobBuilder.ConstructRoot<LineBurst.Font>();
                a.Size = Size;
                var ind = blobBuilder.Allocate(ref a.Indices, Glyphs.Length);
                var l = blobBuilder.Allocate(ref a.Lines, TotalLines);

                var start = 0;
                var lineIndex = 0;

                for (var i = 0; i < Glyphs.Length; i++)
                {
                    var glyph = Glyphs[i];
                    var end = start + glyph.Lines.Length;
                    ind[i] = new int2(start, end);
                    start = end;

                    for (var j = 0; j < glyph.Lines.Length; j++)
                    {
                        var line = glyph.Lines[j];
                        line.Org = ToGlyphSpace(line.Org);
                        line.Dest = ToGlyphSpace(line.Dest);
                        l[lineIndex++] = line;
                    }
                }

                return blobBuilder.CreateBlobAssetReference<LineBurst.Font>(Allocator.Persistent);
            }
        }

        float2 ToGlyphSpace(float2 pos)
        {
            pos.x = MarginSide * Size.x + pos.x * ((1 - 2 * MarginSide) * Size.x);
            pos.y = MarginBottom * Size.y + pos.y * ((1 - (MarginBottom + MarginTop)) * Size.y);
            return pos;
        }
    }
}