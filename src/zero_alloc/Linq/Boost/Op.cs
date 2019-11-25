using System.Runtime.CompilerServices;

namespace ZeroAlloc.Linq.Boost
{
    public struct Op<TOp, T0, TResult> //: ILinqUnaryOp<T0, TResult>
        where TOp : ILinqUnaryOp<T0, TResult>
    {
        public TOp InnerOp { get; }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Op(TOp innerOp)
        {
            InnerOp = innerOp;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator Op<TOp, T0, TResult>(TOp innerOp)
        {
            return new Op<TOp, T0, TResult>(innerOp);
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator TOp(Op<TOp, T0, TResult> outerOp)
        {
            return outerOp.InnerOp;
        }
    }
}