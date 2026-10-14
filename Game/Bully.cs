namespace LFG;

/// <summary>A bully who chases you and throws Red Bull cans at your dice.</summary>
public class Bully : Actor
{
    public readonly Weapon ThrowStuff;
    public int Hits = 2;

    Player _player = null!;
    ScoreManager _score = null!;
    float _patrolTime, _waitTime, _threshold = 80, _attackDelay = 0.7f;
    bool _waiting, _hasSpoken;

    public Bully()
    {
        Drag = new Vector2(400, 400);
        MoveSpeed = 400;

        ThrowStuff = new Weapon(this)
        {
            FireRateMs = 1250,
            BulletSpeed = 100,
            BulletLifeSpanMs = 1500,
            RandomAngle = 5,
            RandomSpeed = 10,
            BulletElasticity = 0,
            Bounds = new Rectangle(0, 0, 960, 960),
            OnFire = () => FlxG.Sound.Play(LfgAssets.Sound("Bully_redbullHurl")),
        };
        ThrowStuff.MakeImageBullet(5, LfgAssets.Image("Bully_bullyBullet"), rotated: true);
        foreach (var can in ThrowStuff.Group.Members) can.AngularVelocity = 2000;
        // As in the Flash version the cans are added to the state here and to the bullet group in Init,
        // so each can is updated twice a frame (they fly twice as fast as BulletSpeed says).
        FlxG.State.Add(ThrowStuff.Group);

        LoadGraphic(LfgAssets.Image("Bully_bullySprite"), true, 16, 16);
        Animation.Add("idle", new[] { 0 }, 1, false);
        Animation.Add("walk", new[] { 0, 1, 0, 2 }, 10);
    }

    public void Init(float x, float y, FlxGroup bulletGroup, Player player, ScoreManager score)
    {
        _player = player;
        _score = score;
        Reset(x, y);
        GetDirection();
        foreach (var can in ThrowStuff.Group.Members) bulletGroup.Add(can);
    }

    public override void Update(float elapsed)
    {
        if (IsAlive)
        {
            if (RoundedDistance(new Vector2(_player.X, _player.Y), new Vector2(X, Y)) < _threshold) Attack(elapsed);
            else Patrol(elapsed);
        }
        base.Update(elapsed);
    }

    public void Attack(float elapsed)
    {
        _threshold = 250;
        if (!_hasSpoken)
        {
            Speak();
            _hasSpoken = true;
        }
        Animation.Play("walk");
        MaxVelocity = new Vector2(60, 60);
        Steer(AngleFromTo(new Vector2(_player.X, _player.Y), new Vector2(X, Y)) + 90);
        _attackDelay -= elapsed;
        if (_attackDelay < 0) ThrowStuff.FireAtTarget(_player);
    }

    void Patrol(float elapsed)
    {
        _attackDelay = 0.7f;
        MaxVelocity = new Vector2(25, 25);
        if (_patrolTime < 3) _patrolTime += elapsed;
        else
        {
            _waiting = true;
            Animation.Play("idle");
            Acceleration = Vector2.Zero;
            Velocity = Vector2.Zero;
            _waitTime += elapsed;
            if (_waitTime > 1)
            {
                _patrolTime = 0;
                _waitTime = 0;
                GetDirection();
                _waiting = false;
            }
        }
        if (!_waiting)
        {
            Animation.Play("walk");
            Angle = PatrolDir + 90;
            Acceleration = new Vector2(CalcX, CalcY);
        }
    }

    /// <summary>The bully is beaten: they leave their sword on the ground and a bag of dice.</summary>
    public void NerdifyBully()
    {
        IsAlive = false;
        FlxFlicker.Stop(this);       // a hit flicker still running would otherwise end by switching the bully back on
        LoadGraphic(LfgAssets.Image("Bully_warriorDeath"));
        Visible = true;
        Angle = 0;
        Velocity = Vector2.Zero;
        Acceleration = Vector2.Zero;
        var drops = new Drops(_player, X, Y, _score);
        FlxG.State.Add(drops);
        drops.Init();
    }

    void Speak() => new SpeechBubble(3, 2, this);
}
