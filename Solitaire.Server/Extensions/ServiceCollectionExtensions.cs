using Solitaire.Server.Services;

namespace Solitaire.Server.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddGameServices(this IServiceCollection services)
    {
        services.AddSingleton<DeckService>();
        services.AddSingleton<MoveValidationService>();
        services.AddSingleton<ScoringService>();
        services.AddSingleton<UndoService>();
        services.AddSingleton<IGameService, GameService>();
        return services;
    }
}
