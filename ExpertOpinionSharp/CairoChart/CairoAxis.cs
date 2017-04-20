using System;
using System.Collections.Generic;
using Cairo;
using System.Linq;

namespace UCLouvain.ExpertOpinionSharp
{
	public class CairoAxis
	{
		CairoBarChart cairoBarChart;

		public double LowerBound {
			get;
			set;
		}

		public double UpperBound {
			get;
			set;
		}

		public CairoAxis (CairoBarChart cairoBarChart, double lowerBound, double upperBound)
		{
			this.cairoBarChart = cairoBarChart;
			this.LowerBound = lowerBound;
			this.UpperBound = upperBound;
		}

		public double Scale (double point)
		{
			return (point - LowerBound) / (UpperBound - LowerBound);
		}

	}

}

