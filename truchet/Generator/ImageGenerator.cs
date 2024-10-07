using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Truchet.Perlin;
using Truchet.Tiles;

namespace Truchet.Generator
{

    class ImageGenerator
    {

        readonly int tileSize;
        readonly int rowCount;
        readonly int columnCount;

        private readonly int canvasWidth;
        private readonly int canvasHeight;
        private readonly bool borderless;

        readonly int levelCount = 3;

        private readonly Random rand;
        private readonly Tileset tileset;
        private readonly NoiseMap noisemap;

        public ImageGenerator(int tileSize, 
                                int rowCount, 
                                int columnCount, 
                                int levelCount)
        {
            this.tileSize = tileSize;
            this.rowCount = rowCount;
            this.columnCount = columnCount;
            this.levelCount = levelCount;

            canvasWidth = tileSize * columnCount;
            canvasHeight = tileSize * rowCount;
            if(!borderless)
            {
                canvasWidth += tileSize;
                canvasHeight += tileSize;
            }
        }
    }
}
