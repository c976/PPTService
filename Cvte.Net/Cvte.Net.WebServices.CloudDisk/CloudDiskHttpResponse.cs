using System;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using Cvte.Net.Http;
using Cvte.Net.WebServices.Monitors;
using Newtonsoft.Json;

namespace Cvte.Net.WebServices.CloudDisk
{
	
	
	[DataContract]
	public class CloudDiskHttpResponse : JsonResponse, IApiMonitorResponse
	{
		[DataMember(Name = "statusCode")]
		public int Code { get; internal set; }

		[DataMember(Name = "errorMsg")]
		public string ErrorMsg { get; internal set; }

		public CloudDiskResponseCodes ResponseCode
		{
			get
			{
				CloudDiskResponseCodes result;
				if (!Enum.TryParse<CloudDiskResponseCodes>(Code.ToString(), out result))
				{
					return CloudDiskResponseCodes.Unknow;
				}
				return result;
			}
		}

		public override bool IsSuccessBusiness => ResponseCode == CloudDiskResponseCodes.Success;

		protected sealed override void ParseHtml(string html)
		{
			var anon = JsonConvert.DeserializeAnonymousType(html, new
			{
				statusCode = 200,
				errorMsg = "",
				data = (object)null
			});
			if (anon.data != null)
			{
				ParseHtmlData(anon.data.ToString());
			}
			Code = anon.statusCode;
			ErrorMsg = anon.errorMsg;
		}

		protected virtual void ParseHtmlData(string data)
		{
			object obj = JsonConvert.DeserializeObject(data, GetType());
			if (obj == null)
			{
				throw new InvalidOperationException("dataEntry is null when parsing html! dataEntry string is " + data);
			}
			SelfAssignment(obj);
		}
	}
}
