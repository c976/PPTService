using System;
using System.Diagnostics;

namespace Cvte.Paint.Framework
{
	public interface IEnforceResult
	{
		[DebuggerStepThrough]
		void OtherwiseThrow(Exception exception);
	}
}
