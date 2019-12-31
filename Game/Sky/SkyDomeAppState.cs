using main;
using mob;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using Game;

namespace Sky
{
    /*
     * TODO 
     * set Skydome to first render
     */
    public class SkyDomeAppState : AppState
    {
        MainWindow MainWindow;
        Player Player;

        ImageBrush SkyBrush;
        DiffuseMaterial material;
        MeshGeometry3D SkyDomeMesh;
        GeometryModel3D SkyDomeModel;
        TranslateTransform3D SkyDomeTransform;

        Point3DCollection PositionBuffer;
        Vector3DCollection NormalBuffer;
        PointCollection TexcoordBuffer;
        Int32Collection IndexBuffer;

        public SkyDomeAppState()
        {
            
        }

        public override void initialize(MainWindow mainWindow)
        {
            MainWindow = mainWindow;
            Player = MainWindow.GameState.Player;
            GenerateSkyDome();
        }

        public override void update(float tpf, long FrameTime)
        {
            //move skydome to playe position
            if (Player.ModelPosition != null)
            {
                SkyDomeTransform = new TranslateTransform3D(Player.ModelPosition.X, 0, Player.ModelPosition.Z);
                SkyDomeModel.Transform = SkyDomeTransform;
            }
        }

        
        public void GenerateSkyDome()
        {
            //use blender model
            SkyBrush = new ImageBrush();
            SkyBrush.ViewportUnits = BrushMappingMode.Absolute;
            SkyBrush.ImageSource = new BitmapImage(new Uri("gfx/SkyDome.png", UriKind.Relative));
            material = new DiffuseMaterial(SkyBrush);

            SkyDomeMesh = new MeshGeometry3D();
            SkyDomeModel = new GeometryModel3D();

            PositionBuffer = SkyDomeMesh.Positions;
            NormalBuffer = SkyDomeMesh.Normals;
            TexcoordBuffer = SkyDomeMesh.TextureCoordinates;
            IndexBuffer = SkyDomeMesh.TriangleIndices;

            int SkyDomeDist = ApplicationSettings.chunkSize*ApplicationSettings.renderDistance + ApplicationSettings.chunkSize;
            //int SkyDomeDist = 500;

            Vector3D Normal = new Vector3D(0, 0, -1);

            Point[] tex = new Point[14];
            tex[0] = new Point(0.2505, 0.005);
            tex[1] = new Point(0.4995, 0.005);

            tex[2] = new Point(0, 0.3383);
            tex[3] = new Point(0.250, 0.3383);
            tex[4] = new Point(0.5, 0.3383);
            tex[5] = new Point(0.75, 0.3383);
            tex[6] = new Point(1, 0.3383);

            tex[7] = new Point(0, 0.6661);
            tex[8] = new Point(0.25, 0.6661);
            tex[9] = new Point(0.5, 0.6661);
            tex[10] = new Point(0.75, 0.6661);
            tex[11] = new Point(1, 0.6661);

            tex[12] = new Point(0.2505, 0.995);
            tex[13] = new Point(0.4995, 0.995);

            Point3D[] pos = new Point3D[8];
            pos[0] = new Point3D(-SkyDomeDist, -SkyDomeDist, -SkyDomeDist);
            pos[1] = new Point3D(-SkyDomeDist, SkyDomeDist, -SkyDomeDist);
            pos[2] = new Point3D(SkyDomeDist, -SkyDomeDist, -SkyDomeDist);
            pos[3] = new Point3D(SkyDomeDist, SkyDomeDist, -SkyDomeDist);
            pos[4] = new Point3D(-SkyDomeDist, -SkyDomeDist, SkyDomeDist);
            pos[5] = new Point3D(-SkyDomeDist, SkyDomeDist, SkyDomeDist);
            pos[6] = new Point3D(SkyDomeDist, -SkyDomeDist, SkyDomeDist);
            pos[7] = new Point3D(SkyDomeDist, SkyDomeDist, SkyDomeDist);
            
            //front
            PositionBuffer.Add(pos[1]);
            PositionBuffer.Add(pos[0]);
            PositionBuffer.Add(pos[2]);
            PositionBuffer.Add(pos[2]);
            PositionBuffer.Add(pos[3]);
            PositionBuffer.Add(pos[1]);
            //right
            PositionBuffer.Add(pos[3]);
            PositionBuffer.Add(pos[2]);
            PositionBuffer.Add(pos[6]);
            PositionBuffer.Add(pos[6]);
            PositionBuffer.Add(pos[7]);
            PositionBuffer.Add(pos[3]);
            //back
            PositionBuffer.Add(pos[7]);
            PositionBuffer.Add(pos[6]);
            PositionBuffer.Add(pos[4]);
            PositionBuffer.Add(pos[4]);
            PositionBuffer.Add(pos[5]);
            PositionBuffer.Add(pos[7]);
            //left
            PositionBuffer.Add(pos[5]);
            PositionBuffer.Add(pos[4]);
            PositionBuffer.Add(pos[0]);
            PositionBuffer.Add(pos[0]);
            PositionBuffer.Add(pos[1]);
            PositionBuffer.Add(pos[5]);
            //top
            PositionBuffer.Add(pos[3]);
            PositionBuffer.Add(pos[7]);
            PositionBuffer.Add(pos[5]);
            PositionBuffer.Add(pos[5]);
            PositionBuffer.Add(pos[1]);
            PositionBuffer.Add(pos[3]);
            //bottom
            PositionBuffer.Add(pos[0]);
            PositionBuffer.Add(pos[4]);
            PositionBuffer.Add(pos[6]);
            PositionBuffer.Add(pos[6]);
            PositionBuffer.Add(pos[2]);
            PositionBuffer.Add(pos[0]);

            for (int i = 0; i < SkyDomeMesh.Positions.Count; i++)
            {
                IndexBuffer.Add(i);
                NormalBuffer.Add(Normal);
            }
            //front
            TexcoordBuffer.Add(tex[3]);
            TexcoordBuffer.Add(tex[8]);
            TexcoordBuffer.Add(tex[9]);
            TexcoordBuffer.Add(tex[9]);
            TexcoordBuffer.Add(tex[4]);
            TexcoordBuffer.Add(tex[3]);
            //right
            TexcoordBuffer.Add(tex[4]);
            TexcoordBuffer.Add(tex[9]);
            TexcoordBuffer.Add(tex[10]);
            TexcoordBuffer.Add(tex[10]);
            TexcoordBuffer.Add(tex[5]);
            TexcoordBuffer.Add(tex[4]);
            //back
            TexcoordBuffer.Add(tex[5]);
            TexcoordBuffer.Add(tex[10]);
            TexcoordBuffer.Add(tex[11]);
            TexcoordBuffer.Add(tex[11]);
            TexcoordBuffer.Add(tex[6]);
            TexcoordBuffer.Add(tex[5]);
            //left
            TexcoordBuffer.Add(tex[2]);
            TexcoordBuffer.Add(tex[7]);
            TexcoordBuffer.Add(tex[8]);
            TexcoordBuffer.Add(tex[8]);
            TexcoordBuffer.Add(tex[3]);
            TexcoordBuffer.Add(tex[2]);
            //top
            TexcoordBuffer.Add(tex[0]);
            TexcoordBuffer.Add(tex[3]);
            TexcoordBuffer.Add(tex[4]);
            TexcoordBuffer.Add(tex[4]);
            TexcoordBuffer.Add(tex[1]);
            TexcoordBuffer.Add(tex[0]);
            //bottom
            TexcoordBuffer.Add(tex[8]);
            TexcoordBuffer.Add(tex[12]);
            TexcoordBuffer.Add(tex[13]);
            TexcoordBuffer.Add(tex[13]);
            TexcoordBuffer.Add(tex[9]);
            TexcoordBuffer.Add(tex[8]);

            SkyDomeModel = new GeometryModel3D();
            SkyDomeModel.Geometry = SkyDomeMesh;
            SkyDomeModel.Material = material;

            MainWindow.AttachGeometry(SkyDomeModel, "SkyDome");
        }
        

        public override void close()
        {
        }
    }

   

}
