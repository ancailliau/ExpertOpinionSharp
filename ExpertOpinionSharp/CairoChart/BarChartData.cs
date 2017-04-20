using System;
using System.Collections.Generic;
using System.Linq;

namespace UCLouvain.ExpertOpinionSharp
{
	public class BarChartData : IEnumerable<BarChartDataItem>
	{
		IList<BarChartDataItem> _set;

		public IEnumerator<BarChartDataItem> GetEnumerator ()
		{
			return _set.GetEnumerator ();
		}

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator ()
		{
			return _set.GetEnumerator ();
		}

		public BarChartData ()
		{
			_set = new List<BarChartDataItem> ();
		}

		public void Add (double l, double h, int v)
		{
			_set.Add (new BarChartDataItem (l, h, v));
		}


		public double UpperBound () {
			return _set.Max (x => x.UpperBound);
		}

		public double LowerBound () {
			return _set.Min (x => x.LowerBound);
		}

		public double MinValue () {
			return _set.Min (x => x.Value);
		}

		public double MaxValue () {
			return _set.Max (x => x.Value);
		}
	}
	public class BarChartDataItem
	{
		public double LowerBound {
			get;
			private set;
		}

		public double UpperBound {
			get;
			private set;
		}

		public int Value {
			get;
			private set;
		}

		public BarChartDataItem (double lowerBound, double upperBound, int value)
		{
			LowerBound = lowerBound;
			UpperBound = upperBound;
			Value = value;
		}

	}
}

