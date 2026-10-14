namespace LFG;

/// <summary>The part of Flixel Power Tools' FlxWeapon the game used: a pool of bullets fired from a parent object.</summary>
public class Weapon
{
    public readonly FlxTypedGroup<Bullet> Group = new();
    public Rectangle Bounds;
    public float BulletSpeed;
    public float FireRateMs;
    public float BulletLifeSpanMs;
    public int RandomAngle, RandomSpeed;
    public float BulletElasticity;
    public Action? OnFire;

    readonly FlxObject _parent;
    double _nextFire;

    public Weapon(FlxObject parent)
    {
        _parent = parent;
        Bounds = new Rectangle(0, 0, FlxG.Width, FlxG.Height);
    }

    /// <summary>Makes the bullet pool. With rotated=true the picture is baked at 16 angles so a spinning bullet looks right.</summary>
    public void MakeImageBullet(int quantity, string imagePath, bool rotated = false)
    {
        for (int i = 0; i < quantity; i++)
        {
            var b = new Bullet(this);
            if (rotated) b.LoadRotatedGraphic(imagePath, 16);
            else b.LoadGraphic(imagePath);
            Group.Add(b);
        }
    }

    Bullet? GetFreeBullet() => Group.Members.Find(b => !b.Exists);

    /// <summary>Fires one bullet in a direction (degrees, 0 = right) from the middle of the parent. False while still reloading or out of bullets.</summary>
    public bool FireFromAngle(float angle) => Fire((b, x, y) => b.FireFromAngle(x, y, angle, BulletSpeed));

    /// <summary>Fires one bullet at another sprite.</summary>
    public bool FireAtTarget(FlxSprite target) => Fire((b, x, y) => b.FireAtTarget(x, y, target, BulletSpeed));

    bool Fire(Action<Bullet, float, float> launch)
    {
        double now = FlxG.RealTime * 1000;
        if (FireRateMs > 0 && now < _nextFire) return false;
        var bullet = GetFreeBullet();
        if (bullet == null) return false;

        bullet.Velocity = Vector2.Zero;
        _nextFire = now + FireRateMs;
        var origin = _parent.Midpoint;
        launch(bullet, origin.X - bullet.Width / 2, origin.Y - bullet.Height / 2);   // from the middle of the shooter
        return true;
    }
}

public class Bullet : FlxSprite
{
    readonly Weapon _weapon;
    double _expiresAt;
    bool _expires;

    public Bullet(Weapon weapon) : base(0, 0)
    {
        _weapon = weapon;
        Exists = false;
    }

    public void FireFromAngle(float x, float y, float angle, float speed)
    {
        X = x; Y = y;
        float a = MathHelper.ToRadians(angle + RandomInt(_weapon.RandomAngle));
        float s = speed + RandomInt(_weapon.RandomSpeed);
        Velocity = new Vector2((int)(MathF.Cos(a) * s), (int)(MathF.Sin(a) * s));
        PostFire();
    }

    public void FireAtTarget(float x, float y, FlxSprite target, float speed)
    {
        X = x; Y = y;
        float dx = target.X + target.Origin.X - (X + Origin.X);
        float dy = target.Y + target.Origin.Y - (Y + Origin.Y);
        float a = MathF.Atan2(dy, dx);
        float s = speed + RandomInt(_weapon.RandomSpeed);
        Velocity = new Vector2(MathF.Cos(a) * s, MathF.Sin(a) * s);
        PostFire();
    }

    static int RandomInt(int range) => range == 0 ? 0 : FlxG.Random.Int(-range, range);

    void PostFire()
    {
        if (_weapon.BulletElasticity > 0) Elasticity = _weapon.BulletElasticity;
        Revive();
        _expires = _weapon.BulletLifeSpanMs > 0;
        if (_expires) _expiresAt = FlxG.RealTime * 1000 + _weapon.BulletLifeSpanMs;
        _weapon.OnFire?.Invoke();
    }

    public override void Update(float elapsed)
    {
        if (_expires && FlxG.RealTime * 1000 > _expiresAt) Kill();
        var b = _weapon.Bounds;
        if (X < b.Left || X > b.Right || Y < b.Top || Y > b.Bottom) Kill();
        base.Update(elapsed);
    }
}
