using System;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using Cvte.Net.WebServices.Monitors;
using Newtonsoft.Json;

namespace Cvte.Net.Http
{
	
	
	public class CStoreUploadFileResponse : JsonResponse, IApiMonitorResponse
	{
		private const int SUCCESS_CODE = 0;

		[DataMember(Name = "code")]
		public int Code { get; set; }

		[DataMember(Name = "message")]
		public string Message { get; set; }

		[DataMember(Name = "fileKey")]
		public string FileKey { get; set; }

		[DataMember(Name = "fileSize")]
		public string FileSize { get; set; }

		[DataMember(Name = "downloadUrl")]
		public string DownloadUrl { get; set; }

		public override bool IsSuccessBusiness
		{
			get
			{
				if (Code == 0)
				{
					return Error == null;
				}
				return false;
			}
		}

		protected override void ParseHtml(string html)
		{
			try
			{
				base.ParseHtml(html);
				var anon = JsonConvert.DeserializeAnonymousType(html, new
				{
					code = 0,
					message = "",
					data = (object)null
				});
				if (anon.data != null)
				{
					string text = anon.data.ToString();
					object obj = JsonConvert.DeserializeObject(text, GetType());
					if (obj == null)
					{
						throw new InvalidOperationException("body is null when parsing html! body string is " + text);
					}
					SelfAssignment(obj);
				}
				Message = anon.message;
				Code = anon.code;
			}
			catch (Exception ex)
			{
				if (Code == 0)
				{
					Code = (int)StatusCode;
				}
				if (string.IsNullOrWhiteSpace(Message))
				{
					Message = $"{StatusCode} {Error?.Message ?? ex.Message}";
				}
				throw new InvalidOperationException("ParseHtml Error." + ex.Message, ex);
			}
		}
	}
}
