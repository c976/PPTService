using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Cvte.Pipe.Http;
using Cvte.Pipe.Http.Request;
using Cvte.WebService.Internal;

namespace Cvte.WebService.Lychee
{
	public class LycheeService
	{
		private sealed class Class18
		{
			public string string_0;

			public string string_1;

			public string string_2;

			internal void method_0(RequestBase requestBase_0)
			{
				LycheeRequestBase lycheeRequestBase = requestBase_0 as LycheeRequestBase;
				if (lycheeRequestBase == null)
				{
					return;
				}
				lycheeRequestBase.App = string_0;
				lycheeRequestBase.TimeStamp = (long)(DateTime.Now - new DateTime(1970, 1, 1)).TotalMilliseconds;
				lycheeRequestBase.Version = 1;
				requestBase_0.Host = string_1;
				Dictionary<string, string> dictionary = new Dictionary<string, string>();
				PropertyInfo[] properties = lycheeRequestBase.GetType().GetProperties();
				foreach (PropertyInfo propertyInfo in properties)
				{
					DataMemberAttribute dataMemberAttribute = PropertyAttributeHelper<DataMemberAttribute>.smethod_0(propertyInfo);
					if (dataMemberAttribute == null || dataMemberAttribute.Name == "sign" || dictionary.ContainsKey(dataMemberAttribute.Name))
					{
						continue;
					}
					object value = propertyInfo.GetValue(lycheeRequestBase, null);
					string name = dataMemberAttribute.Name;
					object obj;
					if (value == null)
					{
						obj = null;
					}
					else
					{
						obj = value.ToString();
						if (obj != null)
						{
							goto IL_00ca;
						}
					}
					obj = "";
					goto IL_00ca;
					IL_00ca:
					dictionary.Add(name, (string)obj);
				}
				StringBuilder stringBuilder = new StringBuilder();
				if (dictionary.Any())
				{
					bool flag = true;
					foreach (KeyValuePair<string, string> item in dictionary.OrderBy((KeyValuePair<string, string> keyValuePair_0) => keyValuePair_0.Key))
					{
						string text = SignatureUrlEncoder.smethod_0(item.Value, Encoding.UTF8);
						if (flag)
						{
							stringBuilder.Append(item.Key + "=" + text);
							flag = false;
						}
						else
						{
							stringBuilder.Append("&" + item.Key + "=" + text);
						}
					}
				}
				lycheeRequestBase.Sign = stringBuilder.ToString();
				lycheeRequestBase.Sign = smethod_0(lycheeRequestBase.Sign, string_2);
			}
		}

		private sealed class Class19
		{
			public Action<double> action_0;

			public LycheeService lycheeService_0;

			public IList<string> ilist_0;

			public CancellationToken cancellationToken_0;

			internal ApplyTokenContent method_0(object object_0)
			{
				action_0?.Invoke(0.05);
				PublicInfo publicInfo = (PublicInfo)object_0;
				ApplyTokenResponse applyTokenResponse = lycheeService_0.basicHttpPipe_0.Post<ApplyTokenRequest, ApplyTokenResponse>(new ApplyTokenRequest
				{
					Pid = string.Empty,
					Title = publicInfo.Title,
					Author = publicInfo.Author,
					Scale = publicInfo.Scale,
					Device = publicInfo.Device,
					Count = ilist_0.Count
				});
				applyTokenResponse.ThrowIfBadReqeust();
				cancellationToken_0.ThrowIfCancellationRequested();
				action_0?.Invoke(0.1);
				lycheeService_0.basicHttpPipe_0.Post<StartRequest, StartResponse>(new StartRequest
				{
					Pid = applyTokenResponse.Content.Pid
				}).ThrowIfBadReqeust();
				cancellationToken_0.ThrowIfCancellationRequested();
				action_0?.Invoke(0.2);
				return applyTokenResponse.Content;
			}

