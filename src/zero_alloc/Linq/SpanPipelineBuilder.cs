using System;
using System.Runtime.CompilerServices;

namespace ZeroAlloc.Linq
{
    /// <summary>
    /// SpanEnumerationBuilder starting point of enumeration. 
    /// </summary>
    /// <typeparam name="T">The element type of a <see cref="Span{T}"/>
    /// which will be the source of enumeration.
    /// </typeparam>
    public readonly struct SpanPipelineBuilder<T> : ISpanPipeline<T, T>
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool MoveNext(ref Span<T>.Enumerator enumerator)
        {
            return enumerator.MoveNext();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public T GetCurrent(ref Span<T>.Enumerator enumerator)
        {
            return enumerator.Current;
        }
    }
}