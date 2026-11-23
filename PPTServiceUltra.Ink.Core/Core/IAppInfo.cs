using System.Runtime.CompilerServices;

namespace PPTServiceUltra.Ink.Core
{
	
	public interface IAppInfo
	{
		string CopyRight { get; }

		string Version { get; }

		string ProductName { get; }

		string Company { get; }

		string this[string key] { get; }
	}
}
