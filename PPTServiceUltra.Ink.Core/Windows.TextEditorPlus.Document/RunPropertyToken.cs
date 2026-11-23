using System.Runtime.CompilerServices;

namespace PPTServiceUltra.Ink.Core.Windows.TextEditorPlus.Document
{
	
	public struct RunPropertyToken
	{
		public int Token { get; }

		public PropertyManager PropertyManager { get; }

		public RunPropertyToken(int token, PropertyManager propertyManager)
		{
			Token = token;
			PropertyManager = propertyManager;
		}
	}
}
