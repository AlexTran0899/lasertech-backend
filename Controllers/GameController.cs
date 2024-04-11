using lasertech_backend.DTOs;
using lasertech_backend.Enum;
using lasertech_backend.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace lasertech_backend.Controllers;

[Route("api/[controller]")]
[ApiController]
public class GameController : ControllerBase
{
    private readonly GameContext Context;
    private CancellationTokenSource udpListenCancellationTokenSource;
    private CancellationTokenSource udpBroadcastCancellationTokenSource;
    private GameUdpService gameUdp;
    private Game game;
    
    public GameController(GameContext context, GameUdpService gameUdp)
    {
        this.Context = context;
        this.udpListenCancellationTokenSource = new CancellationTokenSource();
        this.udpBroadcastCancellationTokenSource = new CancellationTokenSource();
        this.gameUdp = gameUdp;
    }

    [HttpGet]
    public async Task<ActionResult<List<Game>>> Get()
    {
        var games = await Context.games.ToListAsync();
        return Ok(games);
    }

    [HttpGet("{gameID}")]
    public async Task<ActionResult<Game>> Get(int gameID)
    {
        var game = await Context.games
            .Include(g => g.PlayerSessions)
            .FirstOrDefaultAsync(g => g.GameID == gameID);
        return Ok(game);
    }
    
    [HttpPatch("status")]
    public async Task<ActionResult<Game>> Patch(GameStatus gameStatus)
    {
        if (gameStatus == GameStatus.Start)
        {
            udpBroadcastCancellationTokenSource.Cancel();
            gameUdp.broadcastGameStatus(gameStatus);
            Task.Run(() => gameUdp.StartListening(udpListenCancellationTokenSource.Token), 
                udpListenCancellationTokenSource.Token);
        }
        else
        {
            gameUdp.broadcastGameStatus(gameStatus);
        }
        return Ok(game);
    }

    [HttpPost]
    public async Task<ActionResult<Game>> Post()
    {
        this.game = new Game();
        Context.Add(game);
        await Context.SaveChangesAsync();
        Task.Run(() => gameUdp.StartBroadcastEquipment(udpBroadcastCancellationTokenSource.Token),
            udpBroadcastCancellationTokenSource.Token);
        return Ok(game);
    }

    private void ManageScore(string data)
    {
        string[] nums = data.Split(':');
        if (nums[1] == "53" || nums[1] == "43")
        {
            
        }
        else
        {
            
        }
    }
    
}