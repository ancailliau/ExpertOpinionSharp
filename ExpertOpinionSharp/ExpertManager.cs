using System;
using System.Collections.Generic;
using System.Linq;
using MathNet.Numerics.Distributions;

namespace ExpertOpinionModelling
{
    class ExpertManager {

        /// <summary>
        /// Gets or sets the overshoot factor. (default: 10)
        /// </summary>
        /// <value>The k.</value>
        public int K {
            get;
            set;
        }

        /// <summary>
        /// Gets the experts.
        /// </summary>
        /// <value>The experts.</value>
        public IList<Expert> Experts {
            get;
            private set;
        }

        /// <summary>
        /// Gets the variables.
        /// </summary>
        /// <value>The variables.</value>
        public IList<ExpertVariable> Variables {
            get;
            private set;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ExpertOpinionModelling.ExpertManager"/> class.
        /// </summary>
        /// <param name="experts">Experts.</param>
        /// <param name="variables">Variables.</param>
        public ExpertManager(IEnumerable<Expert> experts, IEnumerable<ExpertVariable> variables)
        {
            this.K = 10;
            this.Experts = new List<Expert> (experts);
            this.Variables = new List<ExpertVariable> (variables);
        }

        /// <summary>
        /// Gets the lower and upper bound, i.e. q0 and q1 values, for the specified variable.
        /// </summary>
        /// <param name="var">Variable.</param>
        public Tuple<double, double> LH (ExpertVariable var)
        {
            var m = Experts.Min (x => x.Estimates[var].Estimates[0]);
            if (var is CalibrationVariable && (var as CalibrationVariable).TrueValue < m)
                m = (var as CalibrationVariable).TrueValue;

            var h = Experts.Max (x => x.Estimates[var].Estimates[2]);
            if (var is CalibrationVariable && (var as CalibrationVariable).TrueValue > h)
                h = (var as CalibrationVariable).TrueValue;

            var l = (h - m) * K / 100.0;
            return new Tuple<double, double> (m - l, h + l);
        }

        /// <summary>
        /// Gets the interpolated distribution for the specified variable <c>v</c> and expert <c>e</c>
        /// </summary>
        /// <returns>The interpolated distribution.</returns>
        /// <param name="v">The variable.</param>
        /// <param name="e">The expert.</param>
        public List<double> GetInterpolatedDistribution (CalibrationVariable v, Expert e)
        {
            var res = new List<double> ();
            var t = LH(v);
            var l = t.Item1;
            var h = t.Item2;

            for (int i = 0; i < 4; i++) {
                var l0 = (i == 0) ? l : e.Estimates[v].Estimates[i - 1];
                var l1 = (i == 3) ? h : e.Estimates[v].Estimates[i];
                res.Add (1.0d * (l1 - l0) / (h - l));
            }

            return res;
        }

        /// <summary>
        /// Gets the information score for the specified variable <c>v</c> and specified expert <c>e</c>.
        /// </summary>
        /// <returns>The information score.</returns>
        /// <param name="v">The variable.</param>
        /// <param name="e">The expert.</param>
        public double GetInformationScore (CalibrationVariable v, Expert e)
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

        /// <summary>
        /// Gets the information score for the specified expert <c>e</c>.
        /// </summary>
        /// <returns>The information score.</returns>
        /// <param name="e">The expert.</param>
        public double GetInformationScore (Expert e)
        {
            var score = 0d;
            foreach (var v in Variables.OfType<CalibrationVariable>()) {
                var lscore = GetInformationScore(v, e);
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
                foreach (var v in Variables.OfType<CalibrationVariable> ()) {
                    var trueValue = v.TrueValue;
                    if ((!(i > 0) || e.Estimates[v].Estimates[i - 1] <= trueValue)
                        & (!(i < 3) || trueValue < e.Estimates[v].Estimates[i])) {
                        s++;
                    }
                }
                res.Add ((s / Variables.Count ()));
            }
            return res;
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
        public IEnumerable<Tuple<Expert, double>> GetWeights ()
        {
            var scores = Experts.Select (x => new Tuple<Expert, double> (x, GetCalibrationScore (x) * GetInformationScore (x)))
                .ToList ();
            scores.Sort ((x, y) => y.Item2.CompareTo(x.Item2));
            var scaling = scores.Sum (x => x.Item2);
            return scores.Select (x => new Tuple<Expert, double> (x.Item1, x.Item2 / scaling));
        }
    }

}