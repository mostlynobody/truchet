using System;

namespace Truchet.Perlin
{
    public struct Vec2
    {
        public double x { get; }
        public double y { get; }
        public Vec2(double x, double y)
        {
            this.x = x;
            this.y = y;
        }

        public static Vec2 operator +(Vec2 a) => a;
        public static Vec2 operator -(Vec2 a) => new Vec2(-a.x, -a.y);
        public static Vec2 operator +(Vec2 a, Vec2 b) => new Vec2(a.x + b.x, a.y + b.y);
        public static Vec2 operator -(Vec2 a, Vec2 b) => a + (-b);
        
        public static double Length(Vec2 a)
        {
            return Math.Sqrt(LengthSquared(a));
        }

        public static double LengthSquared(Vec2 a)
        {
            return (a.x * a.x) + (a.y * a.y);
        }

        public static double DotProduct(Vec2 a, Vec2 b)
        {
            return (a.x * b.x) + (a.y * b.y);
        }

        public static Vec2 Normalize(Vec2 a)
        {
            double len = LengthSquared(a);
            if (len > 0)
            {
                len = 1 / Math.Sqrt(len);
                return new Vec2(a.x * len, a.y * len);
            }
            else return a;
        }
    }
}
