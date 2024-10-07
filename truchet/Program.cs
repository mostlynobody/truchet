using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;

namespace Truchet
{
    class Program
    {

        private static readonly int TILE_SIZE = 120;
        private static readonly int TILE_ROWS = 100;
        private static readonly int TILE_COLUMNS = 200;

        private static readonly int DIVISION_LEVELS = 2;

        private static readonly int SEED = 123019491;
        private static readonly int PRIMARY = 0xFFFFFF;
        private static readonly int SECONDARY = 0x000000;
        private static readonly Tileset TILESET = new Tileset(TILE_SIZE, DIVISION_LEVELS, PRIMARY, SECONDARY);
        private static readonly Random RANDOM = new Random(1234);

        private static readonly bool BORDERLESS = false;

        static void Main(string[] args)
        {

            int canvas_width = (TILE_COLUMNS) * TILE_SIZE;
            int canvas_height = (TILE_ROWS) * TILE_SIZE;
            if(!BORDERLESS)
            {
                canvas_width += TILE_SIZE;
                canvas_height += TILE_SIZE;
            }

            Image canvas = new Bitmap(canvas_width, canvas_height, PixelFormat.Format32bppArgb);
            Graphics grp = Graphics.FromImage(canvas);

            //block matrix for the tiles
            //var tiles = new Queue<Tile>();
            var tiles = createRandomDebugQueue();

            //MAIN LOOP FOR DRAWING


            TILESET.GenerateDebugImage();
            for (int currentLevel = 0; currentLevel < DIVISION_LEVELS; currentLevel ++)
            {
                var subTiles = new Queue<Tile>();

                //since the images are actually bigger, they need to be offset by an increasing ammount
                //the deeper we get into the division levels
                //each level above 0 gains TileSize/(2^(level+1) offset
                //  if (currentLevel == 1) offset = TILE_SIZE / 4;
                // else if (currentLevel > 1) offset +=  2;
                //offset is also used if borderlesss is on.
                int offset = 0;
                if (BORDERLESS) offset -= TILE_SIZE / 2;
                if (currentLevel > 0)
                {
                    offset += TILE_SIZE / 4;
                    int temp = offset;
                    for (int pow = 1; pow < currentLevel; pow++)
                    {
                        temp /= 2;
                        offset += temp;
                    }
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
                        grp.DrawImage(gt.image, gt.x+offset, gt.y+offset);
                    }
                }
                tiles = subTiles;
            }
            

            canvas.Save("test.png", ImageFormat.Png);
        }

        private static Queue<Tile> createRandomDebugQueue()
        {
            Queue<Tile> q = new Queue<Tile>();
            for(int x = 0; x < TILE_COLUMNS; x++)
            {
                for (int y = 0; y < TILE_ROWS; y++)
                {
                    q.Enqueue(generateRandomTile(1, x*TILE_SIZE, y*TILE_SIZE));
                }
            }
            return q;
        }

        private static Tile generateRandomTile(int level, int x, int y)
        {
            Tile t;
            if (level < DIVISION_LEVELS && RANDOM.Next(level+1) == 0)
            {
                //NW; NE; SE; SW
                Tile[] subdivision = new Tile[4];
                int offset = TILE_SIZE / (Convert.ToInt32(Math.Pow(2, level)));
                subdivision[0] = generateRandomTile(level + 1, x, y);
                subdivision[1] = generateRandomTile(level + 1, x + offset, y);
                subdivision[2] = generateRandomTile(level + 1, x + offset, y + offset);
                subdivision[3] = generateRandomTile(level + 1, x, y + offset);
                t = new ContainerTile(x, y, level, subdivision);
            }
            else
            {
                int randomType = RANDOM.Next(14);
                t = new GraphicTile(x, y, level, (TileType)randomType, TILESET.GetTile(level-1, randomType));
            }
            return t;
        }
    }


}
