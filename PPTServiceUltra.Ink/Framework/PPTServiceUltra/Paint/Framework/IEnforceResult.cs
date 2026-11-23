using System;
using System.Diagnostics;

namespace PPTServiceUltra.Paint.Framework
{
	public interface IEnforceResult
	{
		[DebuggerStepThrough]
		void OtherwiseThrow(Exception exception);
	}
}
