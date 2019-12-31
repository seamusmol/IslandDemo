using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Diagnostics;
using mapGen;
using main;
using System.IO;
using System.ComponentModel;
using System.Windows.Media.Media3D;

namespace Game
{
    public static class MapUtil
    {
        public static Boolean HasSurrounding(int X, int Y, Boolean[,] Map, int Radius)
        {
            for (int countX = -Radius; countX <= Radius; countX++)
            {
                for (int countY = -Radius; countY <= Radius; countY++)
                {
                    if (Map[X + countX, Y + countY])
                    {
                        int dist = (int)Math.Sqrt(countX * countX + countY * countY);
                        if (dist <= Radius)
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        public static Boolean HasSurrounding(Point3D Position, List<Point3D> Points, double Distance)
        {
            for (int i = 0; i < Points.Count; i++)
            {
                if (MathUtil.MathUtil.distance(Position, Points[i]) <= Distance)
                {
                    return true;
                }
            }
            return false;
        }

        public static Boolean HasSurrounding(int X, int Y, int[,] Map, int Radius)
        {
            for (int countX = -Radius; countX <= Radius; countX++)
            {
                for (int countY = -Radius; countY <= Radius; countY++)
                {
                    if (Map[X + countX, Y + countY] != 0)
                    {
                        int dist = (int)Math.Sqrt(countX * countX + countY * countY);
                        if (dist <= Radius)
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        //4 sided search
        public static int GetDirection(int X, int Y, Boolean[,] Map)
        {
            int[] vals = new int[4] { 999, 999, 999, 999 };

            for (int i = X; i < Map.GetLength(0); i++)
            {
                if (Map[i, Y])
                {
                    vals[1] = i - X;
                    break;
                }
            }
            for (int i = X; i >= 0; i--)
            {
                if (Map[i, Y])
                {
                    vals[3] = X - i;
                    break;
                }
            }
            for (int j = Y; j < Map.GetLength(1); j++)
            {
                if (Map[X, j])
                {
                    vals[2] = j - Y;
                    break;
                }
            }
            for (int j = Y; j >= 0; j--)
            {
                if (Map[X, j])
                {
                    vals[0] = Y - j;
                    break;
                }
            }

            int lowestIndex = 0;
            for (int i = 0; i < vals.Length; i++)
            {
                if (vals[lowestIndex] > vals[i])
                {
                    lowestIndex = i;
                }
            }
            return vals[0] == 999 && vals[1] == 999 && vals[2] == 999 && vals[3] == 999 ? 4 : lowestIndex;
        }

        public static Boolean HasStructureSurrounding(Structure NewStructure, Boolean[,] Map, int Radius)
        {
            for (int nx = 0; nx < NewStructure.WX; nx++)
            {
                for (int nz = 0; nz < NewStructure.WZ; nz++)
                {
                    Point NRotPoint = MathUtil.MathUtil.RotatePoint(nx, nz, NewStructure.Angle);
                    NRotPoint.X += NewStructure.Position.X;
                    NRotPoint.Y += NewStructure.Position.Z;

                    if (HasSurrounding((int)(NRotPoint.X), (int)(NRotPoint.Y), Map, Radius))
                    {
                        return true;
                    }
                }
            }
            return false;
        }



        public static Boolean HasStructureSurroundingStructure(Structure NewStructure, Village Village, int Radius)
        {
            for (int i = 0; i < Village.StructureList.Count; i++)
            {
                //check if intersecting
                for (int nx = 0; nx < NewStructure.WX; nx++)
                {
                    for (int nz = 0; nz < NewStructure.WZ; nz++)
                    {
                        Point NRotPoint = MathUtil.MathUtil.RotatePoint(nx, nz, NewStructure.Angle);
                        NRotPoint.X += NewStructure.Position.X;
                        NRotPoint.Y += NewStructure.Position.Z;

                        for (int tx = 0; tx < Village.StructureList[i].WX; tx++)
                        {
                            for (int tz = 0; tz < Village.StructureList[i].WZ; tz++)
                            {
                                Point TRotPoint = MathUtil.MathUtil.RotatePoint(tx, tz, Village.StructureList[i].Angle);
                                TRotPoint.X += Village.StructureList[i].Position.X;
                                TRotPoint.Y += Village.StructureList[i].Position.Z;

                                if (MathUtil.MathUtil.distance(NRotPoint, TRotPoint) < Radius)
                                {
                                    return true;
                                }

                            }
                        }
                    }

                }
            }

            return false;
        }

        public static Boolean HasStructureIntersection(Structure NewStructure, Village Village)
        {
            for (int i = 0; i < Village.StructureList.Count; i++)
            {
                //check if intersecting
                for (int nx = 0; nx < NewStructure.WX; nx++)
                {
                    for (int nz = 0; nz < NewStructure.WZ; nz++)
                    {
                        Point NRotPoint = MathUtil.MathUtil.RotatePoint(nx, nz, NewStructure.Angle);
                        NRotPoint.X += NewStructure.Position.X;
                        NRotPoint.Y += NewStructure.Position.Z;

                        for (int tx = 0; tx < Village.StructureList[i].WX; tx++)
                        {
                            for (int tz = 0; tz < Village.StructureList[i].WZ; tz++)
                            {
                                Point TRotPoint = MathUtil.MathUtil.RotatePoint(tx, tz, Village.StructureList[i].Angle);
                                TRotPoint.X += Village.StructureList[i].Position.X;
                                TRotPoint.Y += Village.StructureList[i].Position.Z;

                                if (NRotPoint.X == TRotPoint.X && NRotPoint.Y == TRotPoint.Y)
                                {
                                    return true;
                                }
                            }
                        }
                    }

                }
            }
            return false;
        }

        public static int GetDirectionAngle(int X, int Y, Boolean[,] Map)
        {
            int[] vals = new int[5] { 0, 90, 180, 270, -999 };

            return vals[GetDirection(X, Y, Map)];
        }

        public static int GetSurroundingCount(int X, int Y, Boolean[,] Map, int Radius)
        {
            int count = 0;
            for (int countX = -Radius; countX <= Radius; countX++)
            {
                for (int countY = -Radius; countY <= Radius; countY++)
                {
                    if (Map[X + countX, Y + countY])
                    {
                        int dist = (int)Math.Sqrt(countX * countX + countY * countY);
                        if (dist <= Radius * Radius)
                        {
                            count++;
                        }
                    }
                }
            }
            return count;
        }

        public static Boolean[,] createCurve(Boolean[,] Map, Point P1, Point P2, int width)
        {
            Boolean[,] newMap = Map;

            Vector dir = new Vector(P2.X - P1.X, P2.Y - P1.Y);
            dir.Normalize();

            int distance = (int)MathUtil.MathUtil.distance(P1, P2);

            for (int i = 0; i < distance; i++)
            {
                float wavePosX = (float)(dir.X * i + dir.X * Math.Sin(Math.PI * i / distance) * 10);
                float wavePosY = (float)(dir.Y * i + dir.Y * Math.Sin(Math.PI * i / distance) / 10);

                int px = (int)(P1.X + wavePosX);
                int py = (int)(P1.Y + wavePosY);

                for (int countX = -width; countX < width; countX++)
                {
                    for (int countY = -width; countY < width; countY++)
                    {
                        //Debug.WriteLine((px + countX) +  "-" +  (py + countY));
                        newMap[px + countX, py + countY] = true;
                    }
                }
                for (int countX = -width; countX < width; countX++)
                {
                    for (int countY = -width; countY < width; countY++)
                    {
                        newMap[(int)P2.X + countX, (int)P2.Y + countY] = true;
                    }
                }

            }
            return newMap;
        }

        public static Boolean[,] generateSineWave(Boolean[,] Map, Point StartPoint, Point EndPoint, int WaveCount, int Width, int SX, int WX)
        {
            Boolean[,] WaveMap = Map;

            Vector direction = new Vector(EndPoint.X - StartPoint.X, EndPoint.Y - StartPoint.Y);
            direction.Normalize();
            Vector crossDir = new Vector(direction.Y, direction.X);

            int distance = (int)MathUtil.MathUtil.distance(StartPoint, EndPoint);

            for (int i = 0; i < distance; i++)
            {
                int posX = (int)Math.Round(StartPoint.X + direction.X * i);
                int posY = (int)Math.Round(StartPoint.Y + direction.Y * i);

                float waveX = (float)((20 * Math.Sin(Math.PI * i / distance * WaveCount)) * direction.Y);
                float waveY = (float)((20 * Math.Sin(Math.PI * i / distance * WaveCount)) * direction.X);

                Point position = new Point(posX + waveX, posY + waveY);

                int width = (int)(SX % (WX / 64) * Math.Sin(Math.PI * i / distance * WaveCount * WaveCount) + (WX / 64) + 1);

                for (int j = -width; j < width; j++)
                {
                    int crossOffsetX = (int)Math.Round(crossDir.X * j);
                    int crossOffsetY = (int)Math.Round(crossDir.Y * j);
                    if (position.X + crossOffsetX > 1 && position.X + crossOffsetX < WX - 1 && position.Y + crossOffsetY > 1 && position.Y + crossOffsetY < WX - 1)
                    {
                        WaveMap[(int)position.X + crossOffsetX, (int)position.Y + crossOffsetY] = true;
                        WaveMap[(int)position.X + crossOffsetX - 1, (int)position.Y + crossOffsetY] = true;
                        WaveMap[(int)position.X + crossOffsetX, (int)position.Y + crossOffsetY - 1] = true;
                        WaveMap[(int)position.X + crossOffsetX + 1, (int)position.Y + crossOffsetY] = true;
                        WaveMap[(int)position.X + crossOffsetX, (int)position.Y + crossOffsetY + 1] = true;
                    }
                }
            }
            return WaveMap;
        }

        public static double GetDist(int X, int Y, Boolean[,] Map, int Radius)
        {
            double dist = Radius * 4;
            int minX = X - Radius >= 0 ? X - Radius : 0;
            int minY = Y - Radius >= 0 ? Y - Radius : 0;
            int maxX = X + Radius < Map.GetLength(0) ? X + Radius : Map.GetLength(0);
            int maxY = Y + Radius < Map.GetLength(1) ? Y + Radius : Map.GetLength(1);
            for (int i = minX; i < maxX; i++)
            {
                for (int j = minY; j < maxY; j++)
                {
                    if (Map[i, j])
                    {
                        double tempDist = MathUtil.MathUtil.distance(new Point(X, Y), new Point(i, j));
                        if (tempDist <= dist)
                        {
                            dist = tempDist;
                        }
                    }
                }
            }
            return dist == Radius * 4 ? -1 : dist;
        }

        public static Point getClosest(int X, int Y, Boolean[,] Map, int Radius)
        {
            double curDist = Radius * 4;
            Point p = new Point(-1, -1);
            Point Position = new Point(X, Y);
            double dist = Radius * 4;
            int minX = X - Radius >= 0 ? X - Radius : 0;
            int minY = Y - Radius >= 0 ? Y - Radius : 0;
            int maxX = X + Radius < Map.GetLength(0) ? X + Radius : Map.GetLength(0);
            int maxY = Y + Radius < Map.GetLength(1) ? Y + Radius : Map.GetLength(1);

            for (int i = minY; i < maxY; i++)
            {
                for (int j = minX; j < maxX; j++)
                {
                    if (Map[i, j])
                    {
                        Point newPoint = new Point(i, j);
                        double distance = MathUtil.MathUtil.distance(Position, newPoint);
                        if (distance < curDist)
                        {
                            p = newPoint;
                            curDist = distance;
                        }
                    }
                }
            }
            return p;
        }

        public static Point getClosest(int X, int Y, int[,] Map, int Radius)
        {
            double curDist = Radius * 4;
            Point p = new Point(-1, -1);
            Point Position = new Point(X, Y);
            double dist = Radius * 4;
            int minX = X - Radius >= 0 ? X - Radius : 0;
            int minY = Y - Radius >= 0 ? Y - Radius : 0;
            int maxX = X + Radius < Map.GetLength(0) ? X + Radius : Map.GetLength(0);
            int maxY = Y + Radius < Map.GetLength(1) ? Y + Radius : Map.GetLength(1);

            for (int i = minY; i < maxY; i++)
            {
                for (int j = minX; j < maxX; j++)
                {
                    if (Map[i, j] != 0)
                    {
                        Point newPoint = new Point(i, j);
                        double distance = MathUtil.MathUtil.distance(Position, newPoint);
                        if (distance < curDist)
                        {
                            p = newPoint;
                            curDist = distance;
                        }
                    }
                }
            }
            return p;
        }


        public static Boolean[,] OverrideMap(Boolean[,] ShadeMap, Boolean[,] Map)
        {
            Boolean[,] newMap = ShadeMap;
            for (int i = 0; i < newMap.GetLength(0); i++)
            {
                for (int j = 0; j < newMap.GetLength(0); j++)
                {
                    if (Map[i, j])
                    {
                        newMap[i, j] = Map[i, j];
                    }
                }
            }
            return newMap;
        }

        public static Boolean[,] FillShape(List<Point> PointList, Boolean[,] ShadeMap)
        {
            Point Center = new Point();
            for (int i = 0; i < PointList.Count; i++)
            {
                Center.X += PointList[i].X;
                Center.Y += PointList[i].Y;
            }
            Center.X /= PointList.Count;
            Center.Y /= PointList.Count;

            List<Point> SortedList = PointList;
            SortedList.Sort((a, b) => MathUtil.MathUtil.GetAngle(Center, a).CompareTo(MathUtil.MathUtil.GetAngle(Center, b)));

            SortedList.Add(SortedList[0]);
            for (int i = 0; i < SortedList.Count - 1; i++)
            {
                RayTrace(SortedList[i], SortedList[i + 1], ShadeMap);
            }

            Boolean[,] FillMap = basicFill(ShadeMap);
            Boolean[,] NewMap = new Boolean[FillMap.GetLength(0), FillMap.GetLength(1)];
            for (int i = 0; i < NewMap.GetLength(0); i++)
            {
                for (int j = 0; j < NewMap.GetLength(1); j++)
                {
                    NewMap[i, j] = !FillMap[i, j];
                }
            }
            return NewMap;
        }

        public static Boolean[,] basicFill(Boolean[,] PointMap)
        {
            Boolean[,] shadeMap = new Boolean[PointMap.GetLength(1), PointMap.GetLength(1)];
            //j++;
            for (int i = 0; i < shadeMap.GetLength(1); i++)
            {
                for (int j = 0; j < shadeMap.GetLength(1); j++)
                {
                    if (PointMap[i, j])
                    {
                        break;
                    }
                    shadeMap[i, j] = true;
                }
            }
            //j--;
            for (int i = 0; i < shadeMap.GetLength(1); i++)
            {
                for (int j = shadeMap.GetLength(1) - 1; j >= 0; j--)
                {
                    if (PointMap[i, j])
                    {
                        break;
                    }
                    shadeMap[i, j] = true;
                }
            }

            //i++;
            for (int i = 0; i < shadeMap.GetLength(0); i++)
            {
                for (int j = 0; j < shadeMap.GetLength(1); j++)
                {
                    if (PointMap[i, j])
                    {
                        break;
                    }
                    shadeMap[i, j] = true;
                }
            }
            //i--;
            for (int j = 0; j < shadeMap.GetLength(1); j++)
            {
                for (int i = shadeMap.GetLength(0) - 1; i >= 0; i--)
                {
                    if (PointMap[i, j])
                    {
                        break;
                    }
                    shadeMap[i, j] = true;
                }
            }

            Boolean hasMove = true;
            while (hasMove)
            {
                hasMove = false;
                for (int i = 1; i < PointMap.GetLength(0) - 1; i++)
                {
                    for (int j = 1; j < PointMap.GetLength(1) - 1; j++)
                    {
                        if (shadeMap[i, j])
                        {
                            if (!PointMap[i - 1, j] && !shadeMap[i - 1, j])
                            {
                                shadeMap[i - 1, j] = true;
                                hasMove = true;
                            }
                            if (!PointMap[i + 1, j] && !shadeMap[i + 1, j])
                            {
                                shadeMap[i + 1, j] = true;
                                hasMove = true;
                            }
                            if (!PointMap[i, j - 1] && !shadeMap[i, j - 1])
                            {
                                shadeMap[i, j - 1] = true;
                                hasMove = true;
                            }
                            if (!PointMap[i, j + 1] && !shadeMap[i, j + 1])
                            {
                                shadeMap[i, j + 1] = true;
                                hasMove = true;
                            }
                        }
                    }
                }
            }
            return shadeMap;
        }

        public static void nonFill(Boolean[,] PointMap)
        {
            for (int i = 1; i < PointMap.GetLength(0) - 1; i++)
            {
                for (int j = 1; j < PointMap.GetLength(1) - 1; j++)
                {
                    if (!PointMap[i, j])
                    {
                        int val = 0;
                        val += rayTest(new Point(i, j), new Point(0, j), PointMap) ? 1 : 0;
                        val += rayTest(new Point(i, j), new Point(PointMap.GetLength(0), j), PointMap) ? 1 : 0;
                        val += rayTest(new Point(i, j), new Point(i, 0), PointMap) ? 1 : 0;
                        val += rayTest(new Point(i, j), new Point(i, PointMap.GetLength(1)), PointMap) ? 1 : 0;

                        if (val == 4)
                        {
                            PointMap[i, j] = true;
                        }
                    }
                }
            }
        }

        public static void RayTrace(Point Start, Point End, Boolean[,] ShadeMap)
        {
            Vector dir = new Vector(End.X - Start.X, End.Y - Start.Y);
            dir.Normalize();
            int distance = (int)MathUtil.MathUtil.distance(Start, End);
            for (int i = 0; i <= distance; i++)
            {
                ShadeMap[(int)(Start.X + dir.X * i), (int)(Start.Y + dir.Y * i)] = true;
            }

        }

        public static Boolean rayTest(Point Start, Point End, Boolean[,] PointMap)
        {
            Vector dir = new Vector(End.X - Start.X, End.Y - Start.Y);
            dir.Normalize();
            int distance = (int)MathUtil.MathUtil.distance(Start, End);
            for (int i = 0; i <= distance; i++)
            {
                if (PointMap[(int)(Start.X + dir.X * i), (int)(Start.Y + dir.Y * i)])
                {
                    return true;
                }
            }
            return false;
        }

        /*
        public Boolean[,] l { get; set; }
        public Boolean[,] o { get; set; }
        public Boolean[,] lk { get; set; }
        public Boolean[,] r { get; set; }
        public Boolean[,] m { get; set; }
        public Boolean[,] rd { get; set; }

        public int[,] ma { get; set; }
        public int[,] h { get; set; }
        */

        //Generate CompressedLists
        //Add size of Buffers
        //add buffers
        //convert to byte array
        //create file
        public static void SaveMap(String FileName, IslandMap IslandMap)
        {
            BinaryWriter writer = new BinaryWriter(File.Open("WorldSave/" + FileName, FileMode.Create));

            List<byte> MapData = new List<byte>();

            byte[] CompressedSX = BitConverter.GetBytes(IslandMap.SX);
            byte[] CompressedSY = BitConverter.GetBytes(IslandMap.SY);

            byte[] CompressedLValues = (byte[])CompressBooleanArray2D(IslandMap.l)[0];
            byte[] CompressedOValues = (byte[])CompressBooleanArray2D(IslandMap.o)[0];
            byte[] CompressedLKValues = (byte[])CompressBooleanArray2D(IslandMap.lk)[0];
            byte[] CompressedRValues = (byte[])CompressBooleanArray2D(IslandMap.r)[0];
            byte[] CompressedMValues = (byte[])CompressBooleanArray2D(IslandMap.m)[0];
            byte[] CompressedRDValues = (byte[])CompressBooleanArray2D(IslandMap.rd)[0];
            byte[] CompressedMaValues = (byte[])CompressIntArray2D(IslandMap.ma)[0];
            byte[] CompressedHValues = (byte[])CompressIntArray2D(IslandMap.h)[0];

            byte[] CompressedLQuantities = (byte[])CompressBooleanArray2D(IslandMap.l)[1];
            byte[] CompressedOQuantities = (byte[])CompressBooleanArray2D(IslandMap.o)[1];
            byte[] CompressedLKQuantities = (byte[])CompressBooleanArray2D(IslandMap.lk)[1];
            byte[] CompressedRQuantities = (byte[])CompressBooleanArray2D(IslandMap.r)[1];
            byte[] CompressedMQuantities = (byte[])CompressBooleanArray2D(IslandMap.m)[1];
            byte[] CompressedRDQuantities = (byte[])CompressBooleanArray2D(IslandMap.rd)[1];
            byte[] CompressedMaQuantities = (byte[])CompressIntArray2D(IslandMap.ma)[1];
            byte[] CompressedHQuantities = (byte[])CompressIntArray2D(IslandMap.h)[1];

            byte[] CompressedLSize = BitConverter.GetBytes(CompressedLValues.Length);
            byte[] CompressedOSize = BitConverter.GetBytes(CompressedOValues.Length);
            byte[] CompressedLKSize = BitConverter.GetBytes(CompressedLKValues.Length);
            byte[] CompressedRSize = BitConverter.GetBytes(CompressedRValues.Length);
            byte[] CompressedMSize = BitConverter.GetBytes(CompressedMValues.Length);
            byte[] CompressedRDSize = BitConverter.GetBytes(CompressedRDValues.Length);
            byte[] CompressedMaSize = BitConverter.GetBytes(CompressedMaValues.Length / 4);
            byte[] CompressedHSize = BitConverter.GetBytes(CompressedHValues.Length / 4);

            if (File.Exists(FileName))
            {
                //overwrite file
            }
            else
            {
                //create new file
                BinaryWriter FileWriter = new BinaryWriter(File.Open(FileName, FileMode.Create));

                //seedX,seexY
                FileWriter.Write(CompressedSX);
                FileWriter.Write(CompressedSY);

                //buffer value length
                FileWriter.Write(CompressedLSize);
                FileWriter.Write(CompressedOSize);
                FileWriter.Write(CompressedLKSize);
                FileWriter.Write(CompressedRSize);
                FileWriter.Write(CompressedMSize);
                FileWriter.Write(CompressedRDSize);
                FileWriter.Write(CompressedMaSize);
                FileWriter.Write(CompressedHSize);

                //values
                //Quantities
                FileWriter.Write(CompressedLValues);
                FileWriter.Write(CompressedLQuantities);
                FileWriter.Write(CompressedOValues);
                FileWriter.Write(CompressedOQuantities);
                FileWriter.Write(CompressedLKValues);
                FileWriter.Write(CompressedLKQuantities);
                FileWriter.Write(CompressedRValues);
                FileWriter.Write(CompressedRQuantities);
                FileWriter.Write(CompressedMValues);
                FileWriter.Write(CompressedMQuantities);
                FileWriter.Write(CompressedRDValues);
                FileWriter.Write(CompressedRDQuantities);
                FileWriter.Write(CompressedMaValues);
                FileWriter.Write(CompressedMaQuantities);
                FileWriter.Write(CompressedHValues);
                FileWriter.Write(CompressedHQuantities);

                FileWriter.Close();
            }
        }

        public static IslandMap LoadMap(String FileName)
        {
            //retrieve file
            List<byte> Data = new List<byte>();

            using (BinaryReader FileReader = new BinaryReader(File.Open(FileName, FileMode.Open)))
            {
                Data.Add(FileReader.ReadByte());
            }

            int byteCount = 0;

            int SX = Data[byteCount++] & 0xFF | (Data[byteCount++] & 0xFF) << 8 | (Data[byteCount++] & 0xFF) << 16 | (Data[byteCount++] & 0xFF) << 24;
            int SY = Data[byteCount++] & 0xFF | (Data[byteCount++] & 0xFF) << 8 | (Data[byteCount++] & 0xFF) << 16 | (Data[byteCount++] & 0xFF) << 24;

            int CompressedLSize = Data[byteCount++] & 0xFF | (Data[byteCount++] & 0xFF) << 8 | (Data[byteCount++] & 0xFF) << 16 | (Data[byteCount++] & 0xFF) << 24;
            int CompressedOSize = Data[byteCount++] & 0xFF | (Data[byteCount++] & 0xFF) << 8 | (Data[byteCount++] & 0xFF) << 16 | (Data[byteCount++] & 0xFF) << 24;
            int CompressedLKSize = Data[byteCount++] & 0xFF | (Data[byteCount++] & 0xFF) << 8 | (Data[byteCount++] & 0xFF) << 16 | (Data[byteCount++] & 0xFF) << 24;
            int CompressedRSize = Data[byteCount++] & 0xFF | (Data[byteCount++] & 0xFF) << 8 | (Data[byteCount++] & 0xFF) << 16 | (Data[byteCount++] & 0xFF) << 24;
            int CompressedMSize = Data[byteCount++] & 0xFF | (Data[byteCount++] & 0xFF) << 8 | (Data[byteCount++] & 0xFF) << 16 | (Data[byteCount++] & 0xFF) << 24;
            int CompressedRDSize = Data[byteCount++] & 0xFF | (Data[byteCount++] & 0xFF) << 8 | (Data[byteCount++] & 0xFF) << 16 | (Data[byteCount++] & 0xFF) << 24;
            int CompressedMaSize = Data[byteCount++] & 0xFF | (Data[byteCount++] & 0xFF) << 8 | (Data[byteCount++] & 0xFF) << 16 | (Data[byteCount++] & 0xFF) << 24;
            int CompressedHSize = Data[byteCount++] & 0xFF | (Data[byteCount++] & 0xFF) << 8 | (Data[byteCount++] & 0xFF) << 16 | (Data[byteCount++] & 0xFF) << 24;

            List<Boolean> LValues = new List<Boolean>();
            List<Boolean> OValues = new List<Boolean>();
            List<Boolean> LKValues = new List<Boolean>();
            List<Boolean> RValues = new List<Boolean>();
            List<Boolean> MValues = new List<Boolean>();
            List<Boolean> RDValues = new List<Boolean>();
            List<int> MaValues = new List<int>();
            List<int> HValues = new List<int>();

            List<int> LQuantities = new List<int>();
            List<int> OQuantities = new List<int>();
            List<int> LKQuantities = new List<int>();
            List<int> RQuantities = new List<int>();
            List<int> MQuantities = new List<int>();
            List<int> RDQuantities = new List<int>();
            List<int> MaQuantities = new List<int>();
            List<int> HQuantities = new List<int>();

            Boolean[,] L = new Boolean[ApplicationSettings.chunkSize + 1, ApplicationSettings.chunkSize + 1];
            Boolean[,] O = new Boolean[ApplicationSettings.chunkSize + 1, ApplicationSettings.chunkSize + 1];
            Boolean[,] LK = new Boolean[ApplicationSettings.chunkSize + 1, ApplicationSettings.chunkSize + 1];
            Boolean[,] R = new Boolean[ApplicationSettings.chunkSize + 1, ApplicationSettings.chunkSize + 1];
            Boolean[,] M = new Boolean[ApplicationSettings.chunkSize + 1, ApplicationSettings.chunkSize + 1];
            Boolean[,] RD = new Boolean[ApplicationSettings.chunkSize + 1, ApplicationSettings.chunkSize + 1];
            int[,] Ma = new int[ApplicationSettings.chunkSize + 1, ApplicationSettings.chunkSize + 1];
            int[,] H = new int[ApplicationSettings.chunkSize + 1, ApplicationSettings.chunkSize + 1];

            Decompress(LQuantities, LValues, CompressedLSize, byteCount, Data);
            Decompress(OQuantities, OValues, CompressedOSize, byteCount, Data);
            Decompress(LKQuantities, LKValues, CompressedLKSize, byteCount, Data);
            Decompress(RQuantities, RValues, CompressedRSize, byteCount, Data);
            Decompress(MQuantities, MValues, CompressedMSize, byteCount, Data);
            Decompress(RDQuantities, RDValues, CompressedRDSize, byteCount, Data);
            Decompress(MaQuantities, MaValues, CompressedMaSize, byteCount, Data);
            Decompress(HQuantities, HValues, CompressedHSize, byteCount, Data);

            ListTo2DArray(L, LQuantities, LValues);
            ListTo2DArray(O, OQuantities, OValues);
            ListTo2DArray(LK, LKQuantities, LKValues);
            ListTo2DArray(R, RQuantities, RValues);
            ListTo2DArray(M, MQuantities, MValues);
            ListTo2DArray(RD, RDQuantities, RDValues);
            ListTo2DArray(Ma, MaQuantities, MaValues);
            ListTo2DArray(H, HQuantities, HValues);

            IslandMap IslandMap = new IslandMap(SX, SY, L, O, LK, R, M, RD, Ma, H);

            return IslandMap;
        }

        private static void Decompress(List<int> Quantities, List<Boolean> Values, int ValueSize, int ByteCount, List<byte> Data)
        {
            for (int i = 0; i < ValueSize; i++)
            {
                Boolean Value = Convert.ToBoolean(Data[ByteCount++]);
                int Quantity = Data[ByteCount++] & 0xFF | (Data[ByteCount++] & 0xFF) << 8 | (Data[ByteCount++] & 0xFF) << 16 | (Data[ByteCount++] & 0xFF) << 24;
                Values.Add(Value);
                Quantities.Add(Quantity);
            }
        }

        private static void Decompress(List<int> Quantities, List<int> Values, int ValueSize, int ByteCount, List<byte> Data)
        {
            for (int i = 0; i < ValueSize; i++)
            {
                int Value = Data[ByteCount++] & 0xFF | (Data[ByteCount++] & 0xFF) << 8 | (Data[ByteCount++] & 0xFF) << 16 | (Data[ByteCount++] & 0xFF) << 24;
                int Quantity = Data[ByteCount++] & 0xFF | (Data[ByteCount++] & 0xFF) << 8 | (Data[ByteCount++] & 0xFF) << 16 | (Data[ByteCount++] & 0xFF) << 24;
                Values.Add(Value);
                Quantities.Add(Quantity);
            }
        }

        private static void ListTo2DArray(Boolean[,] Map, List<int> Quantities, List<Boolean> Values)
        {
            int QuantityCount = 0;
            for (int i = 0; i < Values.Count; i++)
            {
                for (int j = 0; j < Quantities[i]; j++)
                {
                    Map[QuantityCount / Map.GetLength(0), QuantityCount % Map.GetLength(1)] = Values[i];
                    QuantityCount++;
                }
            }
        }

        private static void ListTo2DArray(int[,] Map, List<int> Quantities, List<int> Values)
        {
            int QuantityCount = 0;
            for (int i = 0; i < Values.Count; i++)
            {
                for (int j = 0; j < Quantities[i]; j++)
                {
                    Map[QuantityCount / Map.GetLength(0), QuantityCount % Map.GetLength(1)] = Values[i];
                    QuantityCount++;
                }
            }
        }

        private static object[] CompressIntArray2D(int[,] Data)
        {
            List<Byte> quantities = new List<Byte>();
            List<Byte> values = new List<Byte>();

            int currentValue = Data[0, 0];
            int currentQuantity = 0;

            for (int i = 0; i < Data.GetLength(0); i++)
            {
                for (int j = 0; j < Data.GetLength(1); j++)
                {
                    if (i == Data.GetLength(0) - 1 && j == Data.GetLength(1) - 1)
                    {
                        //last value
                        if (currentValue == Data[i, j])
                        {
                            currentQuantity++;
                        }
                        else
                        {
                            byte[] AddBytes = BitConverter.GetBytes(currentQuantity);
                            quantities.Add(AddBytes[3]);
                            quantities.Add(AddBytes[2]);
                            quantities.Add(AddBytes[1]);
                            quantities.Add(AddBytes[0]);

                            byte[] AddFinalValueBytes = BitConverter.GetBytes(currentValue);
                            values.Add(AddFinalValueBytes[3]);
                            values.Add(AddFinalValueBytes[2]);
                            values.Add(AddFinalValueBytes[1]);
                            values.Add(AddFinalValueBytes[0]);

                            currentValue = Data[i, j];
                            currentQuantity = 1;
                        }

                        byte[] bytes = BitConverter.GetBytes(currentQuantity);
                        quantities.Add(bytes[3]);
                        quantities.Add(bytes[2]);
                        quantities.Add(bytes[1]);
                        quantities.Add(bytes[0]);

                        byte[] AddValueBytes = BitConverter.GetBytes(currentValue);
                        values.Add(AddValueBytes[3]);
                        values.Add(AddValueBytes[2]);
                        values.Add(AddValueBytes[1]);
                        values.Add(AddValueBytes[0]);
                    }
                    else if (currentValue == Data[i, j])
                    {
                        currentQuantity++;
                    }
                    else
                    {
                        byte[] bytes = BitConverter.GetBytes(currentQuantity);
                        quantities.Add(bytes[3]);
                        quantities.Add(bytes[2]);
                        quantities.Add(bytes[1]);
                        quantities.Add(bytes[0]);

                        byte[] AddValueBytes = BitConverter.GetBytes(currentValue);
                        values.Add(AddValueBytes[3]);
                        values.Add(AddValueBytes[2]);
                        values.Add(AddValueBytes[1]);
                        values.Add(AddValueBytes[0]);

                        currentValue = Data[i, j];
                        currentQuantity = 1;
                    }
                }
            }

            byte[] compressedValues = new byte[values.Count];
            byte[] compressedQuantities = new byte[quantities.Count];

            for (int i = 0; i < values.Count; i += 4)
            {
                compressedValues[i] = values[i];
                compressedValues[i + 1] = values[i + 1];
                compressedValues[i + 2] = values[i + 2];
                compressedValues[i + 3] = values[i + 3];

                compressedQuantities[i] = quantities[i];
                compressedQuantities[i + 1] = quantities[i + 1];
                compressedQuantities[i + 2] = quantities[i + 2];
                compressedQuantities[i + 3] = quantities[i + 3];
            }
            Object[] CompressedData = new Object[2];
            CompressedData[0] = compressedValues;
            CompressedData[1] = compressedQuantities;

            return CompressedData;
        }

        private static object[] CompressBooleanArray2D(Boolean[,] Data)
        {
            List<Byte> quantities = new List<Byte>();
            List<Boolean> values = new List<Boolean>();

            Boolean currentValue = Data[0, 0];
            int currentQuantity = 0;

            for (int i = 0; i < Data.GetLength(0); i++)
            {
                for (int j = 0; j < Data.GetLength(1); j++)
                {
                    if (i == Data.GetLength(0) - 1 && j == Data.GetLength(1) - 1)
                    {
                        //last value
                        if (currentValue == Data[i, j])
                        {
                            currentQuantity++;
                        }
                        else
                        {
                            byte[] AddBytes = BitConverter.GetBytes(currentQuantity);
                            quantities.Add(AddBytes[3]);
                            quantities.Add(AddBytes[2]);
                            quantities.Add(AddBytes[1]);
                            quantities.Add(AddBytes[0]);

                            values.Add(currentValue);

                            currentValue = Data[i, j];
                            currentQuantity = 1;
                        }

                        byte[] bytes = BitConverter.GetBytes(currentQuantity);
                        quantities.Add(bytes[3]);
                        quantities.Add(bytes[2]);
                        quantities.Add(bytes[1]);
                        quantities.Add(bytes[0]);

                        values.Add(currentValue);
                    }
                    else if (currentValue == Data[i, j])
                    {
                        currentQuantity++;
                    }
                    else
                    {
                        byte[] bytes = BitConverter.GetBytes(currentQuantity);
                        quantities.Add(bytes[3]);
                        quantities.Add(bytes[2]);
                        quantities.Add(bytes[1]);
                        quantities.Add(bytes[0]);

                        values.Add(currentValue);
                        currentValue = Data[i, j];
                        currentQuantity = 1;
                    }
                }
            }

            byte[] compressedValues = new byte[values.Count];
            byte[] compressedQuantities = new byte[quantities.Count];

            for (int i = 0; i < values.Count; i += 4)
            {
                compressedValues[i] = Convert.ToByte(values[i]);

                compressedQuantities[i] = quantities[i];
                compressedQuantities[i + 1] = quantities[i + 1];
                compressedQuantities[i + 2] = quantities[i + 2];
                compressedQuantities[i + 3] = quantities[i + 3];
            }
            Object[] CompressedData = new Object[2];
            CompressedData[0] = compressedValues;
            CompressedData[1] = compressedQuantities;

            return CompressedData;
        }

    }

}
