/*
 * Author:			Vex Tatarevic
 * Date Created:	2013-06-12
 * Copyright:       VEX IT Pty Ltd 2013 - www.vexit.com
 * 
 * Description:     Extension methods for dynamic IQueryable Ordering
 * 
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Web;

namespace vEX.DataAccess
{
    /// <summary>
    ///  This class provides Extension methods for dynamic IQueryable Ordering used in VEXIT ModMVC framework
    /// </summary>
    public static class ExtIQueryable
    {
        public static IOrderedQueryable<T> OrderBy<T>(this IQueryable<T> source, string sortField, bool sortAscending = true)
        {
            string direction = sortAscending ? "" : "Descending";
            return ApplyOrder<T>(source, sortField, "OrderBy" + direction);
        }
        public static IOrderedQueryable<T> ThenBy<T>(this IOrderedQueryable<T> source, string sortField, bool sortAscending = true)
        {
            string direction = sortAscending ? "" : "Descending";
            return ApplyOrder<T>(source, sortField, "ThenBy" + direction);
        }
        static IOrderedQueryable<T> ApplyOrder<T>(IQueryable<T> source, string property, string methodName)
        {
            string[] props = property.Split('.');
            Type type = typeof(T);
            ParameterExpression arg = Expression.Parameter(type, "x");
            Expression expr = arg;
            foreach (string prop in props)
            {
                // use reflection (not ComponentModel) to mirror LINQ
                PropertyInfo pi = type.GetProperty(prop);
                expr = Expression.Property(expr, pi);
                type = pi.PropertyType;
            }
            Type delegateType = typeof(Func<,>).MakeGenericType(typeof(T), type);
            LambdaExpression lambda = Expression.Lambda(delegateType, expr, arg);

            object result = typeof(Queryable).GetMethods().Single(
                    method => method.Name == methodName
                            && method.IsGenericMethodDefinition
                            && method.GetGenericArguments().Length == 2
                            && method.GetParameters().Length == 2
                            )
                    .MakeGenericMethod(typeof(T), type)
                    .Invoke(null, new object[] { source, lambda });
            return (IOrderedQueryable<T>)result;
        }


    }
}