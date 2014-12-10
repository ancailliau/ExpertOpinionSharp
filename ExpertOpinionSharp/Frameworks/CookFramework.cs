using System;
using System.Collections.Generic;
using System.Linq;
using MathNet.Numerics.Distributions;
using ExpertOpinionSharp.Distributions;

namespace ExpertOpinionSharp.Frameworks
{
	public class CookFramework : ExpertOpinionFramework {

		double _alpha;

		public double Alpha {
			get { 
				return _alpha;
			}
			set {
				_alpha = value;
				UseOptimalAlpha = false;
			}
		}

		public bool UseOptimalAlpha {
			get;
			set;
		}

		public CookFramework()
		{
			UseOptimalAlpha = true;
        }

		public CookFramework(double[] quantileVector) : base (quantileVector)
		{
			UseOptimalAlpha = true;
		}


        /// <summary>
        /// Gets the interpolated distribution for the specified variable <c>v</c> and expert <c>e</c>
        /// </summary>
        /// <returns>The interpolated distribution.</returns>
        /// <param name="v">The variable.</param>
        /// <param name="e">The expert.</param>
        public List<double> GetInterpolatedDistribution (Variable v, Expert e)
        {
            var res = new List<double> ();
            var t = GetBounds(v);
            var l = t.Item1;
            var h = t.Item2;

            for (int i = 0; i < 4; i++) {
				var l0 = (i == 0) ? l : Estimates[e,v][i - 1];
				var l1 = (i == 3) ? h : Estimates[e,v][i];
                res.Add (1.0d * (l1 - l0) / (h - l));
            }

            return res;
        }

		public List<double> GetDMInterpolatedDistribution (Variable variable, double alpha)
		{
			var weight = GetWeights (alpha).ToDictionary(tuple => tuple.Item1, tuple => tuple.Item2);

			var res = new List<double> ();
			var bounds = GetBounds(variable);
			var lowerBound = bounds.Item1;
			var upperBound = bounds.Item2;

			for (int i = 0; i < 4; i++) {
				double probability = 0;
				double sum = 0;
				foreach (var e in Experts) {
					var estimate = Estimates [e, variable];
					var l0 = (i == 0) ? lowerBound : estimate[i - 1];
					var l1 = (i == 3) ? upperBound : estimate[i];
					probability += weight[e] * (1.0d * (l1 - l0) / (upperBound - lowerBound));
					sum += weight [e];
				}
				res.Add (sum > 0 ? (probability / sum) : probability);
			}

			return res;
		}

        /// <summary>
        /// Gets the information score for the specified variable <c>v</c> and specified expert <c>e</c>.
        /// </summary>
        /// <returns>The information score.</returns>
        /// <param name="v">The variable.</param>
        /// <param name="e">The expert.</param>
        public double GetInformationScore (Variable v, Expert e)
        {
            var p = new [] { .05, .45, .45, .05 };
            var r = GetInterpolatedDistribution (v, e);

            var score = 0d;
            for (int i = 0; i < p.Length; i++) {
                var lscore = (p[i] * Math.Log(p[i] / r[i]));
                score += lscore;
            }

            return score;
        }

		public double GetDMInformationScore (Variable v, double alpha)
		{
			var p = new [] { .05, .45, .45, .05 };
			var r = GetDMInterpolatedDistribution (v, alpha);

			var score = 0d;
			for (int i = 0; i < p.Length; i++) {
				var lscore = r[i] > 0 ? (p[i] * Math.Log(p[i] / r[i])) : 0;
				score += lscore;
			}

			return score;
		}

        /// <summary>
        /// Gets the information score for the specified expert <c>e</c>.
        /// </summary>
        /// <returns>The information score.</returns>
        /// <param name="e">The expert.</param>
        public double GetInformationScore (Expert e)
        {
            var score = 0d;
			foreach (var v in Variables) {
                var lscore = GetInformationScore(v, e);
                score += lscore;
            }
            return score / Variables.Count ();
        }

		public double GetDMInformationScore (double alpha)
		{
			var score = 0d;
			foreach (var v in Variables) {
				var lscore = GetDMInformationScore (v, alpha);
				score += lscore;
			}
			return score / Variables.Count ();
		}

        /// <summary>
        /// Gets the calibration score for the specified expert <c>e</c>.
        /// </summary>
        /// <returns>The calibration score.</returns>
        /// <param name="e">The expert.</param>
        public double GetCalibrationScore (Expert e)
        {
            var p = new [] { .05, .45, .45, .05 };
            var score = 0d;
            var s = GetEmpiricalDistributions (e);

            for (int i = 0; i < 4; i++) {
                var lscore = (s[i] * (s[i] > 0 ? Math.Log(s[i] / p[i]) : 0));
                score += lscore;
            }
            return 1 - ChiSquared.CDF (3, 2 * Variables.Count () * score);
        }

