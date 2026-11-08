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
using _0005;
using _0008;
using Cvte.Pipe.Http;
using Cvte.Pipe.Http.Request;

namespace Cvte.WebService.Lychee
{
	public class LycheeService
	{
		[CompilerGenerated]
		private sealed class SA_C5
		{
			public dynamic _0001;

			public string _0002;

			public string _0003;

			internal void _0001_M2(RequestBase P_0)
			{
				LycheeRequestBase lycheeRequestBase = P_0 as LycheeRequestBase;
				if (lycheeRequestBase == null)
				{
					return;
				}
				lycheeRequestBase.App = this._0001;
				lycheeRequestBase.TimeStamp = (long)SA_Proxy._0001_0002___008C(SA_Proxy._009F___008B(), new DateTime(1970, 1, 1)).TotalMilliseconds;
				lycheeRequestBase.Version = 1;
				SA_Proxy._007E_0092___0082(P_0, _0002);
				Dictionary<string, string> dictionary;
				StringBuilder stringBuilder;
				if (true)
				{
					dictionary = new Dictionary<string, string>();
					PropertyInfo[] array = SA_Proxy._007E_008D___001F(SA_Proxy._007E_008C___001E(lycheeRequestBase));
					foreach (PropertyInfo propertyInfo in array)
					{
						DataMemberAttribute dataMemberAttribute = SA_Attr<DataMemberAttribute>._0001(propertyInfo, false);
						if (dataMemberAttribute != null && !SA_Proxy._008B___001D(SA_Proxy._007E_0081___0018(dataMemberAttribute), "sign") && !dictionary.ContainsKey(SA_Proxy._007E_0081___0018(dataMemberAttribute)))
						{
							object obj = SA_Proxy._007E_008E___007F(propertyInfo, lycheeRequestBase, null);
							dictionary.Add(SA_Proxy._007E_0081___0018(dataMemberAttribute), ((obj != null) ? SA_Proxy._007E_007F___0018(obj) : null) ?? "");
						}
					}
					stringBuilder = new StringBuilder();
					if (dictionary.Any())
					{
						goto IL_0149;
					}
					goto IL_021b;
				}
				goto IL_022c;
				IL_0149:
				bool flag = true;
				IEnumerator<KeyValuePair<string, string>> enumerator = dictionary.OrderBy((KeyValuePair<string, string> keyValuePair) => keyValuePair.Key).GetEnumerator();
				try
				{
					KeyValuePair<string, string> current = default(KeyValuePair<string, string>);
					string text = default(string);
					while (SA_Proxy._007E_0096___0083(enumerator))
					{
						int num;
						if (6 != 0)
						{
							current = enumerator.Current;
							text = SA_Helpers.Str(current.Value, SA_Proxy._0003___0003());
							num = (flag ? 1 : 0);
							goto IL_01a0;
						}
						goto IL_01a2;
						IL_01a2:
						SA_Proxy._007E_0088___001A(stringBuilder, SA_Proxy._008F___0080(current.Key, "=", text));
						num = 0;
						if (num == 0)
						{
							flag = (byte)num != 0;
							continue;
						}
						goto IL_01a0;
						IL_01a0:
						if (num == 0)
						{
							SA_Proxy._007E_0088___001A(stringBuilder, SA_Proxy._0090___0081("&", current.Key, "=", text));
							continue;
						}
						goto IL_01a2;
					}
				}
				finally
				{
					if (enumerator != null)
					{
						SA_Proxy._007E_0013___000F(enumerator);
					}
				}
				goto IL_021b;
				IL_022c:
				if (-1 != 0)
				{
					lycheeRequestBase.Sign = LycheeService._0001(lycheeRequestBase.Sign, _0003);
					return;
				}
				goto IL_0149;
				IL_021b:
				lycheeRequestBase.Sign = SA_Proxy._007E_007F___0018(stringBuilder);
				goto IL_022c;
			}
		}

		[CompilerGenerated]
		private sealed class _0002
		{
			public dynamic _0001;




