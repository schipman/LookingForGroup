using LFG;
using MonoFlixel;

// Looking For Group: a Ludum Dare 22 entry (theme: Alone), ported from the original Flixel/Flash version.
// The Flash game ran at 320x240 shown at 2x (a 640x480 window).
// Testing shortcuts: --play skips the menu, --score opens the end screen with sample numbers.
FlxState Start() =>
    args.Contains("--play") ? new PlayState() :
    args.Contains("--score") ? new HighScore(new ScoreManager { WizardAmount = 7, WarriorAmount = 5, ClericAmount = 4, RogueAmount = 6, TotalDice = 85, BulliesDefeated = 3 }) :
    new MainMenu();

using var game = new FlxGame(320, 240, Start, zoom: 2, framerate: 60, title: "Looking For Group");
game.Run();
