using Microsoft.Xna.Framework;
using System;

namespace Dcrew.Collisions {
    public class ConvexPoly {
        public Vector2[] Verts;
        public Vector2 XY;
        public float Rotation {
            get => _rotation;
            set {
                for (var i = 0; i < Verts.Length; i++) {
                    var p = Verts[i];
                    float cos = MathF.Cos(-_rotation),
                        sin = MathF.Sin(-_rotation),
                        x = p.X,
                        y = p.Y,
                        xcos = x * cos,
                        ycos = y * cos,
                        xsin = x * sin,
                        ysin = y * sin;
                    p = Verts[i] = new Vector2(xcos - ysin, xsin + ycos);
                    cos = MathF.Cos(value);
                    sin = MathF.Sin(value);
                    x = p.X;
                    y = p.Y;
                    xcos = x * cos;
                    ycos = y * cos;
                    xsin = x * sin;
                    ysin = y * sin;
                    Verts[i] = new Vector2(xcos - ysin, xsin + ycos);
                }
                _rotation = value;
            }
        }

        float _rotation;

        public Vector2 Center {
            get {
                var v = Vector2.Zero;
                for (var i = 0; i < Verts.Length; i++)
                    v += Verts[i];
                return (v / Verts.Length) + XY;
            }
        }

        public ConvexPoly(params Vector2[] verts) {
            Verts = verts;
        }

        public virtual Vector2 Clamp(Vector2 xy) {
            var doesIntersect = true;
            var close = Vector2.Zero;
            var dist = float.MaxValue;
            var vert = Verts[0] + XY;
            var firstVert = vert;
            Line line;
            Vector2 iClose;
            for (var i = 1; i < Verts.Length; i++) {
                var nextVert = Verts[i] + XY;
                line = new Line(vert, nextVert);
                if (!Line.IsLeft(line.A, line.B, xy))
                    doesIntersect = false;
                iClose = line.ClosestPoint(xy);
                float iDist = Vector2.DistanceSquared(iClose, xy);
                if (iDist < dist) {
                    close = iClose;
                    dist = iDist;
                }
                vert = nextVert;
            }
            line = new Line(vert, firstVert);
            if (!Line.IsLeft(line.A, line.B, xy))
                doesIntersect = false;
            if (doesIntersect)
                return xy;
            iClose = line.ClosestPoint(xy);
            if (Vector2.DistanceSquared(iClose, xy) < dist)
                close = iClose;
            return close;
        }

        public virtual bool Contains(Vector2 xy) {
            var vert = Verts[0] + XY;
            var firstVert = vert;
            for (var i = 1; i < Verts.Length; i++) {
                var nextVert = Verts[i] + XY;
                if (!Line.IsLeft(vert, nextVert, xy))
                    return false;
                vert = nextVert;
            }
            if (!Line.IsLeft(vert, firstVert, xy))
                return false;
            return true;
        }
        public bool Intersects(ConvexPoly value) {
            for (var i = 0; i < Verts.Length; i++)
                if (Contains(value.Clamp(Verts[i] + XY)))
                    return true;
            for (var i = 0; i < value.Verts.Length; i++)
                if (value.Contains(Clamp(value.Verts[i] + value.XY)))
                    return true;
            return false;
        }
    }
}