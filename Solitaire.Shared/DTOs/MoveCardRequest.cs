namespace Solitaire.Shared.DTOs;

public class MoveCardRequest
{
    public Guid SourcePileId { get; set; }
    public Guid TargetPileId { get; set; }
    public Guid CardId { get; set; }
}
