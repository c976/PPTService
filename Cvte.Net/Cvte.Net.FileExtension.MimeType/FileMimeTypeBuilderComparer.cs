using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Cvte.Net.FileExtension.MimeType
{
	internal class FileMimeTypeBuilderComparer : IComparer<IFileMimeTypeBuilder>
	{
		
		public int Compare(IFileMimeTypeBuilder x, IFileMimeTypeBuilder y)
		{
			if (x == null && y == null)
			{
				return 0;
			}
			if (x == null)
			{
				return 1;
			}
			return y?.PopularLevel.CompareTo(x.PopularLevel) ?? (-1);
		}
	}
}
