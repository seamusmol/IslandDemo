using System;
using System.Windows;
using System.Windows.Media.Media3D;
using main;
using Game;
using System.Diagnostics;
using terrain;
using System.Collections;
using System.Windows.Media;
using System.Collections.Generic;

namespace mob
{
    public class Player : AppState
    {
        MainWindow MainWindow;
        ApplicationInputManager Input;
        public Point3D CameraPosition { get; set; }
        public Vector3D CameraDirection { get; set; }
        public Point3D ModelPosition { get; set; }
       
        public GeometryModel3D Model { get; set; }
        
        public GeometryModel3D CubeOutLineModel { get; set; }

        double cameraOffsetX = 0;
        //float cameraOffsetY = 20;
        //float cameraOffsetZ = 2;

        //map view
        double cameraOffsetY = 200;
        double cameraOffsetZ = 20;

        float vel = 10;
        
        int CameraPos = 1;
        int CameraPosMax = 1;

        double RotXZ = 90;
        double RotY = 0;

        long TimePassed = 0;
        long ModifyTimePassed = 0;
        long OutLineToggleTimePassed = 0;

        Boolean HasOutLineTool { get; set; } = true;

        Point3D CubeOutLinePosition { get; set; } = new Point3D();
        
        ChunkTracker ChunkTracker;
        RenderManager RenderManager;

        public Player(Point3D Position, ChunkTracker chunkTracker)
	    {
            ChunkTracker = chunkTracker;
            ModelPosition = Position;
            CameraPosition = new Point3D(ModelPosition.X + cameraOffsetX, ModelPosition.Y + cameraOffsetY, ModelPosition.Z + cameraOffsetZ);
            CameraDirection = new Vector3D(0, -10, -1);
        }

        override
        public void initialize(MainWindow mainWindow)
        {
            MainWindow = mainWindow;
            Input = mainWindow.Input;
            mainWindow.Camera.Position = CameraPosition;
            RenderManager = MainWindow.RenderManager;
            initModel();
            CreateCubeOutLineGeometry();
        }

        public void initModel()
        {
            MeshGeometry3D Mesh = new MeshGeometry3D();
            Mesh.Positions.Add(new Point3D(-0.5f, 0, -0.5f));
            Mesh.Positions.Add(new Point3D(-0.5f, 0, 0.5f));
            Mesh.Positions.Add(new Point3D(0.5f, 0, 0.5f));

            Mesh.Positions.Add(new Point3D(0.5f, 0, 0.5f));
            Mesh.Positions.Add(new Point3D(0.5f, 0, -0.5f));
            Mesh.Positions.Add(new Point3D(-0.5f, 0, -0.5f));

            Mesh.TriangleIndices.Add(0);
            Mesh.TriangleIndices.Add(1);
            Mesh.TriangleIndices.Add(2);
            Mesh.TriangleIndices.Add(3);
            Mesh.TriangleIndices.Add(4);
            Mesh.TriangleIndices.Add(5);

            Mesh.Normals.Add(new Vector3D(0, 0, -1));
            Mesh.Normals.Add(new Vector3D(0, 0, -1));
            Mesh.Normals.Add(new Vector3D(0, 0, -1));
            Mesh.Normals.Add(new Vector3D(0, 0, -1));
            Mesh.Normals.Add(new Vector3D(0, 0, -1));
            Mesh.Normals.Add(new Vector3D(0, 0, -1));
            
            Mesh.TextureCoordinates.Add(new Point(1, 0));
            Mesh.TextureCoordinates.Add(new Point(1, 1));
            Mesh.TextureCoordinates.Add(new Point(0, 1));
            Mesh.TextureCoordinates.Add(new Point(1, 0));
            Mesh.TextureCoordinates.Add(new Point(1, 1));
            Mesh.TextureCoordinates.Add(new Point(0, 1));

            DiffuseMaterial material = new DiffuseMaterial(Brushes.Red);

            Model = new GeometryModel3D();
            Model.Geometry = Mesh;
            Model.Material = material;
            Model.BackMaterial = material;
            
            RenderManager.addPlayerModel(Model, ModelPosition);
        }

