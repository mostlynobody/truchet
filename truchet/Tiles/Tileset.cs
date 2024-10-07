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
        //how many different tiles there are, minus the container tile
        public readonly int tileCount;

        /* using a rectangular array because 
         * it's a neat c# feature to show 
         */
        Image[,] tileArray;

        Brush primaryBrush;
        Brush secondaryBrush;

        //debug
        private const bool SmartConnections = true;
        private const bool Smoothing = true;


        public Tileset(int tileSize, int levels, int primaryColor, int secondaryColor)
        {
            this.tileSize = tileSize;
            this.levels = levels;
            tileCount = Enum.GetNames(typeof(TileType)).Length;
            


            primaryBrush = GetBrushFromHexCode(primaryColor, 0xFF);
            secondaryBrush = GetBrushFromHexCode(secondaryColor, 0xFF);

            //this is temporary
            /*
              secondaryBrush = new LinearGradientBrush(
                  new Point(0, 10), 
                  new Point(200, 10),
                  Color.FromArgb(255, 255, 0, 0),   // Opaque red
                  Color.FromArgb(255, 0, 0, 255));  // Opaque blue
              */

            tileArray = InitializeTileset();
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
            ///TODO: check if size and levels are initialzied to show exceptions
            Image[,] tileArray = new Image[levels, tileCount];
            int currentTileSize = tileSize;
            Brush primaryColor = primaryBrush;
            Brush secondaryColor = secondaryBrush;
            for(int currentLevel = 0; currentLevel < levels; currentLevel++)
            {
                for(int i = 0; i < tileCount; i++)
                {
                    tileArray[currentLevel, i] = DrawTileImage((TileType)i, primaryColor, secondaryColor, currentTileSize);
                }

                currentTileSize /= 2;
                //switch colors at each subdivision step 
                var temp = primaryColor;
                primaryColor = secondaryColor;
                secondaryColor = temp;
            }

            return tileArray;
        }

        /* function to draw all the tiles
         * This is seperate for readability and debug reasons
         * I used to just draw the necessary ones and copy and rotate the rest
           but that turned out to be a terrible idea because of floating point
           pixel-level imperfectness */
        private Image DrawTileImage(TileType type, Brush primary, Brush secondary, int tileSize)
        {
            Image i = GetEmptyImage(tileSize * 2);
            Graphics g = GetGraphicsFromImage(i);
            switch(type)
            {

                case TileType.Empty:
                    FillWhiteCube(g, tileSize, primary);
                    FillCornerWhiteCircles(g, tileSize, primary);
                    FillMiddleBlackCircles(g, tileSize, secondary);
                    break;

                case TileType.Vertical:
                    FillWhiteCube(g, tileSize, primary);
                    FillVerticalBlackQuad(g, tileSize, secondary);
                    FillCornerWhiteCircles(g, tileSize, primary);
                    FillMiddleBlackCircles(g, tileSize, secondary);
                    break;

                case TileType.Horizontal:
                    FillWhiteCube(g, tileSize, primary);
                    FillHorizontalBlackQuad(g, tileSize, secondary);
                    FillCornerWhiteCircles(g, tileSize, primary);
                    FillMiddleBlackCircles(g, tileSize, secondary);
                    break;

                case TileType.Cross:
                    FillWhiteCube(g, tileSize, primary);
                    bool[] crossBool = { true, true, true, true };
                    FillCornerBlackPies(g, tileSize, crossBool, secondary);
                    FillVerticalBlackQuad(g, tileSize, secondary);
                    FillCornerWhiteCircles(g, tileSize, primary);
                    FillMiddleBlackCircles(g, tileSize, secondary);
                    break;

                case TileType.Forwardslash:
                    FillWhiteCube(g, tileSize, primary);
                    bool[] fwsBool = { true, false, true, false };
                    FillCornerBlackPies(g, tileSize, fwsBool, secondary);
                    FillCornerWhiteCircles(g, tileSize, primary);
                    FillMiddleBlackCircles(g, tileSize, secondary);
                    break;

                case TileType.Backslash:
                    FillWhiteCube(g, tileSize, primary);
                    bool[] bsBool = { false, true, false, true };
                    FillCornerBlackPies(g, tileSize, bsBool, secondary);
                    FillCornerWhiteCircles(g, tileSize, primary);
                    FillMiddleBlackCircles(g, tileSize, secondary);
                    break;

                case TileType.Frown_NW:
                    FillWhiteCube(g, tileSize, primary);
                    bool[] fnwBool = { true, false, false, false };
                    FillCornerBlackPies(g, tileSize, fnwBool, secondary);
                    FillCornerWhiteCircles(g, tileSize, primary);
                    FillMiddleBlackCircles(g, tileSize, secondary);
                    break;

                case TileType.Frown_NE:
                    FillWhiteCube(g, tileSize, primary);
                    bool[] fneBool = { false, true, false, false };
                    FillCornerBlackPies(g, tileSize, fneBool, secondary);
                    FillCornerWhiteCircles(g, tileSize, primary);
                    FillMiddleBlackCircles(g, tileSize, secondary);
                    break;

                case TileType.Frown_SW:
                    FillWhiteCube(g, tileSize, primary);
                    bool[] fswBool = { false, false, true, false };
                    FillCornerBlackPies(g, tileSize, fswBool, secondary);
                    FillCornerWhiteCircles(g, tileSize, primary);
                    FillMiddleBlackCircles(g, tileSize, secondary);
                    break;

                case TileType.Frown_SE:
                    FillWhiteCube(g, tileSize, primary);
                    bool[] fseBool = { false, false, false, true };
                    FillCornerBlackPies(g, tileSize, fseBool, secondary);
                    FillCornerWhiteCircles(g, tileSize, primary);
                    FillMiddleBlackCircles(g, tileSize, secondary);
                    break;

                case TileType.T_N:
                    FillWhiteCube(g, tileSize, primary);
                    bool[] tn_Bool = { true, true, false, false };
                    FillCornerBlackPies(g, tileSize, tn_Bool, secondary);
                    FillHorizontalBlackQuad(g, tileSize, secondary);
                    FillCornerWhiteCircles(g, tileSize, primary);
                    FillMiddleBlackCircles(g, tileSize, secondary);
                    break;

                case TileType.T_E:
                    FillWhiteCube(g, tileSize, primary);
                    bool[] teBool = { false, true, true, false };
                    FillCornerBlackPies(g, tileSize, teBool, secondary);
                    FillVerticalBlackQuad(g, tileSize, secondary);
                    FillCornerWhiteCircles(g, tileSize, primary);
                    FillMiddleBlackCircles(g, tileSize, secondary);
                    break;

                case TileType.T_S:
                    FillWhiteCube(g, tileSize, primary);
                    bool[] ts_Bool = { false, false, true, true };
                    FillCornerBlackPies(g, tileSize, ts_Bool, secondary);
                    FillHorizontalBlackQuad(g, tileSize, secondary);
                    FillCornerWhiteCircles(g, tileSize, primary);
                    FillMiddleBlackCircles(g, tileSize, secondary);
                    break;

                case TileType.T_W:
                    FillWhiteCube(g, tileSize, primary);
                    bool[] twBool = { true, false, false, true };
                    FillCornerBlackPies(g, tileSize, twBool, secondary);
                    FillVerticalBlackQuad(g, tileSize, secondary);
                    FillCornerWhiteCircles(g, tileSize, primary);
                    FillMiddleBlackCircles(g, tileSize, secondary);
                    break;

                default:
                    throw new Exception("Not a valid TileType");
            }
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

        private static Brush GetBrushFromHexCode(int rgb, int alpha)
        {
            return new SolidBrush(Color.FromArgb((alpha << 24) ^ rgb));
        }
    }

    public enum TileType
    {
        Empty   = 0,
        Vertical = 1,
        Horizontal = 2,
        Cross = 3,
        Forwardslash = 4,
        Backslash = 5,
        Frown_NW = 6,
        Frown_NE = 7,
        Frown_SE = 8,
        Frown_SW = 9,
        T_N = 10,
        T_E = 11,
        T_S = 12,
        T_W = 13
    }

    //using bit flags as directions. from MSB to LSB: NESW

    public enum Direction
    {           // NESW
        None  = 0b_0000,
        North = 0b_0001,
        East  = 0b_0010,
        South = 0b_0100,
        West  = 0b_1000
    }
    public enum TileType2
    {
        Empty           = Direction.None,
        Vertical        = Direction.North | Direction.South,
        Horizontal      = Direction.East  | Direction.West,
        Cross           = Direction.North | Direction.East  | Direction.South | Direction.West,
        Forwardslash    = Direction.North | Direction.East  | Direction.South | Direction.West,
        Backslash       = Direction.North | Direction.East  | Direction.South | Direction.West,
        Frown_NW        = Direction.North | Direction.West, 
        Frown_NE        = Direction.North | Direction.East, 
        Frown_SE        = Direction.South | Direction.East, 
        Frown_SW        = Direction.South | Direction.West, 
        T_N             = Direction.North | Direction.East  | Direction.West,
        T_E             = Direction.North | Direction.East  | Direction.South,
        T_S             = Direction.East  | Direction.South | Direction.West,
        T_W             = Direction.North | Direction.South | Direction.West
    }


}
