using System;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;

namespace Cvte.Net.WebServices.Courseware
{
	
	
	public class CoursewareFile
	{
		private int _typeCode;

		[DataMember(Name = "name")]
		public string Name { get; set; }

		[DataMember(Name = "hash")]
		public string Hash { get; set; }

		[DataMember(Name = "path")]
		public string Path { get; set; }

		[DataMember(Name = "size")]
		public long Size { get; set; }

		public CoursewareFileType Type { get; set; }

		[DataMember(Name = "type")]
		internal int TypeCode
		{
			get
			{
				return _typeCode;
			}
			set
			{
				CoursewareFileType result;
				Type = (Enum.TryParse<CoursewareFileType>(value.ToString(), out result) ? result : CoursewareFileType.Unknown);
				_typeCode = value;
			}
		}
	}
}
