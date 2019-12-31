using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Media3D;
using System.Diagnostics;
using System.Reflection;

namespace VoxelUtil
{
    public static class VoxelUtil
    {
        
        public class Cell
        {
            public int value { get; set; }
            public int px { get; set; }
            public int py { get; set; }
            public int pz { get; set; }
            public int[] m { get; set; }

            public Cell(int Value, int PX, int PY, int PZ, int[] M)
            {
                value = Value;
                px = PX;
                py = PY;
                pz = PZ;
                m = M;
            }
        }

        public static Point3D GetDimensions(PropertyInfo[] Fields, String Name)
        {
            for (int i = 0; i < Fields.Length; i++)
            {
                if (Fields[i].Name == Name)
                {
                    return (Point3D)Fields[i].GetValue(null, null);
                }
            }
            return new Point3D();
        }

        public static GeometryModel3D[] GenerateVoxelGeometries(Boolean[,,] voxels, int[,,] materials)
        {
            Object[] SortedFields = SplitFields(voxels,materials);

            List<Cell> CellList = GenerateVoxelValues((Boolean[,,]) SortedFields[0], (int[,,]) SortedFields[2]);
            List<Cell> TransLucentCellList = GenerateVoxelValues((Boolean[,,]) SortedFields[1], (int[,,]) SortedFields[3]);
            
            Object[] Data = createCellData(CellList);
            Object[] TransLucentData = createCellData(TransLucentCellList);

            GeometryModel3D[] models = new GeometryModel3D[2];
            models[0] = GenerateGeometry((List<Point3D>)Data[0], (List<Point>)Data[1]);
            models[1] = GenerateGeometry((List<Point3D>)TransLucentData[0], (List<Point>)TransLucentData[1]);

            return models;
        }

        public static GeometryModel3D GenerateVoxelGeometry(Boolean[,,] voxels, int[,,] materials)
        {
            List<Cell> CellList = GenerateVoxelValues(voxels, materials);
            Object[] Data = createCellData(CellList);
            return GenerateGeometry((List<Point3D>)Data[0], (List<Point>)Data[1]);
        }

        private static Object[] SplitFields(Boolean[,,] voxels, int[,,] materials)
        {
            Boolean[,,] SolidVoxels = new Boolean[voxels.GetLength(0), voxels.GetLength(1), voxels.GetLength(2)];
            Boolean[,,] TranslucentVoxels = new Boolean[voxels.GetLength(0), voxels.GetLength(1), voxels.GetLength(2)];

            int[,,] SolidMaterials = new int[materials.GetLength(0), materials.GetLength(1), materials.GetLength(2)];
            int[,,] TranslucentMaterials = new int[materials.GetLength(0), materials.GetLength(1), materials.GetLength(2)];

            for (int i = 0; i < voxels.GetLength(0); i++)
            {
                for (int j = 0; j < voxels.GetLength(1); j++)
                {
                    for (int k = 0; k < voxels.GetLength(2); k++)
                    {
                        if (voxels[i, j, k] && materials[i, j, k] < 224)
                        {
                            SolidVoxels[i, j, k] = true;
                            SolidMaterials[i, j, k] = materials[i, j, k];
                        }
                        else if(voxels[i, j, k] && materials[i, j, k] >= 224)
                        {
                            TranslucentVoxels[i, j, k] = true;
                            TranslucentMaterials[i, j, k] = materials[i, j, k];
                        }
                    }
                }
            }
            Object[] Data = new Object[4];
            Data[0] = SolidVoxels;
            Data[1] = TranslucentVoxels;
            Data[2] = SolidMaterials;
            Data[3] = TranslucentMaterials;
            return Data;
        }
        
