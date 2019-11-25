using System.Runtime.CompilerServices;

namespace ZeroAlloc.Linq.Boost
{
    public struct LongMultiplyOp<TLhs> : ILinqUnaryOp<long, long>
        where TLhs : ILinqUnaryOp<long, long>
    {
        private readonly long _rhs;
        private readonly TLhs _lhs;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public LongMultiplyOp(TLhs lhs, long rhs)
        {
            _rhs = rhs;
            _lhs = lhs;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public long Invoke(long arg)
        {
            return _lhs.Invoke(arg) + _rhs;
        }
    }
}