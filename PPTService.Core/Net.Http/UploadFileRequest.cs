using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace PPTService.Core.Net.Http
{
	
	
	public class UploadFileRequest : HttpRequest
	{
		public override string ContentType => "multipart/form-data;";

		public override byte[] GetRequestData()
		{
			return null;
		}

		public virtual IDictionary<string, string> GetFormDataParams()
		{
			return this.GetParams();
		}
	}
}