			internal ApplyTokenContent _0001_M5(object P_0)
			{
				this._0001?.Invoke(0.05);
				PublicInfo publicInfo = (PublicInfo)P_0;
				ApplyTokenResponse applyTokenResponse = this._0001.m__0001.Post<ApplyTokenRequest, ApplyTokenResponse>(new ApplyTokenRequest
				{
					Pid = string.Empty,
					Title = publicInfo.Title,
					Author = publicInfo.Author,
					Scale = publicInfo.Scale,
					Device = publicInfo.Device,
					Count = this._0001.Count
				});
				applyTokenResponse.ThrowIfBadReqeust();
				this._0001.ThrowIfCancellationRequested();
				this._0001?.Invoke(0.1);
				this._0001.m__0001.Post<StartRequest, StartResponse>(new StartRequest
				{
					Pid = applyTokenResponse.Content.Pid
				}).ThrowIfBadReqeust();
				this._0001.ThrowIfCancellationRequested();
				this._0001?.Invoke(0.2);
				return applyTokenResponse.Content;
			}

			internal Tuple<string, string, string> _0001_M6(Task<ApplyTokenContent> P_0)
			{
				ApplyTokenContent result = P_0.Result;
				int num = 0;
				Parallel.ForEach(result.Pages, (ApplyTokenContent.Page page) =>
				{
					this._0001.ThrowIfCancellationRequested();
					UploadFileLycheeRequest request = new UploadFileLycheeRequest(result.UpUrl)
					{
						Key = result.Token,
						FilePath = this._0001[SA_Proxy._0098___0086(page.SortNo) - 1],
						AppId = result.AppId,
						Token = result.Token,
						PageName = page.Name
					};
					num++;
					double num2 = (double)num / ((double)this._0001.Count * 1.0);
					this._0001?.Invoke(0.7 * num2 + 0.2);
					this._0001.m__0001.UploadFile<UploadFileLycheeRequest, UploadFileLycheeResponse>(request);
				});
				this._0001.ThrowIfCancellationRequested();
				this._0001?.Invoke(0.95);
				CompleteResponse completeResponse = this._0001.m__0001.Post<CompleteRequest, CompleteResponse>(new CompleteRequest
				{
					Pid = result.Pid
				});
				this._0001.ThrowIfCancellationRequested();
				completeResponse.ThrowIfBadReqeust();
				this._0001?.Invoke(1.0);
				return new Tuple<string, string, string>(result.Uuid, result.Pid, result.PUrlQr);
			}
		}

		[CompilerGenerated]
		private sealed class _0003
		{
			public dynamic _0001;



			internal void _0001_M4(ApplyTokenContent.Page P_0)
			{
				this._0001._0001.ThrowIfCancellationRequested();
				UploadFileLycheeRequest request = new UploadFileLycheeRequest(this._0001.UpUrl)
				{
					Key = this._0001.Token,
					FilePath = this._0001._0001[SA_Proxy._0098___0086(P_0.SortNo) - 1],
					AppId = this._0001.AppId,
					Token = this._0001.Token,
					PageName = P_0.Name
				};
				this._0001++;
				double num = (double)this._0001 / ((double)this._0001._0001.Count * 1.0);
				this._0001._0001?.Invoke(0.7 * num + 0.2);
				this._0001._0001.m__0001.UploadFile<UploadFileLycheeRequest, UploadFileLycheeResponse>(request);
			}
		}

		private readonly BasicHttpPipe m__0001 = new BasicHttpPipe();

