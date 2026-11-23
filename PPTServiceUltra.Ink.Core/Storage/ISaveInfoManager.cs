using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace PPTServiceUltra.Ink.Core.Storage
{
	
	public interface ISaveInfoManager
	{
		bool TryGet(string typeName, [NotNullWhen(true)] out SaveInfo saveInfo);
	}
}
