
namespace EscapeFromDungeon.Core
{
    public class GameStateManager
    {
        private static GameStateManager? _instance;
        public static GameStateManager Instance => _instance ??= new GameStateManager();

        public GameMode CurrentMode { get; private set; } = GameMode.Title;

        public void ChangeMode(GameMode newMode) => CurrentMode = newMode;
    }

    public enum GameMode
    {
        Title,
        Explore,
        Battle,
        Escaped,
        BattleEnd,
        Gameover,
        GameClear,
        Reset
    }
}
