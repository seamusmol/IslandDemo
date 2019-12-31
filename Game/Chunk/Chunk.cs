using System;
using System.Collections.Generic;
using static main.ApplicationSettings;
using terrain;
using System.Windows.Media.Media3D;
using System.Windows;
using System.Windows.Media;
using System.Diagnostics;
using System.Windows.Media.Imaging;
using System.IO;
using System.Windows.Controls;
using main;

namespace terrain
{
    public class Chunk
    {
        public Boolean[,,] voxels { get; }
        int[,,] materials;

        public GeometryModel3D Model { get; set; }
        public GeometryModel3D TransparentModel { get; set; }

        public Boolean hasProcessed { get; set; } = false;

        public int chunkIDX { get; set; }
        public int chunkIDZ { get; set; }

        public Chunk(int ChunkIDX, int ChunkIDZ, Boolean[,,] Voxels, int[,,] Materials)
        {
            chunkIDX = ChunkIDX;
            chunkIDZ = ChunkIDZ;
            voxels = Voxels;
            materials = Materials;
        }

        public void processChunk()
        {
            hasProcessed = true;
            //generateVoxelValues();
            //createCellData();
            GeometryModel3D[] Models = VoxelUtil.VoxelUtil.GenerateVoxelGeometries(voxels, materials);

            Model = Models[0];
            TransparentModel = Models[1];

            TranslateTransform3D translation = new TranslateTransform3D(chunkIDX * ApplicationSettings.chunkSize, 0, chunkIDZ * ApplicationSettings.chunkSize);
            Model.Transform = translation;
            TransparentModel.Transform = translation;
        }

        public Boolean[] getVoxelValue(int x, int y, int z)
        {
            return VoxelUtil.VoxelUtil.getVoxelValue(voxels, x, y, z);
        }

        public Boolean GetVertexValue(int x, int y, int z)
        {
            return voxels[x, y, z];
        }

        public void SetVertexValue(int x, int y, int z, Boolean value)
        {
            voxels[x, y, z] = value;
        }

        public void SetVertexMaterial(int x, int y, int z, int material)
        {
            materials[x, y, z] = material;
        }

        public int GetVertexMaterial(int x, int y, int z)
        {
            return materials[x, y, z];
        }
        

    }
}

