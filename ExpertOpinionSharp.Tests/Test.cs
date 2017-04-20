using NUnit.Framework;
using System;
using UCLouvain.ExpertOpinionSharp.Frameworks;

namespace ExpertOpinionSharp.Tests
{
	[TestFixture ()]
	public class Test
	{
		[Test ()]
		public void TestEarthMesureCase ()
		{
			var ef = new CookFramework (new double[] { 0, .05, .5, .95, 1 });

			ef.AddEstimate ("Simon", "Nil", 3000, 10000, 13000);
			ef.AddEstimate ("Simon", "K2", 6500, 8000, 8500);

			ef.AddEstimate ("Adrien", "Nil", 4000, 5500, 7000);
			ef.AddEstimate ("Adrien", "K2", 8000, 8300, 8600);

			ef.AddEstimate ("Samuel", "Nil", 3000, 6500, 7000);
			ef.AddEstimate ("Samuel", "K2", 8000, 8600, 9000);

			ef.SetValue ("Nil", 6550);
			// ef.SetValue ("K2", 8611);

			var dist = ef.Fit ("K2");
			// Console.WriteLine (dist.Mean);
		}
	}
}