        private static List<Cell> GenerateVoxelValues(Boolean[,,] voxels, int[,,] materials)
        {
            List<Cell> cellList = new List<Cell>();

            for (int i = 0; i < voxels.GetLength(0) - 1; i++)
            {
                for (int j = 0; j < voxels.GetLength(1) - 1; j++)
                {
                    for (int k = 0; k < voxels.GetLength(2) - 1; k++)
                    {
                        int binaryValue = 0;
                        binaryValue += voxels[i, j + 1, k + 1] ? 128 : 0;
                        binaryValue += voxels[i + 1, j + 1, k + 1] ? 64 : 0;
                        binaryValue += voxels[i + 1, j + 1, k] ? 32 : 0;
                        binaryValue += voxels[i, j + 1, k] ? 16 : 0;
                        binaryValue += voxels[i, j, k + 1] ? 8 : 0;
                        binaryValue += voxels[i + 1, j, k + 1] ? 4 : 0;
                        binaryValue += voxels[i + 1, j, k] ? 2 : 0;
                        binaryValue += voxels[i, j, k] ? 1 : 0;

                        if (binaryValue != 255 && binaryValue != 0)
                        {
                            cellList.Add(new Cell(binaryValue, i, j, k,
                               new int[]{
                               materials[i,j + 1,k + 1],
                               materials[i + 1,j + 1,k + 1],
                               materials[i + 1,j + 1,k],
                               materials[i,j + 1,k],
                               materials[i,j,k + 1],
                               materials[i + 1,j,k + 1],
                               materials[i + 1,j,k],
                               materials[i,j,k]
                               }));
                        }
                    }
                }
            }
            return cellList;
        }
        
