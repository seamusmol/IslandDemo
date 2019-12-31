using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;
using main;
using System.Diagnostics;

namespace terrain
{
    public class WaterChunk
    {
        public GeometryModel3D Model { get; set; }

        public Boolean hasProcessed { get; set; } = false;
        public Boolean needsUpdate { get; set; } = false;

        public int chunkIDX { get; set; }
        public int chunkIDZ { get; set; }

        Boolean[,,] waterVoxels;

        public WaterChunk(int ChunkIDX, int ChunkIDZ)
        {
            chunkIDX = ChunkIDX;
            chunkIDZ = ChunkIDZ;
            waterVoxels = new Boolean[ApplicationSettings.chunkSize + 1, ApplicationSettings.worldHeight, ApplicationSettings.chunkSize + 1];
        }

        private void generateWaterChunk(Boolean[,,] Voxels)
        {
            int count = 0;
            for (int i = 0; i < waterVoxels.GetLength(0); i++)
            {
                for (int j = 0; j <= ApplicationSettings.seaLevel + 1; j++)
                {
                    for (int k = 0; k < waterVoxels.GetLength(2); k++)
                    {
                        if (!Voxels[i, j, k])
                        {
                            count++;
                        }
                        waterVoxels[i, j, k] = !Voxels[i, j, k];
                    }
                }
            }
            
        } 

        public GeometryModel3D processChunk(Boolean[,,] Voxels)
        {
            hasProcessed = true;
            //generateVoxelValues();
            //createCellData();
            generateWaterChunk(Voxels);
            Model = VoxelUtil.VoxelUtil.GenerateVoxelGeometry(waterVoxels, new int[waterVoxels.GetLength(0), waterVoxels.GetLength(1), waterVoxels.GetLength(2)]);
            TranslateTransform3D translation = new TranslateTransform3D(chunkIDX * ApplicationSettings.chunkSize, 0, chunkIDZ * ApplicationSettings.chunkSize);
            Model.Transform = translation;
            return Model;
        }

    }


}
