using UnityEngine;

public static class GalagaPaths
{

    public static readonly Vector2[] BeeLeft =
    {
        new(-9f, 8f),
        new(-8f, 6f),
        new(-7f, 3f),
        new(-5f, 0f),
        new(-2f, -3f),
        new(1f, -5f),
        new(4f, -4f),
        new(5f, -1f),
        new(4f, 2f),
        new(2f, 4f),
        new(0f, 5f)
    };

    public static readonly Vector2[] BeeRight =
    {
        new(9f, 8f),
        new(8f, 6f),
        new(7f, 3f),
        new(5f, 0f),
        new(2f, -3f),
        new(-1f, -5f),
        new(-4f, -4f),
        new(-5f, -1f),
        new(-4f, 2f),
        new(-2f, 4f),
        new(0f, 5f)
    };

    public static readonly Vector2[] ButterflyLeft =
    {
        new(-10f, 10f),
        new(-8f, 6f),
        new(-3f, 2f),
        new(3f, 1f),
        new(6f, 4f),
        new(4f, 8f),
        new(0f, 10f)
    };

    public static readonly Vector2[] ButterflyRight =
    {
        new(10f, 10f),
        new(8f, 6f),
        new(3f, 2f),
        new(-3f, 1f),
        new(-6f, 4f),
        new(-4f, 8f),
        new(0f, 10f)
    };

    public static readonly Vector2[] BossEntryLeft =
    {
        new(-12f, 12f),
        new(-10f, 7f),
        new(-4f, 2f),
        new(3f, 1f),
        new(8f, 4f),
        new(10f, 9f),
        new(6f, 13f),
        new(0f, 14f)
    };

    public static readonly Vector2[] BossEntryRight =
    {
        new(12f, 12f),
        new(10f, 7f),
        new(4f, 2f),
        new(-3f, 1f),
        new(-8f, 4f),
        new(-10f, 9f),
        new(-6f, 13f),
        new(0f, 14f)
    };

    public static readonly Vector2[] AttackDiveLeft =
    {
        new(-2f, 5f),
        new(-4f, 2f),
        new(-6f, -2f),
        new(-5f, -8f),
        new(-1f, -12f),
        new(2f, -15f),
        new(4f, -20f)
    };

    public static readonly Vector2[] AttackDiveRight =
    {
        new(2f, 5f),
        new(4f, 2f),
        new(6f, -2f),
        new(5f, -8f),
        new(1f, -12f),
        new(-2f, -15f),
        new(-4f, -20f)
    };

    public static readonly Vector2[] FigureEight =
    {
        new(0f, 0f),
        new(4f, 4f),
        new(7f, 0f),
        new(4f, -4f),
        new(0f, 0f),
        new(-4f, 4f),
        new(-7f, 0f),
        new(-4f, -4f),
        new(0f, 0f)
    };

    // =========================
    public static Vector2 CatmullRom(
    Vector2 p0,
    Vector2 p1,
    Vector2 p2,
    Vector2 p3,
    float t)
    {
        float t2 = t * t;
        float t3 = t2 * t;

        return 0.5f * (
            (2f * p1) +
            (-p0 + p2) * t +
            (2f * p0 - 5f * p1 + 4f * p2 - p3) * t2 +
            (-p0 + 3f * p1 - 3f * p2 + p3) * t3
        );
    }
}