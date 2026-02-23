namespace Solitaire.Server.Services;

public class ScoringService
{
    public const int MoveToFoundation = 10;
    public const int FlipCard = 5;
    public const int WasteToTableau = 5;
    public const int UndoPenalty = -15;

    public int ApplyFoundationMove(int score) => score + MoveToFoundation;
    public int ApplyFlip(int score) => score + FlipCard;
    public int ApplyWasteToTableau(int score) => score + WasteToTableau;
    public int ApplyUndoPenalty(int score) => Math.Max(0, score + UndoPenalty);
}
