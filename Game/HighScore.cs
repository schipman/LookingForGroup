namespace LFG;

/// <summary>The end screen: how many of each class you recruited, typed out one line after another.</summary>
public class HighScore : FlxState
{
    static readonly Color Gold = new(255, 214, 90);
    static readonly Color Cream = new(238, 228, 205);

    readonly ScoreManager _score;
    readonly List<FlxTypeText> _lines = new();

    public HighScore(ScoreManager score) => _score = score;

    public override void Create()
    {
        FlxG.Mouse.Visible = true;
        Add(FlxGradient.CreateSprite(FlxG.Width, FlxG.Height,
            new[] { new Color(14, 10, 38), new Color(58, 22, 84), new Color(128, 38, 58) }));

        int total = (_score.WizardAmount + _score.ClericAmount + _score.RogueAmount + _score.TotalDice + _score.BulliesDefeated) * 100;

        AddLine(50, 22, "Congratulations Dungeon Master!", Gold);
        AddLine(70, 50, $"Wizards Recruited: {_score.WizardAmount}", Cream);
        AddLine(70, 66, $"Warriors Recruited: {_score.WarriorAmount}", Cream);
        AddLine(70, 82, $"Clerics Recruited: {_score.ClericAmount}", Cream);
        AddLine(70, 98, $"Rogues Recruited: {_score.RogueAmount}", Cream);
        AddLine(70, 114, $"D20s Collected: {_score.TotalDice}", Cream);
        AddLine(70, 130, $"Bullys Defeated: {_score.BulliesDefeated}", Cream);
        AddLine(70, 156, $"Final Score: {total}", Gold, size: 16);

        base.Create();
        FlxG.Sound.PlayMusic(LfgAssets.Sound("HighScore_menuMusic"), 0.3f);
        TypeLine(0);
    }

    void AddLine(float x, float y, string text, Color color, int size = 8)
    {
        var line = new FlxTypeText(x, y, 500, text, size) { UseDefaultSound = true, DefaultSoundVolume = 0.2f, SkipOnClick = true };
        line.SetFormat(size, color);
        _lines.Add(line);
        Add(line);
    }

    /// <summary>Types line n, then the next one when it is done; the button shows up after the last.</summary>
    void TypeLine(int n)
    {
        if (n >= _lines.Count)
        {
            Add(new FlxButton(130, 204, "Play Again?", PlayAgain));
            return;
        }
        _lines[n].Start(0.03f, skipKeys: new[] { Keys.Space, Keys.Enter }, callback: () => TypeLine(n + 1));
    }

    void PlayAgain()
    {
        FlxG.Mouse.Visible = false;
        FlxG.Sound.Music?.FadeOut(0.1f);
        FlxG.SwitchState(new PlayState());
    }

    public override void Update(float elapsed)
    {
        base.Update(elapsed);
        if (FlxG.Keys.JustPressed(Keys.Escape)) FlxG.SwitchState(new MainMenu());
    }
}
