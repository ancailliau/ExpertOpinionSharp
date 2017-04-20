using System;
using System.Collections.Generic;
using Cairo;
using System.Linq;

namespace UCLouvain.ExpertOpinionSharp
{
	public class CairoBarChart
	{
		public CairoBarChart ()
		{
			ImageWidth = 320;
			ImageHeight = 240;
			Padding = 10;
			Data = new BarChartData ();
		}

		public BarChartData Data {
			get;
			set;
		}

		ImageSurface surface;

		Context context;

		public int ImageWidth { get; set; }
		public int ImageHeight { get; set; }

		public int Padding {
			get;
			set;
		}

		public int DrawingWidth {
			get { return ImageWidth - Padding * 2; }
		}

		public int DrawingHeight {
			get { return ImageHeight - Padding * 2; }
		}

		public CairoAxis XAxis { get; set; }

		public CairoAxis YAxis { get; set; }

		PointD ToDrawingArea (PointD p) {
			p.X *= DrawingWidth; p.X += Padding;
			p.Y *= DrawingHeight; p.X += Padding;
			return p;
		}

		PointD Scale (PointD p)
		{
			return ToDrawingArea (new PointD (XAxis.Scale (p.X), YAxis.Scale (p.Y)));
		}

		void Display ()
		{
			XAxis = new CairoAxis (this, Data.LowerBound (), Data.UpperBound ());
			YAxis = new CairoAxis (this, Data.MaxValue (), Data.MinValue ());

			surface = new ImageSurface (Format.Argb32, ImageWidth, ImageHeight);
			context = new Context (surface);

			context.SetSourceColor (new Color (1, 1, 1));
			context.Rectangle (0, 0, ImageWidth, ImageHeight);
			context.Fill ();


			context.SetSourceRGB (0, 0, 0);
			context.LineWidth = 1;

			foreach (var d in Data.Where (x => x.Value > 0)) {

				var width = Math.Round (XAxis.Scale (d.UpperBound - d.LowerBound));
				var height = Math.Round (YAxis.Scale (d.Value));

				var p = new PointD (XAxis.Scale (d.LowerBound), YAxis.Scale (d.Value));

				context.Rectangle (p, width, -height);
				context.Fill ();
			}

			/*
			context.SetSourceColor (new Color (0, 0, 0));
			context.Arrow (Padding + .5, ImageHeight - Padding + .5,
				ImageWidth - Padding + .5, ImageHeight - Padding + .5,
				10, Math.PI / 8);

			context.Arrow (Padding, ImageHeight - Padding,
				Padding, Padding,
				10, Math.PI / 8);
			*/

		}

		public void WriteToPng (string filename)
		{
			Display ();

			surface.WriteToPng (filename);
			context.Dispose ();
			surface.Dispose ();

		}
	}
}

