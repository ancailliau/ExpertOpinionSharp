using System;
using System.Collections.Generic;
using MathNet.Numerics.Distributions;
using System.Linq;
using MathNet.Numerics.Random;

namespace ExpertOpinionModelling
{
	public class MixtureDistribution
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

	public class QuantileDistribution 
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

	public class MaDistribution : IContinuousDistribution
	{
		private class WeighedContinuousUniform
		{
			public ContinuousUniform distribution;
			public double weight;
		}

		private ISet<WeighedContinuousUniform> distributions;

		public MaDistribution ()
		{
			distributions = new HashSet<WeighedContinuousUniform> ();
			_random = SystemRandomSource.Default;
		}

		public void Add (double weight, double[] probabilities, double[] quantiles)
		{
			for (int i = 0; i < quantiles.Length - 1; i++) {
				var q1 = quantiles [i];
				var q2 = quantiles [i + 1];
				var p = probabilities [i];
				distributions.Add (new WeighedContinuousUniform {
					distribution = new ContinuousUniform (q1, q2),
					weight = weight * p
				});
			}
		}

		#region IContinuousDistribution implementation


		public double Density (double x)
		{
			throw new NotImplementedException ();
		}


		public double DensityLn (double x)
		{
			throw new NotImplementedException ();
		}


		public double Sample ()
		{
			var s = _random.NextDouble ();
			var sumweight = distributions.Sum (d => d.weight);

			var total = distributions.Sum (d => {
				var _lower = d.distribution.LowerBound;
				var _upper = d.distribution.UpperBound;
				var dcdf = s * (_upper - _lower) + _lower;
				var d2 = d.weight * dcdf / sumweight;
				return d2;
			});

			return total;
		}


		public void Samples (double[] values)
		{
			throw new NotImplementedException ();
		}

		public IEnumerable<double> Samples ()
		{
			throw new NotImplementedException ();
		}


		public double Mode {
			get {
				throw new NotImplementedException ();
			}
		}


		public double Minimum {
			get {
				return distributions.Min (x => x.distribution.Minimum);
			}
		}


		public double Maximum {
			get {
				return distributions.Max (x => x.distribution.Maximum);
			}
		}


		#endregion


		#region IUnivariateDistribution implementation


		public double CumulativeDistribution (double x)
		{
			throw new NotImplementedException ();
		}


		public double Mean {
			get {
				return distributions.Sum (x => x.weight * x.distribution.Mean);
			}
		}


		public double Variance {
			get {
				throw new NotImplementedException ();
			}
		}


		public double StdDev {
			get {
				throw new NotImplementedException ();
			}
		}


		public double Entropy {
			get {
				throw new NotImplementedException ();
			}
		}


		public double Skewness {
			get {
				throw new NotImplementedException ();
			}
		}


		public double Median {
			get {
				throw new NotImplementedException ();
			}
		}


		#endregion


		#region IDistribution implementation

		System.Random _random;

		public System.Random RandomSource
		{
			get { return _random; }
			set { _random = value ?? SystemRandomSource.Default; }
		}

		#endregion

	}
}