			internal Tuple<string, string, string> method_1(Task<ApplyTokenContent> task_0)
			{
				ApplyTokenContent applyTokenContent_0 = task_0.Result;
				int int_0 = 0;
				Parallel.ForEach(applyTokenContent_0.Pages, (ApplyTokenContent.Page page_0) =>
				{
					cancellationToken_0.ThrowIfCancellationRequested();
					UploadFileLycheeRequest request = new UploadFileLycheeRequest(applyTokenContent_0.UpUrl)
					{
						Key = applyTokenContent_0.Token,
						FilePath = ilist_0[int.Parse(page_0.SortNo) - 1],
						AppId = applyTokenContent_0.AppId,
						Token = applyTokenContent_0.Token,
						PageName = page_0.Name
					};
					int_0++;
					double num = (double)int_0 / ((double)ilist_0.Count * 1.0);
					action_0?.Invoke(0.7 * num + 0.2);
					lycheeService_0.basicHttpPipe_0.UploadFile<UploadFileLycheeRequest, UploadFileLycheeResponse>(request);
				});
				cancellationToken_0.ThrowIfCancellationRequested();
				action_0?.Invoke(0.95);
				CompleteResponse completeResponse = lycheeService_0.basicHttpPipe_0.Post<CompleteRequest, CompleteResponse>(new CompleteRequest
				{
					Pid = applyTokenContent_0.Pid
				});
				cancellationToken_0.ThrowIfCancellationRequested();
				completeResponse.ThrowIfBadReqeust();
				action_0?.Invoke(1.0);
				return new Tuple<string, string, string>(applyTokenContent_0.Uuid, applyTokenContent_0.Pid, applyTokenContent_0.PUrlQr);
			}
		}

		private sealed class Class20
		{
			public ApplyTokenContent applyTokenContent_0;

			public int int_0;

			public Class19 class19_0;

			internal void method_0(ApplyTokenContent.Page page_0)
			{
				class19_0.cancellationToken_0.ThrowIfCancellationRequested();
				UploadFileLycheeRequest request = new UploadFileLycheeRequest(applyTokenContent_0.UpUrl)
				{
					Key = applyTokenContent_0.Token,
					FilePath = class19_0.ilist_0[int.Parse(page_0.SortNo) - 1],
					AppId = applyTokenContent_0.AppId,
					Token = applyTokenContent_0.Token,
					PageName = page_0.Name
				};
				int_0++;
				double num = (double)int_0 / ((double)class19_0.ilist_0.Count * 1.0);
				class19_0.action_0?.Invoke(0.7 * num + 0.2);
				class19_0.lycheeService_0.basicHttpPipe_0.UploadFile<UploadFileLycheeRequest, UploadFileLycheeResponse>(request);
			}
		}

		private readonly BasicHttpPipe basicHttpPipe_0 = new BasicHttpPipe();

		public LycheeService(string host, string appKay, string signKey)
		{
			basicHttpPipe_0.AddRequestFilter((RequestBase requestBase_0) => requestBase_0 is LycheeRequestBase, (RequestBase requestBase_0) =>
			{
				LycheeRequestBase lycheeRequestBase = requestBase_0 as LycheeRequestBase;
				if (lycheeRequestBase != null)
				{
					lycheeRequestBase.App = appKay;
					lycheeRequestBase.TimeStamp = (long)(DateTime.Now - new DateTime(1970, 1, 1)).TotalMilliseconds;
					lycheeRequestBase.Version = 1;
					requestBase_0.Host = host;
					Dictionary<string, string> dictionary = new Dictionary<string, string>();
					PropertyInfo[] properties = lycheeRequestBase.GetType().GetProperties();
					foreach (PropertyInfo propertyInfo in properties)
					{
						DataMemberAttribute dataMemberAttribute = PropertyAttributeHelper<DataMemberAttribute>.smethod_0(propertyInfo);
						if (dataMemberAttribute == null || dataMemberAttribute.Name == "sign" || dictionary.ContainsKey(dataMemberAttribute.Name))
						{
							continue;
						}
						object value = propertyInfo.GetValue(lycheeRequestBase, null);
						string name = dataMemberAttribute.Name;
						object obj;
						if (value == null)
						{
							obj = null;
						}
						else
						{
							obj = value.ToString();
							if (obj != null)
							{
								goto IL_00ca;
							}
						}
						obj = "";
						goto IL_00ca;
						IL_00ca:
						dictionary.Add(name, (string)obj);
					}
					StringBuilder stringBuilder = new StringBuilder();
					if (dictionary.Any())
					{
						bool flag = true;
						foreach (KeyValuePair<string, string> item in dictionary.OrderBy((KeyValuePair<string, string> keyValuePair_0) => keyValuePair_0.Key))
						{
							string text = SignatureUrlEncoder.smethod_0(item.Value, Encoding.UTF8);
							if (flag)
							{
								stringBuilder.Append(item.Key + "=" + text);
								flag = false;
							}
							else
							{
								stringBuilder.Append("&" + item.Key + "=" + text);
							}
						}
					}
					lycheeRequestBase.Sign = stringBuilder.ToString();
					lycheeRequestBase.Sign = smethod_0(lycheeRequestBase.Sign, signKey);
				}
			});
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
			url = string.Empty;
			string uuid = string.Empty;
			string pid = string.Empty;
			return Publish(files, info, cancellationToken, progressChanged, out url, out uuid, out pid);
		}

