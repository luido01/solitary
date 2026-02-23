using Solitaire.Shared.Enums;
using Solitaire.Shared.Models;

namespace Solitaire.Client.Services;

public class GameStateStore(GameApiClient apiClient)
{
    public GameState? State { get; private set; }

    public event Action? OnChange;

    public async Task InitializeAsync()
    {
        State = await apiClient.GetStateAsync();
        Notify();
    }

    public async Task NewGameAsync(int drawMode)
    {
        State = await apiClient.NewGameAsync(drawMode);
        Notify();
    }

    public async Task DrawAsync()
    {
        State = await apiClient.DrawAsync();
        Notify();
    }

    public async Task MoveAsync(Guid sourcePileId, Guid targetPileId, Guid cardId)
    {
        State = await apiClient.MoveAsync(sourcePileId, targetPileId, cardId);
        Notify();
    }

    public async Task UndoAsync()
    {
        State = await apiClient.UndoAsync();
        Notify();
    }

    public Pile? GetPile(PileType type, int index)
    {
        return State?.Piles.FirstOrDefault(x => x.Type == type && x.Index == index);
    }

    private void Notify() => OnChange?.Invoke();
}
