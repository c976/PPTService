using System;
using Cvte.Diagnostics.Annotations;
using Cvte.Windows.Media;

namespace Cvte.Paint.Framework
{
	public static class MemoryExtensions
	{
		public static void ReloadElements(this Slide slide, ReloadMode reloadMode = ReloadMode.AncestorOutput)
		{
			FlexibleImage.ReloadAllTemporarily(slide, 1.0, reloadMode);
		}

		public static void UseReloadedElements([NotNull] this Slide slide, [NotNull] Action action)
		{
			using (FlexibleImage.ReloadAllTemporarily(slide))
			{
				action();
			}
		}

		public static void UseReloadedElements([NotNull] this Slide slide, [NotNull] Action<Slide> action)
		{
			using (FlexibleImage.ReloadAllTemporarily(slide))
			{
				action(slide);
			}
		}

		public static T UseReloadedElements<T>([NotNull] this Slide slide, [NotNull] Func<T> func)
		{
			using (FlexibleImage.ReloadAllTemporarily(slide))
			{
				return func();
			}
		}

		public static T UseReloadedElements<T>([NotNull] this Slide slide, [NotNull] Func<Slide, T> func)
		{
			using (FlexibleImage.ReloadAllTemporarily(slide))
			{
				return func(slide);
			}
		}

		public static void RefreshFlexibleImageRendering([NotNull] this Slide slide)
		{
			FlexibleImage.RenderAllManually(slide);
		}
	}
}
