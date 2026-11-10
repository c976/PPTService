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
			if (serviceProvider == null)
			{
				throw new ArgumentNullException("serviceProvider");
			}
			return (serviceProvider.GetService(typeof(IProvideValueTarget)) as IProvideValueTarget)?.TargetObject;
		}

		[CanBeNull]
		internal static T GetTargetObject<T>([NotNull] this IServiceProvider serviceProvider) where T : class
		{
			if (serviceProvider == null)
			{
				throw new ArgumentNullException("serviceProvider");
			}
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
			if (serviceProvider == null)
			{
				throw new ArgumentNullException("serviceProvider");
			}
			return (serviceProvider.GetService(typeof(IProvideValueTarget)) as IProvideValueTarget)?.TargetProperty;
		}

		[CanBeNull]
		internal static T GetTargetProperty<T>([NotNull] this IServiceProvider serviceProvider) where T : class
		{
			if (serviceProvider == null)
			{
				throw new ArgumentNullException("serviceProvider");
			}
			IProvideValueTarget provideValueTarget = serviceProvider.GetService(typeof(IProvideValueTarget)) as IProvideValueTarget;
			if (provideValueTarget != null)
			{
				return provideValueTarget.TargetProperty as T;
			}
			return null;
		}
	}
}
