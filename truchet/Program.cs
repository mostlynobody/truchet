using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using Truchet.Tiles;
using Truchet.Perlin;

namespace Truchet
{
    class Program
    {
        //compile time variables
        private const int TILE_SIZE = 200;
        private const int TILE_ROWS = 20;
        private const int TILE_COLUMNS = 40;

        private const int DIVISION_LEVELS = 3;

        private const int SEED = 234243;

        private const int PRIMARY = 0xA6C36F;
        private const int SECONDARY = 0x335145;

        private const bool BORDERLESS = true;


        private static readonly Tileset TILESET = new Tileset(TILE_SIZE, DIVISION_LEVELS, PRIMARY, SECONDARY);
        private static readonly Random RANDOM = new Random(SEED);



        

        static void Main(string[] args)
        {
            // initialize all variables that are not done at compile time;
            int canvas_width = (TILE_COLUMNS) * TILE_SIZE;
            int canvas_height = (TILE_ROWS) * TILE_SIZE;

            if(!BORDERLESS)
            {
                canvas_width += TILE_SIZE;
                canvas_height += TILE_SIZE;
            }


            Image canvas = new Bitmap(canvas_width, canvas_height, PixelFormat.Format32bppArgb);
            Graphics grp = Graphics.FromImage(canvas);

            var noise = NoiseMap.GenerateNoiseMap(RANDOM, canvas_width / 10, canvas_height / 10, 2d, 1d, 3);

            generateNoiseDebugImage(noise);
            TILESET.GenerateDebugImage();

           
            //block matrix for the tiles
            Tile[,] tileMatrix = createPerlinTileMatrix(noise);

            var tileQueue = new Queue<Tile>();
            foreach (Tile t in tileMatrix) tileQueue.Enqueue(t);
             
            for (int currentLevel = 0; currentLevel < DIVISION_LEVELS; currentLevel ++)
            {

                var subTileQueue = new Queue<Tile>();

                /*ince the images are actually bigger, they need to be offset 
                 * by an increasing ammount
                the deeper we get into the division levels
                each level above 0 gains TileSize/(2^(level+1) offset
                offset is also used if borderlesss is on. */
                int offset = 0;

                if (BORDERLESS) offset -= TILE_SIZE / 2;
                if (currentLevel > 0)
                {
                    int temp = TILE_SIZE / 4;
                    offset += temp;
                    for (int pow = 1; pow < currentLevel; pow++)
                    {
                        temp /= 2;
                        offset += temp;
                    }
                }
 

                foreach (Tile t in tileQueue)
                {
                    if(t.type == TileType.Container) {
                        var ct = (ContainerTile)t;
                        foreach(Tile child in ct.container)
                        {
                            subTileQueue.Enqueue(child);
                        }
                    }
                    else
                    {
                        var gt = (GraphicTile)t;
                        grp.DrawImage(gt.image, gt.x+offset, gt.y+offset);
                    }
                }
                tileQueue = subTileQueue;
            }
            

            canvas.Save("test.png", ImageFormat.Png);
        }

        private static Tile[,] createPseudorandomTileMatrix()
        {
            Tile[,] t = new Tile[TILE_ROWS, TILE_COLUMNS];
            for(int x = 0; x < TILE_COLUMNS; x++)
            {
                for (int y = 0; y < TILE_ROWS; y++)
                {
                    t[y,x] = (generateRandomTile(1, x*TILE_SIZE, y*TILE_SIZE));
                }
            }
            return t;
        }

        private static Tile[,] createPerlinTileMatrix(double [,] noise)
        {
            Tile[,] t = new Tile[TILE_ROWS, TILE_COLUMNS];
            for (int x = 0; x < TILE_COLUMNS; x++)
            {
                for (int y = 0; y < TILE_ROWS; y++)
                {
                    t[y, x] = generateRandomPerlinTile(1, x * TILE_SIZE, y * TILE_SIZE, noise);
                }
            }
            return t;
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
                //int randomType = RANDOM.Next(TILESET.tileCount-1);
                int randomType = 1 + RANDOM.Next(13);
                t = new GraphicTile(x, y, level, (TileType)randomType, TILESET.GetTile(level-1, randomType));
            }
            return t;
        }

        private static Tile generateRandomPerlinTile(int level, int x, int y, double[,] noise)
        {
            Tile t;
            //NOISE MAL ZEHN DU WEI?T DEI NOISE IS UM EIN FAKTOR 10 KLEINER ALS DIE EIGENTLICHE DINGS ALSO DU WEIßt WAS ICH MEINE OK
            // OK DANN IST JA GUT :)))
            double noiseValue = noise[x / 10, y / 10];

            //DEBUG
            double limit1 = 0.5d;
            double risinglimit = 0.10;
            //LIL BIT OF RANDOMNESS
            noiseValue += (RANDOM.NextDouble() - 0.5) / 3;

            bool isContainer = false;
            if (noiseValue < limit1 - ((level-1) * (risinglimit))) isContainer = true;
            if (level < DIVISION_LEVELS && isContainer)
            {   
                //NW; NE; SE; SW
                Tile[] subdivision = new Tile[4];
                int offset = TILE_SIZE / (Convert.ToInt32(Math.Pow(2, level)));
                subdivision[0] = generateRandomPerlinTile(level + 1, x, y, noise);
                subdivision[1] = generateRandomPerlinTile(level + 1, x + offset, y, noise);
                subdivision[2] = generateRandomPerlinTile(level + 1, x + offset, y + offset, noise);
                subdivision[3] = generateRandomPerlinTile(level + 1, x, y + offset, noise);
                t = new ContainerTile(x, y, level, subdivision);
            }
            else
            {
                //int randomType = RANDOM.Next(TILESET.tileCount-1);
                int randomType = 1 + RANDOM.Next(13);
                t = new GraphicTile(x, y, level, (TileType)randomType, TILESET.GetTile(level - 1, randomType));
            }
            return t;
        }



        private static void generateNoiseDebugImage(double[,] noiseMatrix)
        {
            int width = noiseMatrix.GetLength(0);
            int height = noiseMatrix.GetLength(1);
            Brush[] greyscale = new Brush[256];
            for(int i = 0; i < 256; i++)
            {
                int rgb = (((i << 8) + i) << 8) + i;
                greyscale[i] = new SolidBrush(Color.FromArgb((0xFF << 24) ^ rgb));
            }

            Image img = new Bitmap(width, height, PixelFormat.Format32bppArgb);
            Graphics g = Graphics.FromImage(img);
            for(int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    int xf = (int)Math.Floor(255 * noiseMatrix[x, y]);
                    g.FillRectangle(greyscale[xf], x, y, 1, 1);
                }
            }

            img.Save("perlindebug.png", ImageFormat.Png);
        }
    }


}
