namespace UCLouvain.ExpertOpinionSharp
{
    public class Expert {

        public string Name {
            get;
            private set;
        }

        public Expert(string name)
        {
            Name = name;
        }

		public override bool Equals (object obj)
    	{
    		if (obj == null)
    			return false;
    		if (ReferenceEquals (this, obj))
    			return true;
    		if (obj.GetType () != typeof(Expert))
    			return false;
    		var other = (Expert)obj;
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