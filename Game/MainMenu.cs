namespace LFG;

public class MainMenu : FlxState
{
    FloodFillFX _floodFill = null!;

    public override void Create()
    {
        FlxG.Sound.PlayMusic(LfgAssets.Sound("MainMenu_menuMusic"), 0.3f);
        FlxG.Mouse.Visible = true;

        var background = new FlxSprite(0, 0, LfgAssets.Image("MainMenu_menuBack"));

        // the title picture fills itself in from the bottom up
        _floodFill = FlxSpecialFX.FloodFill();
        var title = new FlxSprite(0, 0, LfgAssets.Image("MainMenu_menuSprite"));
        var fill = _floodFill.Create(title, 0, 0, (int)title.Width, (int)title.Height);
        fill.ScrollFactor = Vector2.Zero;
        _floodFill.Start();

        Add(background);
        Add(fill);
        Add(new FlxButton(120, 150, "Start Game", StartGame));
        Add(new FlxButton(120, 180, "Credits", () => FlxG.SwitchState(new Credits())));
        _effects = new FlxButton(120, 210, "", OpenEffects);
        Add(_effects);
        base.Create();
    }

    FlxButton _effects = null!;

    void OpenEffects() => OpenSubState(new PostProcessMenuState());

    public override void Update(float elapsed)
    {
        bool effectsWereOpen = SubState != null;     // Escape that closes the effects menu must not also quit
        base.Update(elapsed);
        if (SubState != null || effectsWereOpen) return;
        if (FlxG.Keys.JustPressed(Keys.Escape)) { FlxG.Game.Exit(); return; }
        int count = FlxG.PostProcess.Global.Count;
        _effects.Label.Text = count == 0 ? "Effects (F)" : $"Effects: {count} (F)";
        if (FlxG.Keys.JustPressed(Keys.F)) OpenEffects();
    }

    void StartGame()
    {
        FlxG.Sound.Music?.FadeOut(2);
        FlxG.Camera.Fade(Color.Black, 2, false, () => FlxG.SwitchState(new PlayState()));
        FlxG.Mouse.Visible = false;
    }

    public override void Destroy()
    {
        FlxSpecialFX.Clear();
        base.Destroy();
    }
}
