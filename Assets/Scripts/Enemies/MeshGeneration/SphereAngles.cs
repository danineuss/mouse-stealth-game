using UnityEngine;

public partial class HexMeshVerticesGenerator
{
    private struct SphereAngles
    {
        public float X { get; }
        public float Y { get; }

        public SphereAngles(float angleAroundX, float angleAroundY)
        {
            X = angleAroundX;
            Y = angleAroundY;
        }

        public Vector3 Vertice(float radius)
        {
            return new Vector3(
                radius * Mathf.Cos(X) * Mathf.Sin(Y),
                -radius * Mathf.Sin(X),
                radius * Mathf.Cos(X) * Mathf.Cos(Y)
            );
        }

        public static SphereAngles operator +(SphereAngles a) => a;
        public static SphereAngles operator -(SphereAngles a) => new SphereAngles(-a.X, -a.Y);
        public static SphereAngles operator +(SphereAngles a, SphereAngles b)
            => new SphereAngles(a.X + b.X, a.Y + b.Y);
        public static SphereAngles operator -(SphereAngles a, SphereAngles b) => a + (-b);
        public static SphereAngles operator *(float f, SphereAngles a) 
            => new SphereAngles(f * a.X, f * a.Y);
    }
}
