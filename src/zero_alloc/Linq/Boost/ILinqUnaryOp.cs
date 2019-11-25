using System.Diagnostics;

namespace ZeroAlloc.Linq.Boost
{
    public interface ILinqUnaryOp<in T0, out TResult>
    {
        TResult Invoke(T0 arg);
    }

    public struct Param<T0> : ILinqUnaryOp<T0, T0>
    {
        public T0 Invoke(T0 arg)
        {
            return arg;
        }
    }
    
    public struct Const<T0, TResult> : ILinqUnaryOp<T0, TResult>
    {
        private readonly TResult _value;

        public Const(TResult value)
        {
            _value = value;
        }

        public TResult Invoke(T0 arg)
        {
            _ = arg;
            return _value;
        }
    }

    public static class Operations
    {
        public static Op<Param<TSource>, TSource, TSource> Param<TSource>()
        {
            return new Op<Param<TSource>, TSource, TSource>(new Param<TSource>());
        }

        public static Op<Const<TSource, TResult>, TSource, TResult> Const<TSource, TResult>(TResult value)
        {
            return new Op<Const<TSource, TResult>, TSource, TResult>(new Const<TSource, TResult>(value));    
        }
        
        public static Op<LongPlusLongs<TSource, TLeftOp, TRightOp>, TSource, long> Plus<TSource, TLeftOp, TRightOp>(Op<TLeftOp, TSource, long> left, Op<TRightOp, TSource, long> right)
            where TLeftOp : ILinqUnaryOp<TSource, long>
            where TRightOp : ILinqUnaryOp<TSource, long>
        {
            return new LongPlusLongs<TSource, TLeftOp, TRightOp>(left.InnerOp, right.InnerOp);
        }
    }

    public struct LongPlusLongs<TSource, TLeftOp, TRightOp> : ILinqUnaryOp<TSource, long>
        where TLeftOp : ILinqUnaryOp<TSource, long>
        where TRightOp : ILinqUnaryOp<TSource, long>
    {
        private readonly TLeftOp _left;

        private readonly TRightOp _right;

        public LongPlusLongs(TLeftOp left, TRightOp right)
        {
            _left = left;
            _right = right;
        }

        public long Invoke(TSource arg)
        {
            return _left.Invoke(arg) + _right.Invoke(arg);
        }
    }
}