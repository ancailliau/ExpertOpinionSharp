using System;

namespace ExpertOpinionSharp.Distributions
{
	public class MixtureDistribution : IDistribution
	{
		readonly Random _random;
		readonly double[] cummulativeWeight;
		readonly QuantileDistribution[] distributions;

		public MixtureDistribution (double[] cummulativeWeight, QuantileDistribution[] distributions)
		{
			this.cummulativeWeight = cummulativeWeight;
			this.distributions = distributions;
			_random = new Random ();
		}

		public double Sample ()
		{
			double s = _random.NextDouble ();

			int i;
			for (i = 1; i < cummulativeWeight.Length - 1; i++) {
				if (s < cummulativeWeight [i]) {
					break;
				}
			}
			return distributions[i-1].Sample ();
		}
	}
}

