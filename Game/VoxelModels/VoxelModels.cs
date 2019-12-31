using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Media3D;
using System.Diagnostics;

namespace VoxelModels
{
    public static class VoxelModels
    {
        public static String[] LibraryName = new String[2] { "HouseModels", "VegetationModels" };

        public static Dictionary<String, int[,,]> ModelVoxelLibrary { get; set; } = new Dictionary<string, int[,,]>();
        public static Dictionary<String, int[,,]> ModelMaterialLibrary { get; set; } = new Dictionary<string, int[,,]>();
        public static Dictionary<String, Point3D> ModelDimensionLibrary { get; set; } = new Dictionary<string, Point3D>();

        public static void LoadModels()
        {
            for (int i = 0; i < LibraryName.Length; i++)
            {
                Type ModelsType = Type.GetType("VoxelModels." + LibraryName[i]);


                PropertyInfo[] Fields = ModelsType.GetProperties(BindingFlags.Public | BindingFlags.Static);

                for (int j = 0; j < Fields.Length; j++)
                {
                    Debug.WriteLine(Fields[j].Name);

                    if (Fields[j].Name.Contains("Voxels"))
                    {
                        ModelVoxelLibrary.Add(Fields[j].Name, (int[,,])Fields[j].GetValue(null, null));
                    }
                    else if (Fields[j].Name.Contains("Materials"))
                    {
                        ModelMaterialLibrary.Add(Fields[j].Name, (int[,,])Fields[j].GetValue(null, null));
                    }
                    //(Boolean[,,])Fields[i].GetValue(null, null);
                    else if (Fields[j].Name.Contains("Dimensions"))
                    {
                        ModelDimensionLibrary.Add(Fields[j].Name, (Point3D)Fields[j].GetValue(null, null));
                    }
                }
            }
        }
    }
}




