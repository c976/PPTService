using System;
using System.Collections.Generic;

namespace PPTService.Ink.Core.Windows.Notifications
{
	public sealed class DialogService
	{
		private static IDialogProvider _currentProvider;

		private static DialogService _customService;

		public static readonly IDialogProvider Default = new DialogProvider();

		private readonly Dictionary<string, IDialogProvider> _providerDictionary = new Dictionary<string, IDialogProvider>();

		public static IDialogProvider Current
		{
			get
			{
				return _currentProvider ?? Default;
			}
			set
			{
				_currentProvider = value;
			}
		}

		public static DialogService Custom => _customService ?? (_customService = new DialogService());

		public IDialogProvider this[string key]
		{
			get
			{
				return _providerDictionary[key];
			}
			set
			{
				if (_providerDictionary.ContainsKey(key))
				{
					UnregisterDialogProvider(key);
				}
				if (value != null)
				{
					RegisterDialogProvider(key, value);
				}
			}
		}

		public void RegisterDialogProvider(string key, IDialogProvider provider)
		{
			if (string.IsNullOrEmpty(key))
			{
				throw new ArgumentNullException("key");
			}
			if (provider == null)
			{
				throw new ArgumentNullException("provider");
			}
			_providerDictionary.Add(key, provider);
		}

		public void UnregisterDialogProvider(string key)
		{
			if (string.IsNullOrEmpty(key))
			{
				throw new ArgumentNullException("key");
			}
			_providerDictionary.Remove(key);
		}
	}
}
