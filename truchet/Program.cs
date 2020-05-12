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
        static Parameters parameters;
        static Random random;
        static Tileset tileset;
        static List<Palette> palleteList;
        static void Main(string[] args)
        {
            parameters = new Parameters(args);
            random = new Random(parameters.Seed);
            palleteList = InitializePaletteList();
            tileset = new Tileset(parameters.TileSize, parameters.DivisionLevels, palleteList[parameters.Palette]);

            if (parameters.DisplayHelp)
            {
                PrintHelp();
                return;
            }

            int canvasWidth = (parameters.ColumnCount) * parameters.TileSize;
            int canvasHeight = (parameters.RowCount) * parameters.TileSize;

            if (!parameters.Borderless)
            {
                canvasWidth += parameters.TileSize;
                canvasHeight += parameters.TileSize;
            }

            double[,] noise = null;

            //block matrix for the tiles
            Tile[,] tileMatrix;
            if (parameters.Perlin)
            {
                noise = NoiseMap.GenerateNoiseMap(random, canvasWidth / 10, canvasHeight / 10, 2d, 1d, 3);
                tileMatrix = CreatePerlinTileMatrix(noise);
            }
            else
            {
                tileMatrix = CreatePseudorandomTileMatrix();
            }
             
            GenerateImage(canvasWidth, canvasHeight, tileMatrix);

            //debug
            if(parameters.Debug)
            {
                if (parameters.Perlin)
                {
                    GenerateNoiseImage(noise);
                }
                tileset.GenerateDebugImage();
            }
        }

        private static void GenerateImage(int canvasWidth, int canvasHeight, Tile[,] tileMatrix)
        {
            Image canvas = new Bitmap(canvasWidth, canvasHeight, PixelFormat.Format32bppArgb);
            Graphics grp = Graphics.FromImage(canvas);
            var tileQueue = new Queue<Tile>();
            foreach (Tile t in tileMatrix) tileQueue.Enqueue(t);
            for (int currentLevel = 0; currentLevel < parameters.DivisionLevels; currentLevel++)
            {

                var subTileQueue = new Queue<Tile>();

                /*ince the images are actually bigger, they need to be offset 
                 * by an increasing ammount
                the deeper we get into the division levels
                each level above 0 gains TileSize/(2^(level+1) offset
                offset is also used if borderlesss is on. */
                int offset = 0;

                if (parameters.Borderless) offset -= parameters.TileSize / 2;
                if (currentLevel > 0)
                {
                    int temp = parameters.TileSize / 4;
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
             canvas.Save(String.Format("truchet{0}.png", DateTime.Now.ToString("yyyy-MM-ddTHH-mm-ss")), ImageFormat.Png); 
        }

        /** FOR THE GENERATION WITHOUT PERLIN NOISE **/
        private static Tile[,] CreatePseudorandomTileMatrix()
        {
            Tile[,] t = new Tile[parameters.RowCount, parameters.ColumnCount];
            for (int x = 0; x < parameters.ColumnCount; x++)
            {
                for (int y = 0; y < parameters.RowCount; y++)
                {
                    t[y, x] = (GenerateRandomTile(1, x * parameters.TileSize, y * parameters.TileSize));
                }
            }
            return t;
        }

        private static Tile GenerateRandomTile(int level, int x, int y)
        {
            Tile t;
            if (level < parameters.DivisionLevels && random.Next(level + 1) == 0)
            {
                //NW; NE; SE; SW
                Tile[] subdivision = new Tile[4];
                int offset = parameters.TileSize / (Convert.ToInt32(Math.Pow(2, level)));
                subdivision[0] = GenerateRandomTile(level + 1, x, y);
                subdivision[1] = GenerateRandomTile(level + 1, x + offset, y);
                subdivision[2] = GenerateRandomTile(level + 1, x + offset, y + offset);
                subdivision[3] = GenerateRandomTile(level + 1, x, y + offset);
                t = new ContainerTile(x, y, level, subdivision);
            }
            else
            {
                //int randomType = RANDOM.Next(TILESET.tileCount-1);
                int randomType = 1 + random.Next(13);
                t = new GraphicTile(x, y, level, (TileType)randomType, tileset.GetTile(level - 1, randomType));
            }
            return t;
        }

        /** FOR THE GENERATION WITH PERLIN NOISE **/
        private static Tile[,] CreatePerlinTileMatrix(double[,] noise)
        {
            Tile[,] t = new Tile[parameters.RowCount, parameters.ColumnCount];
            for (int x = 0; x < parameters.ColumnCount; x++)
            {
                for (int y = 0; y < parameters.RowCount; y++)
                {
                    t[y, x] = GenerateRandomPerlinTile(1, x * parameters.TileSize, y * parameters.TileSize, noise);
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
            noiseValue += (random.NextDouble() - 0.5) / 5;

            bool isContainer = false;
            if (noiseValue < limit1 - ((level - 1) * (risinglimit))) isContainer = true;
            if (level < parameters.DivisionLevels && isContainer)
            {
                //NW; NE; SE; SW
                Tile[] subdivision = new Tile[4];
                int offset = parameters.TileSize / (Convert.ToInt32(Math.Pow(2, level)));
                subdivision[0] = GenerateRandomPerlinTile(level + 1, x, y, noise);
                subdivision[1] = GenerateRandomPerlinTile(level + 1, x + offset, y, noise);
                subdivision[2] = GenerateRandomPerlinTile(level + 1, x + offset, y + offset, noise);
                subdivision[3] = GenerateRandomPerlinTile(level + 1, x, y + offset, noise);
                t = new ContainerTile(x, y, level, subdivision);
            }
            else
            {
                //int randomType = RANDOM.Next(TILESET.tileCount-1);
                int randomType = 1 + random.Next(13);
                t = new GraphicTile(x, y, level, (TileType)randomType, tileset.GetTile(level - 1, randomType));
            }
            return t;
        }

        private static List<Palette> InitializePaletteList()
        {
            List<Palette> PaletteList = new List<Palette>
            {
                new SolidColorPalette(0xFFFFFF, 0x000000, "Monochrome"),
                new SolidColorPalette(0x05668D, 0xF0F3BD, "Sapphire"),
                new SolidColorPalette(0xE63946, 0x1D3557, "Imperial"),
                new SolidColorPalette(0x2D00F7, 0xE500A4, "Deep"),
                new SolidColorPalette(0xFFCDB2, 0x6D6875, "Apricot"),
                new SolidColorPalette(0x03071E, 0xFFBA08, "Xiketic"),
                new SolidColorPalette(0x3D315B, 0xF8F991, "Canary"),
                new SolidColorPalette(0x034732, 0xc1292e, "Meadow")
            };
            return PaletteList;
        }

        /** DEBUG **/

        private static void PrintHelp()
        {
            System.Console.Write(
                "Syntax: Truchet [-h] [-d] [-r] [-p] [-b]\n" +
                "                [--Palette id] [-l count] [-s seed]\n" +
                "                [-rc count] [-cc count] [-ts size]\n" +
                "\n" +
                "Options:\n" +
                "   -h              Displays this help screen.\n" +
                "   -d              Generates additional debug images. (default: off)\n" +
                "   -r              Sets generating method to random. (default: off)\n" +
                "   -p              Sets generating method to perlin noise.(default: on)\n" +
                "   -b              Turns on border cropping. (default: off)\n" +
                "   --Palette id    Specifies a palette. (default: Monochrome)\n" +
                "   -l count        Specifies the number of subdivision levels. (default: 3)\n" +
                "   -s seed         Specifies a seed. (default: random seed)\n" +
                "   -rc count       Specifies the amount of rows. (default: 10)\n" +
                "   -cc count       Specifies the amount of columns. (default: 10)\n" +
                "   -ts size        Specifies the tile size. (default: 300)\n" +
                "\n" +
                "The following palettes are available:\n");
            for(int i = 0; i < palleteList.Count; i++)
            {
                Console.WriteLine("   " + i + ": " + palleteList[i].Name);
            }
        }

        private static void GenerateNoiseImage(double[,] noiseMatrix)
        {
            int width = noiseMatrix.GetLength(0);
            int height = noiseMatrix.GetLength(1);
            Brush[] greyscale = new Brush[256];
            for (int i = 0; i < 256; i++)
            {
                int rgb = (((i << 8) + i) << 8) + i;
                greyscale[i] = new SolidBrush(Color.FromArgb((0xFF << 24) ^ rgb));
            }

            Image img = new Bitmap(width, height, PixelFormat.Format32bppArgb);
            Graphics g = Graphics.FromImage(img);
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    int xf = (int)Math.Floor(255 * noiseMatrix[x, y]);
                    g.FillRectangle(greyscale[xf], x, y, 1, 1);
                }
            }

            img.Save("noise.png", ImageFormat.Png);
        }
    }

    internal struct Parameters
    {
        public bool DisplayHelp { get; }
        public bool Borderless { get; }
        public bool Perlin { get; }
        public bool Debug { get; }
        public int TileSize { get; }
        public int RowCount { get; }
        public int ColumnCount { get; }
        public int DivisionLevels { get; }
        public int Seed { get; }
        public int Palette { get; }

        public Parameters(string[] args)
        {
            //standard parameters
            DisplayHelp = false;
            Debug = false;
            Borderless = false;
            Perlin = true;
            TileSize = 300;
            RowCount = 10;
            ColumnCount = 10;
            DivisionLevels = 3;
            Palette = 0;
            Seed = (int)DateTime.Now.TimeOfDay.TotalMilliseconds;

            for (int i = 0; i < args.Length; i++)
            {
                switch (args[i])
                {
                    //if -h is anywhere in the parameters, the rest of the params is skipped and help is displayed
                    case "-h":
                        DisplayHelp = true;
                        i = args.Length;
                        break;
                    case "-d":
                        Debug = true;
                        break;
                    case "--Palette":
                        Palette = int.Parse(args[++i]);
                        break;
                    case "-r":
                        Perlin = false;
                        break;
                    case "-p":
                        Perlin = true;
                        break;
                    case "-b":
                        Borderless = true;
                        break;
                    case "-rc":
                        RowCount = int.Parse(args[++i]);
                        break;
                    case "-cc":
                        ColumnCount = int.Parse(args[++i]);
                        break;
                    case "-ts":
                        TileSize = int.Parse(args[++i]);
                        break;
                    case "-l":
                        DivisionLevels = int.Parse(args[++i]);
                        break;
                    case "-s":
                        DivisionLevels = int.Parse(args[++i]);
                        break;
                }
            }
        }
    }
}
