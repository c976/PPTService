using System.Runtime.CompilerServices;

namespace PPTServiceUltra.Ink.Core.Diagnostics.Trace
{
	
	public interface INotifyTracer
	{
		void Changing(ITracer newTracer);
	}
}
