using System;
using System.Runtime.CompilerServices;

namespace Cvte.Net.WebServices.CStoreUpload
{
	public class ServiceProviderExtension
	{
		
		public static ServiceProvider GetServiceProvider(string policyEntryType)
		{
			try
			{
				return (ServiceProvider)Enum.Parse(typeof(ServiceProvider), policyEntryType.ToUpper());
			}
			catch (Exception)
			{
				return ServiceProvider.UNKNOW;
			}
		}
	}
}
