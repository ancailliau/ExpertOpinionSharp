﻿using System;
using System.Collections.Generic;
using System.Linq;
using MathNet.Numerics.Distributions;

namespace ExpertOpinionModelling
{
    class MainClass
    {
    
        public static void ExampleBook (string[] args)
        {
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

            var manager = new ExpertManager (new [] { expert1, expert2, expert3 }, new [] { item1, item2, item3, item4 });

            Console.WriteLine("-- Empirical distributions");
            Console.WriteLine(string.Join ("\n", manager.Experts.Select (x => "s(" + x.Name + ") = (" + string.Join (",", manager.GetEmpiricalDistributions (x)) + ")" ) ) );
            Console.WriteLine();

            Console.WriteLine("-- Calibration");
            Console.WriteLine(string.Join ("\n", manager.GetCalibrationScores ().Select (x => "I(" + x.Item1.Name + ") = " + x.Item2)));
            Console.WriteLine();

            /*
            foreach (var v in manager.Variables) {
                Console.WriteLine("-- Interpolated distributions for " + v.Name );
                Console.WriteLine(manager.LH (v));
                Console.WriteLine(string.Join ("\n", manager.Experts.Select (x => "s(" + x.Name + ") = (" + string.Join (",", manager.GetInterpolatedDistribution (v, x)) + ")" ) ) );
                Console.WriteLine();
            }
            */

            Console.WriteLine("-- Information");
            Console.WriteLine(string.Join ("\n", manager.GetInformationScores ().Select (x => "C(" + x.Item1.Name + ") = " + x.Item2)));
            Console.WriteLine();

            Console.WriteLine("-- Weight");
            Console.WriteLine(string.Join ("\n", manager.GetWeights ().Select (x => "W(" + x.Item1.Name + ") = " + x.Item2)));
            Console.WriteLine();

        }

        public static void ExampleNilK2 (string[] args)
        {
            Console.WriteLine("Hello World!");

            var nil = new CalibrationVariable ("Nil", 6550);
            var k2 = new CalibrationVariable ("K2", 8600);

            var simon = new Expert ("Simon", new [] { 
                new ExpertOpinion (nil, new double [] { 3000, 10000, 13000 }),
                new ExpertOpinion (k2, new double [] { 6500, 8000, 8500 })
            });

            var adrien = new Expert ("Adrien", new [] { 
                new ExpertOpinion (nil, new double [] { 4000, 5500, 7000 }),
                new ExpertOpinion (k2, new double [] { 8000, 8300, 8600 })
            });

            var sam = new Expert ("Samuel", new [] { 
                new ExpertOpinion (nil, new double [] { 3000, 6500, 7000 }),
                new ExpertOpinion (k2, new double [] { 8000, 8600, 9000 })
            });

            var manager = new ExpertManager (new [] { simon, adrien, sam }, new [] { nil, k2 });

            Console.WriteLine(string.Join ("\n", manager.GetWeights ().Select (x => x.Item1.Name + " = " + x.Item2)));

        }

        public static void Main (string[] args)
        {
            ExampleNilK2 (args);
        }
    }
}
