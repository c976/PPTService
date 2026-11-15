using System.Runtime.CompilerServices;

namespace PPTService.Ink.Core.Storage
{
	
	public interface ICloneable< out T> where T : SaveInfo
	{
		T Clone();
	}
}
