namespace LFG;

/// <summary>A passer-by who wanders around and runs from you, until a d20 turns them into an adventurer.</summary>
public class Pedestrian : Actor
{
    public int PedType;          // 1 warrior, 2 rogue, 3 cleric, 4 wizard
    public int Hits = 1;

    Player _player = null!;
    readonly string _deathImage;
    float _patrolTime, _waitTime, _threshold = 20;
    bool _waiting, _hasSpoken;

    public Pedestrian()
    {
        Drag = new Vector2(400, 400);
        MoveSpeed = 400;

        string sheet;
        switch ((int)(FlxG.Random.Float() * 5))
        {
            case 1: sheet = "Pedestrian_pedSprite1"; _deathImage = "Pedestrian_warriorDeath"; PedType = 1; break;
            case 2: sheet = "Pedestrian_pedSprite2"; _deathImage = "Pedestrian_rogueDeath"; PedType = 2; break;
            case 3: sheet = "Pedestrian_pedSprite3"; _deathImage = "Pedestrian_clericDeath"; PedType = 3; break;
            case 4: sheet = "Pedestrian_pedSprite4"; _deathImage = "Pedestrian_wizardDeath"; PedType = 4; break;
            default: sheet = "Pedestrian_pedSprite1"; _deathImage = "Pedestrian_wizardDeath"; PedType = 4; break;
        }
        LoadGraphic(LfgAssets.Image(sheet), true, 16, 16);
        Animation.Add("idle", new[] { 0 }, 1, false);
        Animation.Add("walk", new[] { 0, 1, 0, 2 }, 7);
        Animation.Add("walkfast", new[] { 0, 1, 0, 2 }, 12);
    }

    public void Init(float x, float y, Player player)
    {
        _player = player;
        Reset(x, y);
        GetDirection();
    }

    /// <summary>Turns them into an adventurer: they stop where they stand.</summary>
    public void Nerdify()
    {
        if (Hits > 1)
        {
            Hits--;
            FlxFlicker.Flicker(this, 0.6f);
            return;
        }
        IsAlive = false;
        LoadGraphic(LfgAssets.Image(_deathImage));
        Angle = 0;
        Velocity = Vector2.Zero;
        Acceleration = Vector2.Zero;
    }

    public override void Update(float elapsed)
    {
        if (IsAlive)
        {
            if (RoundedDistance(new Vector2(_player.X, _player.Y), new Vector2(X, Y)) < _threshold) Flee();
            else Patrol(elapsed);
        }
        base.Update(elapsed);
    }

    void Flee()
    {
        if (!_hasSpoken)
        {
            Speak();
            _hasSpoken = true;
        }
        Animation.Play("walkfast");
        _threshold = 250;
        MaxVelocity = new Vector2(50, 50);
        Steer(AngleFromTo(new Vector2(_player.X, _player.Y), new Vector2(X, Y)) + 270);
    }

    void Patrol(float elapsed)
    {
        _hasSpoken = false;
        _threshold = 80;
        MaxVelocity = new Vector2(25, 25);
        if (_patrolTime < 3) _patrolTime += elapsed;
        else
        {
            _waiting = true;
            Animation.Play("idle");
            Acceleration = Vector2.Zero;
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

    void Speak() => new SpeechBubble((int)MathF.Floor(FlxG.Random.Float() * 3), 2, this);
}
