using System;
using System.Collections.Generic;
using System.Linq;
using MathNet.Numerics.Distributions;
using System.IO;

namespace ExpertOpinionModelling
{
    class MainClass
    {
        public static void ExampleBook (string[] args)
        {
			/*
            Console.WriteLine("Hello World!");

            var item1 = new CalibrationVariable ("Item 1", 27);
            var item2 = new CalibrationVariable ("Item 2", 45);
            var item3 = new CalibrationVariable ("Item 3", 60);
            var item4 = new CalibrationVariable ("Item 4", 28);

            var expert1 = new Expert ("Expert 1", new [] { 
                new ExpertOpinion (item1, new double [] { 25, 32, 37 }),
                new ExpertOpinion (item2, new double [] { 35, 42, 47 }),
                new ExpertOpinion (item3, new double [] { 50, 57, 62 }),
                new ExpertOpinion (item4, new double [] { 25, 32, 37 })
            });

            var expert2 = new Expert ("Expert 2", new [] { 
                new ExpertOpinion (item1, new double [] { 0, 45, 78 }),
                new ExpertOpinion (item2, new double [] { 0, 45, 78 }),
                new ExpertOpinion (item3, new double [] { 0, 45, 78 }),
                new ExpertOpinion (item4, new double [] { 0, 45, 78 })
            });

            var expert3 = new Expert ("Expert 3", new [] { 
                new ExpertOpinion (item1, new double [] { 40, 45, 52 }),
                new ExpertOpinion (item2, new double [] { 35, 40, 47 }),
                new ExpertOpinion (item3, new double [] { 39, 44, 51 }),
                new ExpertOpinion (item4, new double [] { 41, 46, 53 })
            });

            var manager = new CookFramework (new [] { expert1, expert2, expert3 }, new [] { item1, item2, item3, item4 });

            Console.WriteLine("-- Empirical distributions");
            Console.WriteLine(string.Join ("\n", manager.Experts.Select (x => "s(" + x.Name + ") = (" + string.Join (",", manager.GetEmpiricalDistributions (x)) + ")" ) ) );
            Console.WriteLine();

            Console.WriteLine("-- Calibration");
            Console.WriteLine(string.Join ("\n", manager.GetCalibrationScores ().Select (x => "I(" + x.Item1.Name + ") = " + x.Item2)));
            Console.WriteLine();

            foreach (var v in manager.Variables) {
                Console.WriteLine("-- Interpolated distributions for " + v.Name );
                Console.WriteLine(manager.LH (v));
                Console.WriteLine(string.Join ("\n", manager.Experts.Select (x => "s(" + x.Name + ") = (" + string.Join (",", manager.GetInterpolatedDistribution (v, x)) + ")" ) ) );
                Console.WriteLine();
            }

            Console.WriteLine("-- Information");
            Console.WriteLine(string.Join ("\n", manager.GetInformationScores ().Select (x => "C(" + x.Item1.Name + ") = " + x.Item2)));
            Console.WriteLine();

            Console.WriteLine("-- Weight");
            Console.WriteLine(string.Join ("\n", manager.GetWeights ().Select (x => "W(" + x.Item1.Name + ") = " + x.Item2)));
            Console.WriteLine();
			*/
        }

        public static void ExampleNilK2 (string[] args)
        {
            Console.WriteLine("Hello World!");

			var ef = new CookFramework ();
			ef.AddEstimate ("Simon", "Nil", 3000, 10000, 13000);
			ef.AddEstimate ("Simon", "K2", 6500, 8000, 8500);

			ef.AddEstimate ("Adrien", "Nil", 4000, 5500, 7000);
			ef.AddEstimate ("Adrien", "K2", 8000, 8300, 8600);

			ef.AddEstimate ("Samuel", "Nil", 3000, 6500, 7000);
			ef.AddEstimate ("Samuel", "K2", 8000, 8600, 9000);

			ef.SetValue ("Nil", 6550);
			ef.SetValue ("K2", 8611);


			Console.WriteLine("-- Empirical distributions");
			Console.WriteLine(string.Join ("\n", ef.Experts.Select (x => "s(" + x.Name + ") = (" + string.Join (",", ef.GetEmpiricalDistributions (x)) + ")" ) ) );
			Console.WriteLine();

			Console.WriteLine("-- Calibration");
			Console.WriteLine(string.Join ("\n", ef.GetCalibrationScores ().Select (x => "I(" + x.Item1.Name + ") = " + x.Item2)));
			Console.WriteLine();

			foreach (var v in ef.Variables) {
				Console.WriteLine("-- Interpolated distributions for " + v.Name );
				Console.WriteLine(ef.GetBounds (v));
				Console.WriteLine(string.Join ("\n", ef.Experts.Select (x => "s(" + x.Name + ") = (" + string.Join (",", ef.GetInterpolatedDistribution (v, x)) + ")" ) ) );
				Console.WriteLine();
			}

			Console.WriteLine("-- Information");
			Console.WriteLine(string.Join ("\n", ef.GetInformationScores ().Select (x => "C(" + x.Item1.Name + ") = " + x.Item2)));
			Console.WriteLine();

			Console.WriteLine("-- Weight");
			Console.WriteLine(string.Join ("\n", ef.GetWeights ().Select (x => "W(" + x.Item1.Name + ") = " + x.Item2)));
			Console.WriteLine();

        }

