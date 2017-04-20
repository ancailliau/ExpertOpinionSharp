using System;
using System.Collections.Generic;
using System.Linq;
using MathNet.Numerics.Distributions;
using System.IO;
using UCLouvain.ExpertOpinionSharp.Frameworks;

namespace UCLouvain.ExpertOpinionSharp
{
    class MainClass
    {

		/*
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
		*/

        public static void Main (string[] args)
		{
			var ef = new MendelSheridanFramework (new double[] { 0, .1, .5, .9, 1 });
			ef.AddEstimate ("Expert 1", "push", 3.81, 4.29, 4.76);
			ef.AddEstimate ("Expert 1", "temp", 2.31, 2.59, 2.88);
			ef.AddEstimate ("Expert 1", "sprinkler", 0.023, 0.026, 0.029);
			ef.AddEstimate ("Expert 1", "battery", 2.31, 2.59, 2.88);
			ef.AddEstimate ("Expert 1", "bell", 1.47, 1.65, 1.83);

			ef.AddEstimate ("Expert 2", "push", 3.96, 4.45, 4.95);
			ef.AddEstimate ("Expert 2", "temp", 4.18, 4.70, 5.22);
			ef.AddEstimate ("Expert 2", "sprinkler", 0.019, 0.022, 0.024);
			ef.AddEstimate ("Expert 2", "battery", 0.16, 0.18, 0.19);
			ef.AddEstimate ("Expert 2", "bell", 1.70, 1.91, 2.12);

			ef.SetValue ("push", 4.37);
			ef.SetValue ("temp", 4.37);
			ef.SetValue ("sprinkler", 0.02);
			// ef.SetValue ("K2", 8611);


			var dm = ef.Fit ("bell");
			Console.WriteLine (dm);

			/*
			var w = ef.GetInformationScores ();
			Console.WriteLine (string.Join ("\n", w.Select (x => x.Item1 + " = " + x.Item2)));

			w = ef.GetCalibrationScores ();
			Console.WriteLine (string.Join ("\n", w.Select (x => x.Item1 + " = " + x.Item2)));

			var w2 = ef.GetWeights ();
			Console.WriteLine (string.Join ("\n", w2.Select (x => x.Item1 + " = " + x.Item2)));
			*/

			/*
			var distSimon = ef.GetDistribution ("Simon", "K2");
			var distSamuel = ef.GetDistribution ("Samuel", "K2");
			var distAdrien = ef.GetDistribution ("Adrien", "K2");

			var dm = ef.Fit ("K2");

			Console.WriteLine (dm);
			*/

			/*
			var data = new List<double> ();
			for (int i = 0; i < 1000000; i++) {
				data.Add (distSimon.Sample ());
			}

			var hist = new Histogram (data.ToArray (), 80, 6000, 10000);

			var chart = hist.ToCairoBarChart ();
			chart.ImageWidth = 600;
			chart.WriteToPng ("/Users/acailliau/Desktop/data.png");
*/
			/*
			var f = new StreamWriter ("/Users/acailliau/Desktop/data.txt");
			f.WriteLine ("sim,sam,adri,dm");
			for (int i = 0; i < 500000; i++) {
				f.Write ("{0:##.####},", distSimon.Sample ());
				f.Write ("{0:##.####},", distSamuel.Sample ());
				f.Write ("{0:##.####},", distAdrien.Sample ());
				f.WriteLine ("{0:##.####}", dm.Sample ());
			}
			f.Close ();
			*/
        }
    }
}
