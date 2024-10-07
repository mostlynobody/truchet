using System;
using System.Drawing;

namespace Truchet.Tiles
{

    abstract class Tile
    {
        public int X { get; }
        public int Y { get; }
        public int Level { get; }
        public bool IsContainer { get; }

        public Tile(int x, int y, int level, bool isContainer)
        {
            this.X = x;
            this.Y = y;
            this.Level = level;
            this.IsContainer = isContainer;
        }
    }

    class ContainerTile : Tile
    {
        //Clockwise from NW: NW, NE, SE, SW, 
        public Tile[] Container { get; }

        public ContainerTile(int x, int y, int level, Tile[] subdivison) 
            : base(x, y, level, true)
        {
            foreach (Tile t in subdivison) if (t == null) throw new Exception("container has to be filled");
            Container = subdivison;
        }
    }

    class GraphicTile : Tile
    {
        //Reference to the tileset img
        public Image Image { get; }
        public TileType Type { get; }
        public GraphicTile(int x, int y, int level, TileType type, Image image) 
            : base(x, y, level, false)
        {
            Type = type;
            Image = image;
        }
    }
}

