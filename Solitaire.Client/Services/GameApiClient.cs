using Solitaire.Shared.DTOs;
using Solitaire.Shared.Models;

namespace Solitaire.Client.Services;

public class GameApiClient(HttpClient http)
{
    public async Task<GameState> NewGameAsync(int drawMode)
    {
        var response = await http.PostAsJsonAsync("api/game/new", new NewGameRequest { DrawMode = drawMode });
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<GameState>() ?? new GameState();
    }

    public async Task<GameState> DrawAsync()
    {
        var response = await http.PostAsync("api/game/draw", null);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<GameState>() ?? new GameState();
    }

    public async Task<GameState> MoveAsync(Guid sourcePileId, Guid targetPileId, Guid cardId)
    {
        var response = await http.PostAsJsonAsync("api/game/move", new MoveCardRequest
        {
            SourcePileId = sourcePileId,
            TargetPileId = targetPileId,
            CardId = cardId
        });

        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<GameState>() ?? new GameState();
    }

    public async Task<GameState> UndoAsync()
    {
        var response = await http.PostAsync("api/game/undo", null);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<GameState>() ?? new GameState();
    }

    public async Task<GameState> GetStateAsync()
    {
        var response = await http.GetAsync("api/game/state");
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<GameState>() ?? new GameState();
    }
}