		public static void ExampleNilK2Optimal (string[] args)
		{
			Console.WriteLine ("Hello World!");

			var ef = new CookFramework ();
			ef.AddEstimate ("Simon", "Nil", 3000, 10000, 13000);
			ef.AddEstimate ("Simon", "K2", 6500, 7500, 8500);

			ef.AddEstimate ("Adrien", "Nil", 4000, 5500, 7000);
			ef.AddEstimate ("Adrien", "K2", 8000, 8300, 8600);

			ef.AddEstimate ("Samuel", "Nil", 3000, 6500, 7000);
			ef.AddEstimate ("Samuel", "K2", 8000, 8600, 9000);

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

		public static void ExampleNilK2OptimalKK (string[] args)
		{

			var ef = new CookFramework ();
			ef.AddEstimate ("Simon", "Nil", 3000, 10000, 13000);
			ef.AddEstimate ("Simon", "K2", 6500, 7500, 8000);

			ef.AddEstimate ("Adrien", "Nil", 4000, 5500, 7000);
			ef.AddEstimate ("Adrien", "K2", 8000, 8300, 8600);

//			ef.AddEstimate ("Samuel", "Nil", 3000, 6500, 7000);
//			ef.AddEstimate ("Samuel", "K2", 8000, 8600, 9000);

			ef.SetValue ("Nil", 6550);
			var d = ef.Fit ("K2");

			var simonDist = ef.GetDistribution ("K2", "Simon");
//			Console.WriteLine (simonDist.Minimum + " - " + simonDist.Maximum);

			var f = new StreamWriter ("/Users/acailliau/Desktop/data.txt");
			f.WriteLine ("sim,adri,raw1,raw2,data");

			for (int i = 0; i < 100; i++) {
				f.Write ("{0:##.####},", simonDist.Sample ());
//				f.Write ("{0:##.####},", ef.GetDistribution ("K2", "Samuel").Sample ());
				f.Write ("{0:##.####},", ef.GetDistribution ("K2", "Adrien").Sample ());
//				var d2 = d.RandomSource.NextDouble ();
//				f.Write ("{0:##.####},", ((d2 * (8000 - 6500) + 6500)));
//				f.Write ("{0:##.####},", ((d2 * (8600 - 8000) + 8000)));
//				f.WriteLine ("{0:##.####}", d.Sample ());
			}
			f.Close ();

			/*
			Console.WriteLine("-- Empirical distributions");
			Console.WriteLine(string.Join ("\n", ef.Experts.Select (x => "s(" + x.Name + ") = (" + string.Join (",", ef.GetEmpiricalDistributions (x)) + ")" ) ) );
			Console.WriteLine();

			Console.WriteLine("-- Calibration");
			Console.WriteLine(string.Join ("\n", ef.GetCalibrationScores ().Select (x => "I(" + x.Item1.Name + ") = " + x.Item2)));
			Console.WriteLine();

			foreach (var v in ef.Variables) {
				Console.WriteLine("-- Interpolated distributions for " + v.Name );
				Console.WriteLine(ef.GetBounds (v));
				Console.WriteLine(string.Join ("\n", ef.Experts.Select (x => "s(" + x.Name + ") = (" + string.Join (",", ef.GetInterpolatedDistribution (v, x)) + ")" ) ) );
				Console.WriteLine();
			}

			Console.WriteLine("-- Information");
			Console.WriteLine(string.Join ("\n", ef.GetInformationScores ().Select (x => "C(" + x.Item1.Name + ") = " + x.Item2)));
			Console.WriteLine();

			Console.WriteLine("-- Weight");
			Console.WriteLine(string.Join ("\n", ef.GetOptimalWeights ().Select (x => "W(" + x.Item1.Name + ") = " + x.Item2)));
			Console.WriteLine();
			*/
		}

        public static void Main (string[] args)
		{
			// ExampleNilK2 (args);
			ExampleNilK2Optimal (args);
        }
    }
}
