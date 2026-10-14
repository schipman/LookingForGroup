namespace LFG;

public class Player : FlxSprite
{
    public int DiceNumber = 70;
    public float AimAngle;

    int _prevFrame;

    public Player(float x, float y) : base(x, y)
    {
        LoadGraphic(LfgAssets.Image("Player_playerPNG"), true, 16, 16);
        Animation.Add("idle", new[] { 0 }, 1, false);
        Animation.Add("walk", new[] { 0, 1, 0, 2 }, 12);
        Width = 10;
        Height = 8;
        CenterOffsets();

        // WASD, accelerating up to a top speed and sliding to a stop (was Flixel Power Tools' FlxControl)
        Drag = new Vector2(500, 500);
        MaxVelocity = new Vector2(75, 75);
    }

    const float Accel = 400;

    public override void Update(float elapsed)
    {
        var accel = Vector2.Zero;
        if (FlxG.Keys.Pressed(Keys.W)) accel.Y = -Accel;
        else if (FlxG.Keys.Pressed(Keys.S)) accel.Y = Accel;
        if (FlxG.Keys.Pressed(Keys.A)) accel.X = -Accel;
        else if (FlxG.Keys.Pressed(Keys.D)) accel.X = Accel;
        Acceleration = accel;

        if (Velocity.X != 0 || Velocity.Y != 0)
        {
            Animation.Play("walk");
            int frame = Frame;
            if (frame == 1 && _prevFrame != 1) FlxG.Sound.Play(LfgAssets.Sound("Player_footstep1"), 0.9f);
            if (frame == 2 && _prevFrame != 2) FlxG.Sound.Play(LfgAssets.Sound("Player_footstep2"), 0.9f);
            _prevFrame = frame;
        }
        else Animation.Play("idle");

        // face the mouse
        var mouse = FlxG.Mouse.GetWorldPosition();
        AimAngle = MathF.Atan2(mouse.Y - Y, mouse.X - X) * 180f / MathF.PI;
        Angle = AimAngle - 270;

        base.Update(elapsed);
    }
}
