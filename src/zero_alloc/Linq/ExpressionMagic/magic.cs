using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace ZeroAlloc.Linq.ExpressionMagic
{
    public class magic
    {
        private ConcurrentDictionary<RuntimeMethodHandle, Delegate> _delegates = new ConcurrentDictionary<RuntimeMethodHandle, Delegate>();

        public void Do<T, TResult>(Span<T> source, Func<
            Expression<Func<IEnumerable<T>, IEnumerable<TResult>>>> expressionFactory)
        {
            //var existing = _delegates.GetOrAdd(expressionFactory.Method.MethodHandle, h => );
        }

        public void Do()
        {
            Span<long> span = stackalloc long[] {1, 2, 3};
            Do<long, long>(span, () => l => l
                .Select(x => x + 1)
                .Select(x => x + 2)
                .Select(x => x + 3)
                .Select(x => x + 4));

            Expression.Block();
        }
    }
}