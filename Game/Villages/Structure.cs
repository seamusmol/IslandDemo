using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Media3D;

namespace Game
{
    public class Structure
    {
        public Point3D Position { get; }
        public int Angle { get; }
        public int WX { get; }
        public int WY { get; }
        public int WZ { get; }

        public Structure(Point3D P, int angle, int wx, int wy, int wz)
        {
            Position = P;
            Angle = angle;
            WX = wx;
            WY = wy;
            WZ = wz;
        }

    }
}
