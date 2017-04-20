using System;
using System.Collections.Generic;
using System.Linq;
using UCLouvain.ExpertOpinionSharp.Distributions;

namespace UCLouvain.ExpertOpinionSharp.Frameworks
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
				return new HashSet<Expert> (Estimates.Keys1);
			}
		}

		public ISet<Variable> Variables {
			get {
				return new HashSet<Variable> (Estimates.Keys2);
			}
		}

		protected CompositeKeyArray<Expert, Variable> Estimates;

		protected int NbQuantiles {
			get {
				return QuantileVector.Length - 2;
			}
		}

		double[] _quantileVector;

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

		protected ExpertOpinionFramework () : this (new double[] { 0, .5, 1 })
		{
		}

		protected ExpertOpinionFramework (double[] quantileVector)
		{
			OvershootFactor = 0.1;
			QuantileVector = quantileVector;
			LowerBound = null;
			UpperBound = null;

			Estimates = new CompositeKeyArray<Expert, Variable> (QuantileVector.Length - 2);
		}

		public void AddEstimate (string expertName, string variableName, params double[] quantiles)
		{
			Expert e;
			Variable v;

			if (!Estimates.TryGetKey1 (x => x.Name == expertName, out e)) {
				e = new Expert (expertName);
			}

			if (!Estimates.TryGetKey2 (x => x.Name == variableName, out v)) {
				v = new Variable (variableName);
			}

			Estimates.Add (e, v, quantiles);
		}

		public void SetValue (string variableName, double value)
		{
			Variable v;
			if (!Estimates.TryGetKey2 (x => x.Name == variableName, out v)) {
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
			var m = Estimates.Min (var);
			if (var.Value != null && var.Value < m)
				m = (double) var.Value;
			m = LowerBound != null ? Math.Max (m, (double) LowerBound) : m; 

			var h = Estimates.Max (var);
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

			var p = Estimates [expert, variable];
			var tt = new List<double> (p.Length + 2);
			tt.Add (bounds.Item1);
			tt.AddRange (p);
			tt.Add (bounds.Item2);
			var t = tt.ToArray ();
			
			return new QuantileDistribution (QuantileVector, t);
		}

		public abstract IDistribution Fit (string variableName);

		#region EstimateTable

		protected class CompositeKeyArray<T, TU> {

			int _arrayLength;

			readonly Dictionary<CompositeKey<T, TU>, double[]> _table;

			public CompositeKeyArray (int arrayLength)
			{
				_arrayLength = arrayLength;
				_table = new Dictionary<CompositeKey<T, TU>, double[]> ();
			}

			public IEnumerable<T> Keys1 {
				get {
					foreach (var ikey in _table.Keys) {
						yield return ikey.Key1;
					}
				}
			}

			public IEnumerable<TU> Keys2 {
				get {
					foreach (var ikey in _table.Keys) {
						yield return ikey.Key2;
					}
				}
			}

			public bool ContainsKey1 (Func<T, bool> predicate) {
				return _table.Keys.Any (x => predicate(x.Key1));
			}

			public bool ContainsKey2 (Func<TU, bool> predicate) {
				return _table.Keys.Any (x => predicate(x.Key2));
			}

			public bool TryGetKey1 (Func<T, bool> predicate, out T value) {
				var tmp = _table.Keys.FirstOrDefault (x => predicate(x.Key1));
				value = (tmp != null) ? tmp.Key1 : default(T);
				return value != null;
			}

			public bool TryGetKey2 (Func<TU, bool> predicate, out TU value) {
				var tmp = _table.Keys.FirstOrDefault (x => predicate(x.Key2));
				value = (tmp != null) ? tmp.Key2 : default(TU);
				return value != null;
			}

			public double[] this[T e, TU v]
			{
				get
				{
					return _table[new CompositeKey<T, TU> (e, v)];
				}
				set
				{
					_table[new CompositeKey<T, TU> (e, v)] = value;
				}
			}

			public int ArrayLength {
				get {
					return _arrayLength;
				}
			}

			public IList<double> Get(TU u)
			{
				var list = _table.Where (x => x.Key.Key2.Equals (u)).SelectMany (x => x.Value).ToList ();
				list.Sort ();
				return list;
			}


			public void Add (T e, TU v, double[] quantiles)
			{
				if (quantiles.Length != _arrayLength)
					throw new ArgumentException ("Value has not the expected length (Expected: "+(quantiles.Length - 2)+", got "+quantiles.Length+")");

				_table.Add (new CompositeKey<T, TU> (e, v), quantiles);
			}

			public double Min (TU v)
			{
				return _table.Where (x => x.Key.Key2.Equals (v)).Min (x => x.Value.Min ());
			}

			public double Max (TU v)
			{
				return _table.Where (x => x.Key.Key2.Equals (v)).Max (x => x.Value.Max ());
			}

			class CompositeKey<TW, TX>
			{
				public TW Key1 { get; private set; }
				public TX Key2  { get; private set; }

				public CompositeKey (TW key1, TX key2)
				{
					Key1 = key1;
					Key2 = key2;
				}

				public override bool Equals (object obj)
				{
					if (obj == null)
						return false;
					if (ReferenceEquals (this, obj))
						return true;
					if (obj.GetType () != typeof(CompositeKey<TW,TX>))
						return false;
					var other = (CompositeKey<TW,TX>)obj;
					return Key1.Equals (other.Key1) && Key2.Equals (other.Key2);
				}

				public override int GetHashCode ()
				{
					unchecked {
						return (Key1 != null ? Key1.GetHashCode () : 0) ^ (Key2 != null ? Key2.GetHashCode () : 0);
					}
				}
				
			}
		}

		#endregion
	}
}

