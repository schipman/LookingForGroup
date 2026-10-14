namespace LFG;

/// <summary>
/// What the Flash version added to FlxObject itself (isAlive, patrolDir, GetDirection, ...), shared by pedestrians and bullies.
/// IsAlive is the game's own flag: a dead actor stays in the world as a grave, so it is not Kill()ed.
/// </summary>
public class Actor : FlxSprite
{
    public bool IsAlive = true;
    public float PatrolDir, PatrolRadians, CalcX, CalcY;
    public int MoveSpeed;

    public Actor(float x = 0, float y = 0) : base(x, y) { }

    /// <summary>Picks a random heading to walk in.</summary>
    public void GetDirection()
    {
        PatrolDir = FlxG.Random.Float() * 360;
        PatrolRadians = PatrolDir * (MathF.PI / 180);
        CalcX = MathF.Cos(PatrolRadians) * MoveSpeed;
        CalcY = MathF.Sin(PatrolRadians) * MoveSpeed;
    }

    /// <summary>Point the actor's acceleration along a heading (degrees) and face it.</summary>
    protected void Steer(float headingDegrees)
    {
        PatrolDir = headingDegrees;
        PatrolRadians = PatrolDir * (MathF.PI / 180);
        CalcX = MathF.Cos(PatrolRadians) * MoveSpeed;
        CalcY = MathF.Sin(PatrolRadians) * MoveSpeed;
        Acceleration = new Vector2(CalcX, CalcY);
        Angle = headingDegrees + 90;
    }

    /// <summary>Flixel's FlxU.getAngle(from, to): the direction from one point to another, 0 = up. (Flixel used a cheap atan2.)</summary>
    protected static float AngleFromTo(Vector2 from, Vector2 to)
    {
        float dx = to.X - from.X, dy = to.Y - from.Y;
        if (dx == 0 && dy == 0) return 0;
        float a = MathF.Atan2(dy, dx) * 180f / MathF.PI;
        return a > 90 ? a - 270 : a + 90;
    }

    /// <summary>Distance to a point, rounded like Flash's Math.round.</summary>
    protected static float RoundedDistance(Vector2 a, Vector2 b) => MathF.Floor(Vector2.Distance(a, b) + 0.5f);
}
