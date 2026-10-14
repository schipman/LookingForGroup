namespace LFG;

public class PlayState : FlxState
{
    // the dice come in seven colors
    static readonly int[] DiceColors = { 0x7711FF, 0x006655, 0xFFDD00, 0xEEDDEE, 0xFF0000, 0x885577, 0xDDAA77 };

    readonly ScoreManager _score = new();

    Player _player = null!;
    FlxText _diceText = null!;
    FlxTilemap _collideLevel = null!;
    Weapon _d20 = null!;
    FlxGroup _pedestrians = null!, _bullies = null!, _bullyBullets = null!, _graves = null!, _pedSpawners = null!;
    bool _justThrew;

    public override void Create()
    {
        FlxG.Mouse.Visible = true;
        _player = new Player(495, 231);

        string tiles = LfgAssets.Image("PlayState_mapTilesPNG");
        var level = new FlxTilemap().LoadMapFromCsv(LfgAssets.Csv("PlayState_mapCSV"), tiles, 16, 16);
        _collideLevel = new FlxTilemap().LoadMapFromCsv(LfgAssets.Csv("PlayState_buildingsCSV"), tiles, 16, 16);
        var decoLevel = new FlxTilemap().LoadMapFromCsv(LfgAssets.Csv("PlayState_decoCSV"), tiles, 16, 16);
        float levelWidth = level.Width, levelHeight = level.Height;

        _diceText = new FlxText(25, 12, 100, "Dice:" + _player.DiceNumber) { ScrollFactor = Vector2.Zero };
        var d20Gui = new FlxSprite(10, 13, LfgAssets.Image("PlayState_d20GUISprite")) { ScrollFactor = Vector2.Zero };

        MakeDice(levelWidth, levelHeight);

        Add(level);
        _graves = new FlxGroup();
        Add(_graves);
        Add(_player);
        Add(_d20.Group);
        Add(_collideLevel);
        Add(decoLevel);

        _score.TotalDice = _player.DiceNumber;

        _pedSpawners = new FlxGroup();
        _pedestrians = new FlxGroup();
        _bullies = new FlxGroup();
        _bullyBullets = new FlxGroup();
        Add(_pedSpawners);
        Add(_pedestrians);
        Add(_bullies);
        Add(_bullyBullets);
        ParsePeds();

        Add(d20Gui);
        Add(_diceText);

        FlxG.Camera.Bounds = new Rectangle(0, 0, (int)levelWidth, (int)levelHeight);
        FlxG.Camera.Follow(_player, FlxCameraFollowStyle.TopDownTight);
        FlxG.Sound.PlayMusic(LfgAssets.Sound("PlayState_actionMusic"), 0.4f);
        base.Create();
    }

    /// <summary>The player's weapon: a pool of 150 d20s that spin, slow down and bounce.</summary>
    void MakeDice(float levelWidth, float levelHeight)
    {
        _d20 = new Weapon(_player)
        {
            FireRateMs = 300,
            BulletSpeed = 200,
            BulletLifeSpanMs = 1000,
            RandomAngle = 5,
            RandomSpeed = 10,
            BulletElasticity = 0.3f,
            Bounds = new Rectangle(0, 0, (int)levelWidth, (int)levelHeight),
            OnFire = DiceAudio,
        };
        _d20.MakeImageBullet(150, LfgAssets.Image("PlayState_d20Sprite"));
        foreach (var die in _d20.Group.Members)
        {
            die.AngularVelocity = 500;
            die.AngularDrag = 100;
            die.Drag = new Vector2(75, 75);
            die.Tint = FlxColor.Rgb(DiceColors[(int)(FlxG.Random.Float() * 7)]);
        }
    }

    void DiceAudio()
    {
        int n = FlxG.Random.Int(1, 2);
        FlxG.Sound.Play(LfgAssets.Sound($"PlayState_diceRoll{n}"), 0.7f);
        FlxG.Sound.Play(LfgAssets.Sound($"PlayState_throw{n}"), 1f);
    }

