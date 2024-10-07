using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;

namespace Truchet.Tiles
{   

    class Tileset
    {

        readonly int levels;
        readonly int tileSize;
        readonly int[][] lookupTable;
        public readonly int tileCount;

        /* using a rectangular array because 
         * it's a neat c# feature to show 
         */
        readonly Image[,] tileArray;
        readonly Palette palette;


        //debug
        private const bool Smoothing = true;


        public Tileset(int tileSize, int levels, Palette palette)
        {
            this.tileSize = tileSize;
            this.levels = levels;
            this.palette = palette;
            tileCount = Enum.GetNames(typeof(TileType)).Length;

           
            tileArray = InitializeTileset();
            //lookupTable = GenerateLookupTable();
        }

        public Image GetTile(int level, int index)
        {   
            return tileArray[level, index];
        }

        public void GenerateDebugImage()
        {
            int width = tileSize * 2 * tileCount;
            int height = 0;
            int f = tileSize * 2;
            //calculate debug image height
            for (int i = 0; i < levels; i++) {
                height += f;
                f /= 2;
            }

            Image image = new Bitmap(width, height, PixelFormat.Format32bppArgb);
            Graphics graphics = Graphics.FromImage(image);
            int x = 0, y = 0, mod = tileSize * 2;
            for(int i = 0; i < levels; i++) {
                for (int j = 0; j < tileCount; j++)
                {
                    graphics.DrawImage(tileArray[i, j], x, y);
                    x += mod;
                }
                y += mod;
                mod /= 2;
                x = 0;
            }
            image.Save("tileset_debug.png", ImageFormat.Png);
        }


        private Image[,] InitializeTileset()
        {
            Image[,] tileArray = new Image[levels, tileCount];
            int currentTileSize = tileSize;
            var tiles = Enum.GetValues(typeof(TileType));
            Brush primary = palette.PrimaryBrush;
            Brush secondary = palette.SecondaryBrush;

            for (int currentLevel = 0; currentLevel < levels; currentLevel++)
            {
                int i = 0;
                foreach(TileType tile in tiles)
                {
                    tileArray[currentLevel, i++] = DrawTileImage(tile, currentTileSize, primary, secondary);
                }

                currentTileSize /= 2;
                //switch colors at each subdivision step 
                var temp = primary;
                primary = secondary;
                secondary = temp;
            }

            return tileArray;
        }


        private int[][] GenerateLookupTable()
        {
            var directions = Enum.GetValues(typeof(Direction));
            var tiles = Enum.GetValues(typeof(TileType));
            int[][] table = new int[directions.Length][];
            int i = 0;
            foreach (Direction dir in directions)
            {
                int count = 0;
                foreach (TileType tile in tiles)
                {
                    if (((int)tile & (int)dir) != 0) count++;
                }
                table[i] = new int[count];
                count = 0;
                foreach (TileType tile in tiles)
                {
                    if (((int)tile & (int)dir) != 0) table[i][count++] = (int)tile;
                }
                i++;
            }
            return table;
        }

