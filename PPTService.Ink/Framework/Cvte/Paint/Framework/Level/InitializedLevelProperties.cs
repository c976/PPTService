using System;

namespace Cvte.Paint.Framework.Level
{
	public class InitializedLevelProperties
	{
		public ILevelElement TargetElement { get; }

		public InsertAction InsertAction { get; private set; }

		public InitializedLevelProperties(ILevelElement targetElement, InsertAction insertAction)
		{
			if ((insertAction == InsertAction.Before || insertAction == InsertAction.After) && targetElement == null)
			{
				throw new ArgumentException("在设置相对一个元素的" + ((insertAction == InsertAction.Before) ? "下" : "上") + "一层的时候，相对的元素不能是空");
			}
			TargetElement = targetElement;
			InsertAction = insertAction;
		}

		public static InitializedLevelProperties AddAfter(ILevelElement targetElement)
		{
			return new InitializedLevelProperties(targetElement, InsertAction.After);
		}

		public static InitializedLevelProperties AddBefore(ILevelElement targetLevelInfo)
		{
			return new InitializedLevelProperties(targetLevelInfo, InsertAction.Before);
		}
	}
}
