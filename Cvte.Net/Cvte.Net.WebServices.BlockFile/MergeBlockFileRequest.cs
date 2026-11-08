using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using System.Text;
using Cvte.Net.Http;

namespace Cvte.Net.WebServices.BlockFile
{
	
	
	[DataContract]
	public class MergeBlockFileRequest : JsonRequest
	{
		public string Data { get; set; }

		public override byte[] GetRequestData()
		{
			return Encoding.Default.GetBytes(Data);
		}
	}
}
