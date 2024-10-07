using System;
using System.Threading.Tasks;

namespace Truchet.Perlin
{
    class NoiseMap
    {   

        public static double[,] GenerateNoiseMap(Random rand, int width, int height,  double frequency,  double amplitude, int octaves)
        {
            PerlinNoise perlin = new PerlinNoise(rand);
            double[,] matrix = new double[width, height];

            /// track min and max noise value. Used to normalize the result to the 0 to 1.0 range.
            double min = double.MaxValue;
            double max = double.MinValue;

            for (var octave = 0; octave < octaves; octave++)
            {
                Parallel.For(0, width, (x) =>
                {
                    for (int y = 0; y < height; y++)
                    {
                        double noise = perlin.Noise(x * frequency * 1d / width, y * frequency * 1d / height);
                        noise *= amplitude;
                        matrix[x, y] = noise;
                        min = Math.Min(min, noise);
                        max = Math.Max(max, noise);
                    }
                }
                );
                frequency *= 2;
                amplitude /= 2;
            }

            //normalize matrix
            for (int x = 0; x < width; x++)
                for (int y = 0; y < height; y++)
                    matrix[x, y] = (matrix[x, y] - min) / (max - min);
            return matrix;
        }
    }
}
