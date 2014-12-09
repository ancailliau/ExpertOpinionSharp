using System;
using System.Collections.Generic;
using System.Linq;

namespace ExpertOpinionModelling
{
	public class MendelSheridanFramework : ExpertOpinionFramework
    {
		private CompositeKeyArray<Expert,Variable> z2;
		private Dictionary<Variable, int[]> Z2;
		private int[] S;
		private double[] Pr;

		private string[] orderedExpertNames;

        /// <summary>
        /// The number of combination between all quantiles of all experts
        /// </summary>
        /// <remarks>
        /// For instance, consider that we have 2 experts, each specifying 2 quantiles. We have 9 possibles
        /// arrangment for theses:
        /// <list type="bullet">
        /// <item>Before the first quantiles of the first and second expert;</item>
        /// <item>Before the first quantiles of the first expert and before the second quantile of the second expert 
        /// (but after the first);</item>
        /// <item>...</item>
        /// </list>
        /// </remarks>
		private int M {
			get {
				return (int)Math.Pow(z2.ArrayLength + 1, Experts.Count());
			}
		}

		public MendelSheridanFramework () : base ()
		{
		}

        public MendelSheridanFramework(double[] quantileVector) : base (quantileVector)
        {
        }

		private IEnumerable<Expert> OrderedExperts {
			get {
				foreach (var e in orderedExpertNames.Select (x => this.Experts.First (y => y.Name == x))) {
					yield return e;
				}
			}
		}

        /// <summary>
        /// Fit the specified <c>expertEstimates</c> using the calibration variables provided in the constructor.
        /// </summary>
        /// <remarks>
        /// It fills <c>stops</c> with the intervals boundaries and <c>probabilities</c> with the corresponding
        /// probabilities.
        /// The function will assume a 10% overshoot for getting lower and upper bounds.
        /// </remarks>
        /// <param name="expertEstimates">Expert estimates.</param>
        /// <param name="stops">Stops.</param>
        /// <param name="probabilities">Probabilities.</param>
		public QuantileDistribution Fit(string variableName)
        {

			/* **** */

			orderedExpertNames = new string[this.Experts.Count ()];
			int j = 0;
			foreach (var e in this.Experts) {
				orderedExpertNames [j] = e.Name;
				j++;
			}

			/* ---- */

			this.Buildz();
			this.BuildZ();
			this.BuildS();
			this.BuildPr();

			/* **** */

			var variable = Variables.Single (x => x.Name == variableName);

			var bounds = GetBounds(variable);
			var min = bounds.Item1;
			var max = bounds.Item2;

			var xx = estimates.Get (variable);
            // var xx = expertEstimates.SelectMany(t => t);

            var xlist = xx.Union(new[] { min, max }).ToList();
            xlist.Sort();

            var probabilities = new double[xlist.Count - 1];

            for (int i = 0; i < xlist.Count - 1; i++)
            {
                var v = xlist[i];
                probabilities[i] = 1;
				var h = 0;

                // for getting the correct h, we need to compute it in the opposite
                // direction; as h = ((...) * (m + 1) + j ) * (m + 1) + k for [...,j,k]
				for (int l = this.Experts.Count() - 1; l >= 0; l--)
                {
					var currentExpert = Experts.Single (x => x.Name == orderedExpertNames [l]);
					var ll = estimates [currentExpert, variable].ToList ();
					// expertEstimates[l].ToList();
                    var index = ll.FindIndex(y => y > v);
                    if (index < 0)
                    {
                        index = this.NbQuantiles;
                    }

                    h = (h * (this.NbQuantiles + 1)) + index;
				}
                probabilities[i] *= this.Pr[h];
            }

            var sum = probabilities.Sum();
            probabilities = probabilities.Select(t => t / sum).ToArray();
            var stops = xlist.ToArray();

			var quantiles = new double [probabilities.Length + 1];
			quantiles [0] = 0;
			int ij;
			for (ij = 1; ij < probabilities.Length; ij++) {
				quantiles [ij] = quantiles[ij-1] + probabilities[ij-1];
			}
			quantiles [ij] = 1;

			var dist = new QuantileDistribution (quantiles, stops);

			return dist;
        }

        /// <summary>
        /// Build the array <c>z</c>.
        /// </summary>
        protected void Buildz()
        {
			this.z2 = new CompositeKeyArray<Expert, Variable> (this.NbQuantiles + 1);
			foreach (var expert in OrderedExperts)
            {
				foreach (var variable in Variables.Where (x => x.Calibration))
                {
					var vector = new double[this.NbQuantiles + 1];
        	
					for (int t = 0; t < vector.Length; t++)
                    {
                        double upper, lower;

                        if (t == 0)
                        {
							upper = this.estimates[expert, variable][t]; 
							vector[t] = (variable.Value <= upper) ? 1 : 0;
                        } 
						else if (t == this.estimates.ArrayLength)
                        {
							lower = this.estimates[expert, variable][t - 1];
							vector[t] = (variable.Value > lower) ? 1 : 0;
                        }
                        else
                        {
							lower = this.estimates[expert, variable][t - 1];
							upper = this.estimates[expert, variable][t];
							vector[t] = (variable.Value > lower & variable.Value <= upper) ? 1 : 0;
                        }
                    }

					this.z2.Add (expert, variable, vector);
                }
            }
        }

        /// <summary>
        /// Build the array <c>Z</c>.
        /// </summary>
        protected void BuildZ()
        {
			this.Z2 = new Dictionary<Variable, int[]> ();
			foreach (var variable in Variables.Where (x => x.Calibration))
            {
				this.Z2[variable] = new int[this.M];
                for (int h = 0; h < this.M; h++)
                {
                    var hashI = h;
                    var result = true;
					foreach (var expert in OrderedExperts)
                    {
                        var index = hashI % (this.NbQuantiles + 1);
                        result &= this.z2[expert, variable][index] == 1;
                        hashI = hashI / (this.NbQuantiles + 1);
                    }

                    this.Z2[variable][h] = result ? 1 : 0;
                }
            }
        }

        /// <summary>
        /// Build the array <c>S</c>.
        /// </summary>
        protected void BuildS()
        {
            this.S = new int[this.M];
            for (int h = 0; h < this.M; h++)
            {
				foreach (var variable in Variables.Where (x => x.Calibration))
                {
					this.S[h] += this.Z2[variable][h];
                }
            }
        }

        /// <summary>
        /// Build the array <c>Pr</c>.
        /// </summary>
        protected void BuildPr()
        {
            this.Pr = new double[this.M];
            for (int h = 0; h < this.M; h++)
            {
                double product = 1;
                var hashI = h;
				for (int i = 0; i < this.Experts.Count(); i++)
                {
                    var index = hashI % (this.NbQuantiles + 1);
                    product *= this.ExpectedDensity[index];
                    hashI = hashI / (this.NbQuantiles + 1);
                }

				this.Pr[h] = (this.S[h] + 1) / (this.Variables.Count (x => x.Calibration) + (1 / product));
            }

            this.Pr = this.Pr.Select(x => x / this.Pr.Sum()).ToArray();
        }
    }
}