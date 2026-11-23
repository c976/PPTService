using System.Runtime.CompilerServices;

namespace PPTServiceUltra.Ink.Core.Storage
{
	
	public interface ICloneable< out T> where T : SaveInfo
	{
		T Clone();
	}
}
