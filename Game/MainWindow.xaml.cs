using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using main;
using System.Windows.Media.Media3D;
using System.Diagnostics;
using System.Collections.Concurrent;
using System.Threading;
using System.Windows.Threading;
using System.Windows.Interop;
using System.Windows.Media.Animation;
using main;
using System.Runtime.InteropServices;
using System.Windows.Input;

namespace Game
{
    public partial class MainWindow : Window
    {
        Viewport3D Viewport = new Viewport3D();

        Model3DGroup ModelGroup = new Model3DGroup();
        ModelVisual3D ModelVisual = new ModelVisual3D();
        
        Model3DGroup TransparentModelGroup = new Model3DGroup();
        ModelVisual3D TransparentModelVisual = new ModelVisual3D();

        Model3DGroup WaterModelGroup = new Model3DGroup();
        ModelVisual3D WaterModelVisual = new ModelVisual3D();
        
        public PerspectiveCamera Camera { get; set; } = new PerspectiveCamera();
        AmbientLight Light;

        public GameState GameState { get; }
        public AppStateManager appStateManager { get; set; }
        public RenderManager RenderManager { get; set; }
        public ApplicationInputManager Input { get; set; }

        //public Dictionary<String, Point3D> OrderPositions = new Dictionary<String, Point3D>();
        public Dictionary<GeometryModel3D, String> OrderModels = new Dictionary<GeometryModel3D, String>();
        
        //public Dictionary<String, Point3D> OrderTransparentPositions = new Dictionary<String, Point3D>();
        public Dictionary<GeometryModel3D, String> OrderTransparentModels = new Dictionary<GeometryModel3D, String>();

        //public Dictionary<String, Point3D> OrderWaterPositions = new Dictionary<String, Point3D>();
        public Dictionary<GeometryModel3D, String> OrderWaterModels = new Dictionary<GeometryModel3D, String>();
        
        MeshGeometry3D ForceRenderMesh = new MeshGeometry3D();
        GeometryModel3D ForceRenderGeom = new GeometryModel3D();
        
        long lastTick = DateTime.Now.Millisecond;
        public Boolean HasFocus { get; set; } = true;
        Point LastPoint = new Point();

        public MainWindow()
        {
            InitializeComponent();
            InitScene();
            ComponentDispatcher.ThreadIdle += new System.EventHandler(UpdateLogic);
            ModelManager.LoadModels();

            this.MouseDown += OnMouseKey;

            RenderOptions.ProcessRenderMode = RenderMode.Default;
            Debug.WriteLine("Render Tier: " + (RenderCapability.Tier >> 16));

            Input = new ApplicationInputManager(this);
            appStateManager = new AppStateManager(this);
            GameState = new GameState();
            RenderManager = new RenderManager(this);
            appStateManager.addAppState("GameState", GameState);
        }

        public void OnMouseKey(Object sender, MouseEventArgs Event)
        {
            Mouse.OverrideCursor = Cursors.None;
            HasFocus = true;
        }
        
        float tpf = 0.0166f;
        public void UpdateLogic(object sender, EventArgs e)
        {
            Stopwatch Watch = new Stopwatch();
            Watch.Start();

            long startTime = DateTime.Now.Millisecond;

            Input.update();
            //float tpf = (startTime - lastTick) == 0 ? 0.16f : 1f / ((startTime - lastTick)*1000f);
            if (HasFocus)
            {
                appStateManager.processAppStates(tpf, startTime);
                //ForceDraw();
                this.InvalidateVisual();
                this.InvalidateMeasure();
                this.InvalidateArrange();
                this.UpdateDefaultStyle();
                this.UpdateLayout();
                //this.Focus();
            }
            lastTick = startTime;
            int sleepTime = (int)(15 - (startTime - lastTick));

            if (sleepTime < 0)
            {
                sleepTime = 0;
            }
            else if (sleepTime > 15)
            {
                sleepTime = 15;
            }
            //Debug.WriteLine(DateTime.Now.Millisecond - lastTick);
            System.Threading.Thread.Sleep((int)(15-(startTime - lastTick)));
            Watch.Stop();
            TimeSpan ts = Watch.Elapsed;
            tpf = (float)ts.TotalSeconds;
            //Debug.WriteLine(tpf);
            //System.Threading.Thread.Sleep((int)sleepTime);

        }
        
        public void InitScene()
        {
            Camera.LookDirection = new Vector3D(0, -2, -1);
            //Camera.Position = new Point3D(ApplicationSettings.worldSize * ApplicationSettings.chunkSize/2, 11, ApplicationSettings.worldSize * ApplicationSettings.chunkSize/2);
            Camera.Position = new Point3D();
            Camera.UpDirection = new Vector3D(0, 1, 0);
            Camera.NearPlaneDistance = 0.01;
            Camera.FarPlaneDistance = 1010;
            Camera.FieldOfView = 90;
            
            Viewport = new Viewport3D();
            Viewport.Height = 720;
            Viewport.Width = 1280;
            Viewport.BringIntoView();

            Light = new AmbientLight();
            Light.Color = Color.FromRgb(64,64,64);
            
            ModelVisual.Content = ModelGroup;
            WaterModelVisual.Content = WaterModelGroup;
            TransparentModelVisual.Content = TransparentModelGroup;

            //TransparentModelVisual
            Viewport.Children.Add(ModelVisual);
            Viewport.Children.Add(WaterModelVisual);
            Viewport.Children.Add(TransparentModelVisual);
            Viewport.Camera = Camera;
            Viewport.IsHitTestVisible = false;

            ModelGroup.Children.Add(Light);
            TransparentModelGroup.Children.Add(Light);
            WaterModelGroup.Children.Add(Light);

            this.Content = Viewport;
            this.Width = 1280;
            this.Height = 720;
            this.Show();
            this.Focus();
        }

