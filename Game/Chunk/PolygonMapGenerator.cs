using System;
using System.Windows;
using System.Collections.Generic;
using MathUtil;
using System.Diagnostics;
using main;
using System.Windows.Media.Media3D;
using Game;
/*
* TODO
* Add comperator for island points
*/

namespace mapGen
{
    public class IslandMapGenerator
    {
        int sx, sy;
        int wx;
        int wy;

        int pl = 10;
        int spl = 20;
        int ipc;

        int ipd;
        int sipd;

        int buffer;
        int useRegion;

        public IslandMap IslandMap { get; set; }

        public IslandMapGenerator(int SeedX, int SeedY, int WX, int WY)
        {
            wx = WX;
            wy = WY;

            ipd = wx / 4;
            sipd = ipd / 2;

            buffer = wx / 8;
            useRegion = wx - (buffer * 2);

            sx = SeedX;
            sy = SeedY;

            ipc = 20;
            IslandMap = new IslandMap(wx, sx, sy);

            Stopwatch Watch = new Stopwatch();
            Watch.Start();
            generateMainLand();
            Debug.WriteLine("MainLand: " + Watch.Elapsed.TotalMilliseconds);
            Watch.Restart();
            generateLake();
            Debug.WriteLine("Lake: " + Watch.Elapsed.TotalMilliseconds);
            Watch.Restart();
            generateMountains();
            Debug.WriteLine("Mountain: " + Watch.Elapsed.TotalMilliseconds);
            Watch.Restart();
            GenerateHeightMap();
            Debug.WriteLine("Heightmap: " + Watch.Elapsed.TotalMilliseconds);
            Watch.Restart();
            GenerateMoistureMap();
            Debug.WriteLine("Moisture: " + Watch.Elapsed.TotalMilliseconds);
            Watch.Restart();
            GenerateLakeVillage();
            Debug.WriteLine("Village: " + Watch.Elapsed.TotalMilliseconds);
            Watch.Restart();
            GenerateVegation();
            Debug.WriteLine("Vegetation: " + Watch.Elapsed.TotalMilliseconds);
            Watch.Stop();
        }

        public void generateMainLand()
        {
            int[] Points = MathUtil.MathUtil.generateFibonocciNumbers(sx, sy, (ipc + 1) * 2, (int)Math.Log10(wx));
            List<Point> mainLandPoints = new List<Point>();
            for (int i = 0; i < Points.GetLength(0); i += 2)
            {
                mainLandPoints.Add(new Point(buffer + Points[i] % (useRegion / 2), buffer + Points[i + 1] % (useRegion / 2)));
            }
            mainLandPoints.Sort((p1, p2) => MathUtil.MathUtil.distance(p2, new Point(wx / 2, wy / 2)).CompareTo(MathUtil.MathUtil.distance(p1, new Point(wx / 2, wy / 2))));
            IslandMap.MainLandPoints = mainLandPoints;
            List<Point> p = new List<Point>();
            for (int i = 0; i < ipc; i++)
            {
                Point pos = mainLandPoints[i];
                int[] sectionPoints = MathUtil.MathUtil.generateFibonocciNumbers(sx + i + 10, sy + i + 10, ((sx + i + sy) % 40) * 2, (int)Math.Log10(wx));

                for (int j = 0; j < sectionPoints.GetLength(0); j += 2)
                {
                    Point sectionPointPos = new Point(sectionPoints[j] % (useRegion / 8), sectionPoints[j + 1] % (useRegion / 8));
                    p.Add(new Point(buffer + ((pos.X + sectionPointPos.X) % useRegion), buffer + ((pos.Y + sectionPointPos.Y) % useRegion)));
                }
            }
            p.Add(p[0]);
            Boolean[,] map = new Boolean[(wx + 1), (wy + 1)];
            for (int i = 0; i < p.Count - 1; i++)
            {
                map = MapUtil.createCurve(map, p[i], p[i + 1], 1);
            }

            Boolean[,] shadeMap = MapUtil.basicFill(map);
            for (int i = 0; i < map.GetLength(0); i++)
            {
                for (int j = 0; j < map.GetLength(1); j++)
                {
                    if (shadeMap[i, j])
                    {
                        IslandMap.o[i, j] = shadeMap[i, j];
                    }
                }
            }
            for (int i = 0; i < map.GetLength(0); i++)
            {
                for (int j = 0; j < map.GetLength(1); j++)
                {
                    IslandMap.l[i, j] = !shadeMap[i, j];
                }
            }
        }

