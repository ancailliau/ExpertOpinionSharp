using System;
using System.Collections.Generic;
using MathNet.Numerics.Distributions;
using System.Linq;
using MathNet.Numerics.Random;

namespace ExpertOpinionSharp.Distributions
{
	public class QuantileDistribution : IDistribution
	{
		private Random _random;
		private double[] probabilities;
		private double[] quantiles;

		public QuantileDistribution (double[] quantiles)
		{
			_random = new Random ();
			probabilities = new [] { 0, .05, .50, .95, 1 }; // TODO
			this.quantiles = quantiles; // { 6500 7000 8000 }
		}

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
	}
}

