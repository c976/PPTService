using System.Runtime.CompilerServices;

namespace PPTService.Ink.Core.Storage.Standard
{
	
	
	[ActionType("OpenTool")]
	public class OpenToolLink : Hyperlink
	{
		[ActionParameter("Name")]
		public string Name { get; set; }

		public OpenToolLink()
		{
		}

		public OpenToolLink(string name)
		{
			Name = name;
		}
	}
}
