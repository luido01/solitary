using Solitaire.Shared.Enums;
using Solitaire.Shared.Models;

namespace Solitaire.Server.Services;

public class GameService(
    DeckService deckService,
    MoveValidationService moveValidationService,
    ScoringService scoringService,
    UndoService undoService) : IGameService
{
    private readonly object _sync = new();
    private GameState _state = new();
    private DateTimeOffset _startedAt = DateTimeOffset.UtcNow;

    public Task<GameState> InitializeNewGameAsync(int drawMode)
    {
        lock (_sync)
        {
            undoService.Clear();
            var deck = deckService.CreateShuffledDeck();

            var piles = new List<Pile>();
            var stock = new Pile { Type = PileType.Stock, Index = 0 };
            var waste = new Pile { Type = PileType.Waste, Index = 0 };
            piles.Add(stock);
            piles.Add(waste);

            for (var i = 0; i < 4; i++)
            {
                piles.Add(new Pile { Type = PileType.Foundation, Index = i });
            }

            for (var i = 0; i < 7; i++)
            {
                var tableau = new Pile { Type = PileType.Tableau, Index = i };
                for (var j = 0; j <= i; j++)
                {
                    var card = deck[0];
                    deck.RemoveAt(0);
                    card.IsFaceUp = j == i;
                    tableau.Cards.Add(card);
                }
                piles.Add(tableau);
            }

            stock.Cards.AddRange(deck);

            _state = new GameState
            {
                Id = Guid.NewGuid(),
                DrawMode = drawMode is 3 ? 3 : 1,
                Piles = piles,
                Score = 0,
                IsGameWon = false,
                ElapsedTime = TimeSpan.Zero
            };

            _startedAt = DateTimeOffset.UtcNow;
            return Task.FromResult(CloneAndRefreshElapsed());
        }
    }

    public Task<GameState> DrawCardAsync()
    {
        lock (_sync)
        {
            EnsureInitialized();
            undoService.Save(_state);

            var stock = GetPile(PileType.Stock, 0);
            var waste = GetPile(PileType.Waste, 0);

            if (stock.Cards.Count == 0)
            {
                foreach (var card in waste.Cards)
                {
                    card.IsFaceUp = false;
                }

                stock.Cards.AddRange(waste.Cards.AsEnumerable().Reverse());
                waste.Cards.Clear();
                return Task.FromResult(CloneAndRefreshElapsed());
            }

            var drawCount = Math.Min(_state.DrawMode, stock.Cards.Count);
            for (var i = 0; i < drawCount; i++)
            {
                var card = stock.Cards.Last();
                stock.Cards.RemoveAt(stock.Cards.Count - 1);
                card.IsFaceUp = true;
                waste.Cards.Add(card);
            }

            CheckWinCondition();
            return Task.FromResult(CloneAndRefreshElapsed());
        }
    }

    public Task<GameState> MoveCardAsync(Guid sourcePileId, Guid targetPileId, Guid cardId)
    {
        lock (_sync)
        {
            EnsureInitialized();

            var source = _state.Piles.FirstOrDefault(p => p.Id == sourcePileId)
                ?? throw new InvalidOperationException("Source pile not found.");
            var target = _state.Piles.FirstOrDefault(p => p.Id == targetPileId)
                ?? throw new InvalidOperationException("Target pile not found.");

            var cardIndex = source.Cards.FindIndex(c => c.Id == cardId);
            if (cardIndex < 0)
            {
                throw new InvalidOperationException("Card not found in source pile.");
            }

            var cardsToMove = source.Type == PileType.Tableau
                ? source.Cards.Skip(cardIndex).ToList()
                : [source.Cards[cardIndex]];

            if (!moveValidationService.IsValidMove(source, target, cardsToMove))
            {
                throw new InvalidOperationException("Illegal move.");
            }

            undoService.Save(_state);

            foreach (var card in cardsToMove)
            {
                source.Cards.Remove(card);
                target.Cards.Add(card);
            }

            ApplyScoreForMove(source, target);
            FlipTopCardIfNeeded(source);
            CheckWinCondition();

            return Task.FromResult(CloneAndRefreshElapsed());
        }
    }

    public Task<GameState> UndoLastMoveAsync()
    {
        lock (_sync)
        {
            EnsureInitialized();
            var restored = undoService.Undo() ?? _state;
            restored.Score = scoringService.ApplyUndoPenalty(restored.Score);
            _state = restored;
            CheckWinCondition();
            return Task.FromResult(CloneAndRefreshElapsed());
        }
    }

    public Task<GameState> GetGameStateAsync()
    {
        lock (_sync)
        {
            EnsureInitialized();
            return Task.FromResult(CloneAndRefreshElapsed());
        }
    }

    private void ApplyScoreForMove(Pile source, Pile target)
    {
        if (target.Type == PileType.Foundation)
        {
            _state.Score = scoringService.ApplyFoundationMove(_state.Score);
        }

        if (source.Type == PileType.Waste && target.Type == PileType.Tableau)
        {
            _state.Score = scoringService.ApplyWasteToTableau(_state.Score);
        }
    }

    private void FlipTopCardIfNeeded(Pile source)
    {
        if (source.Type != PileType.Tableau || source.Cards.Count == 0)
        {
            return;
        }

        var top = source.Cards.Last();
        if (!top.IsFaceUp)
        {
            top.IsFaceUp = true;
            _state.Score = scoringService.ApplyFlip(_state.Score);
        }
    }

    private void CheckWinCondition()
    {
        _state.IsGameWon = _state.Piles
            .Where(p => p.Type == PileType.Foundation)
            .All(p => p.Cards.Count == 13);
    }

    private Pile GetPile(PileType type, int index)
    {
        return _state.Piles.First(p => p.Type == type && p.Index == index);
    }

    private void EnsureInitialized()
    {
        if (_state.Piles.Count == 0)
        {
            InitializeNewGameAsync(1).GetAwaiter().GetResult();
        }
    }

    private GameState CloneAndRefreshElapsed()
    {
        var clone = Clone(_state);
        clone.ElapsedTime = DateTimeOffset.UtcNow - _startedAt;
        return clone;
    }

    private static GameState Clone(GameState state)
    {
        return new GameState
        {
            Id = state.Id,
            DrawMode = state.DrawMode,
            Score = state.Score,
            IsGameWon = state.IsGameWon,
            ElapsedTime = state.ElapsedTime,
            Piles = state.Piles.Select(p => new Pile
            {
                Id = p.Id,
                Type = p.Type,
                Index = p.Index,
                Cards = p.Cards.Select(c => new Card
                {
                    Id = c.Id,
                    Suit = c.Suit,
                    Rank = c.Rank,
                    IsFaceUp = c.IsFaceUp
                }).ToList()
            }).ToList()
        };
    }
}