		public LycheeService(string host, string appKay, string signKey)
		{
			this.m__0001.AddRequestFilter((RequestBase P_0) => P_0 is LycheeRequestBase, (RequestBase P_0) =>
			{
				LycheeRequestBase lycheeRequestBase = P_0 as LycheeRequestBase;
				if (lycheeRequestBase == null)
				{
					return;
				}
				lycheeRequestBase.App = appKay;
				lycheeRequestBase.TimeStamp = (long)SA_Proxy._0001_0002___008C(SA_Proxy._009F___008B(), new DateTime(1970, 1, 1)).TotalMilliseconds;
				lycheeRequestBase.Version = 1;
				SA_Proxy._007E_0092___0082(P_0, host);
				Dictionary<string, string> dictionary;
				StringBuilder stringBuilder;
				if (true)
				{
					dictionary = new Dictionary<string, string>();
					PropertyInfo[] array = SA_Proxy._007E_008D___001F(SA_Proxy._007E_008C___001E(lycheeRequestBase));
					foreach (PropertyInfo propertyInfo in array)
					{
						DataMemberAttribute dataMemberAttribute = SA_Attr<DataMemberAttribute>._0001(propertyInfo, false);
						if (dataMemberAttribute != null && !SA_Proxy._008B___001D(SA_Proxy._007E_0081___0018(dataMemberAttribute), "sign") && !dictionary.ContainsKey(SA_Proxy._007E_0081___0018(dataMemberAttribute)))
						{
							object obj = SA_Proxy._007E_008E___007F(propertyInfo, lycheeRequestBase, null);
							dictionary.Add(SA_Proxy._007E_0081___0018(dataMemberAttribute), ((obj != null) ? SA_Proxy._007E_007F___0018(obj) : null) ?? "");
						}
					}
					stringBuilder = new StringBuilder();
					if (dictionary.Any())
					{
						goto IL_0149;
					}
					goto IL_021b;
				}
				goto IL_022c;
				IL_0149:
				bool flag = true;
				IEnumerator<KeyValuePair<string, string>> enumerator = dictionary.OrderBy((KeyValuePair<string, string> keyValuePair) => keyValuePair.Key).GetEnumerator();
				try
				{
					KeyValuePair<string, string> current = default(KeyValuePair<string, string>);
					string text = default(string);
					while (SA_Proxy._007E_0096___0083(enumerator))
					{
						int num;
						if (6 != 0)
						{
							current = enumerator.Current;
							text = SA_Helpers.Str(current.Value, SA_Proxy._0003___0003());
							num = (flag ? 1 : 0);
							goto IL_01a0;
						}
						goto IL_01a2;
						IL_01a2:
						SA_Proxy._007E_0088___001A(stringBuilder, SA_Proxy._008F___0080(current.Key, "=", text));
						num = 0;
						if (num == 0)
						{
							flag = (byte)num != 0;
							continue;
						}
						goto IL_01a0;
						IL_01a0:
						if (num == 0)
						{
							SA_Proxy._007E_0088___001A(stringBuilder, SA_Proxy._0090___0081("&", current.Key, "=", text));
							continue;
						}
						goto IL_01a2;
					}
				}
				finally
				{
					if (enumerator != null)
					{
						SA_Proxy._007E_0013___000F(enumerator);
					}
				}
				goto IL_021b;
				IL_022c:
				if (-1 != 0)
				{
					lycheeRequestBase.Sign = _0001(lycheeRequestBase.Sign, signKey);
					return;
				}
				goto IL_0149;
				IL_021b:
				lycheeRequestBase.Sign = SA_Proxy._007E_007F___0018(stringBuilder);
				goto IL_022c;
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
			string uuid;
			string pid;
			if (0 == 0)
			{
				uuid = string.Empty;
				pid = string.Empty;
			}
			return Publish(files, info, cancellationToken, progressChanged, out url, out uuid, out pid);
		}

		public bool Publish(IList<string> files, PublicInfo info, CancellationToken cancellationToken, Action<double> progressChanged, out string url, out string uuid, out string pid)
		{
			IList<string> list;
			CancellationToken cancellationToken2;
			do
			{
				list = files;
				cancellationToken2 = cancellationToken;
				url = string.Empty;
			}
			while (7 == 0);
			uuid = string.Empty;
			pid = string.Empty;
			int num = list.Count;
			bool flag;
			if (0 == 0)
			{
				if (num != 0)
				{
					try
					{
						while (true)
						{
							Tuple<string, string, string> result = Task.Factory.StartNew((object P_0) =>
							{
								progressChanged?.Invoke(0.05);
								PublicInfo publicInfo = (PublicInfo)P_0;
								ApplyTokenResponse applyTokenResponse = this.m__0001.Post<ApplyTokenRequest, ApplyTokenResponse>(new ApplyTokenRequest
								{
									Pid = string.Empty,
									Title = publicInfo.Title,
									Author = publicInfo.Author,
									Scale = publicInfo.Scale,
									Device = publicInfo.Device,
									Count = list.Count
								});
								applyTokenResponse.ThrowIfBadReqeust();
								cancellationToken2.ThrowIfCancellationRequested();
								progressChanged?.Invoke(0.1);
								this.m__0001.Post<StartRequest, StartResponse>(new StartRequest
								{
									Pid = applyTokenResponse.Content.Pid
								}).ThrowIfBadReqeust();
								cancellationToken2.ThrowIfCancellationRequested();
								progressChanged?.Invoke(0.2);
								return applyTokenResponse.Content;
							}, info, cancellationToken2).ContinueWith((Task<ApplyTokenContent> P_0) =>
							{
								ApplyTokenContent result2 = P_0.Result;
								int num2 = 0;
								Parallel.ForEach(result2.Pages, (ApplyTokenContent.Page page) =>
								{
									cancellationToken2.ThrowIfCancellationRequested();
									UploadFileLycheeRequest request = new UploadFileLycheeRequest(result2.UpUrl)
									{
										Key = result2.Token,
										FilePath = list[SA_Proxy._0098___0086(page.SortNo) - 1],
										AppId = result2.AppId,
										Token = result2.Token,
										PageName = page.Name
									};
									num2++;
									double num3 = (double)num2 / ((double)list.Count * 1.0);
									progressChanged?.Invoke(0.7 * num3 + 0.2);
									this.m__0001.UploadFile<UploadFileLycheeRequest, UploadFileLycheeResponse>(request);
								});
								cancellationToken2.ThrowIfCancellationRequested();
								progressChanged?.Invoke(0.95);
								CompleteResponse completeResponse = this.m__0001.Post<CompleteRequest, CompleteResponse>(new CompleteRequest
								{
									Pid = result2.Pid
								});
								cancellationToken2.ThrowIfCancellationRequested();
								completeResponse.ThrowIfBadReqeust();
								progressChanged?.Invoke(1.0);
								return new Tuple<string, string, string>(result2.Uuid, result2.Pid, result2.PUrlQr);
							}, cancellationToken2).Result;
							if (6 == 0)
							{
								continue;
							}
							uuid = result.Item1;
							while (4 != 0)
							{
								pid = result.Item2;
								url = result.Item3;
								if (0 == 0)
								{
									goto end_IL_0073;
								}
							}
							continue;
							end_IL_0073:
							break;
						}
					}
					catch (AggregateException)
					{
						flag = false;
						goto IL_00df;
					}
					catch (Exception)
					{
						flag = false;
						goto IL_00df;
					}
					return true;
				}
				num = 0;
				if (num == 0)
				{
					return (byte)num != 0;
				}
			}
			goto IL_00e0;
			IL_00e0:
			return (byte)num != 0;
			IL_00df:
			num = (flag ? 1 : 0);
			goto IL_00e0;
		}

		public bool UpdatePassword(string uuid, string pid, string password)
		{
			BasicHttpPipe basicHttpPipe = this.m__0001;
			UpdatePasswordRequest updatePasswordRequest = new UpdatePasswordRequest
			{
				Uuid = uuid,
				Pid = pid,
				Password = password
			};
			string authType = (SA_Proxy._001D___0016(password) ? "public" : "password");
			if (0 == 0)
			{
				updatePasswordRequest.AuthType = authType;
			}
			return basicHttpPipe.Post<UpdatePasswordRequest, UpdatePasswordResponse>(updatePasswordRequest).Succeed;
		}

		private unsafe static string _0001(string P_0, string P_1)
		{
			HMACSHA256 hMACSHA = new HMACSHA256(SA_Proxy._007E_0005___0004(SA_Proxy._0003___0003(), P_1));
			try
			{
				byte[] array = SA_Proxy._007E_0087___0019(hMACSHA, SA_Proxy._007E_0005___0004(SA_Proxy._0003___0003(), P_0));
				StringBuilder stringBuilder = new StringBuilder();
				byte[] array2 = array;
				int num = 0;
				while (true)
				{
					int num2 = num;
					while (true)
					{
						IntPtr intPtr = (IntPtr)(void*)array2.LongLength;
						int num3;
						while (true)
						{
							num3 = (int)(long)intPtr;
							if (5 == 0)
							{
								break;
							}
							intPtr = (IntPtr)num3;
							if (false)
							{
								continue;
							}
							goto IL_007b;
						}
						goto IL_006c;
						IL_0068:
						if (7 == 0)
						{
							continue;
						}
						num3 = 1;
						goto IL_006c;
						IL_007b:
						if (num2 < num3)
						{
							byte b = array2[num];
							SA_Proxy._007E_0088___001A(stringBuilder, b.ToString("x2"));
							num2 = num;
							goto IL_0068;
						}
						return SA_Proxy._007E_007F___0018(stringBuilder);
						IL_006c:
						num2 += num3;
						if (0 == 0)
						{
							break;
						}
						goto IL_0068;
					}
					num = num2;
				}
			}
			finally
			{
				if (hMACSHA != null)
				{
					SA_Proxy._007E_0013___000F(hMACSHA);
				}
			}
		}
	}
}