        private static Object[] createCellData(List<Cell> cellList)
        {
            List<Point3D> PointList = new List<Point3D>();
            List<Point> texCoordList = new List<Point>();
            int[][] indices = terrain.VoxelList.getList();
            
            float texsize = 0.0625f;
            float buffer = texsize * 0.015625f;

            float sizeX = 1f;
            float sizeY = 1f;
            float sizeZ = 1f;

            Point3D[] voxelVertices = new Point3D[12];
            voxelVertices[0] = new Point3D(sizeX / 2f, 0, 0);
            voxelVertices[1] = new Point3D(sizeX, 0, sizeZ / 2f);
            voxelVertices[2] = new Point3D(sizeX / 2f, 0, sizeZ);
            voxelVertices[3] = new Point3D(0, 0, sizeZ / 2f);

            voxelVertices[4] = new Point3D(sizeX / 2f, sizeY, 0);
            voxelVertices[5] = new Point3D(sizeX, sizeY, sizeZ / 2f);
            voxelVertices[6] = new Point3D(sizeX / 2f, sizeY, sizeZ);
            voxelVertices[7] = new Point3D(0, sizeY, sizeZ / 2);

            voxelVertices[8] = new Point3D(0, sizeY / 2f, 0);
            voxelVertices[9] = new Point3D(sizeX, sizeY / 2f, 0);
            voxelVertices[10] = new Point3D(sizeX, sizeY / 2f, sizeZ);
            voxelVertices[11] = new Point3D(0, sizeY / 2f, sizeZ);

            Point[] texCoords = new Point[12];
            texCoords[0] = new Point(texsize / 2, texsize / 2);
            texCoords[1] = new Point(texsize, 0);
            texCoords[2] = new Point(texsize / 2, texsize / 2);
            texCoords[3] = new Point(0, texsize / 2);

            texCoords[4] = new Point(texsize / 2, texsize / 2);
            texCoords[5] = new Point(texsize, 0);
            texCoords[6] = new Point(texsize / 2, texsize / 2);
            texCoords[7] = new Point(0, texsize / 2);

            texCoords[8] = new Point(0 + buffer, 0 + buffer);
            texCoords[9] = new Point(texsize - buffer, 0 + buffer);
            texCoords[10] = new Point(texsize - buffer, texsize - buffer);
            texCoords[11] = new Point(0 + buffer, texsize - buffer);
            
            for (int i = 0; i < cellList.Count; i++)
            {
                int value = cellList[i].value;
                int px = main.ApplicationSettings.chunkSize + cellList[i].px;
                int py = cellList[i].py;
                int pz = main.ApplicationSettings.chunkSize + cellList[i].pz;
                int[] materials = cellList[i].m;
                
                int most = 999;
                int count = 0;
                for (int countX = 0; countX < cellList[i].m.Length; countX++)
                {
                    if (cellList[i].m[countX] == most)
                    {
                        continue;
                    }
                    int curCount = 0;
                    for (int countY = 0; countY < cellList[i].m.Length; countY++)
                    {
                        if (cellList[i].m[countX] == cellList[i].m[countY])
                        {
                            curCount++;
                        }
                    }
                    if (curCount >= count && cellList[i].m[countX] < most && cellList[i].m[countX] != 0)
                    {
                        most = cellList[i].m[countX];
                        count = curCount;
                    }
                }
                
                float matX = 0.0625f * ((most-1) / 16);
                float matY = 0.0625f * ((most-1) % 16);

                //polygon-3
                for (int indexCount = 0; indexCount < indices[value].GetLength(0); indexCount+=3)
                {
                    Point3D p1 = new Point3D(voxelVertices[indices[value][indexCount]].X, voxelVertices[indices[value][indexCount]].Y, voxelVertices[indices[value][indexCount]].Z);
                    Point3D p2 = new Point3D(voxelVertices[indices[value][indexCount + 1]].X, voxelVertices[indices[value][indexCount + 1]].Y, voxelVertices[indices[value][indexCount + 1]].Z);
                    Point3D p3 = new Point3D(voxelVertices[indices[value][indexCount + 2]].X, voxelVertices[indices[value][indexCount + 2]].Y, voxelVertices[indices[value][indexCount + 2]].Z);

                    Vector3D pv1 = Vector3D.CrossProduct(p2 - p1, p3 - p1);
                    Vector3D pv2 = Vector3D.CrossProduct(p3 - p2, p1 - p2);
                    Vector3D pv3 = Vector3D.CrossProduct(p1 - p3, p2 - p3);
                    
                    Point v1 = NormUV(pv1, p1);
                    Point v2 = NormUV(pv2, p2);
                    Point v3 = NormUV(pv3, p3);
                    
                    double bfx1 = Math.Round(v1.X) == 0 ? buffer : -buffer;
                    double bfx2 = Math.Round(v2.X) == 0 ? buffer : -buffer;
                    double bfx3 = Math.Round(v3.X) == 0 ? buffer : -buffer;
                    double bfy1 = Math.Round(v1.Y) == 0 ? buffer : -buffer;
                    double bfy2 = Math.Round(v2.Y) == 0 ? buffer : -buffer;
                    double bfy3 = Math.Round(v3.Y) == 0 ? buffer : -buffer;

                    v1.X *= texsize;
                    v2.X *= texsize;
                    v3.X *= texsize;
                    v1.Y *= texsize;
                    v2.Y *= texsize;
                    v3.Y *= texsize;
                    
                    v1.X += matX + bfx1;
                    v2.X += matX + bfx2;
                    v3.X += matX + bfx3;
                    v1.Y += matY + bfy1;
                    v2.Y += matY + bfy2;
                    v3.Y += matY + bfy3;

                    texCoordList.Add(v1);
                    texCoordList.Add(v2);
                    texCoordList.Add(v3);
                    
                    p1.X += px;
                    p2.X += px;
                    p3.X += px;
                    p1.Y += py;
                    p2.Y += py;
                    p3.Y += py;
                    p1.Z += pz;
                    p2.Z += pz;
                    p3.Z += pz;

                    PointList.Add(p1);
                    PointList.Add(p2);
                    PointList.Add(p3);
                    //texCoordList.Add(texCoords[8]);
                    //texCoordList.Add(texCoords[9]);
                    //texCoordList.Add(texCoords[10]);

                    //texCoordList.Add(new Point(texCoords[indices[value][indexCount]].X + matX, texCoords[indices[value][indexCount]].Y + matY));
                }
            }
            Object[] data = new Object[2];
            data[0] = PointList;
            data[1] = texCoordList;
            return data;
        }

