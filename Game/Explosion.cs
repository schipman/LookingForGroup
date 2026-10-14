namespace LFG;

/// <summary>The puff of smoke when someone becomes a nerd.</summary>
public class Explosion : FlxSprite
{
    public Explosion(float x, float y) : base(x, y)
    {
        LoadGraphic(LfgAssets.Image("Explosion_poof"), true, 32, 32);
        Animation.Add("explode", new[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 }, 25, false);
        FlxG.State.Add(this);
        Animation.Play("explode");
    }

    public override void Update(float elapsed)
    {
        base.Update(elapsed);
        if (Animation.Finished) Kill();
    }
}
