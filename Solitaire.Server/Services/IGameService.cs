using Solitaire.Shared.Models;

namespace Solitaire.Server.Services;

public interface IGameService
{
    Task<GameState> InitializeNewGameAsync(int drawMode);
    Task<GameState> DrawCardAsync();
    Task<GameState> MoveCardAsync(Guid sourcePileId, Guid targetPileId, Guid cardId);
    Task<GameState> UndoLastMoveAsync();
    Task<GameState> GetGameStateAsync();
}
