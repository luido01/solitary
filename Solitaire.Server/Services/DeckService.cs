using Solitaire.Shared.Enums;
using Solitaire.Shared.Models;

namespace Solitaire.Server.Services;

public class DeckService
{
    public List<Card> CreateShuffledDeck()
    {
        var deck = Enum.GetValues<Suit>()
            .SelectMany(suit => Enum.GetValues<Rank>()
                .Select(rank => new Card
                {
                    Id = Guid.NewGuid(),
                    Suit = suit,
                    Rank = rank,
                    IsFaceUp = false
                }))
            .OrderBy(_ => Random.Shared.Next())
            .ToList();

        return deck;
    }
}
