using lasertech_backend.DTOs;
using lasertech_backend.Interface;
using lasertech_backend.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace lasertech_backend.Controllers;

[Route("api/[controller]")]
[ApiController]
public class PlayerSessionController : ControllerBase
{
    private readonly GameContext Context;
    private GameUdpService Udp;

    public PlayerSessionController(GameContext context, GameUdpService Udp)
    {
        this.Context = context;
        this.Udp = Udp;
    }

    [HttpGet]
    public async Task<ActionResult<List<PlayerSession>>> Get()
    {
        var playerSessions = await Context.playerSessions.ToListAsync();
        return Ok(playerSessions);
    }

    [HttpPost]
    public async Task<ActionResult<PlayerSession>> Post([FromBody] PlayerSessionDTO p)
    {
        var playerSession = new PlayerSession(p);
        Context.Add(playerSession);
        await Context.SaveChangesAsync();
        Udp.AddEquipmentID(p.EquipmentID);
        return Ok(playerSession);
    }
    
    
}