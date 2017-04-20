using NUnit.Framework;
using System;
using ExpertOpinionModelling;
using System.IO;

namespace ExpertOpinionSharp.Tests
{
	[TestFixture ()]
	public class TestMendelSheridan
	{
		[Test ()]
		public void TestEarthMesureCase ()
		{
			var ef = new MendelSheridanFramework (new double[] { 0, .05, .5, .95, 1 });
			ef.AddEstimate ("Simon", "Nil", 3000, 10000, 13000);
			ef.AddEstimate ("Simon", "K2", 6500, 7500, 8500);

			ef.AddEstimate ("Adrien", "Nil", 4000, 5500, 7000);
			ef.AddEstimate ("Adrien", "K2", 8000, 8300, 8600);

			ef.AddEstimate ("Samuel", "Nil", 3000, 6500, 7000);
			ef.AddEstimate ("Samuel", "K2", 8000, 8600, 9000);

			ef.SetValue ("Nil", 6550);
			// ef.SetValue ("K2", 8611);

			var distSimon = ef.GetDistribution ("Simon", "K2");
			var distSamuel = ef.GetDistribution ("Samuel", "K2");
			var distAdrien = ef.GetDistribution ("Adrien", "K2");

			var dm = ef.Fit ("K2");

			var f = new StreamWriter ("/Users/acailliau/Desktop/data.txt");
			f.WriteLine ("sim,sam,adri,dm");
			for (int i = 0; i < 500000; i++) {
				f.Write ("{0:##.####},", distSimon.Sample ());
				f.Write ("{0:##.####},", distSamuel.Sample ());
				f.Write ("{0:##.####},", distAdrien.Sample ());
				f.WriteLine ("{0:##.####}", dm.Sample ());
			}
			f.Close ();
		}

		[Test ()]
		public void TestCase ()
		{
			var ef = new MendelSheridanFramework (new double[] { 0, .4, 1 });

			ef.AddEstimate ("Expert 0", "Variable 0", 140);
			ef.AddEstimate ("Expert 0", "Variable 1", .2);
			ef.AddEstimate ("Expert 0", "Variable 2", .4);
			ef.AddEstimate ("Expert 0", "Variable 3", 2500);
			ef.AddEstimate ("Expert 0", "Variable 4", 1000);

			ef.AddEstimate ("Expert 1", "Variable 0", 200);
			ef.AddEstimate ("Expert 1", "Variable 1", .22);
			ef.AddEstimate ("Expert 1", "Variable 2", .6);
			ef.AddEstimate ("Expert 1", "Variable 3", 1500);
			ef.AddEstimate ("Expert 1", "Variable 4", 1200);

			ef.SetValue ("Variable 0", 195);
			ef.SetValue ("Variable 1", .21);
			ef.SetValue ("Variable 2", .5);
			ef.SetValue ("Variable 3", 2000);

			var dm = ef.Fit ("Variable 4");
			var d1 = ef.GetDistribution ("Expert 0", "Variable 4");
			var d2 = ef.GetDistribution ("Expert 1", "Variable 4");

			var f = new StreamWriter ("/Users/acailliau/Desktop/data.txt");
			f.WriteLine ("e1,e2,dm");
			for (int i = 0; i < 100000; i++) {
				f.Write ("{0:##.####},", d1.Sample ());
				f.Write ("{0:##.####},", d2.Sample ());
				f.WriteLine ("{0:##.####}", dm.Sample ());
			}
			f.Close ();
		}

		[Test ()]
		public void TestCase2 ()
		{
			var ef = new MendelSheridanFramework (new double[] { 0, .25, .75, 1 });

			ef.AddEstimate ("Expert 0", "Variable 0", 130, 170);
			ef.AddEstimate ("Expert 0", "Variable 1", .1, .3);
			ef.AddEstimate ("Expert 0", "Variable 2", .3, .5);
			ef.AddEstimate ("Expert 0", "Variable 3", 2000, 3000);
			ef.AddEstimate ("Expert 0", "Variable 4", 1000, 1200);

			ef.AddEstimate ("Expert 1", "Variable 0", 180, 220);
			ef.AddEstimate ("Expert 1", "Variable 1", .20, .23);
			ef.AddEstimate ("Expert 1", "Variable 2", .5, .7);
			ef.AddEstimate ("Expert 1", "Variable 3", 1500, 1600);
			ef.AddEstimate ("Expert 1", "Variable 4", 1200, 1300);

			ef.SetValue ("Variable 0", 195);
			ef.SetValue ("Variable 1", .21);
			ef.SetValue ("Variable 2", .5);
			ef.SetValue ("Variable 3", 2000);

			var dm = ef.Fit ("Variable 4");
			var d1 = ef.GetDistribution ("Expert 0", "Variable 4");
			var d2 = ef.GetDistribution ("Expert 1", "Variable 4");

			var f = new StreamWriter ("/Users/acailliau/Desktop/data.txt");
			f.WriteLine ("e1,e2,dm");
			for (int i = 0; i < 100000; i++) {
				f.Write ("{0:##.####},", d1.Sample ());
				f.Write ("{0:##.####},", d2.Sample ());
				f.WriteLine ("{0:##.####}", dm.Sample ());
			}
			f.Close ();
		}
	}
}

