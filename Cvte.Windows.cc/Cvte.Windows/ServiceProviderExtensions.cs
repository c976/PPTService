using System;
using System.Windows.Markup;
using Cvte.Diagnostics.Annotations;

namespace Cvte.Windows
{
	internal static class ServiceProviderExtensions
	{
		[CanBeNull]
		internal static object GetTargetObject([NotNull] this IServiceProvider serviceProvider)
		{
			do
			{
				if (serviceProvider == null)
				{
					continue;
				}
				while (4 != 0)
				{
					IProvideValueTarget provideValueTarget = serviceProvider.GetService(typeof(IProvideValueTarget)) as IProvideValueTarget;
					if (provideValueTarget == null)
					{
						if (-1 != 0)
						{
							return null;
						}
						continue;
					}
					return provideValueTarget.TargetObject;
				}
			}
			while (false);
			throw new ArgumentNullException("serviceProvider");
		}

		[CanBeNull]
		internal static T GetTargetObject<T>([NotNull] this IServiceProvider serviceProvider) where T : class
		{
			do
			{
				if (serviceProvider == null)
				{
					throw new ArgumentNullException("serviceProvider");
				}
			}
			while (false);
			IProvideValueTarget provideValueTarget = serviceProvider.GetService(typeof(IProvideValueTarget)) as IProvideValueTarget;
			if (provideValueTarget != null)
			{
				return provideValueTarget.TargetObject as T;
			}
			return null;
		}

		[CanBeNull]
		internal static object GetTargetProperty([NotNull] this IServiceProvider serviceProvider)
		{
			do
			{
				if (serviceProvider == null)
				{
					continue;
				}
				while (4 != 0)
				{
					IProvideValueTarget provideValueTarget = serviceProvider.GetService(typeof(IProvideValueTarget)) as IProvideValueTarget;
					if (provideValueTarget == null)
					{
						if (-1 != 0)
						{
							return null;
						}
						continue;
					}
					return provideValueTarget.TargetProperty;
				}
			}
			while (false);
			throw new ArgumentNullException("serviceProvider");
		}

		[CanBeNull]
		internal static T GetTargetProperty<T>([NotNull] this IServiceProvider serviceProvider) where T : class
		{
			do
			{
				if (serviceProvider == null)
				{
					throw new ArgumentNullException("serviceProvider");
				}
			}
			while (false);
			IProvideValueTarget provideValueTarget = serviceProvider.GetService(typeof(IProvideValueTarget)) as IProvideValueTarget;
			if (provideValueTarget != null)
			{
				return provideValueTarget.TargetProperty as T;
			}
			return null;
		}
	}
}
