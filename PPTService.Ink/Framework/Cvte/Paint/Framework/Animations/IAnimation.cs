using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Media.Animation;
using Cvte.Storage;

namespace Cvte.Paint.Framework.Animations
{
	public interface IAnimation : ISavable<AnimationSaveInfo>, ISavable, IExtensible<IAnimation>, INotifyPropertyChanged
	{
		UIElement Owner { get; set; }

		TimeSpan Duration { get; set; }

		double AccelerationRatio { get; set; }

		double DecelerationRatio { get; set; }

		IEasingFunction EasingFunction { get; set; }

		TimeSpan BeginTime { get; set; }

		IAnimationAudio Audio { get; set; }

		bool IsHideOwnerInBeginTime { get; set; }

		double RepeatBehavior { get; set; }

		event EventHandler Completed;

		void Begin();

		void Stop();

		void BeginWithoutAudio();
	}
}