        public void generateLake()
        {
            int lakeCount = sx % 5 + 1;
            int lakeRange = 12;

            int px = sx;
            int py = sy;

            int attempt = 0;
            int CurrentLakeCount = 0;

            while (attempt < 100 && CurrentLakeCount < 1)
            {
                Point Position = new Point(buffer + (px % (useRegion)), buffer + (py % (useRegion)));
                double Oceandistance = MapUtil.GetDist((int)Position.X, (int)Position.Y, IslandMap.o, wx / 2);

                //Debug.WriteLine("Center: " + Position);
                if (Oceandistance > lakeRange * 2 || Oceandistance == -1)
                {
                    IslandMap.LakePoints.Add(Position);
                    int[] LakePoints = MathUtil.MathUtil.generateFibonocciNumbers(sx + px + 12, sy + py + 13, 20, 2);

                    List<Point> P = new List<Point>();
                    for (int j = 0; j < LakePoints.GetLength(0) / 2; j++)
                    {
                        //Debug.WriteLine(new Point(Position.X + ((LakePoints[j] % lakeRange) - lakeRange / 2), Position.Y + ((LakePoints[j + 1] % lakeRange) - lakeRange / 2)));
                        Point newPoint = new Point(Position.X + ((LakePoints[j] % lakeRange) - lakeRange / 2), Position.Y + ((LakePoints[j + 1] % lakeRange) - lakeRange / 2));
                        P.Add(newPoint);

                    }
                    //add 5 points
                    P.Add(P[0]);
                    Boolean[,] Map = new Boolean[(wx + 1), (wy + 1)];
                    for (int j = 0; j < P.Count - 1; j++)
                    {
                        Map = MapUtil.createCurve(Map, P[j], P[j + 1], 2);
                    }
                    Boolean[,] shadeMap = MapUtil.basicFill(Map);
                    for (int countX = 0; countX < Map.GetLength(0); countX++)
                    {
                        for (int countY = 0; countY < Map.GetLength(1); countY++)
                        {
                            if (!IslandMap.lk[countX, countY])
                            {
                                IslandMap.lk[countX, countY] = !shadeMap[countX, countY];
                            }
                        }
                    }

                    Boolean[,] RiverMap = new Boolean[(wx + 1), (wy + 1)];
                    Point OceanPoint = MapUtil.getClosest((int)Position.X, (int)Position.Y, IslandMap.o, wx / 4);
                    Point RiverPoint = MapUtil.getClosest((int)Position.X, (int)Position.Y, IslandMap.r, wx / 4);

                    Point ClosestPoint = MathUtil.MathUtil.distance(Position, OceanPoint) < MathUtil.MathUtil.distance(Position, RiverPoint) ? OceanPoint : RiverPoint;
                    IslandMap.DeltaPoints.Add(ClosestPoint);

                    MapUtil.createCurve(RiverMap, Position, ClosestPoint, 2);
                    for (int countX = 0; countX < Map.GetLength(0); countX++)
                    {
                        for (int countY = 0; countY < Map.GetLength(1); countY++)
                        {
                            if (!IslandMap.r[countX, countY] && RiverMap[countX, countY])
                            {
                                IslandMap.r[countX, countY] = RiverMap[countX, countY];
                            }
                        }
                    }
                    CurrentLakeCount++;
                }
                attempt++;
                int tx = px;
                px += py + 1;
                px %= wx;
                py = (tx) % wx;

            }

            for (int i = 0; i < IslandMap.r.GetLength(0); i++)
            {
                for (int j = 0; j < IslandMap.r.GetLength(1); j++)
                {
                    if (IslandMap.r[i, j])
                    {
                        IslandMap.l[i, j] = false;
                    }
                    if (IslandMap.lk[i, j])
                    {
                        IslandMap.l[i, j] = false;
                    }
                }
            }
            //Debug.WriteLine("Attempts: " + attempt);
        }
        
