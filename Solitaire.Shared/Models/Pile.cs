using Solitaire.Shared.Enums;

namespace Solitaire.Shared.Models;

public class Pile
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public PileType Type { get; set; }
    public List<Card> Cards { get; set; } = [];
    public int Index { get; set; }
}
