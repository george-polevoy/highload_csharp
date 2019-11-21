using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace ZeroAlloc.Linq
{
    public static class OmniEnumerableExtensions
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long Sum<TState, TSource>(this OmniEnumerable<int, TState, TSource> source)
        {
            var sum = 0;
            foreach (var x in source)
            {
                sum += x;
            }
            return sum;
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static OmniEnumerable<
                T, StateTriple<TState, OmniEnumerator<TInit, TInitState>, Func<TState, TInit, T>>, StateTriple<
                    OmniEnumerable<TInit, TInitState, TInitSource>, TState, Func<TState, TInit, T>>>
            Select<T, TState, TInit, TInitState, TInitSource>(
                this OmniEnumerable<TInit, TInitState, TInitSource> source, TState state,
                Func<TState, TInit, T> selector)
        {
            return new
                OmniEnumerable<T, StateTriple<TState, OmniEnumerator<TInit, TInitState>, Func<TState, TInit, T>>,
                    StateTriple<OmniEnumerable<TInit, TInitState, TInitSource>, TState, Func<TState, TInit, T>>>(
                    (ref OmniEnumerator<T, StateTriple<TState, OmniEnumerator<TInit, TInitState>, Func<TState, TInit, T>>> enumerator) =>
                        enumerator.State.Item2.MoveNext(),
                    (ref OmniEnumerator<T, StateTriple<TState, OmniEnumerator<TInit, TInitState>, Func<TState, TInit, T>>> enumerator) =>
                        enumerator.State.Item3(enumerator.State.Item1, enumerator.State.Item2.Current),
                    triple => new StateTriple<TState, OmniEnumerator<TInit, TInitState>, Func<TState, TInit, T>>
                        {Item1 = triple.Item2, Item2 = triple.Item1.GetEnumerator(), Item3 = triple.Item3},
                    new StateTriple<OmniEnumerable<TInit, TInitState, TInitSource>, TState, Func<TState, TInit, T>>
                    {
                        Item1 = source,
                        Item2 = state,
                        Item3 = selector
                    }
                );
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static OmniEnumerable<
                T, StateTriple<TState, List<TInit>.Enumerator, Func<TState, TInit, T>>, StateTriple<
                    List<TInit>, TState, Func<TState, TInit, T>>>
            Select<T, TState, TInit>(
                this List<TInit> source, TState state,
                Func<TState, TInit, T> selector)
        {
            return new
                OmniEnumerable<T, StateTriple<TState, List<TInit>.Enumerator, Func<TState, TInit, T>>,
                    StateTriple<List<TInit>, TState, Func<TState, TInit, T>>>(
                    (ref OmniEnumerator<T, StateTriple<TState, List<TInit>.Enumerator, Func<TState, TInit, T>>> enumerator) =>
                        enumerator.State.Item2.MoveNext(),
                    (ref OmniEnumerator<T, StateTriple<TState, List<TInit>.Enumerator, Func<TState, TInit, T>>> enumerator) =>
                        enumerator.State.Item3(enumerator.State.Item1, enumerator.State.Item2.Current),
                    triple => new StateTriple<TState, List<TInit>.Enumerator, Func<TState, TInit, T>>
                        {Item1 = triple.Item2, Item2 = triple.Item1.GetEnumerator(), Item3 = triple.Item3},
                    new StateTriple<List<TInit>, TState, Func<TState, TInit, T>>
                    {
                        Item1 = source,
                        Item2 = state,
                        Item3 = selector
                    }
                );
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public static OmniEnumerable<T, StatePair<TState, List<T>.Enumerator>, StatePair<List<T>, TState>> Omni<T,
            TState>(this List<T> source,
            TState state)
        {
            return new OmniEnumerable<T, StatePair<TState, List<T>.Enumerator>, StatePair<List<T>, TState>>(
                (ref OmniEnumerator<T, StatePair<TState, List<T>.Enumerator>> enumerator)
                    => enumerator.State.Item2.MoveNext(),
                (ref OmniEnumerator<T, StatePair<TState, List<T>.Enumerator>> enumerator) =>
                    enumerator.State.Item2.Current,
                state1 => new StatePair<TState, List<T>.Enumerator>
                    {Item1 = state1.Item2, Item2 = state1.Item1.GetEnumerator()},
                new StatePair<List<T>, TState> {Item1 = source, Item2 = state});
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static OmniEnumerable<T, StatePair<NoState, List<T>.Enumerator>, StatePair<List<T>, NoState>> Omni<T>(this List<T> source)
        {
            return source.Omni(new NoState());
        }
    }
}