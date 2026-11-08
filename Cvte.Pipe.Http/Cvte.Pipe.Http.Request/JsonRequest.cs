using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;

namespace Cvte.Pipe.Http.Request
{
	[DataContract]
	public abstract class JsonRequest : RequestBase
	{
		public override string ContentType => "application/json;charset=utf-8";

		public override byte[] GetRequestData()
		{
			using (MemoryStream memoryStream = new MemoryStream())
			{
				new DataContractJsonSerializer(GetType()).WriteObject((Stream)memoryStream, (object)this);
				return memoryStream.ToArray();
			}
		}
	}
}
