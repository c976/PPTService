using System;
using System.Runtime.CompilerServices;

namespace PPTServiceUltra.Ink.Core
{
	
	
	public class ValueChangeEventArgs< T> : EventArgs
	{
		public T OldValue { get; private set; }

		public T NewValue { get; private set; }

		public ValueChangeEventArgs(T oldValue, T newValue)
		{
			OldValue = oldValue;
			NewValue = newValue;
		}
	}
}
