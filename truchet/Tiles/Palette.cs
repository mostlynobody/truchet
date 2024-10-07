using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Truchet.Tiles
{
    abstract class Palette
    {

        // PREMADE PALETTES

        public static readonly SolidColorPalette Monochrome = new SolidColorPalette(0xFFFFFF, 0x000000);
        public static readonly SolidColorPalette Sapphire = new SolidColorPalette(0x05668D, 0xF0F3BD);
        public static readonly SolidColorPalette Imperial = new SolidColorPalette(0xE63946, 0x1D3557);
        public static readonly SolidColorPalette Deep = new SolidColorPalette(0x2D00F7, 0xE500A4);
        public static readonly SolidColorPalette Apricot = new SolidColorPalette(0xFFCDB2, 0x6D6875);
        public static readonly SolidColorPalette Xiketic = new SolidColorPalette(0x03071E, 0xFFBA08);
        public static readonly SolidColorPalette Canary = new SolidColorPalette(0x3D315B, 0xF8F991);
        public static readonly SolidColorPalette Meadow = new SolidColorPalette(0x034732, 0xc1292e);

        public static readonly LinearGradientPalette Gradient1 = new LinearGradientPalette(0x0000FF, 0xFF0000, 0x000000, 0x000000);

        public Brush PrimaryBrush { get; }
        public Brush SecondaryBrush { get; }

        public Palette(Brush primary, Brush secondary)
        {
            PrimaryBrush = primary;
            SecondaryBrush = secondary;
        }
    }


    class SolidColorPalette : Palette
    {

        public SolidColorPalette(int primaryColor, int secondaryColor)
            : base(GetBrushFromRGB(primaryColor), GetBrushFromRGB(secondaryColor))
        {

        }

        private static SolidBrush GetBrushFromRGB(int color)
        {
            return new SolidBrush(Color.FromArgb((255 << 24) ^ color));
        }
    }

    class LinearGradientPalette : Palette
    {
        public static int GradientBrushSize = 1000;

        private bool isTransformed;




        public LinearGradientPalette(int primaryColor1, int primaryColor2, int secondaryColor1, int secondaryColor2)
            : base(GetGradientBrush(primaryColor1, primaryColor2), GetGradientBrush(secondaryColor1, secondaryColor2))
            {
           
            isTransformed = false; 
            }

        private static LinearGradientBrush GetGradientBrush(int color1, int color2)
            {
            return new LinearGradientBrush(new Point(0, 0),
                                           new Point(GradientBrushSize, GradientBrushSize),
                                           Color.FromArgb((255 << 24) ^ color1),
                                           Color.FromArgb((255 << 24) ^ color2));
            }

        public void Transform(float scale, float angle)
        {
            LinearGradientBrush primary, secondary;
            primary = (LinearGradientBrush)PrimaryBrush;
            secondary = (LinearGradientBrush)SecondaryBrush;
            if (isTransformed) Reset();
            primary.ScaleTransform(scale, scale);
            primary.RotateTransform(angle);
            secondary.ScaleTransform(scale, scale);
            secondary.RotateTransform(angle);
            isTransformed = true;
        }
        
        public void Reset()
        {
            LinearGradientBrush primary, secondary;
            primary = (LinearGradientBrush)PrimaryBrush;
            secondary = (LinearGradientBrush)SecondaryBrush;
            primary.ResetTransform();
            secondary.ResetTransform();
            isTransformed = false;
        }
    }
}
