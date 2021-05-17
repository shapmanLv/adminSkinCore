using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace AdminSkinCore.Api.Common.Helper.ExpressionExtend
{
    /// <summary>
    /// 合并 lambda 表达式
    /// 来自： https://blog.csdn.net/xuchen_wang/article/details/92622548
    /// </summary>
    public static class ExpressionHelper
    {
        private static Expression<T> Combine<T>(this Expression<T> first, Expression<T> second, Func<Expression, Expression, Expression> merge)
        {
            MyExpressionVisitor visitor = new MyExpressionVisitor(first.Parameters[0]);
            Expression bodyone = visitor.Visit(first.Body);
            Expression bodytwo = visitor.Visit(second.Body);
            return Expression.Lambda<T>(merge(bodyone, bodytwo), first.Parameters[0]);
        }
        public static Expression<Func<T, bool>> ExpressionAnd<T>(this Expression<Func<T, bool>> first, Expression<Func<T, bool>> second)
        {
            return first.Combine(second, Expression.And);
        }
        public static Expression<Func<T, bool>> ExpressionOr<T>(this Expression<Func<T, bool>> first, Expression<Func<T, bool>> second)
        {
            return first.Combine(second, Expression.Or);
        }       
    }
}
