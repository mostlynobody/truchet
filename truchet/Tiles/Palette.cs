using System.Drawing;
using System.Drawing.Drawing2D;

namespace Truchet.Tiles
{

    abstract class Palette
    {
        public Brush PrimaryBrush { get; }
        public Brush SecondaryBrush { get; }
        
        public string Name { get; }

        public Palette(Brush primary, Brush secondary, string name)
        {
            PrimaryBrush = primary;
            SecondaryBrush = secondary;
            Name = name;
        }
    }

    class SolidColorPalette : Palette
    {

        public SolidColorPalette(int primaryColor, int secondaryColor, string name)
            : base(GetBrushFromRGB(primaryColor), GetBrushFromRGB(secondaryColor), name)
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

        public LinearGradientPalette(int primaryColor1, int primaryColor2, int secondaryColor1, int secondaryColor2, string name)
            : base(GetGradientBrush(primaryColor1, primaryColor2), GetGradientBrush(secondaryColor1, secondaryColor2), name)
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
