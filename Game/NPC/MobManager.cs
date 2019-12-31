using main;
using mob;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using terrain;
using System.Diagnostics;
using System.Windows;

namespace Game
{
    public class MobManager : AppState
    {
        List<Entity> EntityList = new List<Entity>();
        public List<Mob> MobList { get; set; } = new List<Mob>();

        //same indices, same texcoords
        
        RenderManager RenderManager;
        public ChunkTracker ChunkTracker { get; }
        public Player Player { get; set; } 

        public MobManager(RenderManager renderManager, ChunkTracker chunktracker, Player player)
        {
            RenderManager = renderManager;
            ChunkTracker = chunktracker;
            Player = player;
        }
        
        public override void initialize(MainWindow MainWindow)
        {

            //create several mobs for testing
            for (int i = 0; i < 10; i++)
            {
                //Monster Monster = new Monster(new Point3D(512 + i * 10 , 10, 512 + i * 10));
                Duck SomeDuck = new Duck(new Point3D(Player.ModelPosition.X - (10 * (i % 5)), ApplicationSettings.worldHeight, Player.ModelPosition.Z - (10 * i)), i);

                EntityList.Add(SomeDuck);
                MobList.Add(SomeDuck);
            }
        }
        
        public override void update(float tpf, long FrameTime)
        {
            //TODO
            //Call Mob behaviour
            CheckMobWorldBounds();
            CheckMobCollision(tpf);
            for (int i = 0; i < EntityList.Count; i++)
            {
                if (EntityList[i].IsEnabled)
                {
                    EntityList[i].OnUpdate(tpf, this);
                }
            }
            UpdateModels();
        }

        public void CheckMobWorldBounds()
        {
            int WorldBoundMax = ApplicationSettings.chunkSize * ApplicationSettings.worldSize;

            for (int i = 0; i < MobList.Count; i++)
            {
                if (MathUtil.MathUtil.distance(MobList[i].Position, Player.ModelPosition) > ApplicationSettings.chunkSize*ApplicationSettings.renderDistance)
                {
                    MobList[i].IsEnabled = false;
                }
                else
                {
                    MobList[i].IsEnabled = true;
                }
            }

        }

        public void UpdateModels()
        {
            for (int i = 0; i < MobList.Count; i++)
            {
                if (MobList[i].IsEnabled)
                {
                    if (MobList[i].Model == null)
                    {
                        //create transform, apply it
                        MeshGeometry3D Geometry = new MeshGeometry3D();

                        List<Point3D> Vertices = ModelManager.VertexBufferMap[MobList[i].GetType().Name];
                        List<Vector3D> Normals = ModelManager.NormalBufferMap[MobList[i].GetType().Name];
                        List<Point> Texcoords = ModelManager.TexCoordBufferMap[MobList[i].GetType().Name];

                        for (int j = 0; j < Vertices.Count; j++)
                        {
                            Geometry.Positions.Add(Vertices[j]);
                            Geometry.Normals.Add(Normals[j]);
                            Geometry.TextureCoordinates.Add(Texcoords[j]);
                            Geometry.TriangleIndices.Add(j);
                        }
                        MobList[i].Model = new GeometryModel3D();

                        MobList[i].Model.Geometry = Geometry;

                        RenderManager.AddModel(MobList[i].Model, "Mobs" + MobList[i].EntityID);
                    }
                    TranslateTransform3D translation = new TranslateTransform3D(MobList[i].Position.X, MobList[i].Position.Y, MobList[i].Position.Z);
                    MobList[i].Model.Transform = translation;
                }
                else if (!MobList[i].IsEnabled && MobList[i].Model != null)
                {
                    if (RenderManager.HasModelAttached(MobList[i].Model))
                    {
                        RenderManager.RemoveModel("Mobs" + MobList[i].EntityID);
                    }
                }
            }

        }
        
        public void CheckMobCollision(float tpf)
        {
            //TODO
            //Calculate if collided and impact force
            //send 
            List<Vector3D> CollistionDirectionsA = new List<Vector3D>();
            List<Vector3D> CollistionDirectionsB = new List<Vector3D>();
            List<Entity> AffectedEntitiesA = new List<Entity>();
            List<Entity> AffectedEntitiesB = new List<Entity>();
        
            for (int i = 0; i < EntityList.Count; i++)
            {
                for(int j = 0; j < EntityList.Count; j++)
                {
                    if (!(AffectedEntitiesA.Contains(EntityList[i]) || AffectedEntitiesB.Contains(EntityList[j])))
                    {
                        if (MathUtil.MathUtil.distance(EntityList[i].Position, EntityList[j].Position) < 2)
                        {
                            /*
                            Vector3D CollisionDirectionA = new Vector3D(EntityList[i].Position.X - EntityList[j].Position.X, EntityList[i].Position.Y - EntityList[j].Position.Y, EntityList[i].Position.Z - EntityList[j].Position.Z);
                            CollisionDirectionA.Normalize();

                            Vector3D InverseCollisionDirection = CollisionDirectionA;
                            InverseCollisionDirection.Negate();
                            
                            CollistionDirectionsA.Add(CollisionDirectionA);
                            CollistionDirectionsB.Add(InverseCollisionDirection);
                            */

                            AffectedEntitiesA.Add(EntityList[i]);
                            AffectedEntitiesB.Add(EntityList[j]);
                        }
                    }
                }
            }

            for (int i = 0; i < EntityList.Count; i++)
            {
                for (int j = 0; j < AffectedEntitiesA.Count; j++)
                {
                    if (EntityList[i].Equals(AffectedEntitiesA[j]))
                    {
                        EntityList[i].OnImpact(tpf, AffectedEntitiesA[j]);
                    }
                    else if(EntityList[i].Equals(AffectedEntitiesB[j]))
                    {
                        EntityList[i].OnImpact(tpf, AffectedEntitiesB[j]);
                    }
                }
            }

        }
        
        public override void close()
        {

        }
    }
}
