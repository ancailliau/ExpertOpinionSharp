namespace UCLouvain.ExpertOpinionSharp
{
    public class Variable {

        public string Name {
            get;
            private set;
		}

		public double? Value {
			get;
			set;
		}

		public bool Calibration {
			get {
				return Value != null;
			}
		}

    	public Variable(string name)
        {
            Name = name;			
        }

		public Variable(string name, double trueValue) : this(name)
        {
            Value = trueValue;
        }

		public override string ToString ()
		{
			return string.Format ("[Variable: Name={0}]", Name);
		}
    }
}

