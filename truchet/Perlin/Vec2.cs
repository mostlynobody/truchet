using System;

namespace Truchet.Perlin
{
    struct Vec2
    {
        public double X { get; }
        public double Y { get; }
        public Vec2(double x, double y)
        {
            X = x;
            Y = y;
        }

        public static Vec2 operator +(Vec2 a) => a;
        public static Vec2 operator -(Vec2 a) => new Vec2(-a.X, -a.Y);
        public static Vec2 operator +(Vec2 a, Vec2 b) => new Vec2(a.X + b.X, a.Y + b.Y);
        public static Vec2 operator -(Vec2 a, Vec2 b) => a + (-b);
        
        public static double Length(Vec2 a)
        {
            return Math.Sqrt(LengthSquared(a));
        }

        public static double LengthSquared(Vec2 a)
        {
            return (a.X * a.X) + (a.Y * a.Y);
        }

        public static double DotProduct(Vec2 a, Vec2 b)
        {
            return (a.X * b.X) + (a.Y * b.Y);
        }

        public static Vec2 Normalize(Vec2 a)
        {
            double len = LengthSquared(a);
            if (len > 0)
            {
                len = 1 / Math.Sqrt(len);
                return new Vec2(a.X * len, a.Y * len);
            }
            else return a;
        }
    }
}
