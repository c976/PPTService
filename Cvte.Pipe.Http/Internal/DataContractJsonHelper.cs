using System;
using System.IO;
using System.Runtime.Serialization.Json;

namespace Cvte.Pipe.Http.Internal
{
	internal class DataContractJsonHelper
	{
		public static byte[] smethod_0<T>(T gparam_0)
		{
			using (MemoryStream memoryStream = new MemoryStream())
			{
				new DataContractJsonSerializer(gparam_0.GetType()).WriteObject((Stream)memoryStream, (object)gparam_0);
				return memoryStream.ToArray();
			}
		}

		public static byte[] smethod_1<T>(T gparam_0)
		{
			try
			{
				using (MemoryStream memoryStream = new MemoryStream())
				{
					new DataContractJsonSerializer(gparam_0.GetType()).WriteObject((Stream)memoryStream, (object)gparam_0);
					return memoryStream.ToArray();
				}
			}
			catch (IOException)
			{
				return new byte[0];
			}
			catch (Exception)
			{
				return new byte[0];
			}
		}

		public static byte[] smethod_2(Type type_0, object object_0)
		{
			using (MemoryStream memoryStream = new MemoryStream())
			{
				new DataContractJsonSerializer(type_0).WriteObject((Stream)memoryStream, object_0);
				return memoryStream.ToArray();
			}
		}

		public static T smethod_3<T>(byte[] byte_0) where T : class
		{
			try
			{
				using (MemoryStream stream = new MemoryStream(byte_0))
				{
					return (T)new DataContractJsonSerializer(typeof(T)).ReadObject((Stream)stream);
				}
			}
			catch (IOException)
			{
				return null;
			}
			catch (Exception)
			{
				return null;
			}
		}

		public static T smethod_4<T>(byte[] byte_0)
		{
			using (MemoryStream stream = new MemoryStream(byte_0))
			{
				return (T)new DataContractJsonSerializer(typeof(T)).ReadObject((Stream)stream);
			}
		}

		public static T smethod_5<T>(Stream stream_0)
		{
			return (T)new DataContractJsonSerializer(typeof(T)).ReadObject(stream_0);
		}

		public static object smethod_6(Type type_0, byte[] byte_0)
		{
			using (MemoryStream stream = new MemoryStream(byte_0))
			{
				return new DataContractJsonSerializer(type_0).ReadObject((Stream)stream);
			}
		}
	}
}