		public double GetDMCalibrationScore ()
		{
			var p = new [] { .05, .45, .45, .05 };
			var score = 0d;
			var s = GetDMEmpiricalDistributions ();

			for (int i = 0; i < 4; i++) {
				var lscore = (s[i] * (s[i] > 0 ? Math.Log(s[i] / p[i]) : 0));
				score += lscore;
			}
			return 1 - ChiSquared.CDF (3, 2 * Variables.Count () * score);
		}

        /// <summary>
        /// Gets the empirical distributions for the specified expert <c>e</c>.
        /// </summary>
        /// <returns>The empirical distributions.</returns>
        /// <param name="e">E.</param>
        public List<double> GetEmpiricalDistributions (Expert e)
        {
            var res = new List<double> ();
            for (int i = 0; i < 4; i++) {
                var s = 0d;
				foreach (var v in Variables) {
                    var trueValue = v.Value;
					if ((i <= 0 || Estimates [e, v] [i - 1] <= trueValue)
						& (i >= 3 || trueValue < Estimates [e, v] [i])) {
                        s++;
                    }
                }
                res.Add ((s / Variables.Count ()));
            }
            return res;
        }

		public List<double> GetDMEmpiricalDistributions ()
		{
			return new [] { .05, .45, .45, .05 }.ToList ();
		}

        /// <summary>
        /// Gets the calibration scores for all experts, sorted by score.
        /// </summary>
        /// <returns>The calibration scores.</returns>
        public List<Tuple<Expert, double>> GetCalibrationScores ()
        {
			var scores = Experts.Select (x => new Tuple<Expert, double> (x, GetCalibrationScore (x)))
                .ToList ();
            scores.Sort ((x, y) => x.Item2.CompareTo(y.Item2));
            return scores;
        }

        /// <summary>
        /// Gets the information scores for all experts, sorted by score.
        /// </summary>
        /// <returns>The information scores.</returns>
        public List<Tuple<Expert, double>> GetInformationScores ()
        {
			var scores = Experts.Select (x => new Tuple<Expert, double> (x, GetInformationScore (x)))
                .ToList ();
            scores.Sort ((x, y) => y.Item2.CompareTo(x.Item2));
            return scores;
        }

        /// <summary>
        /// Gets the weights for all experts, sorted.
        /// </summary>
        /// <returns>The weights.</returns>
		public IEnumerable<Tuple<Expert, double>> GetWeights (double alpha = 0)
        {
			var scores = Experts.Select (x => {
				var c = GetCalibrationScore (x);
				var i = GetInformationScore (x);
				return new Tuple<Expert, double> (x, ((c >= alpha) ? c : 0) * i);
			}).ToList ();
            scores.Sort ((x, y) => y.Item2.CompareTo(x.Item2));
            var scaling = scores.Sum (x => x.Item2);
			return !(scaling > 0) ? scores : scores.Select (x => new Tuple<Expert, double> (x.Item1, x.Item2 / scaling));
        }

		public IEnumerable<Tuple<Expert, double>> GetOptimalWeights ()
		{
			var wdm = new Func<double, double> (alpha => {
				const int cdm = 1; // manager.GetDMCalibrationScore ();
				var idm = GetDMInformationScore (alpha);
				var sum = GetWeights (alpha).Sum (x => x.Item2);
				return sum > 0 ? ((cdm * idm) / sum) : (cdm * idm);
			});

			var upperbound = GetWeights ().Max (x => x.Item2);

			var optimalAlpha = OptimizationHelper.LocalMin(0, upperbound, x => -wdm (x), 1.2e-16, Math.Sqrt (Double.Epsilon));
			
			return GetWeights (optimalAlpha);
		}

		public override ExpertOpinionSharp.Distributions.IDistribution Fit (string variableName) 
		{
			var variable = Variables.Single (x => x.Name == variableName);
			var bounds = GetBounds (variable);

			var w = new double[Experts.Count() + 1];
			var d = new QuantileDistribution[Experts.Count()];

			w [0] = 0;
			w [w.Length - 1] = 1;

			var weights = UseOptimalAlpha ? GetOptimalWeights () : GetWeights (_alpha);

			int i = 0;
			foreach (var kv in weights) {
				var p = Estimates [kv.Item1, variable];
				var t = new [] { bounds.Item1, p [0], p [1], p [2], bounds.Item2 };
				d [i] = new QuantileDistribution (QuantileVector, t);
				w [i+1] = w [i] + kv.Item2;
				i++;
			}

			var dist = new MixtureDistribution (w, d);
			return dist;
		}
    }

}