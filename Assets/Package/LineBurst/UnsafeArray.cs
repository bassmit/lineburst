using System;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;

namespace LineBurst
{
    unsafe struct UnsafeArray<T> : IDisposable where T : unmanaged
    {
        readonly T* _pointer;
        internal T* GetUnsafePtr() => _pointer;

        internal UnsafeArray(int length)
        {
            var size = UnsafeUtility.SizeOf<T>() * length;
            var alignment = UnsafeUtility.AlignOf<T>();
            _pointer = (T*)UnsafeUtility.Malloc(size, alignment, Allocator.Persistent);
            Length = length;
        }

        public void Dispose()
        {
            UnsafeUtility.Free(_pointer, Allocator.Persistent);
        }

        internal int Length { get; }

        internal ref T this[int index] => ref UnsafeUtility.AsRef<T>(_pointer + index);

        internal NativeArray<T> ToNativeArray()
        {
            var array = NativeArrayUnsafeUtility.ConvertExistingDataToNativeArray<T>(_pointer, Length, Allocator.Invalid);
#if ENABLE_UNITY_COLLECTIONS_CHECKS
            NativeArrayUnsafeUtility.SetAtomicSafetyHandle(ref array, AtomicSafetyHandle.GetTempUnsafePtrSliceHandle());
#endif
            return array;
        }
    }
}