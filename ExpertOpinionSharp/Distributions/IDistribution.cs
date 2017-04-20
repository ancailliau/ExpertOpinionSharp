namespace UCLouvain.ExpertOpinionSharp.Distributions
{
	public interface IDistribution
	{
		double Sample ();

		double LowerBound {
			get;
		}

		double UpperBound {
			get;
		}
	}
}

