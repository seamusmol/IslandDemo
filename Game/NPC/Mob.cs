using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using System.Diagnostics;

namespace Game
{
    public class Mob : Entity
    {
        public int ModelID { get; set; }
        int AnimationCycle = 0;
        
        public int Aggression { get; set; }
        public int Awareness { get; set; }
        Boolean HasAction = true;

        public List<Mob> HostileMobs = new List<Mob>();

        public Mob( Point3D position, int EntityID) : base(position, EntityID)
        {
            IsMovable = true;
        }

        public override void OnUpdate(float tpf, MobManager MobManager)
        {
            if (HasAction)
            {
                CallAI(tpf, MobManager);
            }
            
        }

        public override void CallAI(float tpf, MobManager MobManager)
        {
            

        }

        public override void OnImpact(float tpf, Entity OtherEntity)
        {
            Vector3D CollisionDirectionA = new Vector3D(Position.X - OtherEntity.Position.X, Position.Y - OtherEntity.Position.Y, Position.Z - OtherEntity.Position.Z);
            CollisionDirectionA.Normalize();


            //get impact direction
            //apply force based on impact

        }

        public override void OnFire()
        {
        }

        public override void OnMove(float tpf, Vector3D Direction, bool[] VoxelValue)
        {
            float sx = (float)Direction.X * tpf * Velocity;
            float sz = (float)Direction.Z * tpf * Velocity;
            //float sx = (float)Direction.X;
            //float sz = (float)Direction.Z;

            float sy = 0;

            int BinaryValue = 0;
            BinaryValue += VoxelValue[0] ? 1 : 0;
            BinaryValue += VoxelValue[1] ? 2 : 0;
            BinaryValue += VoxelValue[2] ? 4 : 0;
            BinaryValue += VoxelValue[3] ? 8 : 0;
            BinaryValue += VoxelValue[4] ? 16 : 0;
            BinaryValue += VoxelValue[5] ? 32 : 0;
            BinaryValue += VoxelValue[6] ? 64 : 0;
            BinaryValue += VoxelValue[7] ? 128 : 0;
            
            float v1 = VoxelValue[4] ? 1 : 0;
            float v2 = VoxelValue[5] ? 1 : 0;
            float v3 = VoxelValue[6] ? 1 : 0;
            float v4 = VoxelValue[7] ? 1 : 0;
            
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
            else if (BinaryValue == 0 && Position.Y > 0)
            {
                sy = -0.32f;
            }
            else if (BinaryValue == 0 && Position.Y <= 0)
            {
                sy = 0.32f;
            }
            else if (BinaryValue == 15)
            {
                if (Position.Y % 1 < 0.5f)
                {
                    Position = new Point3D(Position.X, Position.Y + (Position.Y % 0.5), Position.Z);
                }
                else if (Position.Y % 1 > 0.5f)
                {
                    Position = new Point3D(Position.X, Position.Y - (Position.Y % 0.5), Position.Z);
                }
            }
            else
            {
                sy = (MathUtil.MathUtil.bilerp((float)Position.X % 1, (float)Position.Z % 1, 0, 1, 0, 1, v1, v2, v3, v4));
            }
            
            Position = new Point3D(Position.X + sx, Position.Y + sy, Position.Z + sz);
            //TranslateTransform3D translation = new TranslateTransform3D(Position.X, Position.Y + 0.5f, Position.Z);

        }
    }


}
