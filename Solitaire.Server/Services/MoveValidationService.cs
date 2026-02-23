using Solitaire.Shared.Enums;
using Solitaire.Shared.Models;

namespace Solitaire.Server.Services;

public class MoveValidationService
{
    public bool IsValidMove(Pile source, Pile target, List<Card> cardsToMove)
    {
        if (cardsToMove.Count == 0 || cardsToMove.Any(c => !c.IsFaceUp))
        {
            return false;
        }

        return target.Type switch
        {
            PileType.Tableau => IsValidTableauMove(target, cardsToMove.First()),
            PileType.Foundation => cardsToMove.Count == 1 && IsValidFoundationMove(target, cardsToMove.First()),
            _ => false
        };
    }

    private static bool IsValidTableauMove(Pile target, Card movingCard)
    {
        if (target.Cards.Count == 0)
        {
            return movingCard.Rank == Rank.King;
        }

        var topCard = target.Cards.Last();
        if (!topCard.IsFaceUp)
        {
            return false;
        }

        return topCard.IsRed != movingCard.IsRed && (int)topCard.Rank == (int)movingCard.Rank + 1;
    }

    private static bool IsValidFoundationMove(Pile target, Card movingCard)
    {
        if (target.Cards.Count == 0)
        {
            return movingCard.Rank == Rank.Ace;
        }

        var topCard = target.Cards.Last();
        return topCard.Suit == movingCard.Suit && (int)movingCard.Rank == (int)topCard.Rank + 1;
    }
}
