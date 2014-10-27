using System;
using System.Collections.Generic;

namespace ExpertOpinionModelling
{
    /// <summary>
    /// Models the opinion of an expert for a given variable. 
    /// </summary>
    class ExpertOpinion {

        /// <summary>
        /// Gets the variable.
        /// </summary>
        /// <value>The variable.</value>
        public ExpertVariable Variable {
            get;
            private set;
        }

        /// <summary>
        /// Gets the estimates.
        /// </summary>
        /// <value>The estimates.</value>
        public List<double> Estimates {
            get;
            private set;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ExpertOpinionModelling.ExpertOpinion"/> class.
        /// </summary>
        /// <param name="variable">Variable.</param>
        /// <param name="estimates">Estimates.</param>
        public ExpertOpinion(ExpertVariable variable, IEnumerable<double> estimates)
        {
            this.Variable = variable;
            this.Estimates = new List<double> (estimates);
        }
    }
}

