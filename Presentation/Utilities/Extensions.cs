using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Presentation
{
    public static class Extensions
    {
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
