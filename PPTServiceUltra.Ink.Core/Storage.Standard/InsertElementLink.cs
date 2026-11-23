using System.Runtime.CompilerServices;

namespace PPTServiceUltra.Ink.Core.Storage.Standard
{
	
	
	[ActionType("InsertElement")]
	public class InsertElementLink : Hyperlink
	{
		[ActionParameter("Name")]
		public string Name { get; set; }

		public InsertElementLink()
		{
		}

		public InsertElementLink(string name)
		{
			Name = name;
		}
	}
}
