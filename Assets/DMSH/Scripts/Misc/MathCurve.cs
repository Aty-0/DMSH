using UnityEngine;

namespace DMSH.Misc
{
    public static class MathCurve
    {
        // How much we need points for curve
        public static readonly int PATH_CURVE_LINE_STEPS = 20;

        public static Vector3 MakeCurve(Vector3 p0, Vector3 p1, Vector3 p2, float t)
        {
            return Vector3.Lerp(Vector3.Lerp(p0, p1, t), Vector3.Lerp(p1, p2, t), t);
        }

        public static void LineFollowedByPath(Vector2 start, Vector2 point, Vector2 end)
        {
            Vector3 lineStart = start;
            for (int i = 1; i <= PATH_CURVE_LINE_STEPS; i++)
            {
                Vector3 lineEnd = MakeCurve(start, point, end, i / (float)PATH_CURVE_LINE_STEPS);
                Gizmos.DrawLine(lineStart, lineEnd);
                lineStart = lineEnd;
            }
        }

        public static void CubeFollowedByPath(Vector2 start, Vector2 point, Vector2 end)
        {
            for (int i = 1; i <= PATH_CURVE_LINE_STEPS; i++)
            {
                Vector3 lineEnd = MakeCurve(start, point, end, i / (float)PATH_CURVE_LINE_STEPS);
                Gizmos.DrawCube(lineEnd, new Vector3(0.2f, 0.2f, 0.2f));
            }
        }
    }
}
