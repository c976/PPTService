using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using PPTServiceUltra.Ink.Core.Linq;

namespace PPTServiceUltra.Ink.Core.Windows.TextEditorPlus.Document
{
	public class Feature : List<string>, IFeature, IReadOnlyCollection<string>, IEnumerable<string>, IEnumerable
	{
		public event Action OnFeatureChanged;

		public Feature()
		{
		}

		public Feature(params SingleFeature[] singleFeatureList)
			: this(singleFeatureList.Select((SingleFeature x) => x.FeatureText))
		{
		}

		internal Feature(IEnumerable<string> collection)
			: base(collection)
		{
		}

		internal Feature(string feature)
			: base(1)
		{
			Add(feature);
		}

		public void Inclusive(params Feature[] features)
		{
			features.SelectMany((Feature x) => x.ToList()).ForEach((string x) =>
			{
				if (!Contains(x))
				{
					Add(x);
				}
			});
			OnFeatureChanged?.Invoke();
		}

		internal void Inclusive(params IFeature[] features)
		{
			foreach (string item in features.SelectMany((IFeature x) => x))
			{
				if (!Contains(item))
				{
					Add(item);
				}
			}
			OnFeatureChanged?.Invoke();
		}

		public void Exclusive(params Feature[] features)
		{
			features.SelectMany((Feature x) => x.ToList()).ForEach((string x) =>
			{
				Remove(x);
			});
			OnFeatureChanged?.Invoke();
		}

		public bool HasFeature(Feature feature)
		{
			return feature.TrueForAll(Contains);
		}

		internal bool HasFeature(SingleFeature feature)
		{
			return Contains(feature.FeatureText);
		}

		public bool HasFeature(IFeature feature)
		{
			if (feature == this)
			{
				return true;
			}
			return feature.All(Contains);
		}

		public Feature Clone()
		{
			Feature feature = new Feature();
			feature.AddRange(this);
			return feature;
		}

		public static implicit operator Feature(ReadOnlyFeature feature)
		{
			Feature feature2 = new Feature();
			feature2.AddRange(feature);
			return feature2;
		}
	}
}
