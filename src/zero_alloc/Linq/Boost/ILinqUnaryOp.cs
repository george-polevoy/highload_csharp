using System.Runtime.CompilerServices;

namespace ZeroAlloc.Linq.Boost
{
    public interface ILinqUnaryOp<in T0, out TResult, TSelectorState>
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        TResult Invoke(T0 arg, ref TSelectorState state);
    }

    public struct Param<T0, TSelectorState> : ILinqUnaryOp<T0, T0, TSelectorState>
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public T0 Invoke(T0 arg, ref TSelectorState state)
        {
            return arg;
        }
    }
    
    public struct Const<T0, TResult, TSelectorState> : ILinqUnaryOp<T0, TResult, TSelectorState>
    {
        private readonly TResult _value;

        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Const(TResult value)
        {
            _value = value;
        }

        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public TResult Invoke(T0 arg, ref TSelectorState state)
        {
            _ = arg;
            return _value;
        }
    }

    public struct SelectorState
    {
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Operations<TSelectorState> With<TSelectorState>()
        {
            return new Operations<TSelectorState>();
        }
    }

    public struct ParamBinder
    {
    }
    
    // ... 
    public struct Operations<TSelectorState>
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Op<Param<TSource, TSelectorState>, TSource, TSource, TSelectorState> Param<TSource>()
        {
            return new Op<Param<TSource, TSelectorState>, TSource, TSource, TSelectorState>(new Param<TSource, TSelectorState>());
        }

        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Op<Const<TSource, TResult, TSelectorState>, TSource, TResult, TSelectorState> Const<TSource, TResult>(TResult value)
        {
            return new Op<Const<TSource, TResult, TSelectorState>, TSource, TResult, TSelectorState>(new Const<TSource, TResult, TSelectorState>(value));    
        }
        
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Op<LongPlusLong<TSource, TLeftOp, TRightOp, TSelectorState>, TSource, long, TSelectorState> Plus<TSource, TLeftOp, TRightOp>(
            Op<TLeftOp, TSource, long, TSelectorState> left, Op<TRightOp, TSource, long, TSelectorState> right)
            where TLeftOp : ILinqUnaryOp<TSource, long, TSelectorState>
            where TRightOp : ILinqUnaryOp<TSource, long, TSelectorState>
        {
            return new LongPlusLong<TSource, TLeftOp, TRightOp, TSelectorState>(left.InnerOp, right.InnerOp);
        }
    }

    public struct LongPlusLong<TSource, TLeftOp, TRightOp, TSelectorState> : ILinqUnaryOp<TSource, long, TSelectorState>
        where TLeftOp : ILinqUnaryOp<TSource, long, TSelectorState>
        where TRightOp : ILinqUnaryOp<TSource, long, TSelectorState>
    {
        private readonly TLeftOp _left;

        private readonly TRightOp _right;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public LongPlusLong(TLeftOp left, TRightOp right)
        {
            _left = left;
            _right = right;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public long Invoke(TSource arg, ref TSelectorState state)
        {
            return _left.Invoke(arg, ref state) + _right.Invoke(arg, ref state);
        }
    }
}