        override
        public void update(float tpf, long frametime)
        {
            UpdateCameraRotation();
            Point3D dir = new Point3D(0, 0, 0);
            
            if (Input.hasInput("W"))
            {
                dir.X += 1.4;
            }
            if (Input.hasInput("S"))
            {
                dir.X += -1.4;
            }
            if (Input.hasInput("A"))
            {
                dir.Z += -1.4;
            }
            if (Input.hasInput("D"))
            {
                dir.Z += 1.4;
            }
            switch (CameraPos)
            {
                case 1:
                    Point Move = MathUtil.MathUtil.RotatePoint(new Point(dir.X, dir.Z), RotXZ);
                    dir.X = Move.X;
                    dir.Z = Move.Y;
                    break;
                default:
                    double tempX = dir.X;
                    dir.X = dir.Z;
                    dir.Z = -tempX;
                    break;
            }

            float sx = (float)dir.X * tpf * vel;
            float sz = (float)dir.Z * tpf * vel;
            float sy = 0;

            Boolean[] value = ChunkTracker.getVoxelValue(new Point3D(ModelPosition.X, Math.Floor(ModelPosition.Y), ModelPosition.Z));
            int BinaryValue = 0;
            BinaryValue += value[0] ? 1 : 0;
            BinaryValue += value[1] ? 2 : 0;
            BinaryValue += value[2] ? 4 : 0;
            BinaryValue += value[3] ? 8 : 0;
            BinaryValue += value[4] ? 16 : 0;
            BinaryValue += value[5] ? 32 : 0;
            BinaryValue += value[6] ? 64 : 0;
            BinaryValue += value[7] ? 128 : 0;
            
            if (dir.X == 0 && dir.Z == 0)
            {
                if (BinaryValue == 255)
                {
                    sy = 0.32f;
                }
                else if (BinaryValue == 15)
                {
                    if (ModelPosition.Y % 1 < 0.5f)
                    {
                        ModelPosition = new Point3D(ModelPosition.X, ModelPosition.Y + (ModelPosition.Y % 0.5), ModelPosition.Z);
                    }
                }
                else if (!value[0] && !value[1] && !value[2] && !value[3])
                {
                    sy = -0.32f;
                }
            }
            else
            {
                float v1 = value[4] ? 1 : 0;
                float v2 = value[5] ? 1 : 0;
                float v3 = value[6] ? 1 : 0;
                float v4 = value[7] ? 1 : 0;

                if (BinaryValue == 255)
                {
                    sy = 0.32f;
                }
                else if (BinaryValue == 153)
                {
                    sx = sx > 0 ? sx : 0;
                }
                else if (BinaryValue == 204)
                {
                    sz = sz < 0 ? sz : 0;
                }
                else if (BinaryValue == 102)
                {
                    sx = sx < 0 ? sx : 0;
                }
                else if (BinaryValue == 51)
                {
                    sz = sz > 0 ? sz : 0;
                }
                else if (BinaryValue == 136 || BinaryValue == 221)
                {
                    sx = sx > 0 ? sx : 0;
                    sz = sz < 0 ? sz : 0;
                }
                else if (BinaryValue == 68 || BinaryValue == 238)
                {
                    sx = sx < 0 ? sx : 0;
                    sz = sz < 0 ? sz : 0;
                }
                else if (BinaryValue == 34 || BinaryValue == 119)
                {
                    sx = sx < 0 ? sx : 0;
                    sz = sz > 0 ? sz : 0;
                }
                else if (BinaryValue == 17 || BinaryValue == 187)
                {
                    sx = sx > 0 ? sx : 0;
                    sz = sz > 0 ? sz : 0;
                }
                else if (!value[0] && !value[1] && !value[2] && !value[3])
                {
                    sy = -0.32f;
                }
                else if (BinaryValue == 15)
                {
                    if (ModelPosition.Y % 1 < 0.5f)
                    {
                        ModelPosition = new Point3D(ModelPosition.X, ModelPosition.Y + (ModelPosition.Y % 0.5), ModelPosition.Z);
                    }
                }
                else
                {
                    sy = (MathUtil.MathUtil.bilerp((float)ModelPosition.X % 1, (float)ModelPosition.Z % 1, 0, 1, 0, 1, v1, v2, v3, v4));
                }
            }
            ModelPosition = new Point3D(ModelPosition.X + sx, ModelPosition.Y + sy, ModelPosition.Z + sz);
            TranslateTransform3D translation = new TranslateTransform3D(ModelPosition.X, ModelPosition.Y + 0.5f, ModelPosition.Z);
            Model.Transform = translation;
            
            if (Input.hasInput("F5"))
            {
                //toggle first/third person camera
                if (TimePassed > 250)
                {
                    //Debug.WriteLine(DateTime.Now.Millisecond - toggleTime);
                    CameraPos = CameraPos >= CameraPosMax ? 0 : CameraPos + 1;
                    TimePassed = 0;
                }   
            }
            //CameraDirection = new Vector3D(Math.Cos(MathUtil.MathUtil.GetRadian(radian)), 0, Math.Sin(MathUtil.MathUtil.GetRadian(radian)));
            DrawCubeOutLine();
            if (ModifyTimePassed > 16)
            {
                if (Input.hasInput("M2"))
                {
                    RayTest(true);
                }

                if (Input.hasInput("M1"))
                {
                    RayTest(false);
                }
                
                ModifyTimePassed = 0;
            }
           
            if (Input.hasInput("F"))
            {
                if (OutLineToggleTimePassed > 100)
                {
                    HasOutLineTool = !HasOutLineTool;
                    OutLineToggleTimePassed = 0;
                }
            }

            TimePassed += (long)(tpf*1000);
            ModifyTimePassed += (long)(tpf * 1000);
            OutLineToggleTimePassed += (long)(tpf * 1000);
            CreateCameraView();
        }

