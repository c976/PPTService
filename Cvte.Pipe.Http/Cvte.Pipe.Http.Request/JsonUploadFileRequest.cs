using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Text;

namespace Cvte.Pipe.Http.Request
{
	[DataContract]
	public abstract class JsonUploadFileRequest : UploadFileRequest
	{
		public override IDictionary<string, string> GetParams()
		{
			using (MemoryStream memoryStream = new MemoryStream())
			{
				new DataContractJsonSerializer(GetType()).WriteObject((Stream)memoryStream, (object)this);
				return new Dictionary<string, string> { 
				{
					GetJsonBodyKey(),
					Encoding.UTF8.GetString(memoryStream.ToArray())
				} };
			}
		}

		protected abstract string GetJsonBodyKey();
	}
}
