using Unity.Burst;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Entities;
using Unity.Mathematics;

namespace LineBurst
{
    struct Unmanaged
    {
        internal static readonly SharedStatic<Unmanaged> Instance = SharedStatic<Unmanaged>.GetOrCreate<Unmanaged>();

        internal bool Initialized;
        internal Unit LineBufferAllocations;
        internal UnsafeArray<float4> LineBuffer;

        internal BlobAssetReference<Font> Font;
        internal GraphSettings GraphSettings;
      
        // internal UnsafeArray<float4> ColorData;

        internal void Initialize(int maxLines, BlobAssetReference<Font> font, GraphSettings graphSettings)
        {
            GraphSettings = graphSettings;
            Font = font;
            if (Initialized == false)
            {
                LineBuffer = new UnsafeArray<float4>(maxLines * 2);
                Clear();
                // ColorData = new UnsafeArray<float4>(maxLines);
                Initialized = true;
            }
        }
        
        internal void SetLine(Line line, int index)
        {
            LineBuffer[index * 2] = line.Begin;
            LineBuffer[index * 2 + 1] = line.End;
        }

        internal unsafe void CopyFrom(void* ptr, int amount, int offset)
        {
            UnsafeUtility.MemCpy(LineBuffer.GetUnsafePtr() + 2 * offset, ptr, amount * UnsafeUtility.SizeOf<Line>());
        }

        internal void Clear()
        {
            LineBufferAllocations = new Unit(LineBuffer.Length / 2);
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