        public void generateMountains()
        {
            //top x,bottom x
            //left y,right y
            int tx = wy / 2 + wy / 4 * (sy % sx) / sx;
            int bx = wy / 2 - wy / 4 * (sy % sx) / sy;

            int ly = wy / 2 + wy / 4 * (sy % sx) / sx;
            int ry = wy / 2 - wy / 4 * (sy % sx) / sy;

            int waveNumX = sx % 10 + 3;
            int waveNumY = sy % 10 + 3;

            int waveWidthX = wx / 32;

            Boolean[,] mountainMap = new Boolean[wx, wy];
            int px = Math.Max(tx, bx);
            int px2 = Math.Min(tx, bx);
            int py = Math.Max(ly, ry);
            int py2 = Math.Max(ly, ry);

            if (sx % 2 == 0)
            {
                mountainMap = MapUtil.generateSineWave(mountainMap, new Point(0, px), new Point(wx, px2), waveNumX, waveWidthX, sx, wx);
            }
            else
            {
                mountainMap = MapUtil.generateSineWave(mountainMap, new Point(px, 0), new Point(px, wy), waveNumX, waveWidthX, sx, wx);
            }

            int waterDistance = 4;

            for (int i = waterDistance; i < mountainMap.GetLength(0) - waterDistance; i++)
            {
                for (int j = waterDistance; j < mountainMap.GetLength(0) - waterDistance; j++)
                {
                    if (IslandMap.l[i, j] && mountainMap[i, j])
                    {
                        Boolean hasWater = false;
                        for (int countX = -waterDistance; countX < waterDistance; countX++)
                        {
                            for (int countY = -waterDistance; countY < waterDistance; countY++)
                            {
                                if (IslandMap.o[i + countX, j + countY] || IslandMap.lk[i + countX, j + countY])
                                {
                                    hasWater = true;
                                    goto completedSearch;
                                }
                            }
                        }
                        completedSearch:
                        if (!hasWater)
                        {
                            IslandMap.m[i, j] = true;
                        }
                    }
                }
            }
        }

        public void GenerateLakeVillage()
        {
            List<Point> LakePoints = IslandMap.LakePoints;
            List<Point> DeltaPoints = IslandMap.DeltaPoints;

            if (LakePoints.Count == 0)
            {
                return;
            }

            Point LakePoint = LakePoints[0];
            Point DeltaPoint = DeltaPoints[0];

            for (int i = 0; i < LakePoints.Count; i++)
            {
                Boolean HasVillageNearby = false;
                for (int j = 0; j < IslandMap.VillageList.Count; j++)
                {
                    if (MathUtil.MathUtil.distance(LakePoints[i], IslandMap.VillageList[j].CenterPosition) < 128)
                    {
                        HasVillageNearby = true;
                        goto Result;
                    }
                }
                Result:
                if (!HasVillageNearby)
                {
                    if (!MapUtil.HasSurrounding((int)LakePoints[i].X, (int)LakePoints[i].Y, IslandMap.m, 10))
                    {
                        LakeVillage Village = new LakeVillage(LakePoints[i], IslandMap, sx, sy);
                        IslandMap.VillageList.Add(Village);
                        Village.GenerateLayout();

                        //Debug.WriteLine(LakePoints[i]);
                        //suitable village location
                        //Debug.WriteLine(DeltaPoints.Count + "-" + LakePoints.Count);
                        //createBridge(LakePoints[i],DeltaPoints[i],IslandMap.r, 5);
                        return;
                    }
                }
            }
        }

        public void GetBodyPoints(int X, int Y, Boolean[,] Map, List<Point> PointList)
        {
            PointList.Add(new Point(X, Y));
            if (X == 0 || X == Map.GetLength(0) - 1 || Y == 0 || Y == Map.GetLength(1) - 1)
            {
                return;
            }
            else
            {
                if (Map[X - 1, Y] && PointList.Contains(new Point(X - 1, Y)))
                {
                    GetBodyPoints(X - 1, Y, Map, PointList);
                }
                if (Map[X + 1, Y] && PointList.Contains(new Point(X + 1, Y)))
                {
                    GetBodyPoints(X + 1, Y, Map, PointList);
                }
                if (Map[X, Y - 1] && PointList.Contains(new Point(X, Y - 1)))
                {
                    GetBodyPoints(X, Y - 1, Map, PointList);
                }
                if (Map[X, Y + 1] && PointList.Contains(new Point(X, Y + 1)))
                {
                    GetBodyPoints(X, Y + 1, Map, PointList);
                }
            }
        }


