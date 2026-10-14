namespace LFG;

/// <summary>The "OMG!" and "!" bubbles that pop up over people who spot you.</summary>
public class SpeechBubble : FlxSprite
{
    readonly Actor _parent;
    float _lifetime;

    /// <param name="kind">0 omg, 1 nerd, 2 panic, 3 exclamation.</param>
    public SpeechBubble(int kind, float lifetime, Actor parent)
    {
        _lifetime = lifetime;
        _parent = parent;
        string name = kind switch { 0 => "SpeechBubble_omgBubble", 1 => "SpeechBubble_nerdBubble", 2 => "SpeechBubble_panicBubble", _ => "SpeechBubble_excBubble" };
        LoadGraphic(LfgAssets.Image(name));
        FlxG.State.Add(this);
    }

    public override void Update(float elapsed)
    {
        if (!_parent.IsAlive) Kill();
        if (_lifetime <= 0)
        {
            Alpha -= elapsed * 30;
            if (Alpha <= 0) Kill();
        }
        _lifetime -= elapsed;
        X = _parent.X + 10;
        Y = _parent.Y - 15;
    }
}
