using System.Runtime.CompilerServices;

namespace ZeroAlloc.Linq.Boost
{
    public struct LongAddConstant<TLhs, TSelectorState> : ILinqUnaryOp<long, long, TSelectorState>
        where TLhs : ILinqUnaryOp<long, long, TSelectorState>
    {
        private readonly long _rhs;
        private readonly TLhs _lhs;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public LongAddConstant(TLhs lhs, long rhs)
        {
            _rhs = rhs;
            _lhs = lhs;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public long Invoke(long arg, ref TSelectorState state)
        {
            return _lhs.Invoke(arg, ref state)  + _rhs;
        }

        public static Op<LongMultiplyOp<
            LongAddConstant<TLhs, TSelectorState>, TSelectorState>, long, long, TSelectorState> operator *(
            LongAddConstant<TLhs, TSelectorState> lhs, long rhs)
        {
            return new LongMultiplyOp<LongAddConstant<TLhs, TSelectorState>, TSelectorState>(lhs, rhs);
        }
    }
}