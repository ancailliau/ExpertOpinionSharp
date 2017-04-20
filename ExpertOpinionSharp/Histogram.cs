using System;
using UCLouvain.ExpertOpinionSharp.Distributions;
using System.Linq;
using System.Collections.Generic;

namespace UCLouvain.ExpertOpinionSharp
{
	public class Histogram
	{
		double[] data;
		int nbin;
		double low;
		double high;

		public Histogram (double[] data, int nbuckets, double low, double high)
		{
			this.data = data;
			this.nbin = nbuckets;
			this.low = low;
			this.high = high;
		}

		public static IEnumerable<double> Arange(double start, int count)
		{
			return Enumerable.Range((int)start, count).Select(v => (double)v);
		}

		public static IEnumerable<double> LinSpace(double start, double stop, int num, bool endpoint = true)
		{
			var result = new List<double>();
			if (num <= 0)
			{
				return result;
			}

			if (endpoint)
			{
				if (num == 1) 
				{
					return new List<double>() { start };
				}

				var step = (stop - start)/ ((double)num - 1.0d);
				result = Arange(0, num).Select(v => (v * step) + start).ToList();
			}
			else 
			{
				var step = (stop - start) / (double)num;
				result = Arange(0, num).Select(v => (v * step) + start).ToList();
			}

			return result;
		}
	}
}

