using System;
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
        private const int TILE_SIZE = 300;
        private const int TILE_ROWS = 20;
        private const int TILE_COLUMNS = 20;

        private const int DIVISION_LEVELS = 3;

        private const int SEED = 465622;

        private const bool BORDERLESS = true;


        private static readonly Tileset TILESET = new Tileset(TILE_SIZE, DIVISION_LEVELS, Palette.Xiketic);
        private static readonly Random RANDOM = new Random();


        static void Main()
        {
            // initialize all variables that are not done at compile time;
            int canvasWidth = (TILE_COLUMNS) * TILE_SIZE;
            int canvasHeight = (TILE_ROWS) * TILE_SIZE;

            if (!BORDERLESS)
            {
                canvasWidth += TILE_SIZE;
                canvasHeight += TILE_SIZE;
            }

            

            var noise = NoiseMap.GenerateNoiseMap(RANDOM, canvasWidth / 10, canvasHeight / 10, 2d, 1d, 3);

            GenerateNoiseImage(noise);
            TILESET.GenerateDebugImage();


            //block matrix for the tiles
            Tile[,] tileMatrix = CreatePerlinTileMatrix(noise);
            //tileMatrix = createPseudorandomTileMatrix();

            GenerateImage(canvasWidth, canvasHeight, tileMatrix);


        }

        private static void GenerateImage(int canvasWidth, int canvasHeight, Tile[,] tileMatrix)
        {
            Image canvas = new Bitmap(canvasWidth, canvasHeight, PixelFormat.Format32bppArgb);
            Graphics grp = Graphics.FromImage(canvas);
            var tileQueue = new Queue<Tile>();
            foreach (Tile t in tileMatrix) tileQueue.Enqueue(t);
            for (int currentLevel = 0; currentLevel < DIVISION_LEVELS; currentLevel++)
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
                    if (t.IsContainer)
                    {
                        var ct = (ContainerTile)t;
                        foreach (Tile child in ct.Container)
                        {
                            subTileQueue.Enqueue(child);
                        }
                    }
                    else
                    {
                        var gt = (GraphicTile)t;
                        grp.DrawImage(gt.Image, gt.X + offset, gt.Y + offset);
                    }
                }
                tileQueue = subTileQueue;
            }


            canvas.Save("test.png", ImageFormat.Png);
        }

        // COMPLETELY RANDOM TILE MATRIX, WITHOUT ANY PERPLIN NOISE
        private static Tile[,] CreatePseudorandomTileMatrix()
        {
            Tile[,] t = new Tile[TILE_ROWS, TILE_COLUMNS];
            for (int x = 0; x < TILE_COLUMNS; x++)
            {
                for (int y = 0; y < TILE_ROWS; y++)
                {
                    t[y, x] = (GenerateRandomTile(1, x * TILE_SIZE, y * TILE_SIZE));
                }
            }
            return t;
        }


        private static Tile GenerateRandomTile(int level, int x, int y)
        {
            Tile t;
            if (level < DIVISION_LEVELS && RANDOM.Next(level + 1) == 0)
            {
                //NW; NE; SE; SW
                Tile[] subdivision = new Tile[4];
                int offset = TILE_SIZE / (Convert.ToInt32(Math.Pow(2, level)));
                subdivision[0] = GenerateRandomTile(level + 1, x, y);
                subdivision[1] = GenerateRandomTile(level + 1, x + offset, y);
                subdivision[2] = GenerateRandomTile(level + 1, x + offset, y + offset);
                subdivision[3] = GenerateRandomTile(level + 1, x, y + offset);
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

        /** FOR THE GENERATION WITH PERLIN NOISE **/
        private static Tile[,] CreatePerlinTileMatrix(double[,] noise)
        {
            Tile[,] t = new Tile[TILE_ROWS, TILE_COLUMNS];
            for (int x = 0; x < TILE_COLUMNS; x++)
            {
                for (int y = 0; y < TILE_ROWS; y++)
                {
                    t[y, x] = GenerateRandomPerlinTile(1, x * TILE_SIZE, y * TILE_SIZE, noise);
                }
            }
            return t;
        }

        private static Tile GenerateRandomPerlinTile(int level, int x, int y, double[,] noise)
        {
            Tile t;
            double noiseValue = noise[x / 10, y / 10];

            //DEBUG
            double limit1 = 0.5d;
            double risinglimit = 0.05d;
            //LIL BIT OF RANDOMNESS
            noiseValue += (RANDOM.NextDouble() - 0.5) / 5;

            bool isContainer = false;
            if (noiseValue < limit1 - ((level - 1) * (risinglimit))) isContainer = true;
            if (level < DIVISION_LEVELS && isContainer)
            {
                //NW; NE; SE; SW
                Tile[] subdivision = new Tile[4];
                int offset = TILE_SIZE / (Convert.ToInt32(Math.Pow(2, level)));
                subdivision[0] = GenerateRandomPerlinTile(level + 1, x, y, noise);
                subdivision[1] = GenerateRandomPerlinTile(level + 1, x + offset, y, noise);
                subdivision[2] = GenerateRandomPerlinTile(level + 1, x + offset, y + offset, noise);
                subdivision[3] = GenerateRandomPerlinTile(level + 1, x, y + offset, noise);
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

        private static Tile[,] CreateEmptyPerlinTileMatrix(double[,] noise)
        {
            Tile[,] t = new Tile[TILE_ROWS, TILE_COLUMNS];
            for (int x = 0; x < TILE_COLUMNS; x++)
            {
                for (int y = 0; y < TILE_ROWS; y++)
                {
                    t[y, x] = FillPerlinContainerTile(1, x * TILE_SIZE, y * TILE_SIZE, noise);
                }
            }
            return t;
        }

        private static Tile FillPerlinContainerTile(int level, int x, int y, double[,] noise)
        {
            Tile t;
            double noiseValue = noise[x / 10, y / 10];

            //DEBUG
            double limit1 = 0.5d;
            double risinglimit = 0.05d;
            //LIL BIT OF RANDOMNESS
            noiseValue += (RANDOM.NextDouble() - 0.5) / 5;

            bool isContainer = false;
            if (noiseValue < limit1 - ((level - 1) * (risinglimit))) isContainer = true;
            if (level < DIVISION_LEVELS && isContainer)
            {
                //NW; NE; SE; SW
                Tile[] subdivision = new Tile[4];
                int offset = TILE_SIZE / (Convert.ToInt32(Math.Pow(2, level)));
                subdivision[0] = GenerateRandomPerlinTile(level + 1, x, y, noise);
                subdivision[1] = GenerateRandomPerlinTile(level + 1, x + offset, y, noise);
                subdivision[2] = GenerateRandomPerlinTile(level + 1, x + offset, y + offset, noise);
                subdivision[3] = GenerateRandomPerlinTile(level + 1, x, y + offset, noise);
                t = new ContainerTile(x, y, level, subdivision);
            }
            else
            {
                t = null;
            }
            return t;
        }

        /** DEBUG **/

        private static void GenerateNoiseImage(double[,] noiseMatrix)
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
