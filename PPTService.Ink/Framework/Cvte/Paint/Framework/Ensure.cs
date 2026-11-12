using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq.Expressions;

namespace Cvte.Paint.Framework
{
	public static class Ensure
	{
		[DebuggerStepThrough]
		public static void ArgumentIsNotNull<T>(Expression<Func<T>> arguementNameExpression) where T : class
		{
			if (arguementNameExpression.Compile()() == null)
			{
				throw new ArgumentNullException(GetMemberName(arguementNameExpression));
			}
		}

		[DebuggerStepThrough]
		public static void ItemIsNotNull<T>(IList<T> list, string paramName = "") where T : class
		{
			for (int i = 0; i < list.Count; i++)
			{
				if (list[i] == null)
				{
					throw new ArgumentException($"集合中第 {i} 个元素为 null", paramName)
					{
						Data = 
						{
							{
								(object)"为空的项",
								(object)i
							},
							{
								(object)"ArgumentName",
								(object)paramName
							}
						}
					};
				}
			}
		}

		[DebuggerStepThrough]
		public static IEnforceResult That(bool conditionSatisfied)
		{
			return new EnforceResult(conditionSatisfied);
		}

		private static string GetMemberName<T>(Expression<Func<T>> memberExpression)
		{
			return ((MemberExpression)memberExpression.Body).Member.Name;
		}
	}
}
