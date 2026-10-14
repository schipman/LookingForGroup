namespace LFG;

/// <summary>The bag of dice a defeated bully drops.</summary>
public class Drops : FlxSprite
{
    readonly Player _player;
    readonly ScoreManager _score;
    readonly float _initialY;

    public Drops(Player player, float x, float y, ScoreManager score)
    {
        X = x + 8;
        Y = y + 8;
        _initialY = Y;
        _player = player;
        _score = score;
        LoadGraphic(LfgAssets.Image("Drops_diceBag"));
        Scale = new Vector2(0.5f, 0.5f);
    }

    /// <summary>Hops up and falls back down.</summary>
    public void Init()
    {
        Velocity = new Vector2(-50 + FlxG.Random.Float() * 100, -150);
    }

    public override void Update(float elapsed)
    {
        base.Update(elapsed);
        if (Y > _initialY)
        {
            Acceleration.Y = 0;
            Velocity = Vector2.Zero;
        }
        else Acceleration.Y = 400;
        FlxG.Overlap(_player, this, (_, _) => GiveDice());
    }

    void GiveDice()
    {
        _score.TotalDice += 5;
        _player.DiceNumber += 5;
        Kill();
    }
}
