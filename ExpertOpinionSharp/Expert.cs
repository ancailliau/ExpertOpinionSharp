using System;
using System.Collections.Generic;

namespace ExpertOpinionModelling
{
    /// <summary>
    /// Models an expert.
    /// </summary>
    public class Expert {

        /// <summary>
        /// Gets the name of the expert
        /// </summary>
        /// <value>The name.</value>
        public string Name {
            get;
            private set;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ExpertOpinionModelling.Expert"/> class.
        /// </summary>
        /// <param name="name">Name.</param>
        /// <param name="estimates">Estimates.</param>
        public Expert(string name)
        {
            this.Name = name;
        }

		public override bool Equals (object obj)
    	{
    		if (obj == null)
    			return false;
    		if (ReferenceEquals (this, obj))
    			return true;
    		if (obj.GetType () != typeof(Expert))
    			return false;
    		Expert other = (Expert)obj;
    		return Name == other.Name;
    	}
    	

    	public override int GetHashCode ()
    	{
    		unchecked {
    			return (Name != null ? Name.GetHashCode () : 0);
    		}
    	}
    	
		public override string ToString ()
    	{
    		return string.Format ("[Expert: Name={0}]", Name);
    	}
    	
    }
}