        /* function to draw all the tiles
         * This is seperate for readability and debug reasons
         * I used to just draw the necessary ones and copy and rotate the rest
           but that turned out to be a terrible idea because of floating point
           pixel-level imperfectness */
        private Image DrawTileImage(TileType type, int tileSize, Brush primary, Brush secondary)
        {
            Image i = GetEmptyImage(tileSize * 2);
            Graphics g = GetGraphicsFromImage(i);

            //in case it's a gradient brush, we have to rotate the gradient for some tiles.
            if(palette.GetType().Equals(typeof(LinearGradientPalette)))
            {
                float rotation;
                float scale = (float)tileSize / (float)LinearGradientPalette.GradientBrushSize;

                switch(type)
                {
                    case TileType.Empty:
                    case TileType.Horizontal:
                    case TileType.T_E:
                    case TileType.T_W:
                        rotation = 0;
                        break;

                    case TileType.T_S:
                    case TileType.T_N:
                    case TileType.Vertical:
                    case TileType.Cross:
                        rotation = 90;
                        break;

                    case TileType.Forwardslash:
                    case TileType.Frown_NW:
                    case TileType.Frown_SE:
                        rotation = 45;
                        break;

                    case TileType.Backslash:
                    case TileType.Frown_NE:
                    case TileType.Frown_SW:
                        rotation = 135;
                        break;

                    default:
                        throw new Exception("Not a valid TileType");
                }
                var castPalette = (LinearGradientPalette) palette;
                castPalette.Transform(scale, rotation);
            }
            FillWhiteCube(g, tileSize, primary);

            switch (type)
            {

                case TileType.Empty:
                    break;

                case TileType.Vertical:
                    FillVerticalBlackQuad(g, tileSize, secondary);
                    break;

                case TileType.Horizontal:
                    FillHorizontalBlackQuad(g, tileSize, secondary);
                    break;

                case TileType.Cross:
                    bool[] crossBool = { true, true, true, true };
                    FillCornerBlackPies(g, tileSize, crossBool, secondary);
                    FillVerticalBlackQuad(g, tileSize, secondary);
                    break;

                case TileType.Forwardslash:
                    bool[] fwsBool = { true, false, true, false };
                    FillCornerBlackPies(g, tileSize, fwsBool, secondary);
                    break;

                case TileType.Backslash:
                    bool[] bsBool = { false, true, false, true };
                    FillCornerBlackPies(g, tileSize, bsBool, secondary);
                    break;

                case TileType.Frown_NW:
                    bool[] fnwBool = { true, false, false, false };
                    FillCornerBlackPies(g, tileSize, fnwBool, secondary);
                    break;

                case TileType.Frown_NE:
                    bool[] fneBool = { false, true, false, false };
                    FillCornerBlackPies(g, tileSize, fneBool, secondary);
                    break;

                case TileType.Frown_SW:
                    bool[] fswBool = { false, false, true, false };
                    FillCornerBlackPies(g, tileSize, fswBool, secondary);
                    break;

                case TileType.Frown_SE:
                    bool[] fseBool = { false, false, false, true };
                    FillCornerBlackPies(g, tileSize, fseBool, secondary);
                    break;

                case TileType.T_N:
                    bool[] tn_Bool = { true, true, false, false };
                    FillCornerBlackPies(g, tileSize, tn_Bool, secondary);
                    FillHorizontalBlackQuad(g, tileSize, secondary);
                    break;

                case TileType.T_E:
                    bool[] teBool = { false, true, true, false };
                    FillCornerBlackPies(g, tileSize, teBool, secondary);
                    FillVerticalBlackQuad(g, tileSize, secondary);
                    break;

                case TileType.T_S:
                    bool[] ts_Bool = { false, false, true, true };
                    FillCornerBlackPies(g, tileSize, ts_Bool, secondary);
                    FillHorizontalBlackQuad(g, tileSize, secondary);
                    break;

                case TileType.T_W:
                    bool[] twBool = { true, false, false, true };
                    FillCornerBlackPies(g, tileSize, twBool, secondary);
                    FillVerticalBlackQuad(g, tileSize, secondary);
                    break;

                default:
                    throw new Exception("Not a valid TileType");
            }
            FillCornerWhiteCircles(g, tileSize, primary);
            FillMiddleBlackCircles(g, tileSize, secondary);
            return i;
        }

        /* helper functions to draw the tiles 
            these functions will not make much sense without understanding
            the actual process the tiles are drawn.
             */

        //fills the white cube in the middle
        private static void FillWhiteCube(Graphics g, int tileSize, Brush primary)
        {
            g.FillRectangle(primary, tileSize / 2, tileSize / 2, tileSize, tileSize);
        }

        private static void FillVerticalBlackQuad(Graphics g, int tileSize, Brush secondary)
        {
            int a = tileSize / 2,  b = tileSize / 3;
            g.FillRectangle(secondary, a + b, a, b, tileSize);
        }

        private static void FillHorizontalBlackQuad(Graphics g, int tileSize, Brush secondary)
        {
            int a = tileSize / 2, b = tileSize / 3;
            g.FillRectangle(secondary, a, a + b, tileSize, b);
        }

