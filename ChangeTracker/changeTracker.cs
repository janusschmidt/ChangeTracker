using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace changeTracker
{
    public class ChangeTracker<TObj>
    {
        private readonly TObj source;
        List<Expression> expressions=new List<Expression>();

        public ChangeTracker(TObj source)
        {
            this.source = source;
        }

        public ChangeTracker<TObj> Set<TProp>(Expression<Func<TObj, TProp>> getter, TProp newvalue)
        {
          var oldval = Expression.Invoke(getter, Expression.Constant(source));
          var newval = Expression.Constant(newvalue);
          var returnTarget = Expression.Label(typeof(bool));

          Expression<Func<TProp,TProp,bool>> valueChangedExpr1 = 
            (o, n) => (o == null && n != null) || (o != null && !o.Equals(n));

          var mainBlock =
            Expression.Block(
            Expression.IfThen(Expression.Invoke(valueChangedExpr1, oldval, newval),
              Expression.Block(
                Expression.Assign(getter.Body, newval),
                Expression.Return(returnTarget, Expression.Constant(true)))),
            Expression.Label(returnTarget, Expression.Constant(false)));

          var lambda = Expression.Lambda<Func<TObj, bool>>(mainBlock, getter.Parameters);
          expressions.Add(lambda);
      
          return this;
        }

        /// <summary>
        /// Executes all set operations and returns a bool if a field has been changed.
        /// </summary>
        /// <returns></returns>
        public bool Execute()
        {
          var obj = Expression.Constant(source);
          var currentExpr = Expression.OrElse(Expression.Constant(false), Expression.Constant(false));
          foreach (var expr in expressions)
          {
            currentExpr = Expression.OrElse(currentExpr, Expression.Invoke(expr, obj));
          }
          return Expression.Lambda<Func<bool>>(currentExpr).Compile()();
        }

        public void Reset()
        {
            expressions = new List<Expression>();
        }
    }
}
