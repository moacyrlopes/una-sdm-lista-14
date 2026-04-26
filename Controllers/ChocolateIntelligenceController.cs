using CacauShowApi.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CacauShowApi.Controllers;

[ApiController]
[Route("api/intelligence")]
public class ChocolateIntelligenceController : ControllerBase
{
    private readonly AppDbContext _context;

    public ChocolateIntelligenceController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet("estoque-regional")]
    public async Task<IActionResult> EstoqueRegional()
    {
        Thread.Sleep(2000);

        var resultado = await _context.Pedidos
            .Include(p => p.Unidade)
            .GroupBy(p => p.Unidade!.Cidade)
            .Select(g => new
            {
                Cidade = g.Key,
                TotalItensVendidos = g.Sum(p => p.Quantidade)
            })
            .ToListAsync();

        return Ok(resultado);
    }
}