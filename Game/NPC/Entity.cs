using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;
using terrain;

namespace Game
{
    public interface IEntity
    {
        void OnUpdate(float tpf, MobManager MobManager);
        void CallAI(float tpf, MobManager MobManager);
        void OnImpact(float tpf, Entity OtherEntity);
        void OnMove(float TPF, Vector3D Direction, Boolean[] VoxelValue);
        void OnFire();
        
    }
    
    public abstract class Entity : IEntity
    {
        public GeometryModel3D Model { get; set; }
        public int EntityID { get; set; }
        public int Health;
        public int mass;
        public Boolean IsFlammable = false;
        public Boolean IsMovable = false;
        public Point3D Position { get; set; }
        public Vector3D Rotation { get; }
        public float Velocity { get; set; }
        public Boolean IsEnabled = true;

        public Entity(Point3D position, int entityID)
        {
            EntityID = entityID;
            Position = position;
            Rotation = new Vector3D(0,1,0);
        }
        //TODO
        //Call Interface methods if true;
        public abstract void OnUpdate(float tpf, MobManager MobManager);

        //TODO
        //set direction
        public abstract void CallAI(float tpf, MobManager MobManager);

        //TODO
        //receive Impact force and direction
        //apply absorbance
        //call OnMove
        public abstract void OnImpact(float tpf, Entity OtherEntity);
        public abstract void OnMove(float tpf, Vector3D Direction, Boolean[] VoxelValue);
        public abstract void OnFire();
    }
    

}
