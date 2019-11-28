using System;

namespace ZeroAlloc.Linq
{
    /// <summary>
    /// SpanEnumerationBuilder starting point of enumeration. 
    /// </summary>
    /// <typeparam name="TStart">The element type of a <see cref="Span{TStart}"/>
    /// which will be the source of enumeration.
    /// </typeparam>
    /// <typeparam name="TSelectorState">State parameter type.</typeparam>
    /// <typeparam name="T">Resulting selection type.</typeparam>
    public interface ISpanPipeline<TStart, out T, TSelectorState>
    {
        bool MoveNext(ref Span<TStart>.Enumerator enumerator, ref TSelectorState state);
        T GetCurrent(ref Span<TStart>.Enumerator enumerator, ref TSelectorState state);
    }
}