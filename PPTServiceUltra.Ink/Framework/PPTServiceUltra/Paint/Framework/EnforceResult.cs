using System;
using System.Diagnostics;

namespace PPTServiceUltra.Paint.Framework
{
	internal class EnforceResult : IEnforceResult
	{
		private readonly bool _result;

		public EnforceResult(bool result)
		{
			_result = result;
		}

		[DebuggerStepThrough]
		public void OtherwiseThrow(Exception exception)
		{
			if (!_result)
			{
				throw exception;
			}
		}
	}
}
