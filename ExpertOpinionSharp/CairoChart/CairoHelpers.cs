using System;
using Cairo;

namespace UCLouvain.ExpertOpinionSharp
{
	public static class CairoHelpers
	{
		public static void Arrow (this Context context, double start_x, double start_y, double end_x, double end_y, 
			double arrow_length, double arrow_degree)
		{
			double angle = Math.Atan2 (end_y - start_y, end_x - start_x) + Math.PI;

			var x1 = end_x + arrow_length * Math.Cos(angle - arrow_degree);
			var y1 = end_y + arrow_length * Math.Sin(angle - arrow_degree);
			var x2 = end_x + arrow_length * Math.Cos(angle + arrow_degree);
			var y2 = end_y + arrow_length * Math.Sin(angle + arrow_degree);

			context.MoveTo (start_x, start_y);
			context.LineTo (end_x, end_y);

			context.MoveTo (end_x, end_y);
			context.LineTo (x1, y1);
			context.Stroke ();

			context.MoveTo (end_x, end_y);
			context.LineTo (x2, y2);
			context.Stroke ();
		}
	}
}

