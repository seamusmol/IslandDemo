using mapGen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Media3D;
using System.Diagnostics;
using main;
using System.Reflection;

namespace Game
{

    class LakeVillage : Village
    {
        public LakeVillage(Point Position, IslandMap Map, int sx, int sy) : base(Position, Map, sx, sy)
        {
            
        }
        
        public override void GenerateLayout()
        {
            createBridge();
            GenerateLakeHouses();
            GenerateLakeRoad();
        }

        public void GenerateLakeRoad()
        {
            Boolean[,] RoadMap = new Boolean[IslandMap.lk.GetLength(0), IslandMap.lk.GetLength(1)];
            List<Point> PointList = new List<Point>();
            for (int i = 0; i < StructureList.Count; i++)
            {
                Vector dir = new Vector(StructureList[i].Position.X-CenterPosition.X, StructureList[i].Position.Z - CenterPosition.Y);
                dir.Normalize();
                Point MidPoint = MathUtil.MathUtil.RotatePoint(new Point(0, StructureList[i].WX), StructureList[i].Angle); 

                PointList.Add(new Point(StructureList[i].Position.X + dir.X*2 + MidPoint.X, StructureList[i].Position.Z + dir.Y * 2 + MidPoint.Y));
            }
            RoadMap = MapUtil.FillShape(PointList, RoadMap);
            for (int i = 0; i < RoadMap.GetLength(0); i++)
            {
                for (int j = 0; j < RoadMap.GetLength(1); j++)
                {
                    if (RoadMap[i,j])
                    {
                        if (!IslandMap.lk[i,j] && !IslandMap.r[i, j])
                        {
                            IslandMap.ma[i,j] = 129;
                            IslandMap.rd[i, j] = true;
                        }
                        else if (IslandMap.lk[i, j] && MapUtil.HasSurrounding(i,j,IslandMap.l,1))
                        {
                            IslandMap.ma[i, j] = 129;
                            IslandMap.rd[i, j] = true;
                        }
                    }
                }
            }

        }

        //find house spots, generate houses
        private void GenerateLakeHouses()
        {
            int HouseMax = 10;
            int[] HouseIDS = MathUtil.MathUtil.generateFibonocciNumbers(SX, SY, HouseMax, (int)Math.Log10(HouseMax));

            for (int i = 1; i <= 10; i++)
            {
                 generateLakeHouse(i);
            }
        }

        private void generateLakeHouse(int HouseID)
        {
            int Radius = 32;
            
            int PX = (int)CenterPosition.X;
            int PY = (int)CenterPosition.Y;
            
            for (int countX = -Radius; countX <= Radius; countX+=2)
            {
                for (int countY = -Radius; countY <= Radius; countY+=2)
                {
                    if (MapUtil.HasSurrounding(PX + countX, PY + countY, IslandMap.lk, 8) && !IslandMap.lk[PX + countX, PY + countY] && !MapUtil.HasSurrounding(PX + countX, PY + countY, IslandMap.m, 1))
                    {
                        Point LakePoint = MapUtil.getClosest(PX + countX, PY + countY, IslandMap.lk, Radius);
                        int angle = MapUtil.GetDirectionAngle(PX + countX, PY + countY, IslandMap.lk) + 90;

                        if (angle >= 0)
                        {
                            Point3D Dimensions = VoxelModels.VoxelModels.ModelDimensionLibrary["House_" + HouseID + "_Dimensions"];

                            //Debug.WriteLine(Dimensions);

                            Structure Structure = new Structure(new Point3D(PX + countX, IslandMap.h[PX + countX, PY + countY], PY + countY), angle, (int)Dimensions.X, (int)Dimensions.Y, (int)Dimensions.Z);
                            Boolean HasNearbyBuilding = MapUtil.HasStructureSurroundingStructure(Structure, this, 3);

                            if (!HasNearbyBuilding)
                            {
                                Boolean HasLake = MapUtil.HasStructureSurrounding(Structure, IslandMap.lk, 6);

                                if (!HasLake)
                                {
                                    //add system for random houses
                                    IslandMap.StructureNames.Add("House_" + HouseID + "_");

                                    //IslandMap.StructurePoints.Add(new Point3D(PX + countX, IslandMap.h[PX + countX, PY + countY], PX + countX));
                                    IslandMap.StructurePoints.Add(new Point3D(PX + countX, IslandMap.h[PX + countX, PY + countY], PY + countY));
                                    //IslandMap.StructurePoints.Add(new Point3D(512, 2, 512));
                                    //IslandMap.StructureRotations.Add(new Vector3D(dir.X, 1, dir.Y));
                                    IslandMap.StructureRotations.Add(angle);

                                    StructureList.Add(Structure);

                                    //Debug.WriteLine("House added");

                                    return;
                                }
                            }
                        }
                        
                    }
                }
            }
        }

        public void createBridge()
        {
            Point DeltaPosition = IslandMap.GetDeltaPoint(CenterPosition);

            Vector dir = new Vector(DeltaPosition.X - CenterPosition.X, DeltaPosition.Y - CenterPosition.Y);
            dir.Normalize();

            int distance = (int)MathUtil.MathUtil.distance(CenterPosition, DeltaPosition);
            dir.Normalize();

            int allowedDistance = (int)(distance * 0.25f);

            for (int i = 1; i < distance; i++)
            {
                float wavePosX = (float)(dir.X * i + dir.X * Math.Sin(Math.PI * i / distance) * 10);
                float wavePosY = (float)(dir.Y * i + dir.Y * Math.Sin(Math.PI * i / distance) / 10);

                int px = (int)(CenterPosition.X + wavePosX);
                int py = (int)(CenterPosition.Y + wavePosY);

                if (!IslandMap.lk[px, py] && IslandMap.r[px, py])
                {
                    if (i > allowedDistance)
                    {
                        Point BridgePosition = new Point(px - 5, py - 5);

                        int angle = MapUtil.GetDirectionAngle(px,py, IslandMap.lk);

                        //Debug.WriteLine(BridgePositionB);
                        
                        IslandMap.StructureNames.Add("Bridge_");
                        IslandMap.StructurePoints.Add(new Point3D(BridgePosition.X, IslandMap.h[px, py], BridgePosition.Y));

                        //IslandMap.StructureRotations.Add(new Vector3D(CrossDir.X, 1, CrossDir.Y));
                        //IslandMap.StructureRotations.Add(new Vector3D(dir.X, 1, dir.Y));
                        IslandMap.StructureRotations.Add(angle + 270);
                        return;
                    }
                }
            }

        }

    }


}
