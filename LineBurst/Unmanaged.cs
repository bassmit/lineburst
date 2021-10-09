using Unity.Burst;
using Unity.Mathematics;

namespace LineBurst
{
    struct Unmanaged
    {
        internal static readonly SharedStatic<Unmanaged> Instance = SharedStatic<Unmanaged>.GetOrCreate<Unmanaged>();

        internal Unit LineBufferAllocations;
        internal LineBuffer LineBuffer;
        internal bool Initialized;

        internal Font Font;
        // internal UnsafeArray<float4> ColorData;

        internal void Initialize(int maxLines, Font font)
        {
            Font = font;
            if (Initialized == false)
            {
                LineBuffer = new LineBuffer(maxLines);
                LineBufferAllocations = LineBuffer.AllocateAll();
                // ColorData = new UnsafeArray<float4>(maxLines);
                Initialized = true;
            }
        }

        internal void Clear()
        {
            LineBufferAllocations = LineBuffer.AllocateAll(); // clear out all the lines
        }

        internal void Dispose()
        {
            if (Initialized)
            {
                LineBuffer.Dispose();
                // ColorData.Dispose();
                Initialized = false;
            }
        }
    }
}
