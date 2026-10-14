namespace LFG;

/// <summary>An invisible marker on the map that spawns a pedestrian (or, more and more often, a bully) whenever it is off screen.</summary>
public class PedSpawner : FlxSprite
{
    readonly FlxGroup _peds, _bullies, _bulletGroup;
    readonly ScoreManager _score;
    readonly Player _player;
    float _timer;
    float _bullyChance = 20;

    public PedSpawner(int column, int row, FlxGroup peds, FlxGroup bullies, FlxGroup bulletGroup, ScoreManager score, Player player)
        : base(column * 16, row * 16)
    {
        MakeGraphic(16, 16, Color.White);
        _timer = FlxG.Random.Float() * 10;
        _peds = peds;
        _bullies = bullies;
        _bulletGroup = bulletGroup;
        _score = score;
        _player = player;
        Visible = false;
    }

    void Spawn()
    {
        if (FlxG.Random.Float() * 100 >= 100 - _bullyChance)
            ((Bully)_bullies.Recycle(() => new Bully())).Init(X, Y, _bulletGroup, _player, _score);
        else
            ((Pedestrian)_peds.Recycle(() => new Pedestrian())).Init(X, Y, _player);
    }

    public override void Update(float elapsed)
    {
        _timer += elapsed;
        _bullyChance += elapsed / 12;
        if (_timer > 10 && !IsOnScreen())
        {
            _timer = 0;
            Spawn();
        }
        base.Update(elapsed);
    }
}
