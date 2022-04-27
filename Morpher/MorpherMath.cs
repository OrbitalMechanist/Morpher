using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Morpher
{
    internal class MorpherMath
    {
        public static Vector CreateVector(VirtualControlLine line) { return new Vector(line.X2 - line.X1, line.Y2 - line.Y1); }
        public static Vector CreateNormal(Vector v) { return new Vector(-v.Y, v.X);}
        public static double Magnitude(Vector v) { return Math.Sqrt(v.X * v.X + v.Y * v.Y); }
        public static double DotProduct(Vector a, Vector b) { return a.X * b.X + a.Y * b.Y; }
        public static double ProjectionMagnitude(Vector top, Vector btm) { return DotProduct(top, btm) / Magnitude(btm); }
    }
}
