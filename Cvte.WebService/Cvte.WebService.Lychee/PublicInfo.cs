using System.Runtime.CompilerServices;

namespace Cvte.WebService.Lychee
{
	public class PublicInfo
	{
		private string string_0;

		private string string_1;

		private double double_0;

		private string string_2;

		public string Title
		{
			get
			{
				return string_0;
			}
			set
			{
				string_0 = value;
			}
		}

		public string Author
		{
			get
			{
				return string_1;
			}
			set
			{
				string_1 = value;
			}
		}

		public double Scale
		{
			get
			{
				return double_0;
			}
			set
			{
				double_0 = value;
			}
		}

		public string Device
		{
			get
			{
				return string_2;
			}
			set
			{
				string_2 = value;
			}
		}
	}
}
