using System;
using System.Linq.Expressions;

namespace CodeSanook.Common.DataTypes
{
    public static class ClassHelper
    {

        public static string PropertyAsString<TObject, TProperty>(this TObject obj, Expression<Func<TObject, TProperty>> propertySelector)
            where TObject : class
        {
            var memberExpression = propertySelector.Body as MemberExpression;
            return memberExpression.Member.Name;
        }
    }
}