        public static Point NormUV(Vector3D Vec, Point3D P)
        {
            Boolean PosX = Vec.X > 0;
            Boolean PosY = Vec.Y > 0;
            Boolean PosZ = Vec.Z > 0;

            double absX = Math.Abs(Vec.X);
            double absY = Math.Abs(Vec.Y);
            double absZ = Math.Abs(Vec.Z);

            if (PosX && absX >= absY && absX >= absZ)
            {
                return new Point(P.Z, P.Y);
            }
            if (!PosX && absX >= absY && absX >= absZ)
            {
                return new Point(P.Z, P.Y);
            }

            if (PosY && absY >= absX && absY >= absZ)
            {
                return new Point(P.X, P.Z);
            }
            if (!PosY && absY >= absX && absY >= absZ)
            {
                return new Point(P.X, P.Z);
            }

            if (PosZ && absZ >= absX && absZ >= absY)
            {
                return new Point(P.X, P.Y);
            }
            if (!PosZ && absZ >= absX && absZ >= absY)
            {
                return new Point(P.X, P.Y);
            }
            return new Point();
        }

        
        //wikipedia article on cube mapping
        private static void NormToUV(Vector3D Vec, int Index, Vector UV)
        {
            double absX = Math.Abs(Vec.X);
            double absY = Math.Abs(Vec.Z);
            double absZ = Math.Abs(Vec.Y);

            Boolean isPositiveX = Vec.X > 0;
            Boolean isPositiveY = Vec.Z > 0;
            Boolean isPositiveZ = Vec.Y > 0;

            double maxAxis = 0;
            double UC = 0;
            double VC = 0;

            if (isPositiveX && absX >= absY && absX >= absZ)
            {
                // u (0 to 1) goes from +z to -z
                // v (0 to 1) goes from -y to +y
                maxAxis = absX;
                UC = -Vec.Z;
                VC = Vec.Y;
                Index = 0;
            }
            // NEGATIVE X
            if (!isPositiveX && absX >= absY && absX >= absZ)
            {
                // u (0 to 1) goes from -z to +z
                // v (0 to 1) goes from -y to +y
                maxAxis = absX;
                UC = Vec.Z;
                VC = Vec.Y;
                Index = 1;
            }
            // POSITIVE Y
            if (isPositiveY && absY >= absX && absY >= absZ)
            {
                // u (0 to 1) goes from -x to +x
                // v (0 to 1) goes from +z to -z
                maxAxis = absY;
                UC = Vec.X;
                VC = -Vec.Z;
                Index = 2;
            }
            // NEGATIVE Y
            if (!isPositiveY && absY >= absX && absY >= absZ)
            {
                // u (0 to 1) goes from -x to +x
                // v (0 to 1) goes from -z to +z
                maxAxis = absY;
                UC = Vec.X;
                VC = Vec.Z;
                Index = 3;
            }
            // POSITIVE Z
            if (isPositiveZ && absZ >= absX && absZ >= absY)
            {
                // u (0 to 1) goes from -x to +x
                // v (0 to 1) goes from -y to +y
                maxAxis = absZ;
                UC = Vec.X;
                VC = Vec.Y;
                Index = 4;
            }
            // NEGATIVE Z
            if (!isPositiveZ && absZ >= absX && absZ >= absY)
            {
                // u (0 to 1) goes from +x to -x
                // v (0 to 1) goes from -y to +y
                maxAxis = absZ;
                UC = -Vec.X;
                VC = Vec.Y;
                Index = 5;
            }
            
            UV = new Vector(0.5f * (UC/maxAxis + 1.0f), 0.5f * (UC / maxAxis + 1.0f));
        }
        

        private static GeometryModel3D GenerateGeometry(List<Point3D> PointList, List<Point> texCoordList)
        {
            GeometryModel3D Model = new GeometryModel3D();
            MeshGeometry3D Geometry = new MeshGeometry3D();
            Model = new GeometryModel3D();
            for (int i = 0; i < PointList.Count; i++)
            {
                Point3D Point = PointList[i];
                Geometry.Positions.Add(Point);
                Geometry.TextureCoordinates.Add(texCoordList[i]);
                Geometry.TriangleIndices.Add(i);
            }

            Model.Geometry = Geometry;
            return Model;
        }


        public static Boolean[] getVoxelValue(Boolean[,,] Voxels, int x, int y,int z)
        {
            Boolean[] result = new Boolean[8];
            if (x >= Voxels.GetLength(0) - 1 || y >= Voxels.GetLength(1) - 1 || z >= Voxels.GetLength(2) - 1)
            {
                return result;
            }
            else if (y < 0)
            {
                return new Boolean[8] { true, true, true, true, true, true, true, true, };
            }
            else
            {
                //Debug.WriteLine(x + "-" + y + "-" + z);
                result[7] = Voxels[x, y + 1, z + 1];
                result[6] = Voxels[x + 1, y + 1, z + 1];
                result[5] = Voxels[x + 1, y + 1, z];
                result[4] = Voxels[x, y + 1, z];
                result[3] = Voxels[x, y, z + 1];
                result[2] = Voxels[x + 1, y, z + 1];
                result[1] = Voxels[x + 1, y, z];
                result[0] = Voxels[x, y, z];
            }
            return result;
        }
    
    }
}
