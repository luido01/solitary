using Microsoft.AspNetCore.Mvc;
using Solitaire.Server.Services;
using Solitaire.Shared.DTOs;

namespace Solitaire.Server.Controllers;

[ApiController]
[Route("api/[controller]")]
public class GameController(IGameService gameService) : ControllerBase
{
    [HttpPost("new")]
    public async Task<IActionResult> NewGame([FromBody] NewGameRequest request)
    {
        var state = await gameService.InitializeNewGameAsync(request.DrawMode);
        return Ok(state);
    }

    [HttpPost("move")]
    public async Task<IActionResult> Move([FromBody] MoveCardRequest request)
    {
        try
        {
            var state = await gameService.MoveCardAsync(request.SourcePileId, request.TargetPileId, request.CardId);
            return Ok(state);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpPost("draw")]
    public async Task<IActionResult> Draw()
    {
        var state = await gameService.DrawCardAsync();
        return Ok(state);
    }

    [HttpPost("undo")]
    public async Task<IActionResult> Undo()
    {
        var state = await gameService.UndoLastMoveAsync();
        return Ok(state);
    }

    [HttpGet("state")]
    public async Task<IActionResult> State()
    {
        var state = await gameService.GetGameStateAsync();
        return Ok(state);
    }
}
