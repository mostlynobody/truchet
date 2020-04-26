using System;
using System.Drawing;

namespace Truchet
{

    abstract class Tile
    {
        public int x { get; }
        public int y { get; }
        public int level { get; }
        public TileType type { get; }

        public Tile(int x, int y, int level, TileType type)
        {
            this.x = x;
            this.y = y;
            this.level = level;
            this.type = type;    
        }
    }

    class ContainerTile : Tile
    {
        //Clockwise from NE: NE, SE, SW, NW
        public Tile[] container { get; }

        public ContainerTile(int x, int y, int level) 
            : base(x, y, level, TileType.Container)
        {
            container = new Tile[4];
        }
    }

    class GraphicTile : Tile
    {
        //Reference to the tileset img
        public Image image { get; }
        public GraphicTile(int x, int y, int level, TileType type, Image image) 
            : base(x, y, level, type)
        {
            if(type == TileType.Container)
            {
                throw new System.Exception("GraphicTile cannot have TileType Container");
            }
            this.image = image;
        }
    }
}
