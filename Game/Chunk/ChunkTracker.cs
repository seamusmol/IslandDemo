using System;
using System.Collections.Generic;
using mapGen;
using mob;
using main;
using System.Diagnostics;
using Game;
using System.Windows.Media.Media3D;

namespace terrain
{
    public class ChunkTracker : AppState
    {
        List<Chunk> chunkList = new List<Chunk>();
        List<WaterChunk> waterChunkList = new List<WaterChunk>();

        MapDataGenerator mapGen;
        MainWindow MainWindow;
        RenderManager RenderManager;
        Player Player;
        public IslandMap Map { get; }

        int SX, SY;

        public ChunkTracker(int sx, int sy)
        {
            SX = sx;
            SY = sy;
            mapGen = new MapDataGenerator(SX, SY);
            Map = mapGen.map;
        }

        override
        public void initialize(MainWindow mainWindow)
        {
            MainWindow = mainWindow;
            RenderManager = MainWindow.RenderManager;
            Player = mainWindow.GameState.Player;
        }

        override
        public void update(float tpf, long frametime)
        {
            generateGrid();
            for (int i = 0; i < chunkList.Count; i++)
            {
                if (!chunkList[i].hasProcessed && tpf <= 0.016f)
                {
                    chunkList[i].processChunk();
                    Point3D ChunkPosition = new Point3D(chunkList[i].chunkIDX * ApplicationSettings.chunkSize, 0, chunkList[i].chunkIDZ * ApplicationSettings.chunkSize);

                    RenderManager.addChunk(chunkList[i].Model, "Solid" + chunkList[i].chunkIDX + "-" + chunkList[i].chunkIDZ, ChunkPosition);
                    RenderManager.addChunk(chunkList[i].TransparentModel, "Transparent" + chunkList[i].chunkIDX + "-" + chunkList[i].chunkIDZ, ChunkPosition);

                    RenderManager.addWaterChunk(waterChunkList[i].processChunk(chunkList[i].voxels), "Water" + waterChunkList[i].chunkIDX + "-" + waterChunkList[i].chunkIDZ, ChunkPosition);
                    return;
                }
            }
        }
        
        public void generateGrid()
        {
            int chunkIDX = (int)Player.ModelPosition.X / ApplicationSettings.chunkSize;
            int chunkIDZ = (int)Player.ModelPosition.Z / ApplicationSettings.chunkSize;

            for (int i = -ApplicationSettings.renderDistance - 1; i < ApplicationSettings.renderDistance; i++)
            {
                for (int j = -ApplicationSettings.renderDistance - 1; j < ApplicationSettings.renderDistance; j++)
                {
                    if (!containsChunk(chunkIDX + i, chunkIDZ + j))
                    {
                        if (chunkIDX + i >= 0 && chunkIDX + i < ApplicationSettings.worldSize)
                        {
                            if (chunkIDZ + j >= 0 && chunkIDZ + j < ApplicationSettings.worldSize)
                            {
                                Object[] ChunkData = mapGen.CreateChunkData(chunkIDX + i, chunkIDZ + j);
                                chunkList.Add(new Chunk(chunkIDX + i, chunkIDZ + j, (Boolean[,,])ChunkData[0], (int[,,])ChunkData[1]));
                                waterChunkList.Add(new WaterChunk(chunkIDX + i, chunkIDZ + j));
                            }
                        }
                    }
                }
            }

            for (int i = 0; i < chunkList.Count; i++)
            {
                if (Math.Abs(chunkList[i].chunkIDX - chunkIDX) > ApplicationSettings.renderDistance + 1 || Math.Abs(chunkList[i].chunkIDZ - chunkIDZ) > ApplicationSettings.renderDistance + 1)
                {
                    RenderManager.removeChunk("Solid" + chunkList[i].chunkIDX + "-" + chunkList[i].chunkIDZ);
                    RenderManager.removeChunk("Transparent" + chunkList[i].chunkIDX + "-" + chunkList[i].chunkIDZ);
                    RenderManager.removeWaterChunk("Water" + waterChunkList[i].chunkIDX + "-" + waterChunkList[i].chunkIDZ);


                    chunkList.RemoveAt(i);
                    waterChunkList.RemoveAt(i);
                }
            }
        }