        public void GenerateVegation()
        {
            int px = sx + wx / 4;
            int py = sy + wx / 6;

            int VegetationDensity = 1;
            int VegetationOffset = 10;

            int VegCount = 0;
            for (int i = buffer / ApplicationSettings.chunkSize; i < ApplicationSettings.worldSize - buffer / ApplicationSettings.chunkSize; i++)
            {
                for (int j = buffer / ApplicationSettings.chunkSize; j < ApplicationSettings.worldSize - buffer / ApplicationSettings.chunkSize; j++)
                {
                    for (int CountX = 0; CountX < VegetationDensity; CountX++)
                    {
                        for (int CountY = 0; CountY < VegetationDensity; CountY++)
                        {
                            int x = (ApplicationSettings.chunkSize * i) + CountX * (ApplicationSettings.chunkSize / VegetationDensity) + (px % VegetationOffset - (VegetationOffset * 2) - 3);
                            int y = (ApplicationSettings.chunkSize * j) + CountY * (ApplicationSettings.chunkSize / VegetationDensity) + (py % VegetationOffset - (VegetationOffset * 2) - 3);

                            Point3D Position = new Point3D(x, IslandMap.h[x, y], y);

                            if (IslandMap.l[x, y] && !MapUtil.HasSurrounding(Position, IslandMap.VegetationPoints, 5) && !MapUtil.HasSurrounding(x, y, IslandMap.rd, 10) && IslandMap.ma[x, y] <= 5)
                            {
                                int PlantType = IslandMap.ma[x, y];
                                IslandMap.VegetationNames.Add("Plant_" + PlantType + "_");
                                IslandMap.VegetationPoints.Add(Position);
                                IslandMap.VegetationRotations.Add((VegCount * 90)%360);
                                VegCount++;
                            }

                            int tx = px;
                            px += py + 1;
                            py = (tx);
                        }
                    }

                }
            }
        }

        public void GenerateHeightMap()
        {
            int radius = 2;

            for (int i = radius; i < IslandMap.m.GetLength(0) - radius; i++)
            {
                for (int j = radius; j < IslandMap.m.GetLength(1) - radius; j++)
                {
                    if (IslandMap.l[i, j])
                    {
                        if (IslandMap.m[i, j])
                        {
                            IslandMap.h[i, j] = MapUtil.GetSurroundingCount(i, j, IslandMap.m, 5) / 16 + ApplicationSettings.seaLevel - 1;
                            IslandMap.h[i, j] = IslandMap.h[i, j] >= ApplicationSettings.seaLevel ? IslandMap.h[i, j] : ApplicationSettings.seaLevel;
                        }
                        else
                        {
                            IslandMap.h[i, j] = ApplicationSettings.seaLevel + 1;
                        }
                    }
                    else if (IslandMap.o[i, j])
                    {
                        if (MapUtil.HasSurrounding(i, j, IslandMap.l, radius))
                        {
                            IslandMap.h[i, j] = ApplicationSettings.seaLevel;
                        }
                        else
                        {
                            IslandMap.h[i, j] = ApplicationSettings.seaLevel - 1;
                        }
                    }
                    else if (IslandMap.lk[i, j])
                    {
                        IslandMap.h[i, j] = ApplicationSettings.seaLevel - 1;
                    }
                    else if (IslandMap.r[i, j])
                    {
                        if (MapUtil.HasSurrounding(i, j, IslandMap.l, radius))
                        {
                            IslandMap.h[i, j] = ApplicationSettings.seaLevel;
                        }
                        else
                        {
                            IslandMap.h[i, j] = ApplicationSettings.seaLevel - 1;
                        }
                    }
                }
            }

        }

