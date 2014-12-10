using System;
using System.Collections.Generic;
using MathNet.Numerics.Distributions;
using System.Linq;
using MathNet.Numerics.Random;

namespace ExpertOpinionSharp.Distributions
{
	public class MixtureDistribution : IDistribution
	{
		private Random _random;
		private double[] cummulativeWeight;
		private QuantileDistribution[] distributions;

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

