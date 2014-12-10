using System;
using System.Collections.Generic;
using System.Linq;

namespace ExpertOpinionModelling
{
	public abstract class ExpertOpinionFramework
	{
		/// <summary>
		/// Gets or sets the overshoot factor. (default: 0.10)
		/// </summary>
		/// <value>The k.</value>
		public double OvershootFactor {
			get;
			set;
		}

		public double? UpperBound {
			get;
			set;
		}

		public double? LowerBound {
			get;
			set;
		}

		public ISet<Expert> Experts {
			get {
				return new HashSet<Expert> (estimates.Keys1);
			}
		}

		public ISet<Variable> Variables {
			get {
				return new HashSet<Variable> (estimates.Keys2);
			}
		}

		protected CompositeKeyArray<Expert, Variable> estimates;

		protected int NbQuantiles {
			get {
				return this.QuantileVector.Length - 2;
			}
		}

		private double[] _quantileVector;

		protected double[] QuantileVector {
			get {
				return _quantileVector;
			}
			set {
				_quantileVector = value;

				var length = _quantileVector.Length - 1;
				ExpectedDensity = new double[length];

				for (int i = 0; i < length; i++)
					ExpectedDensity[i] = _quantileVector[i + 1] - _quantileVector[i];
			}
		}

		protected double[] ExpectedDensity {
			get;
			set;
		}

		public ExpertOpinionFramework () : this (new double[] { 0, .5, 1 })
		{
		}

		public ExpertOpinionFramework (double[] quantileVector)
		{
			this.OvershootFactor = 0.1;
			this.QuantileVector = quantileVector;
			this.LowerBound = null;
			this.UpperBound = null;

			this.estimates = new CompositeKeyArray<Expert, Variable> (this.QuantileVector.Length - 2);
		}

		public void AddEstimate (string expertName, string variableName, params double[] quantiles)
		{
			Expert e;
			Variable v;

			;

			if (!estimates.TryGetKey1 (x => x.Name == expertName, out e)) {
				e = new Expert (expertName);
			}

			if (!estimates.TryGetKey2 (x => x.Name == variableName, out v)) {
				v = new Variable (variableName);
			}

			estimates.Add (e, v, quantiles);
		}

		public void SetValue (string variableName, double value)
		{
			Variable v;
			if (!estimates.TryGetKey2 (x => x.Name == variableName, out v)) {
				throw new NotImplementedException ("Cannot set value of not estimated variable");
			}
			v.Value = value;
		}

		/// <summary>
		/// Gets the lower and upper bound, i.e. q0 and q1 values, for the specified variable.
		/// </summary>
		/// <param name="var">Variable.</param>
		public Tuple<double, double> GetBounds (Variable var)
		{
			var m = estimates.Min (var);
			if (var.Value != null && var.Value < m)
				m = (double) var.Value;
			m = LowerBound != null ? Math.Max (m, (double) LowerBound) : m; 

			var h = estimates.Max (var);
			if (var.Value != null && var.Value > h)
				h = (double) var.Value;
			h = UpperBound != null ? Math.Min (h, (double) UpperBound) : h;

			var l = (h - m) * OvershootFactor;
			return new Tuple<double, double> (m - l, h + l);
		}

		public IDistribution GetDistribution (string expertName, string variableName) 
		{
			var variable = Variables.Single (x => x.Name == variableName);
			var bounds = GetBounds (variable);
			var expert = Experts.Single (x => x.Name == expertName);

			var p = estimates [expert, variable];
			var tt = new List<double> (p.Length + 2);
			tt.Add (bounds.Item1);
			tt.AddRange (p);
			tt.Add (bounds.Item2);
			var t = tt.ToArray ();
			
			return new QuantileDistribution (QuantileVector, t);
		}

		public abstract IDistribution Fit (string variableName);

		#region EstimateTable

		protected class CompositeKeyArray<T, U> {

			private int _arrayLength;

			private Dictionary<CompositeKey<T, U>, double[]> _table;

			public CompositeKeyArray (int arrayLength)
			{
				_arrayLength = arrayLength;
				_table = new Dictionary<CompositeKey<T, U>, double[]> ();
			}

			public IEnumerable<T> Keys1 {
				get {
					foreach (var ikey in _table.Keys) {
						yield return ikey.Expert;
					}
				}
			}

			public IEnumerable<U> Keys2 {
				get {
					foreach (var ikey in _table.Keys) {
						yield return ikey.Variable;
					}
				}
			}

			public bool ContainsKey1 (Func<T, bool> predicate) {
				return _table.Keys.Any (x => predicate(x.Expert));
			}

			public bool ContainsKey2 (Func<U, bool> predicate) {
				return _table.Keys.Any (x => predicate(x.Variable));
			}

			public bool TryGetKey1 (Func<T, bool> predicate, out T value) {
				var tmp = _table.Keys.FirstOrDefault (x => predicate(x.Expert));
				value = (tmp != null) ? tmp.Expert : default(T);
				return value != null;
			}

			public bool TryGetKey2 (Func<U, bool> predicate, out U value) {
				var tmp = _table.Keys.FirstOrDefault (x => predicate(x.Variable));
				value = (tmp != null) ? tmp.Variable : default(U);
				return value != null;
			}

			public double[] this[T e, U v]
			{
				get
				{
					return _table[new CompositeKey<T, U> (e, v)];
				}
				set
				{
					_table[new CompositeKey<T, U> (e, v)] = value;
				}
			}

			public int ArrayLength {
				get {
					return _arrayLength;
				}
			}

			public IList<double> Get(U u)
			{
				var list = _table.Where (x => x.Key.Variable.Equals (u)).SelectMany (x => x.Value).ToList ();
				list.Sort ();
				return list;
			}


			public void Add (T e, U v, double[] quantiles)
			{
				if (quantiles.Length != _arrayLength)
					throw new ArgumentException ("Value has not the expected length (Expected: "+(quantiles.Length - 2)+", got "+quantiles.Length+")");

				_table.Add (new CompositeKey<T, U> (e, v), quantiles);
			}

			public double Min (U v)
			{
				return _table.Where (x => x.Key.Variable.Equals (v)).Select (x => x.Value.Min ()).Min ();
			}

			public double Max (U v)
			{
				return _table.Where (x => x.Key.Variable.Equals (v)).Select (x => x.Value.Max ()).Max();
			}

			private class CompositeKey<W, X>
			{
				public W Expert { get; private set; }
				public X Variable  { get; private set; }

				public CompositeKey (W expert, X variable)
				{
					this.Expert = expert;
					this.Variable = variable;
				}

				public override bool Equals (object obj)
				{
					if (obj == null)
						return false;
					if (ReferenceEquals (this, obj))
						return true;
					if (obj.GetType () != typeof(CompositeKey<W,X>))
						return false;
					var other = (CompositeKey<W,X>)obj;
					return Expert.Equals (other.Expert) && Variable.Equals (other.Variable);
				}

				public override int GetHashCode ()
				{
					unchecked {
						return (Expert != null ? Expert.GetHashCode () : 0) ^ (Variable != null ? Variable.GetHashCode () : 0);
					}
				}
				
			}
		}

		#endregion
	}
}