    public override void Update(float elapsed)
    {
        if (FlxG.Keys.JustPressed(Keys.Escape))
        {
            FlxG.Sound.Music?.FadeOut(0.3f);
            FlxG.SwitchState(new MainMenu());
            return;
        }

        base.Update(elapsed);
        _diceText.Text = "Dice:" + _player.DiceNumber;
        FlxG.Collide(_collideLevel, _player);

        if (_player.DiceNumber <= 0)
        {
            FlxG.Sound.Music?.FadeOut(0.1f);
            FlxG.SwitchState(new HighScore(_score));
            return;
        }

        // one throw per click; a die is only spent when one actually flies (the weapon reloads for 300 ms between throws,
        // and holding the button throws as soon as it is ready)
        if (FlxG.Mouse.Pressed && _player.DiceNumber > 0 && !_justThrew && _d20.FireFromAngle(_player.Angle - 90))
        {
            _player.DiceNumber--;
            _justThrew = true;
        }
        if (FlxG.Mouse.JustReleased) _justThrew = false;

        FlxG.Collide(_collideLevel, _d20.Group);
        FlxG.Collide(_collideLevel, _pedestrians, Bounce);
        FlxG.Collide(_collideLevel, _bullies, Bounce);
        FlxG.Overlap(_d20.Group, _pedestrians, PedHit);
        FlxG.Collide(_d20.Group, _bullies, BullyHit);
        FlxG.Collide(_bullyBullets, _player, PlayerHit);
        FlxG.Collide(_collideLevel, _bullyBullets, CanExplode);
    }

    void CanExplode(FlxObject tiles, FlxObject can)
    {
        new RedBullBlast(can.X, can.Y);
        FlxG.Sound.Play(LfgAssets.Sound("PlayState_redbullBurst"), 0.9f);
        can.Kill();
    }

    void PlayerHit(FlxObject can, FlxObject player)
    {
        new RedBullBlast(player.X, player.Y);
        can.Kill();
        _player.DiceNumber -= 5;
        FlxG.Sound.Play(LfgAssets.Sound("PlayState_playerHitSound"), 0.9f);
        FlxFlicker.Flicker(_player, 1);
    }

    void PedHit(FlxObject die, FlxObject other)
    {
        var ped = (Pedestrian)other;
        if (!ped.IsAlive) return;

        die.Kill();
        _pedestrians.Remove(ped);
        switch (ped.PedType)
        {
            case 1: _score.WarriorAmount++; break;
            case 2: _score.RogueAmount++; break;
            case 3: _score.ClericAmount++; break;
            case 4: _score.WizardAmount++; break;
        }
        _graves.Add(ped);
        new Explosion(ped.X - 9, ped.Y - 8);
        FlxG.Sound.Play(LfgAssets.Sound("PlayState_smokeSound"), 1f);
        ped.Nerdify();
    }

    void BullyHit(FlxObject die, FlxObject other)
    {
        var bully = (Bully)other;
        if (!bully.IsAlive) return;

        bully.Attack(FlxG.Elapsed);
        bully.Hits--;
        die.Kill();
        if (bully.Hits == 0)
        {
            _bullies.Remove(bully);
            _score.BulliesDefeated++;
            _graves.Add(bully);
            bully.NerdifyBully();
            new Explosion(bully.X - 8, bully.Y - 8);
            FlxG.Sound.Play(LfgAssets.Sound("PlayState_smokeSound"), 0.7f);
        }
        else
        {
            FlxFlicker.Flicker(bully, 1);
            FlxG.Sound.Play(LfgAssets.Sound("PlayState_bullyHitSound"), 0.8f);
        }
    }

    /// <summary>Walkers that bump into a wall pick a new direction.</summary>
    static void Bounce(FlxObject tiles, FlxObject walker) => ((Actor)walker).GetDirection();

    /// <summary>Reads the spawner map: a 1 or a 2 marks a place people appear.</summary>
    void ParsePeds()
    {
        var spawnMap = new FlxTilemap().LoadMapFromCsv(LfgAssets.Csv("PlayState_ped1CSV"), LfgAssets.Image("PlayState_pedSprite"), 16, 16);
        for (int row = 0; row < spawnMap.HeightInTiles; row++)
            for (int column = 0; column < spawnMap.WidthInTiles; column++)
            {
                int tile = spawnMap.GetTile(column, row);
                if (tile == 1 || tile == 2)
                    _pedSpawners.Add(new PedSpawner(column, row, _pedestrians, _bullies, _bullyBullets, _score, _player));
            }
    }
}
