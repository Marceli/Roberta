using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Core.Extensions
{
	public static class QueryableExtension
	{
		public static IQueryable<TEntity> OrderBy<TEntity>(this IQueryable<TEntity> source, string sortExpression)
			where TEntity : class
		{
			var type = typeof (TEntity);
			var expressionParts = sortExpression.Split(' ');
			var orderByProperty = expressionParts[0].Split('.');
			var methodName = "OrderBy";
			if (expressionParts.Length > 1 && expressionParts[1] == "desc")
			{
				methodName += "Descending"; // Add sort direction at the end of Method name   
			}
			var property = type.GetProperty(orderByProperty[0]);
			Type type2;
			PropertyInfo property2 = null;
			var parameter = Expression.Parameter(type, "p");
			var propertyAccess = Expression.MakeMemberAccess(parameter, property);
			MemberExpression propertyAccess3 = null;
			if (orderByProperty.Length > 1)
			{
				type2 = property.PropertyType;
				property2 = type2.GetProperty(orderByProperty[1]);
				propertyAccess3 = Expression.MakeMemberAccess(propertyAccess, property2);
			}
			LambdaExpression orderByExp;
			MethodCallExpression resultExp;
			if (propertyAccess3 != null)
			{
				orderByExp = Expression.Lambda(propertyAccess3, parameter);
				resultExp = Expression.Call(typeof (Queryable), methodName,
				                            new[] {type, property2.PropertyType},
				                            source.Expression, Expression.Quote(orderByExp));
			}
			else
			{
				orderByExp = Expression.Lambda(propertyAccess, parameter);
				resultExp = Expression.Call(typeof (Queryable), methodName,
				                            new[] {type, property.PropertyType},
				                            source.Expression, Expression.Quote(orderByExp));
			}
			return (IQueryable<TEntity>) source.Provider.CreateQuery(resultExp);
		}
	}
}