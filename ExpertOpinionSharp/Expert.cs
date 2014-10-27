using System;
using System.Collections.Generic;

namespace ExpertOpinionModelling
{
    /// <summary>
    /// Models an expert.
    /// </summary>
    class Expert {

        /// <summary>
        /// Gets the name of the expert
        /// </summary>
        /// <value>The name.</value>
        public string Name {
            get;
            private set;
        }

        /// <summary>
        /// Gets the estimates for all variables
        /// </summary>
        /// <value>The estimates.</value>
        public IDictionary<ExpertVariable, ExpertOpinion> Estimates {
            get;
            private set;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ExpertOpinionModelling.Expert"/> class.
        /// </summary>
        /// <param name="name">Name.</param>
        /// <param name="estimates">Estimates.</param>
        public Expert(string name, IEnumerable<ExpertOpinion> estimates)
        {
            this.Name = name;
            this.Estimates = new Dictionary<ExpertVariable, ExpertOpinion> ();
            foreach (var e in estimates) {
                Estimates.Add (e.Variable, e);
            }
        }

    }
}