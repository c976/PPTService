using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace PPTService.Ink.Core.Windows.TextEditorPlus.Document
{
	public class ReadOnlyFeature : IFeature, IReadOnlyCollection<string>, IEnumerable<string>, IEnumerable
	{
		private readonly HashSet<string> _feature = new HashSet<string>();

		public int Count => _feature.Count;

		internal ReadOnlyFeature(IEnumerable<ReadOnlyFeature> readOnlyFeatureList, params SingleFeature[] singleFeatureList)
			: this(singleFeatureList)
		{
			AddFeatureList(readOnlyFeatureList);
		}

		public ReadOnlyFeature(params SingleFeature[] singleFeatureList)
		{
			foreach (SingleFeature singleFeature in singleFeatureList)
			{
				_feature.Add(singleFeature.FeatureText);
			}
		}

		public ReadOnlyFeature(params IFeature[] features)
		{
			AddFeatureList(features);
		}

		private void AddFeatureList(IEnumerable<IFeature> features)
		{
			foreach (IFeature feature in features)
			{
				AddFeature(feature);
			}
		}

		private void AddFeature(IFeature feature)
		{
			foreach (string item in feature)
			{
				_feature.Add(item);
			}
		}

		public IEnumerator<string> GetEnumerator()
		{
			return _feature.GetEnumerator();
		}

		public bool HasFeature(IFeature feature)
		{
			if (feature == this)
			{
				return true;
			}
			return feature.All(Contains);
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return _feature.GetEnumerator();
		}

		private bool Contains(string feature)
		{
			return _feature.Contains(feature);
		}
	}
}