        public void GenerateMoistureMap()
        {
            int Radius = wx / 4;

            //boundary loop
            //add all edges
            /*
            for (int i = 0; i < ApplicationSettings.chunkSize; i++)
            {
                for (int j = 0; j < IslandMap.ma.GetLength(0); j++)
                {
                    IslandMap.ma[i, j] = 6;
                    IslandMap.ma[j, i] = 6;
                }
            }
            */

            for (int i = ApplicationSettings.chunkSize; i < IslandMap.ma.GetLength(0) - ApplicationSettings.chunkSize; i++)
            {
                for (int j = ApplicationSettings.chunkSize; j < IslandMap.ma.GetLength(1) - ApplicationSettings.chunkSize; j++)
                {
                    if (MapUtil.HasSurrounding(i, j, IslandMap.o, 1))
                    {
                        IslandMap.ma[i, j] = 82;
                    }
                    else if (MapUtil.HasSurrounding(i, j, IslandMap.lk, 2) || MapUtil.HasSurrounding(i, j, IslandMap.r, 2))
                    {
                        int voxVal = IslandMap.r[i, j] && !IslandMap.lk[i, j] ? 85 : 84;
                        voxVal = voxVal == 84 && IslandMap.l[i, j] && !IslandMap.m[i, j] ? 86 : voxVal;
                        voxVal = voxVal == 84 && IslandMap.l[i, j] && IslandMap.m[i, j] ? 4 : voxVal;

                        IslandMap.ma[i, j] = voxVal;
                        
                        /*
                        for (int countX = -Radius; countX <= Radius; countX++)
                        {
                            for (int countY = -Radius; countY <= Radius; countY++)
                            {
                                //ocean,lake,river
                                if (!(IslandMap.ma[i + countX, j + countY] == 6 || IslandMap.ma[i + countX, j + countY] == 7 || IslandMap.ma[i + countX, j + countY] == 8))
                                {
                                    int dist = countX * countX + countY * countY;
                                    if (dist <= Radius * Radius)
                                    {
                                        int val = (80 - (dist / (wx / 2))) / 20;
                                        if (val > IslandMap.ma[i + countX, j + countY])
                                        {
                                            IslandMap.ma[i + countX, j + countY] = val;
                                        }
                                    }
                                }
                            }
                        }
                        */

                    }
                }
            }

            if (IslandMap.LakePoints.Count > 0)
            {
                for (int i = ApplicationSettings.chunkSize; i < IslandMap.ma.GetLength(0) - ApplicationSettings.chunkSize; i++)
                {
                    for (int j = ApplicationSettings.chunkSize; j < IslandMap.ma.GetLength(1) - ApplicationSettings.chunkSize; j++)
                    {
                        if (IslandMap.ma[i, j] <= 5)
                        {
                            Point LakePoint = IslandMap.LakePoints[0];
                            double distance = MathUtil.MathUtil.distance(IslandMap.LakePoints[0], i, j);

                            for (int k = 1; k < IslandMap.LakePoints.Count; k++)
                            {
                                double dist = MathUtil.MathUtil.distance(IslandMap.LakePoints[k], i, j);
                                if (dist < distance)
                                {
                                    distance = dist;
                                    LakePoint = IslandMap.LakePoints[k];
                                }
                            }
                            //IslandMap.ma[i, j] = (int)(80 - (distance / (wx / 2))) / 20;
                            IslandMap.ma[i, j] = (int)(5 - (distance / (wx / 16)));
                        }
                    }
                }
            }

            for (int i = 0; i < IslandMap.ma.GetLength(0); i++)
            {
                for (int j = 0; j < IslandMap.ma.GetLength(1); j++)
                {
                    IslandMap.ma[i, j] = IslandMap.ma[i, j] < 1 ? 1 : IslandMap.ma[i, j];
                }
            }

        }

        public void SetSurroundingCircular(int X, int Y, int[,] FillMap, int Radius, Boolean[,] Map)
        {
            for (int countX = -Radius; countX <= Radius; countX++)
            {
                for (int countY = -Radius; countY <= Radius; countY++)
                {
                    if (Map[X + countX, Y + countY])
                    {
                        int dist = countX * countX + countY * countY;
                        if (dist <= Radius * Radius)
                        {
                            int val = (100 - dist) / 20;
                            if (val > IslandMap.ma[X + countX, Y + countY])
                            {
                                FillMap[X + countX, Y + countY] = val;
                            }
                        }
                    }
                }
            }
        }




    }
}