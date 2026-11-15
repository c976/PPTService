using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace PPTService.Ink.Core.Windows.Controls
{
	internal class AnimationImageCachePool
	{
		private class GifImageDictionary
		{
			public WeakReference<GifAnimation> GifAnimationReference { get; }

			public List<string> IdList { get; } = new List<string>();

			public GifImageDictionary(GifAnimation gifAnimation)
			{
				GifAnimationReference = new WeakReference<GifAnimation>(gifAnimation);
			}
		}

		[StructLayout(LayoutKind.Auto)]
		[CompilerGenerated]
		private struct _003C_003Ec__DisplayClass2_0
		{
			public AnimationImageCachePool _003C_003E4__this;

			public string fileName;

			public GifImage gifImage;
		}

		private readonly Dictionary<string, GifImageDictionary> _animationImagePool = new Dictionary<string, GifImageDictionary>();

		public GifAnimation GetGifAnimation(string fileName)
		{
			GifAnimation target;
			if (_animationImagePool.ContainsKey(fileName) && _animationImagePool[fileName].GifAnimationReference.TryGetTarget(out target))
			{
				return target;
			}
			target = new GifAnimation(fileName);
			_animationImagePool[fileName] = new GifImageDictionary(target);
			return target;
		}

		public bool TryPlay(string fileName, GifImage gifImage)
		{
			_003C_003Ec__DisplayClass2_0 obj = default(_003C_003Ec__DisplayClass2_0);
			obj._003C_003E4__this = this;
			obj.fileName = fileName;
			obj.gifImage = gifImage;
			if (_animationImagePool.ContainsKey(obj.fileName))
			{
				GifAnimation target;
				if (_animationImagePool[obj.fileName].GifAnimationReference.TryGetTarget(out target))
				{
					if (_animationImagePool[obj.fileName].IdList.Count == 0)
					{
						target.Play();
					}
					_003CTryPlay_003Eg__AddGifImage_007C2_0(ref obj);
					return true;
				}
				GetGifAnimation(obj.fileName).Play();
				_003CTryPlay_003Eg__AddGifImage_007C2_0(ref obj);
				return true;
			}
			return false;
		}

		public bool TryStop(string fileName, GifImage gifImage)
		{
			GifAnimation target;
			if (_animationImagePool.ContainsKey(fileName) && _animationImagePool[fileName].GifAnimationReference.TryGetTarget(out target))
			{
				if (_animationImagePool[fileName].IdList.Contains(gifImage.Id))
				{
					_animationImagePool[fileName].IdList.Remove(gifImage.Id);
				}
				if (_animationImagePool[fileName].IdList.Count == 0)
				{
					target.Stop();
				}
				return true;
			}
			return false;
		}

		[CompilerGenerated]
		private void _003CTryPlay_003Eg__AddGifImage_007C2_0(ref _003C_003Ec__DisplayClass2_0 P_0)
		{
			if (!_animationImagePool[P_0.fileName].IdList.Contains(P_0.gifImage.Id))
			{
				_animationImagePool[P_0.fileName].IdList.Add(P_0.gifImage.Id);
			}
		}
	}
}
