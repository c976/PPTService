using System.Text.RegularExpressions;

namespace PPTService
{
	public static class RegexHelper
	{
		public static bool IsLocalPath(string path)
		{
			return new Regex("^(?<fpath>([a-zA-Z]:\\\\)([\\s\\.\\-\\w]+\\\\)*)(?<fname>[\\w]+.[\\w]+)").Match(path).Success;
		}
	}
}
