using Microsoft.Xna.Framework;
using System;

namespace Dcrew.Collisions {
    public struct Line {
        public Vector2 A, B;

        public Line(Vector2 a, Vector2 b) {
            A = a;
            B = b;
        }

        public bool Intersects(Line line, out Vector2 intersection) {
            (float X, float Y) n1 = (B.X - A.X, B.Y - A.Y);
            (float X, float Y) = (line.B.X - line.A.X, line.B.Y - line.A.Y);
            var denominator = n1.X * Y - n1.Y * X;
            if (MathF.Abs(denominator) < float.Epsilon) {
                intersection = Vector2.Zero;
                return false;
            }
            (float X, float Y) n3 = (line.A.X - A.X, line.A.Y - A.Y);
            var t = (n3.X * Y - n3.Y * X) / denominator;
            if (t < 0 || t > 1) {
                intersection = Vector2.Zero;
                return false;
            }
            var u = (n3.X * n1.Y - n3.Y * n1.X) / denominator;
            if (u < 0 || u > 1) {
                intersection = Vector2.Zero;
                return false;
            }
            intersection = new Vector2(A.X + t * n1.X, A.Y + t * n1.Y);
            return true;
        }
        public bool Intersects(Vector2 xy, float radiusSquared) => Vector2.DistanceSquared(xy, ClosestPoint(xy)) <= radiusSquared;

        public Vector2 ClosestPoint(Vector2 p) {
            var ab = B - A;
            var distance = Vector2.Dot(p - A, ab) / ab.LengthSquared();
            return distance < 0 ? A : distance > 1 ? B : A + ab * distance;
        }
        public static bool IsLeft(Vector2 a, Vector2 b, Vector2 p) => (b.X - a.X) * (p.Y - a.Y) - (p.X - a.X) * (b.Y - a.Y) > 0;
    }
}