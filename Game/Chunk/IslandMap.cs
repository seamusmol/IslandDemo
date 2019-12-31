
using Game;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media.Media3D;

namespace mapGen
{
    public class IslandMap
    {
        public int SX { get; }
        public int SY { get; }

        public List<Point> ic { get; set; } = new List<Point>();

        public Boolean[,] l { get; set; }
        public Boolean[,] o { get; set; }
        public Boolean[,] lk { get; set; }
        public Boolean[,] r { get; set; }
        public Boolean[,] m { get; set; }
        public Boolean[,] rd { get; set; }

        public int[,] ma { get; set; }
        public int[,] h { get; set; }

        public List<Point> MainLandPoints { get; set; } = new List<Point>();
        public List<Point> LakePoints { get; set; } = new List<Point>();
        public List<Point> DeltaPoints { get; set; } = new List<Point>();

        public List<Point3D> VegetationPoints { get; set; } = new List<Point3D>();
        public List<double> VegetationRotations { get; set; } = new List<double>();
        public List<String> VegetationNames { get; set; } = new List<String>();

        public List<Point3D> StructurePoints { get; set; } = new List<Point3D>();
        public List<double> StructureRotations { get; set; } = new List<double>();
        public List<String> StructureNames { get; set; } = new List<String>();

        public List<Village> VillageList = new List<Village>();

        public IslandMap(int X, int sx, int sy)
        {
            SX = sx;
            SY = sy;

            l = new Boolean[X + 1, X + 1];
            o = new Boolean[X + 1, X + 1];
            lk = new Boolean[X + 1, X + 1];
            r = new Boolean[X + 1, X + 1];
            m = new Boolean[X + 1, X + 1];
            rd = new Boolean[X + 1, X + 1];
            ma = new int[X + 1, X + 1];
            h = new int[X + 1, X + 1];
        }

        public IslandMap(int sx, int sy, Boolean[,] L, Boolean[,] O, Boolean[,] LK, Boolean[,] R, Boolean[,] M, Boolean[,] RD, int[,] MA, int[,] H)
        {
            SX = sx;
            SY = sy;
            l = L;
            o = O;
            lk = LK;
            r = R;
            m = M;
            rd = RD;
            ma = MA;
            h = H;
        }

        public Point GetDeltaPoint(Point Point)
        {
            for (int i = 0; i < LakePoints.Count; i++)
            {
                if (LakePoints[i].Equals(Point))
                {
                    return DeltaPoints[i];
                }
            }
            return new Point();
        }

    }
}

