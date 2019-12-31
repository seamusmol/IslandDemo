using System;
using mapGen;
using static main.ApplicationSettings;
using main;
using System.Diagnostics;
using System.Windows.Media.Media3D;
using System.Reflection;
using System.Windows;
using VoxelUtil;

/*
 * TODO
 * Add Moisture Map
 */

namespace mapGen
{
    public class MapDataGenerator
    {
        IslandMapGenerator polyGen;
        public IslandMap map { get; }

        public MapDataGenerator(int SX, int SY)
        {
            polyGen = new IslandMapGenerator(SX, SY, ApplicationSettings.worldSize * ApplicationSettings.chunkSize + 1, ApplicationSettings.worldSize * ApplicationSettings.chunkSize + 1);
            map = polyGen.IslandMap;
        }

        public Object[] CreateChunkData(int ChunkIDX, int ChunkIDZ)
        {
            Boolean[,,] voxels = new Boolean[main.ApplicationSettings.chunkSize + 1, worldHeight, main.ApplicationSettings.chunkSize + 1];
            int[,,] materials = new int[main.ApplicationSettings.chunkSize + 1, worldHeight, main.ApplicationSettings.chunkSize + 1];

            if (ChunkIDX == 0 || ChunkIDX == ApplicationSettings.worldSize || ChunkIDZ == 0 || ChunkIDZ == ApplicationSettings.worldSize)
            {
                //ocean chunk
            }
            else
            {
                Boolean[,] LandMap = CreateMap(ChunkIDX, ChunkIDZ, map.l);
                Boolean[,] MountainMap = CreateMap(ChunkIDX, ChunkIDZ, map.m);
                Boolean[,] LakeMap = CreateMap(ChunkIDX, ChunkIDZ, map.lk);
                Boolean[,] RoadMap = CreateMap(ChunkIDX, ChunkIDZ, map.rd);
                int[,] HeightMap = CreateMap(ChunkIDX, ChunkIDZ, map.h);
                int[,] MaterialMap = CreateMap(ChunkIDX, ChunkIDZ, map.ma);

                for (int i = 0; i < main.ApplicationSettings.chunkSize + 1; i++)
                {
                    for (int j = 0; j < main.ApplicationSettings.chunkSize + 1; j++)
                    {
                        voxels[i, 0, j] = true;
                        materials[i, 0, j] = MaterialMap[i, j];
                        if (LandMap[i, j])
                        {
                            if (MountainMap[i, j])
                            {
                                for (int k = 0; k < HeightMap[i, j]; k++)
                                {
                                    voxels[i, k, j] = true;
                                    materials[i, k, j] = 83;
                                }
                                voxels[i, HeightMap[i, j], j] = true;
                                materials[i, HeightMap[i, j], j] = MaterialMap[i, j];
                            }
                            else if (RoadMap[i, j])
                            {
                                for (int k = 0; k < HeightMap[i, j]; k++)
                                {
                                    voxels[i, k, j] = true;
                                    materials[i, k, j] = 129;
                                }
                                voxels[i, HeightMap[i, j], j] = true;
                                materials[i, HeightMap[i, j], j] = 129;
                            }
                            else
                            {
                                for (int k = 0; k < HeightMap[i, j]; k++)
                                {
                                    voxels[i, k, j] = true;
                                    materials[i, k, j] = 113;
                                }
                                voxels[i, HeightMap[i, j], j] = true;
                                materials[i, HeightMap[i, j], j] = MaterialMap[i, j];
                            }
                        }
                        else if (LakeMap[i, j] && RoadMap[i, j])
                        {
                            for (int k = 0; k < HeightMap[i, j]; k++)
                            {
                                voxels[i, k, j] = true;
                                materials[i, k, j] = 129;
                            }
                            voxels[i, HeightMap[i, j], j] = true;
                            materials[i, HeightMap[i, j], j] = 129;
                        }
                        else
                        {
                            for (int k = 0; k < HeightMap[i, j]; k++)
                            {
                                voxels[i, k, j] = true;
                                materials[i, k, j] = 113;
                            }
                            voxels[i, HeightMap[i, j], j] = true;
                            materials[i, HeightMap[i, j], j] = MaterialMap[i, j];
                        }
                    }
                }

                //replace with more efficient system later on
                for (int i = 0; i < map.StructureNames.Count; i++)
                {
                    String StructureName = map.StructureNames[i];
                    Point3D StructurePosition = map.StructurePoints[i];
                    double StructureRotation = map.StructureRotations[i];

                    DrawVoxelModel(StructureName, ChunkIDX, ChunkIDZ, StructureRotation, StructurePosition, voxels, materials);
                }

                for (int i = 0; i < map.VegetationNames.Count; i++)
                {
                    String StructureName = map.VegetationNames[i];
                    Point3D StructurePosition = map.VegetationPoints[i];
                    double StructureRotation = map.VegetationRotations[i];

                    DrawVoxelModel(StructureName, ChunkIDX, ChunkIDZ, StructureRotation, StructurePosition, voxels, materials);
                }

                //DrawVoxelModel("Voxels", new Point3D(voxels.GetLength(0) / 2, HeightMap[voxels.GetLength(0) / 2, voxels.GetLength(0) / 2], voxels.GetLength(2) / 2), voxels, materials);
            }

            //GenerateVegetationVoxels();

            Object[] Data = new Object[2];
            Data[0] = voxels;
            Data[1] = materials;
            return Data;
        }

