using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using Cvte.Pipe.Http;
using Cvte.Pipe.Http.Request;

namespace Cvte.WebService.Lychee
{
	// 本文件按 de4dot 反混淆后的原版语义重写：原反编译产物中的 SA_Proxy 是
	// SmartAssembly 运行时委托代理的空壳（恒返回 default），会让签名、反射、
	// 编码全部失效，导致分享二维码必然报「网络开小差」。
	public class LycheeService
	{
		private readonly string _host;
		private readonly string _appKey;
		private readonly string _signKey;
		private readonly BasicHttpPipe _pipe = new BasicHttpPipe();

		public LycheeService(string host, string appKey, string signKey)
		{
			_host = host;
			_appKey = appKey;
			_signKey = signKey;
			_pipe.AddRequestFilter((RequestBase request) => request is LycheeRequestBase, PrepareRequest);
		}

		private void PrepareRequest(RequestBase request)
		{
			LycheeRequestBase lychee = request as LycheeRequestBase;
			if (lychee == null)
			{
				return;
			}
			lychee.App = _appKey;
			lychee.TimeStamp = (long)(DateTime.Now - new DateTime(1970, 1, 1)).TotalMilliseconds;
			lychee.Version = 1;
			lychee.Host = _host;

			Dictionary<string, string> values = new Dictionary<string, string>();
			foreach (PropertyInfo property in lychee.GetType().GetProperties())
			{
				DataMemberAttribute dataMember = LycheeSign.GetDataMember(property);
				if (dataMember == null || dataMember.Name == "sign" || values.ContainsKey(dataMember.Name))
				{
					continue;
				}
				object value = property.GetValue(lychee, null);
				values.Add(dataMember.Name, (value == null) ? "" : (value.ToString() ?? ""));
			}

			// 签名串：按参数名排序后拼接，值用原版的逐字符 URL 编码
			StringBuilder builder = new StringBuilder();
			bool first = true;
			foreach (KeyValuePair<string, string> item in values.OrderBy(kv => kv.Key))
			{
				string text = LycheeSign.UrlEncode(item.Value, Encoding.UTF8);
				if (first)
				{
					builder.Append(item.Key + "=" + text);
					first = false;
				}
				else
				{
					builder.Append("&" + item.Key + "=" + text);
				}
			}
			lychee.Sign = LycheeSign.HmacSha256Hex(builder.ToString(), _signKey);
		}

		public bool Publish(IList<string> files, PublicInfo info, out string url)
		{
			return Publish(files, info, default(CancellationToken), null, out url);
		}

		public bool Publish(IList<string> files, PublicInfo info, CancellationToken cancellationToken, out string url)
		{
			return Publish(files, info, cancellationToken, null, out url);
		}

		public bool Publish(IList<string> files, PublicInfo info, CancellationToken cancellationToken, Action<double> progressChanged, out string url)
		{
			string uuid;
			string pid;
			return Publish(files, info, cancellationToken, progressChanged, out url, out uuid, out pid);
		}

