using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Media3D;

namespace Game
{
    /*
        * TODO
        * Fix Face index and only use first value of each x/x/x for index
        * Fix face indices
        * load vertices,texcoords into list
        * 
        * Face values x/y/z x/y/z x/y/z
        * x for vertex
        * y for texcoord
        * z value
        */

    public static class ModelManager
    {

        public static SortedDictionary<String, List<Point3D>> VertexBufferMap { get; set; } = new SortedDictionary<string, List<Point3D>>();
        public static SortedDictionary<String, List<Point>>TexCoordBufferMap { get; set; } = new SortedDictionary<string, List<Point>>();
        public static SortedDictionary<String, List<Vector3D>> NormalBufferMap { get; set; } = new SortedDictionary<string, List<Vector3D>>();
        
        //TODO
        //manual folder check
        public static void LoadModels()
        {
            /*
            object[] Data = LoadModel("");
            VertexBufferMap.Add("Duck", (List<Point3D>)Data[0]);
            TexCoordBufferMap.Add("Duck", (List<Point>)Data[1]);
            NormalBufferMap.Add("Duck", (List<Vector3D>)Data[2]);
            */        

            try
            {
                String[] files = Directory.GetFiles("gfx");
                for (int i = 0; i < files.Length; i++)
                {
                    Debug.WriteLine(files[i]);
                    Debug.WriteLine(files[i].Substring(4, files[i].Length - 8));

                    if (files[i].EndsWith(".obj"))
                    {
                        object[] Data = LoadModel(files[i]);
                        VertexBufferMap.Add(files[i].Substring(4, files[i].Length - 8), (List<Point3D>)Data[0]);
                        TexCoordBufferMap.Add(files[i].Substring(4, files[i].Length - 8), (List<Point>)Data[1]);
                        NormalBufferMap.Add(files[i].Substring(4, files[i].Length - 8), (List<Vector3D>)Data[2]);
                    }
                }


            }
            catch (Exception e)
            {

            }

        }

        private static Object[] LoadModel(String ModelName)
        {
            List<Point3D> VertexBuffer = new List<Point3D>();
            List<Point> TexCoordBuffer = new List<Point>();
            List<Vector3D> NormalBuffer = new List<Vector3D>();
            try
            {
                StreamReader file = new StreamReader(ModelName);
                String NextLine = null;

                List<Point3D> VertexLookupBuffer = new List<Point3D>();
                List<Point> TexCoordLookupBuffer = new List<Point>();
                List<Vector3D> NormalLookupBuffer = new List<Vector3D>();

                while ((NextLine = file.ReadLine()) != null)
                {
                    if (NextLine.StartsWith("v "))
                    {
                        String[] bits = NextLine.Replace("v ", "").Split();
                        VertexLookupBuffer.Add(new Point3D(Double.Parse(bits[0]), Double.Parse(bits[1]), Double.Parse(bits[2])));

                    }
                    else if (NextLine.StartsWith("vt "))
                    {
                        String[] bits = NextLine.Replace("vt ", "").Split();
                        TexCoordLookupBuffer.Add(new Point(Double.Parse(bits[0]), 1.0d - Double.Parse(bits[1])));
                    }
                    else if (NextLine.StartsWith("vn "))
                    {
                        String[] bits = NextLine.Replace("vn ", "").Split();
                        NormalLookupBuffer.Add(new Vector3D(Double.Parse(bits[0]), Double.Parse(bits[1]), Double.Parse(bits[2])));
                    }
                    else if (NextLine.StartsWith("f "))
                    {
                        String[] bits = NextLine.Replace("f ", "").Replace(" ", "/").Split('/');

                        for (int i = 0; i < bits.Length; i+=3)
                        {
                            VertexBuffer.Add(VertexLookupBuffer.ElementAt(int.Parse(bits[i])-1));
                            TexCoordBuffer.Add(TexCoordLookupBuffer.ElementAt(int.Parse(bits[i+1])-1));
                            NormalBuffer.Add(NormalLookupBuffer.ElementAt(int.Parse(bits[i+2])-1));
                        }
                    }
                }
            }
            catch (FileNotFoundException e)
            {
                Debug.WriteLine("Model " + ModelName + " Not found");
            }
            Object[] ModelData = new Object[4];
            ModelData[0] = VertexBuffer;
            ModelData[1] = TexCoordBuffer;
            ModelData[2] = NormalBuffer;
            return ModelData;
        }
    }
}
