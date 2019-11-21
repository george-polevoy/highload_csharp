using System.Runtime.CompilerServices;

namespace ZeroAlloc.Linq
{
    public struct OmniEnumerable<T, TState, TSource> //: IEnumerable<T>
    {
        private readonly TSource _source;
        private MoveNexter<T, TState> _moveNext;
        private CurrentGetter<T, TState> _current;
        private StateCreator<TSource, TState> _createState;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public OmniEnumerable(
            MoveNexter<T, TState> moveNext,
            CurrentGetter<T, TState> current,
            StateCreator<TSource, TState> createState,
            TSource source)
        {
            _moveNext = moveNext;
            _current = current;
            _createState = createState;
            _source = source;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public OmniEnumerator<T, TState> GetEnumerator()
        {
            return new OmniEnumerator<T, TState>(_moveNext, _current, _createState(_source));
        }
    }
}