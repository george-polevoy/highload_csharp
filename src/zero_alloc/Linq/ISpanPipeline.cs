using System;

namespace ZeroAlloc.Linq
{
    /// <summary>
    /// ISpanEnumerator{TStart, T} marker interface for off-tear enumeration.
    /// </summary>
    /// <typeparam name="TStart"></typeparam>
    /// <typeparam name="T"></typeparam>
    public interface ISpanPipeline<TStart, out T>
    {
        bool MoveNext(ref Span<TStart>.Enumerator enumerator);
        T GetCurrent(ref Span<TStart>.Enumerator enumerator);
    }
}