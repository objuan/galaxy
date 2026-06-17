using UnityEngine;

public static class GizmosEx
{
    public static void DrawCircle(Vector3 center, float radius, int segments)
    {
        Vector3 prevPoint = center + Vector3.right * radius;

        for (int i = 1; i <= segments; i++)
        {
            float angle = i * Mathf.PI * 2f / segments;

            Vector3 newPoint = center +
                new Vector3(
                    Mathf.Cos(angle) * radius,
                    0,
                    Mathf.Sin(angle) * radius);

            Gizmos.DrawLine(prevPoint, newPoint);

            prevPoint = newPoint;
        }
    }

    public static void DrawRect(Vector3 center, float width, float height)
    {
        float halfWidth = width * 0.5f;
        float halfHeight = height * 0.5f;

        Vector3 topLeft = center + new Vector3(-halfWidth, 0, halfHeight);
        Vector3 topRight = center + new Vector3(halfWidth, 0, halfHeight);
        Vector3 bottomRight = center + new Vector3(halfWidth, 0, -halfHeight);
        Vector3 bottomLeft = center + new Vector3(-halfWidth, 0, -halfHeight);

        Gizmos.DrawLine(topLeft, topRight);
        Gizmos.DrawLine(topRight, bottomRight);
        Gizmos.DrawLine(bottomRight, bottomLeft);
        Gizmos.DrawLine(bottomLeft, topLeft);
    }

    public static void DrawRect(Vector3 center, Vector3 right, Vector3 forward, float width, float height)
    {
        right.Normalize();
        forward.Normalize();

        Vector3 halfRight = right * (width * 0.5f);
        Vector3 halfForward = forward * (height * 0.5f);

        Vector3 p1 = center - halfRight - halfForward;
        Vector3 p2 = center + halfRight - halfForward;
        Vector3 p3 = center + halfRight + halfForward;
        Vector3 p4 = center - halfRight + halfForward;

        Gizmos.DrawLine(p1, p2);
        Gizmos.DrawLine(p2, p3);
        Gizmos.DrawLine(p3, p4);
        Gizmos.DrawLine(p4, p1);
    }

    public static void DrawSquare(Vector3 center, float size)
    {
        DrawRect(center, size, size);
    }
}