using System.Runtime.CompilerServices;

namespace ZeroAlloc.Linq.Boost
{
    public struct Op<TOp, T0, TResult, TSelectorState>
        where TOp : ILinqUnaryOp<T0, TResult, TSelectorState>
    {
        public TOp InnerOp { get; }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Op(TOp innerOp)
        {
            InnerOp = innerOp;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator Op<TOp, T0, TResult, TSelectorState>(TOp innerOp)
        {
            return new Op<TOp, T0, TResult, TSelectorState>(innerOp);
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator TOp(Op<TOp, T0, TResult, TSelectorState> outerOp)
        {
            return outerOp.InnerOp;
        }
    }
}