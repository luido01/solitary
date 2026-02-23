using Solitaire.Shared.Enums;

namespace Solitaire.Shared.Models;

public class Card
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Suit Suit { get; set; }
    public Rank Rank { get; set; }
    public bool IsFaceUp { get; set; }

    public bool IsRed => Suit is Suit.Hearts or Suit.Diamonds;
}
