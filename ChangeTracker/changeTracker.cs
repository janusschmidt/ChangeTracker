using System;
using System.Linq.Expressions;

namespace changeTracker
{
    public class ChangeTracker<TObj>
    {
        private readonly TObj source;
        public bool HasBeenChanged;

        public ChangeTracker(TObj source)
        {
            this.source = source;
        }

        public ChangeTracker<TObj> Set<TProp>(Expression<Func<TObj, TProp>> getter, TProp newvalue)
        {
            var oldValue = Expression.Lambda<Func<TObj, TProp>>(getter.Body, getter.Parameters).Compile()(source);
            if ((oldValue == null && newvalue != null) || (oldValue != null && !oldValue.Equals(newvalue)))
            {
                var stterExpr= Expression.Assign(getter.Body, Expression.Constant(newvalue));
                var lambda = Expression.Lambda<Action<TObj>>(stterExpr, getter.Parameters).Compile();
                lambda(source);
                HasBeenChanged = true;
            }
            return this;
        }

        public void ResetHasBeenChanged()
        {
            HasBeenChanged = false;
        }
    }
}
