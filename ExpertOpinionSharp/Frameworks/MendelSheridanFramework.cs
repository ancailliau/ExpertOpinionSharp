using System;
using System.Collections.Generic;
using System.Linq;
using ExpertOpinionSharp.Distributions;

namespace ExpertOpinionSharp.Frameworks
{
	public class MendelSheridanFramework : ExpertOpinionFramework
    {
		CompositeKeyArray<Expert,Variable> z2;
		Dictionary<Variable, int[]> Z2;
		int[] S;
		double[] Pr;

		string[] orderedExpertNames;

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
		int M {
			get {
				return (int)Math.Pow(z2.ArrayLength + 1, Experts.Count());
			}
		}

		public MendelSheridanFramework () 
		{
		}

        public MendelSheridanFramework(double[] quantileVector) : base (quantileVector)
        {
        }

		IEnumerable<Expert> OrderedExperts {
			get {
				foreach (var e in orderedExpertNames.Select (x => Experts.First (y => y.Name == x))) {
					yield return e;
				}
			}
		}

		public override IDistribution Fit(string variableName)
        {

			/* **** */

			orderedExpertNames = new string[Experts.Count ()];
			int j = 0;
			foreach (var e in Experts) {
				orderedExpertNames [j] = e.Name;
				j++;
			}

			/* ---- */

			Buildz();
			BuildZ();
			BuildS();
			BuildPr();

			/* **** */

			var variable = Variables.Single (x => x.Name == variableName);

			var bounds = GetBounds(variable);
			var min = bounds.Item1;
			var max = bounds.Item2;

			var xx = Estimates.Get (variable);
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
				for (int l = Experts.Count() - 1; l >= 0; l--)
                {
					var currentExpert = Experts.Single (x => x.Name == orderedExpertNames [l]);
					var ll = Estimates [currentExpert, variable].ToList ();
					// expertEstimates[l].ToList();
                    var index = ll.FindIndex(y => y > v);
                    if (index < 0)
                    {
                        index = NbQuantiles;
                    }

                    h = (h * (NbQuantiles + 1)) + index;
				}
                probabilities[i] *= Pr[h];
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
			z2 = new CompositeKeyArray<Expert, Variable> (NbQuantiles + 1);
			foreach (var expert in OrderedExperts)
            {
				foreach (var variable in Variables.Where (x => x.Calibration))
                {
					var vector = new double[NbQuantiles + 1];
        	
					for (int t = 0; t < vector.Length; t++)
                    {
                        double upper, lower;

                        if (t == 0)
                        {
							upper = Estimates[expert, variable][t]; 
							vector[t] = (variable.Value <= upper) ? 1 : 0;
                        } 
						else if (t == Estimates.ArrayLength)
                        {
							lower = Estimates[expert, variable][t - 1];
							vector[t] = (variable.Value > lower) ? 1 : 0;
                        }
                        else
                        {
							lower = Estimates[expert, variable][t - 1];
							upper = Estimates[expert, variable][t];
							vector[t] = (variable.Value > lower & variable.Value <= upper) ? 1 : 0;
                        }
                    }

					z2.Add (expert, variable, vector);
                }
            }
        }

        /// <summary>
        /// Build the array <c>Z</c>.
        /// </summary>
        protected void BuildZ()
        {
			Z2 = new Dictionary<Variable, int[]> ();
			foreach (var variable in Variables.Where (x => x.Calibration))
            {
				Z2[variable] = new int[M];
                for (int h = 0; h < M; h++)
                {
                    var hashI = h;
                    var result = true;
					foreach (var expert in OrderedExperts)
                    {
                        var index = hashI % (NbQuantiles + 1);
                        result &= z2[expert, variable][index] == 1;
                        hashI = hashI / (NbQuantiles + 1);
                    }

                    Z2[variable][h] = result ? 1 : 0;
                }
            }
        }

        /// <summary>
        /// Build the array <c>S</c>.
        /// </summary>
        protected void BuildS()
        {
            S = new int[M];
            for (int h = 0; h < M; h++)
            {
				foreach (var variable in Variables.Where (x => x.Calibration))
                {
					S[h] += Z2[variable][h];
                }
            }
        }

        /// <summary>
        /// Build the array <c>Pr</c>.
        /// </summary>
        protected void BuildPr()
        {
            Pr = new double[M];
            for (int h = 0; h < M; h++)
            {
                double product = 1;
                var hashI = h;
				for (int i = 0; i < Experts.Count(); i++)
                {
                    var index = hashI % (NbQuantiles + 1);
                    product *= ExpectedDensity[index];
                    hashI = hashI / (NbQuantiles + 1);
                }

				Pr[h] = (S[h] + 1) / (Variables.Count (x => x.Calibration) + (1 / product));
            }

            Pr = Pr.Select(x => x / Pr.Sum()).ToArray();
        }
    }
}