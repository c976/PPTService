using System.Runtime.CompilerServices;

namespace PPTService.Ink.Core.Diagnostics.Trace
{
	
	public interface INotifyTracer
	{
		void Changing(ITracer newTracer);
	}
}
