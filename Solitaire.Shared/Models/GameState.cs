namespace Solitaire.Shared.Models;

public class GameState
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public List<Pile> Piles { get; set; } = [];
    public int Score { get; set; }
    public TimeSpan ElapsedTime { get; set; }
    public bool IsGameWon { get; set; }
    public int DrawMode { get; set; } = 1;
}
