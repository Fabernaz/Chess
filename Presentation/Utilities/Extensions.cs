using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Presentation
{
    public static class Utils
    {
        public static Size SquareSize(this Size size)
        {
            var minSide = GetMinSide(size);
            size.Height = minSide;
            size.Width = minSide;
            return size;
        }

        public static double GetMinSide(this Size size)
        {
            return GetMinSide(size.Height, size.Width);
        }

        public static double GetMinSide(double height, double width)
        {
            return Math.Min(height, width);
        }
    }
}
