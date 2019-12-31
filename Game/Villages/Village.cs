using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using mapGen;

namespace Game
{
    

    public abstract class Village
    {
        public Point CenterPosition { get; }
        public IslandMap IslandMap { get; }

        public List<Structure> StructureList { get; } = new List<Structure>();

        public int SX { get; }
        public int SY { get; }

        public Village(Point Position, IslandMap Map, int sx, int sy)
        {
            CenterPosition = Position;
            IslandMap = Map;
            SX = sx;
            SY = sy;
        }

        public abstract void GenerateLayout();

    }
}
