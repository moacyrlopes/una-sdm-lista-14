using CacauShowApi.Data;
using CacauShowApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CacauShowApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class FranquiaController : ControllerBase
{
    private readonly AppDbContext _context;

    public FranquiaController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> Get()
    {
        var franquias = await _context.Franquias.ToListAsync();
        return Ok(franquias);
    }

    [HttpPost]
    public async Task<IActionResult> Post(Franquia franquia)
    {
        _context.Franquias.Add(franquia);
        await _context.SaveChangesAsync();

        return Ok(franquia);
    }
}