		public bool Publish(IList<string> files, PublicInfo info, CancellationToken cancellationToken, Action<double> progressChanged, out string url, out string uuid, out string pid)
		{
			url = string.Empty;
			uuid = string.Empty;
			pid = string.Empty;
			if (files == null || files.Count == 0)
			{
				return false;
			}
			try
			{
				Tuple<string, string, string> result = Task.Factory.StartNew((object state) =>
				{
					PublicInfo publicInfo = (PublicInfo)state;
					progressChanged?.Invoke(0.05);
					ApplyTokenResponse tokenResponse = _pipe.Post<ApplyTokenRequest, ApplyTokenResponse>(new ApplyTokenRequest
					{
						Pid = string.Empty,
						Title = publicInfo.Title,
						Author = publicInfo.Author,
						Scale = publicInfo.Scale,
						Device = publicInfo.Device,
						Count = files.Count
					});
					tokenResponse.ThrowIfBadReqeust();
					cancellationToken.ThrowIfCancellationRequested();
					progressChanged?.Invoke(0.1);
					_pipe.Post<StartRequest, StartResponse>(new StartRequest
					{
						Pid = tokenResponse.Content.Pid
					}).ThrowIfBadReqeust();
					cancellationToken.ThrowIfCancellationRequested();
					progressChanged?.Invoke(0.2);
					return tokenResponse.Content;
				}, info, cancellationToken).ContinueWith((Task<ApplyTokenContent> task) =>
				{
					ApplyTokenContent content = task.Result;
					int uploaded = 0;
					Parallel.ForEach(content.Pages, (ApplyTokenContent.Page page) =>
					{
						cancellationToken.ThrowIfCancellationRequested();
						UploadFileLycheeRequest upload = new UploadFileLycheeRequest(content.UpUrl)
						{
							Key = content.Token,
							FilePath = files[int.Parse(page.SortNo) - 1],
							AppId = content.AppId,
							Token = content.Token,
							PageName = page.Name
						};
						int current = Interlocked.Increment(ref uploaded);
						progressChanged?.Invoke(0.7 * ((double)current / files.Count) + 0.2);
						_pipe.UploadFile<UploadFileLycheeRequest, UploadFileLycheeResponse>(upload);
					});
					cancellationToken.ThrowIfCancellationRequested();
					progressChanged?.Invoke(0.95);
					CompleteResponse completeResponse = _pipe.Post<CompleteRequest, CompleteResponse>(new CompleteRequest
					{
						Pid = content.Pid
					});
					cancellationToken.ThrowIfCancellationRequested();
					completeResponse.ThrowIfBadReqeust();
					progressChanged?.Invoke(1.0);
					return new Tuple<string, string, string>(content.Uuid, content.Pid, content.PUrlQr);
				}, cancellationToken).Result;

				uuid = result.Item1;
				pid = result.Item2;
				url = result.Item3;
				return true;
			}
			catch (AggregateException)
			{
				return false;
			}
			catch (Exception)
			{
				return false;
			}
		}

		public bool UpdatePassword(string uuid, string pid, string password)
		{
			return _pipe.Post<UpdatePasswordRequest, UpdatePasswordResponse>(new UpdatePasswordRequest
			{
				Uuid = uuid,
				Pid = pid,
				Password = password,
				AuthType = (string.IsNullOrEmpty(password) ? "public" : "password")
			}).Succeed;
		}
	}

	// 反混淆还原出的公共辅助：属性特性读取、原版逐字符 URL 编码、HMAC-SHA256 签名
	internal static class LycheeSign
	{
		internal static DataMemberAttribute GetDataMember(PropertyInfo property)
		{
			return property.GetCustomAttributes(typeof(DataMemberAttribute), false)
				.Cast<DataMemberAttribute>().FirstOrDefault();
		}

		internal static string UrlEncode(string value, Encoding encoding)
		{
			if (string.IsNullOrEmpty(value))
			{
				return string.Empty;
			}
			StringBuilder builder = new StringBuilder();
			for (int i = 0; i < value.Length; i++)
			{
				string ch = value[i].ToString();
				string encoded = HttpUtility.UrlEncode(ch, encoding);
				switch (ch)
				{
				case "'":
					encoded = "'";
					break;
				case "~":
					encoded = "~";
					break;
				case " ":
					encoded = "%20";
					break;
				case "+":
					encoded = "%2b";
					break;
				}
				builder.Append((ch == encoded) ? ch : ((encoded == null) ? string.Empty : encoded.ToUpper()));
			}
			return builder.ToString();
		}

		internal static string HmacSha256Hex(string data, string key)
		{
			using (HMACSHA256 hmac = new HMACSHA256(Encoding.UTF8.GetBytes(key)))
			{
				byte[] hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(data));
				StringBuilder builder = new StringBuilder();
				foreach (byte b in hash)
				{
					builder.Append(b.ToString("x2"));
				}
				return builder.ToString();
			}
		}
	}
}
