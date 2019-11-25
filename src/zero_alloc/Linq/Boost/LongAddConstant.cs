using System.Runtime.CompilerServices;

namespace ZeroAlloc.Linq.Boost
{
    public struct LongAddConstant : ILinqUnaryOp<long, long>
    {
        private readonly long _rhs;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public LongAddConstant(long rhs)
        {
            _rhs = rhs;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public long Invoke(long arg)
        {
            return arg + _rhs;
        }

        public static Op<LongMultiplyOp<LongAddConstant>, long, long> operator *(LongAddConstant lhs, long rhs)
        {
            return new LongMultiplyOp<LongAddConstant>(lhs, rhs);
        }
    }
}