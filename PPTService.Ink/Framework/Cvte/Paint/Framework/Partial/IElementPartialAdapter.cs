using System;
using Cvte.Diagnostics.Annotations;

namespace Cvte.Paint.Framework.Partial
{
	public interface IElementPartialAdapter
	{
		Type ElementType { get; }
	}
	public interface IElementPartialAdapter<in T> : IElementPartialAdapter where T : Element
	{
		[NotNull]
		ElementPartialData GetPartCollection(T element);
	}
}
