using System.Threading;
using Unity.Mathematics;

namespace LineBurst
{
    struct Unit
    {
        internal int Begin;
        internal int Next;
        internal int End;

        internal Unit AllocateAtomic(int count)
        {
            var begin = Next;
            while (true)
            {
                var end = math.min(begin + count, End);
                if (begin == end)
                    return default;
                var found = Interlocked.CompareExchange(ref Next, end, begin);
                if (found == begin)
                    return new Unit { Begin = begin, Next = begin, End = end };
                begin = found;
            }
        }

        internal Unit(int count)
        {
            Begin = Next = 0;
            End = count;
        }

        internal int Filled => Next - Begin;
    }
}
