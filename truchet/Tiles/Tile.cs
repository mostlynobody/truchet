using System;
using System.Drawing;

namespace Truchet.Tiles
{

    abstract class Tile
    {
        public int x { get; }
        public int y { get; }
        public int level { get; }
        public bool isContainer { get; }

        public Tile(int x, int y, int level, bool isContainer)
        {
            this.x = x;
            this.y = y;
            this.level = level;
            this.isContainer = isContainer;
        }
    }

    class ContainerTile : Tile
    {
        //Clockwise from NW: NW, NE, SE, SW, 
        public Tile[] container { get; }

        public ContainerTile(int x, int y, int level, Tile[] subdivison) 
            : base(x, y, level, true)
        {
            foreach (Tile t in subdivison) if (t == null) throw new Exception("container has to be filled");
            container = subdivison;
        }
    }

    class GraphicTile : Tile
    {
        //Reference to the tileset img
        public Image image { get; }
        public TileType type { get; }
        public GraphicTile(int x, int y, int level, TileType type, Image image) 
            : base(x, y, level, false)
        {
            this.type = type;
            this.image = image;
        }
    }
}
