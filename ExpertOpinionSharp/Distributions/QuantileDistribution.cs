using System;
using System.Linq;

namespace UCLouvain.ExpertOpinionSharp.Distributions
{
	public class QuantileDistribution : IDistribution
	{
		readonly Random _random;
		public readonly double[] probabilities;
        public readonly double[] quantiles;

		public QuantileDistribution (double[] probabilities, double[] quantiles)
		{
			_random = new Random ();
			this.probabilities = probabilities;
			this.quantiles = quantiles;
		}

		public double Sample ()
		{
			double s = _random.NextDouble ();

			int i;
			for (i = 1; i < probabilities.Length - 1; i++) {
				if (s < probabilities [i]) {
					break;
				}
			}

			var ss = (s - probabilities [i-1]) / (probabilities [i] - probabilities [i - 1]);
			var ss2 = ss * (quantiles [i] - quantiles [i - 1]) + quantiles[i - 1];
			return ss2;
		}

		public double LowerBound {
			get {
				return quantiles.Min ();
			}
		}

		public double UpperBound {
			get {
				return quantiles.Max ();
			}
		}

		public override string ToString ()
		{
			return string.Format ("[QuantileDistribution: {0}]", 
				string.Join (" ", Enumerable.Range (0, quantiles.Length).Select (i => quantiles[i] + ":" + probabilities[i]))
			);
		}
		
	}
}

