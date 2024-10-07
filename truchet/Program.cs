using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;

namespace Truchet
{
    class Program
    {

        private static readonly int TILE_SIZE = 360;
        private static readonly int TILE_ROWS = 10;
        private static readonly int TILE_COLUMNS = 10;

        private static readonly int DIVISION_LEVELS = 1;

        static void Main(string[] args)
        {
            int canvas_width = (TILE_COLUMNS + 1) * TILE_SIZE;
            int canvas_height = (TILE_ROWS + 1) * TILE_SIZE;

            Image canvas = new Bitmap(canvas_width, canvas_height, PixelFormat.Format32bppArgb);
            Graphics grp = Graphics.FromImage(canvas);

            //block matrix for the tiles
            var tiles = new Queue<Tile>();

            //MAIN LOOP FOR DRAWING
            int offset = 0;


            Tileset tileset = new Tileset(TILE_SIZE, DIVISION_LEVELS, 0xFFFFFF, 0x000000);

            /*
            for (int currentLevel = 0; currentLevel < SUBDIVISION_LEVELS; currentLevel ++)
            {
                var subTiles = new Queue<Tile>();
               
                //since the images are actually bigger, they need to be offset by an increasing ammount
                //the deeper we get into the subdivision levels
                //each level above 0 gains TileSize/(2^(level+1) offset
                            
                if(currentLevel == 1)
                {
                    offset = TILE_SIZE / 4;
                }
                else if(currentLevel > 1)
                {
                    offset /= 2;
                }

                foreach (Tile t in tiles)
                {
                    if(t.type == TileType.Container) {
                        var ct = (ContainerTile)t;
                        foreach(Tile child in ct.container)
                        {
                            subTiles.Enqueue(child);
                        }
                    }
                    else
                    {
                        var gt = (GraphicTile)t;
                        grp.DrawImage(gt.image, gt.x, gt.y);
                    }
                }
                tiles = subTiles;
            }
            */

            //canvas.Save("test.png", ImageFormat.Png);
        }
    }
}