        /*
        public void ForceDraw()
        {
            ModelGroup.Children.Remove(ForceRenderGeom);

            ForceRenderMesh = new MeshGeometry3D();
            ForceRenderMesh.Positions.Add(new Point3D(0, 0, 0));
            ForceRenderMesh.Positions.Add(new Point3D(0, 0, 0));
            ForceRenderMesh.Positions.Add(new Point3D(0, 0, 0));

            ForceRenderMesh.TriangleIndices.Add(0);
            ForceRenderMesh.TriangleIndices.Add(1);
            ForceRenderMesh.TriangleIndices.Add(2);

            ForceRenderMesh.Normals.Add(new Vector3D(0, 0, -1));
            ForceRenderMesh.Normals.Add(new Vector3D(0, 0, -1));
            ForceRenderMesh.Normals.Add(new Vector3D(0, 0, -1));

            ForceRenderMesh.TextureCoordinates.Add(new Point(1, 0));
            ForceRenderMesh.TextureCoordinates.Add(new Point(1, 1));
            ForceRenderMesh.TextureCoordinates.Add(new Point(0, 1));
            DiffuseMaterial material = new DiffuseMaterial(Brushes.Blue);

            ForceRenderGeom = new GeometryModel3D();
            ForceRenderGeom.Geometry = ForceRenderMesh;
            ForceRenderGeom.Material = material;
            ForceRenderGeom.BackMaterial = material;

            ModelGroup.Children.Add(ForceRenderGeom);
        }
        */
        private void SetCameraPosition(Point3D Point)
        {
            Camera.Position = Point;
        }
        
        private void SetCameraDirection(Vector3D Direction)
        {
            Camera.LookDirection = Direction;
        }
        
        public void AttachGeometry(GeometryModel3D Model, String NewName)
        {
            DetachGeometry(NewName);
            if (NewName.Contains("Water"))
            {
                OrderWaterModels.Add(Model, NewName);
                WaterModelGroup.Children.Add(Model);
            }
            else if (NewName.Contains("Transparent"))
            {
                OrderTransparentModels.Add(Model, NewName);
                TransparentModelGroup.Children.Add(Model);
            }
            else
            {
                OrderModels.Add(Model, NewName);
                ModelGroup.Children.Add(Model);
            }
        }
        
        public void DetachGeometry(String ModelName)
        {
            if (ModelName.Contains("Water"))
            {
                GeometryModel3D Value = OrderWaterModels.FirstOrDefault(x => x.Value == ModelName).Key;
                if (Value != null)
                {
                    WaterModelGroup.Children.Remove(Value);
                    OrderWaterModels.Remove(Value);
                }
            }
            else if (ModelName.Contains("Transparent"))
            {
                GeometryModel3D Value = OrderTransparentModels.FirstOrDefault(x => x.Value == ModelName).Key;
                if (Value != null)
                {
                    TransparentModelGroup.Children.Remove(Value);
                    OrderTransparentModels.Remove(Value);
                }
            }
            else
            {
                GeometryModel3D Value = OrderModels.FirstOrDefault(x => x.Value == ModelName).Key;
                if (Value != null)
                {
                    ModelGroup.Children.Remove(Value);
                    OrderModels.Remove(Value);
                }
            }
        }
        
        public Boolean HasModel(String ModelName)
        {
            GeometryModel3D Value = OrderModels.FirstOrDefault(x => x.Value == ModelName).Key;
            if (Value == null)
            {
                Value = OrderTransparentModels.FirstOrDefault(x => x.Value == ModelName).Key;
            }
            if (Value == null)
            {
                Value = OrderWaterModels.FirstOrDefault(x => x.Value == ModelName).Key;
            }
            return Value == null;
        }

        public Boolean HasModel(GeometryModel3D Model)
        {
            return OrderModels.ContainsKey(Model) || OrderTransparentModels.ContainsKey(Model) || OrderWaterModels.ContainsKey(Model); ;
        }

        public int GetNameOrder(GeometryModel3D Model)
        {
            if (OrderWaterModels.ContainsKey(Model))
            {
                return 2;
            }
            else if (OrderTransparentModels.ContainsKey(Model))
            {
                return 1;
            }
            else if (OrderModels.ContainsKey(Model))
            {
                return 3;
            }
            return 0;
        }
        
        public Point3D GetPosition(Point3D PlayerPosition, Point3D ModelPosition, String ModelName)
        {
            Vector Direction = new Vector(ModelPosition.X - PlayerPosition.X, ModelPosition.Z - PlayerPosition.Z);
            Direction.Normalize();

            if (ModelName.Contains("Solid"))
            {
                return new Point3D(ModelPosition.X + Direction.X * 0.05f, 0, ModelPosition.Z + Direction.Y * 0.05f);
            }
            else if (ModelName.Contains("Water"))
            {
                return new Point3D(ModelPosition.X + Direction.X * -0.1f, 0, ModelPosition.Z + Direction.Y * -0.1f);
            }
            else if (ModelName.Contains("Transparent"))
            {
                return new Point3D(ModelPosition.X + Direction.X * -0.2f, 0, ModelPosition.Z + Direction.Y * -0.2f);
            }
            else
            {
                return ModelPosition;
            }
        }

        

    }
}
