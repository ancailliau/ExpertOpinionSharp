namespace ExpertOpinionSharp
{
    /// <summary>
    /// Models a variable to be estimated or a calibration variable whose value is known.
    /// </summary>
	public class Variable {

        /// <summary>
        /// Gets the name of the variable
        /// </summary>
        /// <value>The name.</value>
        public string Name {
            get;
            private set;
		}

		/// <summary>
		/// Gets the true value for the variable.
		/// </summary>
		/// <value>The true value.</value>
		public double? Value {
			get;
			set;
		}

		public bool Calibration {
			get {
				return Value != null;
			}
		}

        /// <summary>
        /// Initializes a new instance of the <see cref="ExpertOpinionModelling.ExpertVariable"/> class.
        /// </summary>
        /// <param name="name">Name.</param>
		public Variable(string name)
        {
            this.Name = name;			
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ExpertOpinionModelling.CalibrationVariable"/> class.
        /// </summary>
        /// <param name="name">Name.</param>
        /// <param name="trueValue">True value.</param>
		public Variable(string name, double trueValue) : this(name)
        {
            this.Value = trueValue;
        }

		public override string ToString ()
		{
			return string.Format ("[Variable: Name={0}]", Name);
		}
    }
}

