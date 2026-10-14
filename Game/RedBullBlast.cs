namespace LFG;

/// <summary>The splash when a thrown Red Bull can hits something.</summary>
public class RedBullBlast : FlxEmitter
{
    public RedBullBlast(float x, float y)
    {
        X = x + 2;
        Y = y + 2;
        Width = 6;
        Height = 6;
        LoadParticles(LfgAssets.Image("RedBullBlast_particle"), 60, 16, false);
        Velocity.Set(-30, -30, 30, 30);
        Lifespan.Set(0.6f);
        Elasticity.Set(0);
        FlxG.State.Add(this);
        Start(true, 0.1f);
    }

    public override void Update(float elapsed)
    {
        base.Update(elapsed);
        if (CountLiving() <= 0) Kill();
    }
}