        public Boolean HasVoxel(Point3D Position)
        {
            int chunkIDX = (int)Position.X / ApplicationSettings.chunkSize - 1;
            int chunkIDZ = (int)Position.Z / ApplicationSettings.chunkSize - 1;
            Chunk Chunk = getChunkByCoords(chunkIDX, chunkIDZ);
            if (Chunk == null)
            {
                //Debug.WriteLine("chunk not found");
                return false;
            }
            else if (Position.Y > ApplicationSettings.worldHeight || Position.Y < 0)
            {
                return false;
            }
            return Chunk.GetVertexValue((int)Position.X % ApplicationSettings.chunkSize, (int)Position.Y, (int)Position.Z % ApplicationSettings.chunkSize);
        }

        public void SetVertex(Point3D Position, Boolean Value, int Material)
        {
            int chunkSize = ApplicationSettings.chunkSize;

            int chunkIDX = (int)Position.X / chunkSize - 1;
            int chunkIDZ = (int)Position.Z / chunkSize - 1;
            
            int px = (int)Position.X % chunkSize;
            int py = (int)Position.Y % ApplicationSettings.worldHeight;
            int pz = (int)Position.Z % chunkSize;

            if (px == 0 && pz == 0)
            {
                Chunk Chunk11 = getChunkByCoords(chunkIDX, chunkIDZ);
                Chunk Chunk12 = getChunkByCoords(chunkIDX - 1, chunkIDZ);
                Chunk Chunk21 = getChunkByCoords(chunkIDX, chunkIDZ-1);
                Chunk Chunk22 = getChunkByCoords(chunkIDX-1, chunkIDZ-1);

                if (Chunk11 != null)
                {
                    if (!HasVoxel(Position) && Value)
                    {
                        Chunk11.SetVertexMaterial(px, (int)Position.Y, pz, Material);
                        Chunk11.SetVertexValue(px, (int)Position.Y, pz, true);
                        Chunk11.hasProcessed = false;

                        Chunk12.SetVertexMaterial(chunkSize, (int)Position.Y, pz, Material);
                        Chunk12.SetVertexValue(chunkSize, (int)Position.Y, pz, true);
                        Chunk12.hasProcessed = false;

                        Chunk21.SetVertexMaterial(px, (int)Position.Y, chunkSize, Material);
                        Chunk21.SetVertexValue(px, (int)Position.Y, chunkSize, true);
                        Chunk21.hasProcessed = false;

                        Chunk22.SetVertexMaterial(chunkSize, (int)Position.Y, chunkSize, Material);
                        Chunk22.SetVertexValue(chunkSize, (int)Position.Y, chunkSize, true);
                        Chunk22.hasProcessed = false;
                    }
                    else if (HasVoxel(Position) && !Value)
                    {
                        Chunk11.SetVertexMaterial(px, (int)Position.Y, pz, 0);
                        Chunk11.SetVertexValue(px, (int)Position.Y, pz, false);
                        Chunk11.hasProcessed = false;

                        Chunk12.SetVertexMaterial(chunkSize, (int)Position.Y, pz, 0);
                        Chunk12.SetVertexValue(chunkSize, (int)Position.Y, pz, false);
                        Chunk12.hasProcessed = false;

                        Chunk21.SetVertexMaterial(px, (int)Position.Y, chunkSize, 0);
                        Chunk21.SetVertexValue(px, (int)Position.Y, chunkSize, false);
                        Chunk21.hasProcessed = false;

                        Chunk22.SetVertexMaterial(chunkSize, (int)Position.Y, chunkSize, 0);
                        Chunk22.SetVertexValue(chunkSize, (int)Position.Y, chunkSize, false);
                        Chunk22.hasProcessed = false;
                    }
                }
            }
            else if (px == 0)
            {
                Chunk Chunk = getChunkByCoords(chunkIDX, chunkIDZ);
                Chunk Chunk2 = getChunkByCoords(chunkIDX-1, chunkIDZ);
                if (Chunk != null)
                {
                    if (!HasVoxel(Position) && Value)
                    {
                        Chunk.SetVertexMaterial(px, (int)Position.Y, pz, Material);
                        Chunk.SetVertexValue(px, (int)Position.Y, pz, true);
                        Chunk.hasProcessed = false;

                        Chunk2.SetVertexMaterial(chunkSize, (int)Position.Y, pz, Material);
                        Chunk2.SetVertexValue(chunkSize, (int)Position.Y, pz, true);
                        Chunk2.hasProcessed = false;
                    }
                    else if (HasVoxel(Position) && !Value)
                    {
                        Chunk.SetVertexMaterial(px, (int)Position.Y, pz, 0);
                        Chunk.SetVertexValue(px, (int)Position.Y, pz, false);
                        Chunk.hasProcessed = false;

                        Chunk2.SetVertexMaterial(chunkSize, (int)Position.Y, pz, 0);
                        Chunk2.SetVertexValue(chunkSize, (int)Position.Y, pz, false);
                        Chunk2.hasProcessed = false;

                    }
                }
            }
            else if (pz == 0)
            {
                Chunk Chunk = getChunkByCoords(chunkIDX, chunkIDZ);
                Chunk Chunk2 = getChunkByCoords(chunkIDX, chunkIDZ-1);
                if (Chunk != null)
                {
                    if (!HasVoxel(Position) && Value)
                    {
                        Chunk.SetVertexMaterial(px, (int)Position.Y, pz, Material);
                        Chunk.SetVertexValue(px, (int)Position.Y, pz, true);
                        Chunk.hasProcessed = false;

                        Chunk2.SetVertexMaterial(px, (int)Position.Y, chunkSize, Material);
                        Chunk2.SetVertexValue(px, (int)Position.Y, chunkSize, true);
                        Chunk2.hasProcessed = false;
                    }
                    else if (HasVoxel(Position) && !Value)
                    {
                        Chunk.SetVertexMaterial(px, (int)Position.Y, pz, 0);
                        Chunk.SetVertexValue(px, (int)Position.Y, pz, false);
                        Chunk.hasProcessed = false;

                        Chunk2.SetVertexMaterial(px, (int)Position.Y, chunkSize, 0);
                        Chunk2.SetVertexValue(px, (int)Position.Y, chunkSize, false);
                        Chunk2.hasProcessed = false;
                    }
                }
            }
            else
            {
                Chunk Chunk = getChunkByCoords(chunkIDX, chunkIDZ);
                if (Chunk != null)
                {

                    if (!HasVoxel(Position) && Value)
                    {
                        Chunk.SetVertexMaterial(px, (int)Position.Y, pz, Material);
                        Chunk.SetVertexValue(px, (int)Position.Y, pz, true);
                    }
                    else if (HasVoxel(Position) && !Value)
                    {
                        Chunk.SetVertexMaterial(px, (int)Position.Y, pz, 0);
                        Chunk.SetVertexValue(px, (int)Position.Y, pz, false);
                    }
                    Chunk.hasProcessed = false;
                }
            }
        }


        public Boolean[] getVoxelValue(Point3D Position)
        {
            int chunkIDX = (int)Position.X / ApplicationSettings.chunkSize - 1;
            int chunkIDZ = (int)Position.Z / ApplicationSettings.chunkSize - 1;

            Chunk Chunk = getChunkByCoords(chunkIDX, chunkIDZ);
            if (Chunk == null)
            {
                return new Boolean[8];
            }
            return Chunk.getVoxelValue((int)Position.X % ApplicationSettings.chunkSize, (int)Position.Y, (int)Position.Z % ApplicationSettings.chunkSize);
        }

        public Chunk getChunkByCoords(int IDX, int IDZ)
        {
            for (int i = 0; i < chunkList.Count; i++)
            {
                if (chunkList[i].chunkIDX == IDX && chunkList[i].chunkIDZ == IDZ)
                {
                    return chunkList[i];
                }
            }
            return null;
        }

        public Boolean containsChunk(int IDX, int IDZ)
        {
            for (int i = 0; i < chunkList.Count; i++)
            {
                if (chunkList[i].chunkIDX == IDX && chunkList[i].chunkIDZ == IDZ)
                {
                    return true;
                }
            }
            return false;
        }

        override
        public void close()
        {
        }
    }
}