        //fills the black pies in the corners
        //like everything else in this project, the boolean array represents: NW, NE, SE, SW
        private static void FillCornerBlackPies(Graphics g, int tileSize, bool[] corners, Brush secondary)
        {
            //get the upper upper left left corner, and then for the other corners we just add 1 tile length
            int corner = -tileSize / 6;
            int diameter = (tileSize / 3) * 4;
            //putting this into a loop would take longer than it's worth
            if (corners[0]) g.FillPie(secondary, corner, corner, diameter, diameter, 0, 90);
            if (corners[1]) g.FillPie(secondary, corner + tileSize, corner, diameter, diameter, 90, 90);
            if (corners[2]) g.FillPie(secondary, corner + tileSize, corner + tileSize, diameter, diameter, 180, 90);
            if (corners[3]) g.FillPie(secondary, corner, corner + tileSize, diameter, diameter, 270, 90);
  
        }

        private static void FillCornerWhiteCircles(Graphics g, int tileSize, Brush primary)
        {
            //get the upper upper left left corner, and then for the other corners we just add 1 tile length
            int corner = tileSize / 6;
            int diameter = (tileSize / 3) * 2;
            g.FillEllipse(primary, corner, corner, diameter, diameter);
            g.FillEllipse(primary, corner, corner + tileSize, diameter, diameter);
            g.FillEllipse(primary, corner + tileSize, corner, diameter, diameter);
            g.FillEllipse(primary, corner + tileSize, corner + tileSize, diameter, diameter);
        }

        private static void FillMiddleBlackCircles(Graphics g, int tileSize, Brush secondary)
        {
            //these circles need a bit more math, but just a bit
            int diameter = (tileSize / 3);
            int a = (tileSize / 2) + (tileSize / 3);
            int b = (tileSize / 3);
            int c = (tileSize * 4) / 3;
            //upper and lower small black circles
            g.FillEllipse(secondary, a, b, diameter, diameter);
            g.FillEllipse(secondary, a, c, diameter, diameter);
            //left and right small black circles
            g.FillEllipse(secondary, b, a, diameter, diameter);
            g.FillEllipse(secondary, c, a, diameter, diameter);
        }

        private static Image GetEmptyImage(int size)
        {
            return new Bitmap(size, size, PixelFormat.Format32bppArgb);
        }

        //used so smoothingMode doesn't have to be manually set every time
        private static Graphics GetGraphicsFromImage(Image i)
        {
            var g = Graphics.FromImage(i);
            if(Smoothing) g.SmoothingMode = SmoothingMode.AntiAlias;
            return g;
        }
    }

    public enum Direction
    {           // NESW
        None  = 0b_0000,
        North = 0b_0001,
        East  = 0b_0010,
        South = 0b_0100,
        West  = 0b_1000
    }

    public enum TileType
    {
        Empty           = (0  << 4) | Direction.None,
        Vertical        = (1  << 4) | Direction.North | Direction.South,
        Horizontal      = (2  << 4) | Direction.East  | Direction.West,
        Cross           = (3  << 4) | Direction.North | Direction.East  | Direction.South | Direction.West,
        Forwardslash    = (4  << 4) | Direction.North | Direction.East  | Direction.South | Direction.West,
        Backslash       = (5  << 4) | Direction.North | Direction.East  | Direction.South | Direction.West,
        Frown_NW        = (6  << 4) | Direction.North | Direction.West, 
        Frown_NE        = (7  << 4) | Direction.North | Direction.East, 
        Frown_SE        = (8  << 4) | Direction.South | Direction.East, 
        Frown_SW        = (9  << 4) | Direction.South | Direction.West, 
        T_N             = (10 << 4) | Direction.North | Direction.East  | Direction.West,
        T_E             = (11 << 4) | Direction.North | Direction.East  | Direction.South,
        T_S             = (12 << 4) | Direction.East  | Direction.South | Direction.West,
        T_W             = (13 << 4) | Direction.North | Direction.South | Direction.West
    }
}
