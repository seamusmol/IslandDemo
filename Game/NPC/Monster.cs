using main;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Media3D;

namespace Game
{
    public class Monster : Mob
    {
        public Monster(Point3D position, int EntityID) : base (position, EntityID)
        {
            Aggression = 5;
            Awareness = 5;
            Velocity = 5;
        }
        
        public override void CallAI(float tpf, MobManager MobManager)
        {
            Vector3D Direction = new Vector3D();
            Point3D PlayerPosition = MobManager.Player.ModelPosition;
            double playerdistance = MathUtil.MathUtil.distance(PlayerPosition, Position);
            //if(playerdistance < Awareness*ApplicationSettings.chunkSize)
            //{
            //    if (playerdistance > 10-Aggression)
            //    {
            //        Direction = new Vector3D(PlayerPosition.X - Position.X, 0, PlayerPosition.Z - Position.Z);
            //        Direction.Normalize();
            //    }
            //}
            if (MathUtil.MathUtil.distance(PlayerPosition, Position) < 20)
            {
                if (MathUtil.MathUtil.distance(PlayerPosition, Position) > 10)
                {
                
                    Vector tempVec = new Vector(PlayerPosition.X - Position.X, PlayerPosition.Z - Position.Z);
                    tempVec.Normalize();

                    Direction = new Vector3D(tempVec.X, 0, tempVec.Y);
                }
            }
            
            Boolean[] value = MobManager.ChunkTracker.getVoxelValue(new Point3D(Position.X, Math.Floor(Position.Y), Position.Z));
           
            OnMove(tpf, Direction, value);

        }

    }
}
