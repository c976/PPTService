using System;
using System.Text;

namespace Cvte.Paint.Framework
{
	public class SaveInfoException : Exception
	{
		public Type ElementType { get; }

		public string PropertyName { get; }

		public object PropertyValue { get; }

		public override string StackTrace
		{
			get
			{
				if (InnerException == null)
				{
					return base.StackTrace;
				}
				return InnerException.StackTrace;
			}
		}

		public SaveInfoException(string message, Type elementType, string propertyName, object propertyValue)
			: base(message)
		{
			ElementType = elementType;
			PropertyName = propertyName;
			PropertyValue = propertyValue;
		}

		public SaveInfoException(string message, Exception innerException, Type elementType)
			: base(message, innerException)
		{
			ElementType = elementType;
		}

		public SaveInfoException(string message, Exception innerException, Type elementType, string propertyName, object propertyValue)
			: base(message, innerException)
		{
			ElementType = elementType;
			PropertyName = propertyName;
			PropertyValue = propertyValue;
		}

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendLine(Message);
			stringBuilder.AppendLine("ElementType:" + ElementType.FullName);
			if (!string.IsNullOrEmpty(PropertyName))
			{
				stringBuilder.AppendLine("Property:" + PropertyName);
				stringBuilder.AppendLine($"PropertyValue:{PropertyValue}");
			}
			if (InnerException != null)
			{
				stringBuilder.AppendLine($"InnerException:{InnerException}");
			}
			return stringBuilder.ToString();
		}
	}
}
