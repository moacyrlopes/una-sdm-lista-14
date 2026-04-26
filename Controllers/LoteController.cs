using CacauShowApi.Data;
using CacauShowApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CacauShowApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class LoteController : ControllerBase
{
    private readonly AppDbContext _context;

    public LoteController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> Get()
    {
        var lotes = await _context.LotesProducao
            .Include(l => l.Produto)
            .ToListAsync();

        return Ok(lotes);
    }

    [HttpPost]
    public async Task<IActionResult> Post(LoteProducao lote)
    {
        var produtoExiste = await _context.Produtos.AnyAsync(p => p.Id == lote.ProdutoId);

        if (!produtoExiste)
        {
            return BadRequest("Produto informado não existe.");
        }

        if (lote.DataFabricacao > DateTime.Now)
        {
            return Conflict("Lote inválido: Data de fabricação não pode ser maior que a data atual.");
        }

        _context.LotesProducao.Add(lote);
        await _context.SaveChangesAsync();

        return Ok(lote);
    }

    [HttpPatch("{id}/status")]
    public async Task<IActionResult> AtualizarStatus(int id, [FromBody] string novoStatus)
    {
        var lote = await _context.LotesProducao.FindAsync(id);

        if (lote == null)
        {
            return NotFound("Lote não encontrado.");
        }

        if (lote.Status == "Descartado" &&
            (novoStatus == "Qualidade Aprovada" || novoStatus == "Distribuído"))
        {
            return BadRequest("Um lote descartado não pode ser alterado para Qualidade Aprovada ou Distribuído.");
        }

        lote.Status = novoStatus;
        await _context.SaveChangesAsync();

        return Ok(lote);
    }
}