        public void DrawVoxelModel(String ModelName, int ChunkIDX, int ChunkIDZ, double Angle, Point3D Position, Boolean[,,] Voxels, int[,,] Materials)
        {
           
            int[,,] ModelVoxels = VoxelModels.VoxelModels.ModelVoxelLibrary[ModelName + "Voxels"];
            int[,,] VoxelMaterials = VoxelModels.VoxelModels.ModelMaterialLibrary[ModelName + "Materials"];

            Point3D Dimensions = VoxelModels.VoxelModels.ModelDimensionLibrary[ModelName + "Dimensions"];

            if (ModelVoxels.GetLength(0) != VoxelMaterials.GetLength(0) || ModelVoxels.GetLength(1) != VoxelMaterials.GetLength(1) || ModelVoxels.GetLength(2) != VoxelMaterials.GetLength(2))
            {
                return;
            }

            for (int i = 0; i < ModelVoxels.GetLength(0); i++)
            {
                for (int j = 0; j < ModelVoxels.GetLength(1); j++)
                {
                    for (int k = 0; k < ModelVoxels.GetLength(2); k++)
                    {
                        Point rotPoint = MathUtil.MathUtil.RotatePoint(new Point(i, k), Angle);

                        int PX = (int)Math.Round(Position.X + rotPoint.X - ChunkIDX * ApplicationSettings.chunkSize);
                        int PY = (int)Math.Round(Position.Y + j);
                        int PZ = (int)Math.Round(Position.Z + rotPoint.Y - ChunkIDZ * ApplicationSettings.chunkSize);

                        //int PX = (int)Math.Round(Position.X + (i * Rotation.X) - (ChunkIDX * ApplicationSettings.chunkSize));

                        //int PZ = (int)Math.Round(Position.Z + (k * -Rotation.X) - (ChunkIDZ * ApplicationSettings.chunkSize));

                        if (PX >= 0 && PX < Voxels.GetLength(0))
                        {
                            if (PY >= 0 && PY < Voxels.GetLength(1))
                            {
                                if (PZ >= 0 && PZ < Voxels.GetLength(2))
                                {
                                    if (ModelVoxels[i, j, k] == 1)
                                    {
                                        Voxels[(int)PX, (int)PY, (int)PZ] = true;
                                    }

                                    if (VoxelMaterials[i, j, k] != 0)
                                    {
                                        Materials[(int)PX, (int)PY, (int)PZ] = VoxelMaterials[i, j, k];
                                    }

                                }
                            }
                        }
                    }
                }
            }
        }
        
        public Boolean[,] CreateMap(int IDX, int IDZ, Boolean[,] ValueMap)
        {
            Boolean[,] Map = new Boolean[main.ApplicationSettings.chunkSize + 1, main.ApplicationSettings.chunkSize + 1];
            for (int i = 0; i < main.ApplicationSettings.chunkSize + 1; i++)
            {
                for (int j = 0; j < main.ApplicationSettings.chunkSize + 1; j++)
                {
                    Map[i, j] = ValueMap[IDX * ApplicationSettings.chunkSize + i, IDZ * ApplicationSettings.chunkSize + j];
                }
            }
            return Map;
        }

        public int[,] CreateMap(int IDX, int IDZ, int[,] ValueMap)
        {
            int[,] Map = new int[main.ApplicationSettings.chunkSize + 1, main.ApplicationSettings.chunkSize + 1];
            for (int i = 0; i < main.ApplicationSettings.chunkSize + 1; i++)
            {
                for (int j = 0; j < main.ApplicationSettings.chunkSize + 1; j++)
                {
                    Map[i, j] = ValueMap[IDX * ApplicationSettings.chunkSize + i, IDZ * ApplicationSettings.chunkSize + j];
                }
            }
            return Map;
        }
        
    }

}
