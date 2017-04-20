using System;

namespace UCLouvain.ExpertOpinionSharp
{
	public static class OptimizationHelper
	{
		public static double LocalMin (double lowerBound, double upperBound, Func<double, double> function,
		                               double eps, double t)
		{
			double c, d, e, m, p, q, r, tol, t2, u, v, w, fu, fv, fw, fx, x;

			c = (3.0 - Math.Sqrt (5.0)) / 2;

			x = v = w = lowerBound + c * (upperBound - lowerBound);
			fx = fv = fw = function (x);

			e = 0.0;
			d = 0.0;

			m = .5 * (lowerBound + upperBound);
			tol = eps * Math.Abs (x) + t;
			t2 = 2.0 * tol;

			while (Math.Abs (x - m) > (t2 - .5 * (upperBound - lowerBound))) {
				p = q = r = 0.0;
				if (Math.Abs (e) > tol) {
					r = (x - w) * (fx - fv);
					q = (x - v) * (fx - fw);
					p = (x - v) * q - (x - w) * r;
					q = 2.0 * (q - r);
					if (q > 0) {
						p = -p;
					} else {
						q = -q;
					}
					r = e;
					e = d;         
				}

				if ((Math.Abs (p) < Math.Abs (.5 * q * r)) & (p > q * (lowerBound - x)) & (p < q * (upperBound - x))) {
					d = p / q;
					u = x + d;
					if (((u - lowerBound) < t2) | ((upperBound - u) < t2)) {
						d = (x < m) ? tol : -tol;
					}
				} else {
					e = ((x < m) ? upperBound : lowerBound);
					d = c * (e - x);
				}

				if (Math.Abs (d) >= tol) {
					u = x + d;
				} else {
					u = x + ((d > 0) ? tol : -tol);
				}

				fu = function(u);

				if (fu <= fx) {
					if (u < x) {
						upperBound = x;
					} else {
						lowerBound = x;
					}

					v = w;
					fv = fw;
					w = x;
					fw = fx;
					x = u;
					fx = fu;

				} else {
					if (u < x) {
						lowerBound = u;
					} else {
						upperBound = u;
					}
					if ((fu <= fw) | (w == x)) {
						v = w;
						fv = fw;
						w = u;
						fw = fu;
					} else if ((fu <= fv) | (v == x) | (v == w)) {
						v = u;
						fv = fu;
					}
				}

				m = .5 * (lowerBound + upperBound);
				tol = eps * Math.Abs (x) + t;
				t2 = 2.0 * tol;
			}

			return x;
		}
	}
}

