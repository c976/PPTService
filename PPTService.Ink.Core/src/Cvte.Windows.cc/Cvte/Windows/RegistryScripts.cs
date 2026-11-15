using System;
using System.Runtime.CompilerServices;
using System.Security;
using Microsoft.Win32;

namespace PPTService.Ink.Core.Windows
{
	
	
	public static class RegistryScripts
	{
		
		public static string Read32(this RegistryHive root,  string path, string key = null)
		{
			using (RegistryKey rootKey = RegistryKey.OpenBaseKey(root, RegistryView.Registry32))
			{
				return rootKey.Read(path, key);
			}
		}

		public static bool Write32(this RegistryHive root, string path, string value)
		{
			using (RegistryKey rootKey = RegistryKey.OpenBaseKey(root, RegistryView.Registry32))
			{
				return rootKey.Write(path, null, value);
			}
		}

		public static bool Write32(this RegistryHive root, string path,  string key, string value)
		{
			using (RegistryKey rootKey = RegistryKey.OpenBaseKey(root, RegistryView.Registry32))
			{
				return rootKey.Write(path, key, value);
			}
		}

		
		public static string Read64(this RegistryHive root,  string path, string key = null)
		{
			using (RegistryKey rootKey = RegistryKey.OpenBaseKey(root, RegistryView.Registry64))
			{
				return rootKey.Read(path, key);
			}
		}

		public static bool Write64(this RegistryHive root, string path, string value)
		{
			using (RegistryKey rootKey = RegistryKey.OpenBaseKey(root, RegistryView.Registry64))
			{
				return rootKey.Write(path, null, value);
			}
		}

		public static bool Write64(this RegistryHive root, string path,  string key, string value)
		{
			using (RegistryKey rootKey = RegistryKey.OpenBaseKey(root, RegistryView.Registry64))
			{
				return rootKey.Write(path, key, value);
			}
		}

		
		public static string Read(this RegistryHive root,  string path, string key = null)
		{
			using (RegistryKey rootKey = RegistryKey.OpenBaseKey(root, RegistryView.Default))
			{
				return rootKey.Read(path, key);
			}
		}

		public static bool Write(this RegistryHive root, string path, string value)
		{
			using (RegistryKey rootKey = RegistryKey.OpenBaseKey(root, RegistryView.Default))
			{
				return rootKey.Write(path, null, value);
			}
		}

		public static bool Write(this RegistryHive root, string path,  string key, string value)
		{
			using (RegistryKey rootKey = RegistryKey.OpenBaseKey(root, RegistryView.Default))
			{
				return rootKey.Write(path, key, value);
			}
		}

		public static bool DeleteSubKey(this RegistryHive root, string path, string subKeyName)
		{
			using (RegistryKey rootKey = RegistryKey.OpenBaseKey(root, RegistryView.Default))
			{
				return rootKey.DeleteSubKey(path, subKeyName);
			}
		}

		public static bool Delete(this RegistryHive root, string path, string key)
		{
			using (RegistryKey rootKey = RegistryKey.OpenBaseKey(root, RegistryView.Default))
			{
				return rootKey.Delete(path, key);
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		
		private static string Read(this RegistryKey rootKey, string path,  string key = null)
		{
			using (RegistryKey registryKey = rootKey.OpenSubKey(path, false))
			{
				return (string)registryKey?.GetValue(key, null);
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static bool Write(this RegistryKey rootKey, string path,  string key, string value)
		{
			if (rootKey == null)
			{
				throw new ArgumentNullException("rootKey");
			}
			if (path == null)
			{
				throw new ArgumentNullException("path");
			}
			if (value == null)
			{
				throw new ArgumentNullException("value");
			}
			RegistryKey registryKey;
			try
			{
				registryKey = rootKey.OpenSubKey(path, true) ?? rootKey.CreateSubKey(path, RegistryKeyPermissionCheck.ReadWriteSubTree);
			}
			catch (SecurityException)
			{
				return false;
			}
			if (registryKey == null)
			{
				return false;
			}
			try
			{
				registryKey.SetValue(key, value);
			}
			catch (UnauthorizedAccessException)
			{
				return false;
			}
			return true;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static bool Delete(this RegistryKey rootKey, string path,  string key)
		{
			if (rootKey == null)
			{
				throw new ArgumentNullException("rootKey");
			}
			if (path == null)
			{
				throw new ArgumentNullException("path");
			}
			RegistryKey registryKey;
			try
			{
				registryKey = rootKey.OpenSubKey(path, true);
				if (registryKey == null)
				{
					return true;
				}
			}
			catch (SecurityException)
			{
				return false;
			}
			try
			{
				if (key == null)
				{
					key = "";
				}
				registryKey.DeleteValue(key);
				return true;
			}
			catch (UnauthorizedAccessException)
			{
				return false;
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static bool DeleteSubKey(this RegistryKey rootKey, string path, string subKeyName)
		{
			if (rootKey == null)
			{
				throw new ArgumentNullException("rootKey");
			}
			if (path == null)
			{
				throw new ArgumentNullException("path");
			}
			if (string.IsNullOrWhiteSpace(subKeyName))
			{
				throw new ArgumentNullException("subKeyName");
			}
			RegistryKey registryKey;
			try
			{
				registryKey = rootKey.OpenSubKey(path, true);
				if (registryKey == null)
				{
					return true;
				}
			}
			catch (SecurityException)
			{
				return false;
			}
			try
			{
				registryKey.DeleteSubKeyTree(subKeyName);
				return true;
			}
			catch (UnauthorizedAccessException)
			{
				return false;
			}
		}
	}
}
