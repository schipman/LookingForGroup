namespace LFG;

public class Credits : FlxState
{
    public override void Create()
    {
        Add(new FlxSprite(0, 0, LfgAssets.Image("Credits_creditsSprite")));
        base.Create();
    }

    public override void Update(float elapsed)
    {
        if (FlxG.Mouse.Pressed) FlxG.SwitchState(new MainMenu());
        base.Update(elapsed);
    }
}
