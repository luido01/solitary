using System.Text.Json;
using Solitaire.Shared.Models;

namespace Solitaire.Server.Services;

public class UndoService
{
    private readonly Stack<GameState> _history = new();

    public void Save(GameState state)
    {
        _history.Push(Clone(state));
    }

    public bool CanUndo => _history.Count > 0;

    public GameState? Undo()
    {
        return _history.Count > 0 ? _history.Pop() : null;
    }

    public void Clear() => _history.Clear();

    private static GameState Clone(GameState state)
    {
        var json = JsonSerializer.Serialize(state);
        return JsonSerializer.Deserialize<GameState>(json) ?? new GameState();
    }
}
