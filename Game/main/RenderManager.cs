using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Media.Media3D;
using System.Threading;
using Game;
using System.Collections.Concurrent;
using System.Diagnostics;
using mob;

namespace main
{
    
    public  class RenderManager
    {
        MainWindow MainWindow;

        public ImageBrush OutLineBrush { get; set; }
        public DiffuseMaterial OutLineMaterial { get; set; }

        ImageBrush colors_brush;
        DiffuseMaterial material;
        
        SolidColorBrush waterBrush;
        DiffuseMaterial waterMaterial;

        ImageBrush ModelBrush;
        DiffuseMaterial ModelMaterial;

        SortedDictionary<String, GeometryModel3D> chunks = new SortedDictionary<string, GeometryModel3D>();
        SortedDictionary<String, GeometryModel3D> waterChunks = new SortedDictionary<string, GeometryModel3D>();
        SortedDictionary<String, GeometryModel3D> models = new SortedDictionary<string, GeometryModel3D>();

        Player Player;

        public RenderManager(MainWindow mainWindow)
        {
            MainWindow = mainWindow;
            initBrush();
        }
        
        private void initBrush()
        {
            colors_brush = new ImageBrush();
            //colors_brush.TileMode = TileMode.Tile;
            colors_brush.ViewportUnits = BrushMappingMode.Absolute;
            colors_brush.ImageSource = new BitmapImage(new Uri("gfx/test.png", UriKind.Relative));
            material = new DiffuseMaterial(colors_brush);
            
            waterBrush = new SolidColorBrush(Color.FromArgb(64,32,32,255));
            waterMaterial = new DiffuseMaterial(waterBrush);

            ModelBrush = new ImageBrush();
            //colors_brush.TileMode = TileMode.Tile;
            ModelBrush.ViewportUnits = BrushMappingMode.Absolute;
            ModelBrush.ImageSource = new BitmapImage(new Uri("gfx/Duck.png", UriKind.Relative));
            ModelMaterial = new DiffuseMaterial(ModelBrush);

            OutLineBrush = new ImageBrush();
            OutLineBrush.ViewportUnits = BrushMappingMode.Absolute;
            OutLineBrush.ImageSource = new BitmapImage(new Uri("gfx/CubeOutLine.png", UriKind.Relative));
            OutLineMaterial = new DiffuseMaterial(OutLineBrush);

        }

        public Boolean HasModelAttached(String ModelName)
        {
            return MainWindow.HasModel(ModelName);
        }

        public Boolean HasModelAttached(GeometryModel3D Model)
        {
            return MainWindow.HasModel(Model);
        }

        public void RemoveModel(String ModelName)
        {
            MainWindow.DetachGeometry(ModelName);   
        }
        
        public void addPlayerModel(GeometryModel3D Model, Point3D Position)
        {
            MainWindow.AttachGeometry(Model, "Player");
        }

        //GeometryModel3D Model, String NewName, Point3D NewPosition

        public void addChunk(GeometryModel3D Model, String ChunkName, Point3D Position)
        {
            Model.Material = material;
            //Model.BackMaterial = material;
            if (chunks.ContainsKey(ChunkName))
            {
                MainWindow.DetachGeometry(ChunkName);
                chunks.Remove(ChunkName);
            }
            chunks.Add(ChunkName, Model);
            MainWindow.AttachGeometry(Model, ChunkName);
        }
        
        public void addWaterChunk(GeometryModel3D Model, String ChunkName, Point3D Position)
        {
            Model.Material = waterMaterial;
            //Model.BackMaterial = material;
            if (waterChunks.ContainsKey(ChunkName))
            {
                MainWindow.DetachGeometry(ChunkName);
                waterChunks.Remove(ChunkName);
            }
            MainWindow.AttachGeometry(Model, ChunkName);
            waterChunks.Add(ChunkName, Model);
        }
        
        public void AddModel(GeometryModel3D Model, String ModelName)
        {
            if (Model.Material == null)
            {
                Model.Material = ModelMaterial;
                //Model.Material = waterMaterial;
            }

            //Model.Material = ModelMaterial;
            //Model.BackMaterial = ModelMaterial;

            if (models.ContainsKey(ModelName))
            {
                MainWindow.DetachGeometry(ModelName);
                models.Remove(ModelName);
                models.Add(ModelName, Model);
            }
            else
            {
                models.Add(ModelName, Model);
            }
            MainWindow.AttachGeometry(Model, ModelName);
        }

        public void AddCubeOutLine(GeometryModel3D Model, String ModelName)
        {
            Model.Material = OutLineMaterial;
            //Model.BackMaterial = material;
            if (chunks.ContainsKey(ModelName))
            {
                MainWindow.DetachGeometry(ModelName);
                chunks.Remove(ModelName);
            }
            chunks.Add(ModelName, Model);
            MainWindow.AttachGeometry(Model, ModelName);
        }

        public void RemoveModel(GeometryModel3D Model, String ModelName, Point3D Position)
        {
            //Model.Material = ModelMaterial;
            //Model.BackMaterial = ModelMaterial;
            if (models.ContainsKey(ModelName))
            {
                MainWindow.DetachGeometry(ModelName);
                models.Remove(ModelName);
                models.Add(ModelName, Model);
            }
        }
        
        public void removeChunk(String ChunkName)
        {
            MainWindow.DetachGeometry(ChunkName);
            chunks.Remove(ChunkName);
        }

        public void removeWaterChunk(String WaterChunkName)
        {
            MainWindow.DetachGeometry(WaterChunkName);
            waterChunks.Remove(WaterChunkName);
        }

        private GeometryModel3D GetModel(String ModelName)
        {
            foreach (KeyValuePair<String, GeometryModel3D> entry in models)
            {
                if (entry.Key == ModelName)
                {
                    return entry.Value;
                }
            }
            return null;
        }

        private GeometryModel3D getWaterChunkModel(String ChunkName)
        {
            foreach (KeyValuePair<String, GeometryModel3D> entry in waterChunks)
            {
                if (entry.Key == ChunkName)
                {
                    return entry.Value;
                }
            }
            return null;
        }

        private GeometryModel3D getChunkModel(String ChunkName)
        {
            foreach (KeyValuePair<String, GeometryModel3D> entry in chunks)
            {
                if (entry.Key == ChunkName)
                {
                    return entry.Value;
                }
            }
            return null;
        }

    }
}