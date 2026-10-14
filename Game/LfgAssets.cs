namespace LFG;

/// <summary>Where the game's files live (replaces the [Embed] classes of the Flash version).</summary>
public static class LfgAssets
{
    /// <summary>Full path of an image, e.g. Image("Player_playerPNG"). Names are the original embed class names.</summary>
    public static string Image(string name) => FlxAssets.Get($"lfg/images/{name}.png");

    /// <summary>Sound name for FlxG.Sound, e.g. Sound("PlayState_throw1").</summary>
    public static string Sound(string name) => $"lfg/sounds/{name}.mp3";

    /// <summary>Contents of a map CSV, e.g. Csv("PlayState_mapCSV").</summary>
    public static string Csv(string name) => File.ReadAllText(FlxAssets.Get($"lfg/data/{name}.csv"));
}
