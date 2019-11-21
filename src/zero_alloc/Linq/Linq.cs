using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace ZeroAlloc.Linq
{
    public delegate bool MoveNexter<T, TState>(ref OmniEnumerator<T, TState> enumerator);

    public delegate T CurrentGetter<T, TState>(ref OmniEnumerator<T, TState> enumerator);

    public delegate TState StateCreator<in TSource, out TState>(TSource source);

    public interface IEnumerationEngine<TState, T>
    {
        bool MoveNext(ref TState state);
        T GetCurrent(ref TState state);
        void Destroy(ref TState state);
    }

    public interface IEnumeratorProvider<in TEnumerableState, out TEnumeratorState>
    {
        TEnumeratorState Create(TEnumerableState source);
    }

    public class SelectEnumerationEngine<T, TResult, TEnumerator, TState>
        where TEnumerator : struct, IEnumerator<T>
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool MoveNext(ref SelectEnumeratorState<T, TResult, TEnumerator, TState> state) =>
            state.Enumerator.MoveNext();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public TResult GetCurrent(ref SelectEnumeratorState<T, TResult, TEnumerator, TState> state) =>
            state.Selector(state.State, state.Enumerator.Current);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Destroy(ref SelectEnumeratorState<T, TResult, TEnumerator, TState> state) =>
            state.Enumerator.Dispose();
    }

    public struct SelectEnumeratorState<T, TResult, TEnumerator, TState>
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public SelectEnumeratorState(TEnumerator enumerator, TState state, Func<TState, T, TResult> selector)
        {
            Enumerator = enumerator;
            State = state;
            Selector = selector;
        }

        public TEnumerator Enumerator;
        public readonly TState State;
        public readonly Func<TState, T, TResult> Selector;
    }

    public struct MyListEnumerator<T> : IEnumerator<T>
    {
        private readonly List<T> _source;
        private int _index;

        public MyListEnumerator(List<T> source)
        {
            _source = source;
            _index = -1;
        }

        public bool MoveNext()
        {
            if (_index >= _source.Count - 1)
            {
                return false;
            }

            _index++;
            return true;
        }

        public void Reset()
        {
            _index = 0;
        }

        public T Current
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => _source[_index];
        }

        object IEnumerator.Current => Current;

        public void Dispose()
        {
        }
    }
    
    public struct ListEnumerableState<T, TResult, TState>
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ListEnumerableState(List<T> enumerable, TState state, Func<TState, T, TResult> selector)
        {
            Enumerable = enumerable;
            State = state;
            Selector = selector;
        }

        public readonly List<T> Enumerable;
        public readonly TState State;
        public readonly Func<TState, T, TResult> Selector;
    }

    public class ListSelectEnumerationProvider<T, TResult, TState>
        : SelectEnumerationEngine<T, TResult, MyListEnumerator<T>, TState>,
            IEnumerationEngine<SelectEnumeratorState<T, TResult, MyListEnumerator<T>, TState>, TResult>,
            IEnumeratorProvider<
                ListEnumerableState<T, TResult, TState>,
                SelectEnumeratorState<T, TResult, MyListEnumerator<T>, TState>>
    {
        public static readonly ListSelectEnumerationProvider<T, TResult, TState> Instance =
            new ListSelectEnumerationProvider<T, TResult, TState>();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public SelectEnumeratorState<T, TResult, MyListEnumerator<T>, TState> Create(
            ListEnumerableState<T, TResult, TState> source)
        {
            return new SelectEnumeratorState<T, TResult, MyListEnumerator<T>, TState>
            (
                new MyListEnumerator<T>(source.Enumerable),
                source.State,
                source.Selector
            );
        }
    }
    
    public static class SelectEnumerableExtensions
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static
            ProviderEnumerable<TResult, ListEnumerableState<T, TResult, TState>,
                SelectEnumeratorState<T, TResult, MyListEnumerator<T>, TState>,
                ListSelectEnumerationProvider<T, TResult, TState>> SelectWithProvider<T, TResult, TState>(
                this List<T> source, TState state, Func<TState, T, TResult> selector)
        {
            return new ProviderEnumerable<
                TResult,
                ListEnumerableState<T, TResult, TState>,
                SelectEnumeratorState<T, TResult, MyListEnumerator<T>, TState>,
                ListSelectEnumerationProvider<T, TResult, TState>>(
                ListSelectEnumerationProvider<T, TResult, TState>.Instance,
                new ListEnumerableState<T, TResult, TState>(

                    source, state, selector
                ));
        }
    }

    public readonly ref struct ProviderEnumerable<T, TEnumerableState, TEnumeratorState, TEnumeratorProviderEngine> //: IEnumerable<T>
        //where TSourceEnumerator : IEnumerator<T>
        where TEnumeratorProviderEngine
        : IEnumeratorProvider<TEnumerableState, TEnumeratorState>, IEnumerationEngine<TEnumeratorState, T>
    {
        private readonly TEnumeratorProviderEngine _engine;
        private readonly TEnumerableState _enumerableState;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ProviderEnumerable(TEnumeratorProviderEngine engine, TEnumerableState enumerableState)
        {
            _engine = engine;
            _enumerableState = enumerableState;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ProviderEnumerator<T, TEnumeratorState> GetEnumerator()
        {
            return new ProviderEnumerator<T, TEnumeratorState>(_engine, _engine.Create(_enumerableState));
        }

//        IEnumerator<T> IEnumerable<T>.GetEnumerator()
//        {
//            return GetEnumerator();
//        }

//        IEnumerator IEnumerable.GetEnumerator()
//        {
//            return GetEnumerator();
//        }
    }

    public ref struct ProviderEnumerator<T, TState>
    {
        private IEnumerationEngine<TState, T> _enumerationEngine;
        private TState _state;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ProviderEnumerator(
            IEnumerationEngine<TState, T> enumerationEngine, TState state)
        {
            _enumerationEngine = enumerationEngine;
            _state = state;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool MoveNext() => _enumerationEngine.MoveNext(ref _state);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Reset() => throw new NotImplementedException();

        public T Current
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => _enumerationEngine.GetCurrent(ref _state);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Dispose() => _enumerationEngine.Destroy(ref _state);
    }

    public ref struct StandaloneSelectEnumerator<T, TResult, TState, TSourceEnumerator>
        //: IEnumerator<TResult>
        where TSourceEnumerator : IEnumerator<T>
    {
        private TState _state;

        private TSourceEnumerator _source;

        private Func<TState, T, TResult> _selector;

        public StandaloneSelectEnumerator(TState state, TSourceEnumerator source, Func<TState, T, TResult> selector)
        {
            _state = state;
            _source = source;
            _selector = selector;
        }

        public bool MoveNext() => _source.MoveNext();

        public TResult Current => _selector(_state, _source.Current);

        public void Dispose()
        {
            _source.Dispose();
        }

        public void Reset()
        {
            _source.Reset();
        }

        //object? IEnumerator.Current => Current;
    }

    public static class FastEnumeration
    {
        public static StandaloneSelectEnumerator<T, TResult, TState, TEnumerator>
            Select1<T, TResult, TEnumerator, TState>(this TEnumerator source, TState state,
                Func<TState, T, TResult> selector) where TEnumerator : IEnumerator<T>
        {
            return new StandaloneSelectEnumerator<T, TResult, TState, TEnumerator>(state, source, selector);
        }

        public static long Sum<TEnumerator>(this TEnumerator source) where TEnumerator : IEnumerator<long>
        {
            long s = 0;
            while (source.MoveNext())
            {
                s += source.Current;
            }

            return s;
        }
        
        public static long Sum<T, TState, TEnumerator>(this StandaloneSelectEnumerator<T, long, TState, TEnumerator> source) where TEnumerator : IEnumerator<T>
        {
            long s = 0;
            while (source.MoveNext())
            {
                s += source.Current;
            }

            return s;
        }
        
        public static long Sum<TEnumerableState, TEnumeratorState, TEnumeratorProviderEngine>(
            this ProviderEnumerable<long, TEnumerableState, TEnumeratorState, TEnumeratorProviderEngine> source)
        where TEnumeratorProviderEngine : IEnumeratorProvider<TEnumerableState, TEnumeratorState>,
        IEnumerationEngine<TEnumeratorState, long>
        {
            long s = 0;
            foreach (var i in source)
            {
                s += i;
            }

            return s;
        }
    }

    public struct StatePair<T1, T2>
    {
        public T1 Item1;
        public T2 Item2;
    }

    public struct StateTriple<T1, T2, T3>
    {
        public T1 Item1;
        public T2 Item2;
        public T3 Item3;
    }

    public struct NoState
    {
    }

    public struct OmniEnumerator<T, TState> : IEnumerator<T>
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public OmniEnumerator(MoveNexter<T, TState> moveNexter, CurrentGetter<T, TState> currentGetter, TState state)
        {
            _moveNexter = moveNexter;
            _currentGetter = currentGetter;
            State = state;
        }

        private readonly MoveNexter<T, TState> _moveNexter;

        private readonly CurrentGetter<T, TState> _currentGetter;

        public TState State;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool MoveNext() => _moveNexter(ref this);

        public void Reset()
        {
            throw new NotImplementedException();
        }

        object? IEnumerator.Current => Current;

        public T Current
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => _currentGetter(ref this);
        }

        public void Dispose()
        {
        }
    }
}