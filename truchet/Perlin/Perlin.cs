/// PERLIN NOISE FUNCTION
/// transcribed and adapted from: 
/// https://en.wikipedia.org/wiki/Smoothstep
/// https://en.wikipedia.org/wiki/Perlin_noise
/// http://www.siafoo.net/snippet/144?nolinenos#perlin2003

using System;

namespace Truchet.Perlin
{

    class PerlinNoise
    {

        private readonly Random rand;
        private readonly int[] permutation;
        private readonly Vec2[] gradients;

        private const int SIZE = 256;

        public PerlinNoise(Random rand)
        {
            this.rand = rand;
            permutation = RandomizePermutation();
            gradients = RandomizeGradients();
        }

        private int[] RandomizePermutation()
        {
            int[] perm = new int[SIZE];
            for (int i = 0; i < SIZE; i++) perm[i] = i;
            // shuffle the array
            for (int i = 0; i < SIZE; i++)
            {
                int source = rand.Next(SIZE);
                int temp = perm[i];
                perm[i] = perm[source];
                perm[source] = temp;
            }
            return perm;

            // just as a sidenote, this has worse performance, but is really cool:
            // return Enumerable.Range(0, SIZE).ToArray().OrderBy(x => rand.Next()).ToArray();
        }

        private Vec2[] RandomizeGradients()
        {
            Vec2[] gradient = new Vec2[SIZE];
            for (int i = 0; i < SIZE; i++)
            {
                Vec2 v;
                do v = new Vec2((rand.NextDouble() * 2 - 1), (rand.NextDouble() * 2 - 1));
                while (Vec2.LengthSquared(v) >= 1);
                gradient[i] = Vec2.Normalize(v);
            }
            return gradient;
        }

        public double Noise(double x, double y)
        {
            Vec2 origin = new Vec2(x, y);
            Vec2 cell = new Vec2(Math.Floor(x), Math.Floor(y));
            
            var corners = new[] { 
                cell,
                new Vec2(cell.X, cell.Y+1), 
                new Vec2(cell.X+1, cell.Y), 
                new Vec2(cell.X+1, cell.Y+1) 
            };

            double sum = 0;
            foreach (Vec2 v in corners)
            {
                Vec2 w = origin - v;
                int i = permutation[(int)v.X % SIZE];
                i = permutation[(i + (int)v.Y) % SIZE];
                Vec2 grad = gradients[i % SIZE];

                sum += Smoothstep(w.X, w.Y) * Vec2.DotProduct(grad, w);
            }
            return Math.Max(Math.Min(sum, 1d), -1d);
        }

        private double Smoothstep(double a, double b)
        {
            a = Math.Abs(a);
            b = Math.Abs(b);
            a = 1d - a * a * a * (a * (a * 6 - 15) + 10);
            b = 1d - b * b * b * (b * (b * 6 - 15) + 10);
            return a * b;
        }
    }
}
