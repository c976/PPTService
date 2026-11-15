using System;
using System.Diagnostics;

namespace PPTService.Paint.Framework
{
	public interface IEnforceResult
	{
		[DebuggerStepThrough]
		void OtherwiseThrow(Exception exception);
	}
}
