using System;
using System.Drawing;
using System.Drawing.Imaging;

namespace Truchet
{   

    class Tileset
    {

        readonly int subdivisionLevels;
        readonly int tileSize;

        //how many different tiles there are, minus the container tile
        readonly private int tileCount;

        /* using a rectangular array because 
         * it's a neat c# feature to show 
         */
        Image[,] imageArray;

        Brush primaryBrush;
        Brush secondaryBrush;
        Brush alphaBrush;

        //debug
        internal static readonly bool SMOOTHING = true;


        public Tileset(int tileSize, int subdivisionLevels, int primaryColor, int secondaryColor)
        {
            this.tileSize = tileSize;
            this.subdivisionLevels = subdivisionLevels;
            tileCount = Enum.GetNames(typeof(TileType)).Length - 1;
            primaryBrush = GetBrushFromHexCode(primaryColor, 0xFF);
            secondaryBrush = GetBrushFromHexCode(secondaryColor, 0xFF);
            alphaBrush = GetBrushFromHexCode(0x000000, 0x00);

            imageArray = InitializeTileset();
        }
        
        


        public Image GetRandomTile(int level)
        {

            return null;
        }

        internal Image[,] InitializeTileset()
        {
            //TODO: CHECK IF tileSIZE and subdivsionLEVELS are initialzied TO SHOW EXCEPTIONS
            Image[,] imageArray = new Image[subdivisionLevels, tileCount];
            int currentTileSize = tileSize;
            Brush primaryColor = primaryBrush;
            Brush secondaryColor = secondaryBrush;
            for(int currentLevel = 0; currentLevel < subdivisionLevels; currentLevel++)
            {
                //Forwardslash
                //the image itself is always twice the size of the tile
                Image forwardslash = GetEmptyTile(currentTileSize * 2);
                Graphics fwsGraphics = GetGraphicsFromImage(forwardslash);
                
                FillWhiteCube(fwsGraphics, currentTileSize, primaryColor);
                forwardslash.Save("forwardslash1.png", ImageFormat.Png);
                bool[] fwsBool = { false, true, false, true };
                FillCornerBlackPies(fwsGraphics, currentTileSize, fwsBool, secondaryColor);
                forwardslash.Save("forwardslash2.png", ImageFormat.Png);
                FillCornerWhiteCircles(fwsGraphics, currentTileSize, primaryColor);
                forwardslash.Save("forwardslash3.png", ImageFormat.Png);
                FillMiddleBlackCircles(fwsGraphics, currentTileSize, secondaryColor);
                forwardslash.Save("forwardslash4.png", ImageFormat.Png);


                forwardslash.Save("forwardslash.png", ImageFormat.Png);


                //Backslash
                //Horizontal
                //Vertical
                //Cross
                //Empty
                //Frown_NE
                //Frown_SE
                //Frown_SW
                //Frown_NW
                //T_NE
                //T_SE
                //T_SW
                //T_NW

                currentTileSize /= 2;
                //switch colors at each subdivision step 
                var temp = primaryColor;
                primaryColor = secondaryColor;
                secondaryColor = temp;
            }

            return imageArray;
        }

        /* helper functions to draw the tiles 
            these functions will not make much sense without understanding
            the actual process the tiles are drawn.
             */

        //fills the white cube in the middle
        internal static void FillWhiteCube(Graphics g, int tileSize, Brush primary)
        {
            g.FillRectangle(primary, tileSize / 2, tileSize / 2, tileSize, tileSize);
        }

        //fills the black pies in the corners
        //like everything else in this project, the boolean array represents: NE, SE, SW, NW
        internal static void FillCornerBlackPies(Graphics g, int tileSize, bool[] corners, Brush secondary)
        {
            //get the upper upper left left corner, and then for the other corners we just add 1 tile length
            int corner = -tileSize / 6;
            int diameter = (tileSize / 3) * 4;
            //putting this into a loop would take longer than it's worth
            if (corners[0]) g.FillPie(secondary, corner + tileSize, corner, diameter, diameter, 90, 90);
            if (corners[1]) g.FillPie(secondary, corner + tileSize, corner + tileSize, diameter, diameter, 180, 90);
            if (corners[2]) g.FillPie(secondary, corner, corner + tileSize, diameter, diameter, 270, 90);
            if (corners[3]) g.FillPie(secondary, corner, corner, diameter, diameter, 0, 90);
        }
        internal static void FillCornerWhiteCircles(Graphics g, int tileSize, Brush primary)
        {
            //get the upper upper left left corner, and then for the other corners we just add 1 tile length
            int corner = tileSize / 6;
            int diameter = (tileSize / 3) * 2;
            g.FillEllipse(primary, corner, corner, diameter, diameter);
            g.FillEllipse(primary, corner, corner + tileSize, diameter, diameter);
            g.FillEllipse(primary, corner + tileSize, corner, diameter, diameter);
            g.FillEllipse(primary, corner + tileSize, corner + tileSize, diameter, diameter);
        }
        internal static void FillMiddleBlackCircles(Graphics g, int tileSize, Brush secondary)
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

        internal static Image GetEmptyTile(int size)
        {
            return new Bitmap(size, size, PixelFormat.Format32bppArgb);
        }

        //used so smoothingMode doesn#t have to be manually set every time
        internal static Graphics GetGraphicsFromImage(Image i)
        {
            var g = Graphics.FromImage(i);
            if(SMOOTHING) g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            return g;
        }

        internal static Brush GetBrushFromHexCode(int rgb, int alpha)
        {
            return (Brush)new SolidBrush(Color.FromArgb((alpha << 24) ^ rgb));
        }
    }

    public enum TileType
    {
        Forwardslash,
        Backslash,
        Horizontal,
        Vertical,
        Cross,
        Empty,
        Frown_NE,
        Frown_SE,
        Frown_SW,
        Frown_NW,
        T_NE,
        T_SE,
        T_SW,
        T_NW,
        Container
    }
}