        public void RayTest(Boolean Value)
        {
            int CurrentMaterial = 129;
            
            //if (ChunkTracker.HasVoxel(CubeOutLinePosition))
            //{
                //remove block
                ChunkTracker.SetVertex(CubeOutLinePosition, Value, CurrentMaterial);
            //}
        }
        
        public void DrawCubeOutLine()
        {
            if (HasOutLineTool)
            {
                Point3D CubePosition = new Point3D(-1, -1, -1);
                double PickDistance = 5;
                Vector3D ViewDir = CameraDirection;
                
                for (double i = 0; i <= PickDistance; i+= 0.1f)
                {
                    Point3D PickPosition = new Point3D(CameraPosition.X + ViewDir.X * i, CameraPosition.Y + ViewDir.Y * i, CameraPosition.Z + ViewDir.Z * i);
                    CubePosition = new Point3D(Math.Round(CameraPosition.X + ViewDir.X * (i+1)), Math.Round(CameraPosition.Y + ViewDir.Y * (i + 1)), Math.Round(CameraPosition.Z + ViewDir.Z * (i + 1)));
                    if (ChunkTracker.HasVoxel(PickPosition))
                    {
                        break;
                    }
                }
                
                if (!CubeOutLinePosition.Equals(CubePosition))
                {
                    //redraw cube outline 
                    CubeOutLinePosition = CubePosition;
                    TranslateTransform3D translation = new TranslateTransform3D(CubeOutLinePosition.X, CubeOutLinePosition.Y, CubeOutLinePosition.Z);
                    CubeOutLineModel.Transform = translation;
                }
                //Debug.WriteLine(CubeOutLinePosition);
            }
            else
            {
                TranslateTransform3D translation = new TranslateTransform3D(-100, -100, -100);
                CubeOutLineModel.Transform = translation;
                CubeOutLinePosition = new Point3D(-100, -100, -100);
            }
        }
        
        public void CreateCubeOutLineGeometry()
        {
            CubeOutLineModel = new GeometryModel3D();
            MeshGeometry3D Geometry = new MeshGeometry3D();

            List<Point3D> Vertices = ModelManager.VertexBufferMap["CubeOutLine"];
            List<Vector3D> Normals = ModelManager.NormalBufferMap["CubeOutLine"];
            List<Point> Texcoords = ModelManager.TexCoordBufferMap["CubeOutLine"];

            for (int j = 0; j < Vertices.Count; j++)
            {
                Geometry.Positions.Add(Vertices[j]);
                Geometry.Normals.Add(Normals[j]);
                Geometry.TextureCoordinates.Add(Texcoords[j]);
                Geometry.TriangleIndices.Add(j);
            }
            CubeOutLineModel = new GeometryModel3D();

            CubeOutLineModel.Geometry = Geometry;
            RenderManager.AddCubeOutLine(CubeOutLineModel, "TransparentCubeOutLine");

            TranslateTransform3D translation = new TranslateTransform3D(CubeOutLinePosition.X, CubeOutLinePosition.Y, CubeOutLinePosition.Z);
            CubeOutLineModel.Transform = translation;
            
        }

        public void UpdateCameraRotation()
        {
            Vector MouseMovement = Input.MouseMovement;
            
            RotY = (RotY + (MouseMovement.Y/ 572.958) * ApplicationSettings.mouse_sens);
            RotY = RotY < -0.99d ? -0.99d : RotY;
            RotY = RotY > 0.99d ? 0.99d : RotY;

            RotXZ = (RotXZ + (MouseMovement.X/10) * ApplicationSettings.mouse_sens);
            
            double radian = MathUtil.MathUtil.GetRadian(RotXZ);
            Point Move = MathUtil.MathUtil.RotatePoint(new Point(1, 1), RotXZ);

            switch (CameraPos)
            {
                case 0:
                    cameraOffsetX = 0;
                    cameraOffsetY = 25;
                    cameraOffsetZ = 2.5f;
                    CameraDirection = new Vector3D(0, -10, -1);
                    break;
                case 1:
                    cameraOffsetY = 1.5f;
                    cameraOffsetZ = 0;
                    cameraOffsetX = 0;
                    CameraDirection = new Vector3D((float)Math.Cos(radian),-RotY, (float)Math.Sin(radian));
                    break;
            }
            
        }
        
        
        private void CreateCameraView()
        {
            CameraPosition = new Point3D(ModelPosition.X, ModelPosition.Y + cameraOffsetY, ModelPosition.Z + cameraOffsetZ);
            //CameraDirection = new Vector3D(0,-10,-1);

            //Map View
            //CameraDirection = new Vector3D(0, -10, -1);

            //CameraDirection.Normalize();

            MainWindow.Camera.LookDirection = CameraDirection;
            MainWindow.Camera.Position = CameraPosition;


        }
        
        override
        public void close()
        {

        }
    }

}
