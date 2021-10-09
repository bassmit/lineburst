using Unity.Mathematics;
using UnityEngine;

namespace LineBurst.Authoring
{
    class Font : MonoBehaviour
    {
        internal const int FirstAscii = 33;
        const int FinalAscii = 126;
        const int GlyphAmount = FinalAscii - FirstAscii + 1;
        public float2 Size;
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
        }
    }
}