		public bool Publish(IList<string> files, PublicInfo info, CancellationToken cancellationToken, Action<double> progressChanged, out string url, out string uuid, out string pid)
		{
			url = string.Empty;
			uuid = string.Empty;
			pid = string.Empty;
			if (files.Count == 0)
			{
				return false;
			}
			try
			{
				Tuple<string, string, string> result = Task.Factory.StartNew((object object_0) =>
				{
					progressChanged?.Invoke(0.05);
					PublicInfo publicInfo = (PublicInfo)object_0;
					ApplyTokenResponse applyTokenResponse = basicHttpPipe_0.Post<ApplyTokenRequest, ApplyTokenResponse>(new ApplyTokenRequest
					{
						Pid = string.Empty,
						Title = publicInfo.Title,
						Author = publicInfo.Author,
						Scale = publicInfo.Scale,
						Device = publicInfo.Device,
						Count = files.Count
					});
					applyTokenResponse.ThrowIfBadReqeust();
					cancellationToken.ThrowIfCancellationRequested();
					progressChanged?.Invoke(0.1);
					basicHttpPipe_0.Post<StartRequest, StartResponse>(new StartRequest
					{
						Pid = applyTokenResponse.Content.Pid
					}).ThrowIfBadReqeust();
					cancellationToken.ThrowIfCancellationRequested();
					progressChanged?.Invoke(0.2);
					return applyTokenResponse.Content;
				}, info, cancellationToken).ContinueWith((Task<ApplyTokenContent> task_0) =>
				{
					ApplyTokenContent applyTokenContent_0 = task_0.Result;
					int int_0 = 0;
					Parallel.ForEach(applyTokenContent_0.Pages, (ApplyTokenContent.Page page_0) =>
					{
						cancellationToken.ThrowIfCancellationRequested();
						UploadFileLycheeRequest request = new UploadFileLycheeRequest(applyTokenContent_0.UpUrl)
						{
							Key = applyTokenContent_0.Token,
							FilePath = files[int.Parse(page_0.SortNo) - 1],
							AppId = applyTokenContent_0.AppId,
							Token = applyTokenContent_0.Token,
							PageName = page_0.Name
						};
						int_0++;
						double num = (double)int_0 / ((double)files.Count * 1.0);
						progressChanged?.Invoke(0.7 * num + 0.2);
						basicHttpPipe_0.UploadFile<UploadFileLycheeRequest, UploadFileLycheeResponse>(request);
					});
					cancellationToken.ThrowIfCancellationRequested();
					progressChanged?.Invoke(0.95);
					CompleteResponse completeResponse = basicHttpPipe_0.Post<CompleteRequest, CompleteResponse>(new CompleteRequest
					{
						Pid = applyTokenContent_0.Pid
					});
					cancellationToken.ThrowIfCancellationRequested();
					completeResponse.ThrowIfBadReqeust();
					progressChanged?.Invoke(1.0);
					return new Tuple<string, string, string>(applyTokenContent_0.Uuid, applyTokenContent_0.Pid, applyTokenContent_0.PUrlQr);
				}, cancellationToken).Result;
				uuid = result.Item1;
				pid = result.Item2;
				url = result.Item3;
			}
			catch (AggregateException)
			{
				return false;
			}
			catch (Exception)
			{
				return false;
			}
			return true;
		}

		public bool UpdatePassword(string uuid, string pid, string password)
		{
			return basicHttpPipe_0.Post<UpdatePasswordRequest, UpdatePasswordResponse>(new UpdatePasswordRequest
			{
				Uuid = uuid,
				Pid = pid,
				Password = password,
				AuthType = (string.IsNullOrEmpty(password) ? "public" : "password")
			}).Succeed;
		}

		private static string smethod_0(string string_0, string string_1)
		{
			using (HMACSHA256 hMACSHA = new HMACSHA256(Encoding.UTF8.GetBytes(string_1)))
			{
				byte[] array = hMACSHA.ComputeHash(Encoding.UTF8.GetBytes(string_0));
				StringBuilder stringBuilder = new StringBuilder();
				byte[] array2 = array;
				foreach (byte b in array2)
				{
					stringBuilder.Append(b.ToString("x2"));
				}
				return stringBuilder.ToString();
			}
		}
	}
}
