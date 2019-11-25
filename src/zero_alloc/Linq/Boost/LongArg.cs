using System.Runtime.CompilerServices;

namespace ZeroAlloc.Linq.Boost
{
    public struct LongArg
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Op<LongAddConstant, long, long> operator +(LongArg lhs, long right)
        {
            return new LongAddConstant(right);
        }
    }
}