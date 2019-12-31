using mapGen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Media3D;
using System.Diagnostics;

namespace Game
{
    public class Duck : Mob
    {
        public Duck(Point3D position, int entityID) : base(position, entityID)
        {
            ModelID = 0;
            Aggression = 1;
            Awareness = 1;
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
            if (MathUtil.MathUtil.distance(PlayerPosition, Position) <= 64)
            {
                if (MathUtil.MathUtil.distance(PlayerPosition, Position) >=4)
                {
                    
                        Vector tempVec = new Vector(PlayerPosition.X - Position.X, PlayerPosition.Z - Position.Z);
                        tempVec.Normalize();
                        if (Double.IsNaN(tempVec.X))
                        {
                            tempVec.X = 0;
                        }
                        if (Double.IsNaN(tempVec.Y))
                        {
                            tempVec.Y = 0;
                        }

                        Direction = new Vector3D(tempVec.X, 0, tempVec.Y);
                    
                }
            }
            /*
            else if (MathUtil.MathUtil.distance(PlayerPosition, Position) <= 100)
            {
                if (MathUtil.MathUtil.distance(PlayerPosition, Position) >= 2)
                {
                    Vector tempVec = new Vector(Position.X - PlayerPosition.X, Position.Z - PlayerPosition.Z);
                    tempVec.Normalize();

                    Direction = new Vector3D(tempVec.X, 0, tempVec.Y);
                }
            }
            */
            //Debug.WriteLine(Position);

            Boolean[] value = MobManager.ChunkTracker.getVoxelValue(new Point3D(Position.X, Math.Floor(Position.Y), Position.Z));

            OnMove(tpf, Direction, value);

        }
        
        /*
        public override void CallAI(float tpf, MobManager MobManager)
        {
            //stays within aggression * 10 WU from water

            IslandMap Map = MobManager.ChunkTracker.Map;

            Vector3D Direction = new Vector3D();
            Point3D PlayerPosition = MobManager.Player.ModelPosition;
            double playerdistance = MathUtil.MathUtil.distance(PlayerPosition, Position);

            switch (Aggression)
            {
                case 1: case 2:
                    //flees from: all mobs except own kind and player
                    //will flee to center of lake
                    if (MathUtil.MathUtil.distance(PlayerPosition, Position) <= 16)
                    {
                        Point LakePosition = MapUtil.getClosest((int)Position.X, (int)Position.Z, Map.lk,64);
                        if (LakePosition.X != -1)
                        {
                            Vector tempVec = new Vector(LakePosition.X - Position.X, LakePosition.Y - Position.Z);
                            //tempVec.Normalize();
                            Direction = new Vector3D(tempVec.X, 0, tempVec.Y);
                        }
                        else
                        {
                            Vector tempVec = new Vector(PlayerPosition.X - Position.X, PlayerPosition.Z - Position.Z);
                            //tempVec.Normalize();
                            Direction = new Vector3D();
                        }
                    }

                    break;
                case 3: case 4:
                //flees from: mobs > 2 aggression
                //will flee to water if player is within awareness * distance
                case 5: case 6:
                //flees from: mobs > 6 aggression

                    break;
                case 7: case 8:
                //

                    break;
                case 9: case 10:
                //will attack player and any mob without weapon

                    break;

            }

            Boolean[] value = MobManager.ChunkTracker.getVoxelValue(new Point3D(Position.X, Math.Floor(Position.Y), Position.Z));

            OnMove(tpf, Direction, value);

        }
        */

    }
}
