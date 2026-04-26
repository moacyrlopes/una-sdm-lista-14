using CacauShowApi.Data;
using CacauShowApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CacauShowApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PedidoController : ControllerBase
{
    private readonly AppDbContext _context;

    public PedidoController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> Get()
    {
        var pedidos = await _context.Pedidos
            .Include(p => p.Produto)
            .Include(p => p.Unidade)
            .ToListAsync();

        return Ok(pedidos);
    }

    [HttpPost]
    public async Task<IActionResult> Post(Pedido pedido)
    {
        var unidade = await _context.Franquias.FindAsync(pedido.UnidadeId);

        if (unidade == null)
        {
            return BadRequest("Unidade informada não existe.");
        }

        var produto = await _context.Produtos.FindAsync(pedido.ProdutoId);

        if (produto == null)
        {
            return BadRequest("Produto informado não existe.");
        }

        var quantidadeAtual = await _context.Pedidos
            .Where(p => p.UnidadeId == pedido.UnidadeId)
            .SumAsync(p => p.Quantidade);

        if (quantidadeAtual + pedido.Quantidade > unidade.CapacidadeEstoque)
        {
            return BadRequest("Capacidade logística da loja excedida. Não é possível receber mais produtos.");
        }

        pedido.ValorTotal = produto.PrecoBase * pedido.Quantidade;

        if (produto.Tipo == "Sazonal")
        {
            pedido.ValorTotal += 15.00m;
            Console.WriteLine("Produto sazonal detectado: Adicionando embalagem de presente premium!");
        }

        _context.Pedidos.Add(pedido);
        await _context.SaveChangesAsync();

        return Ok(pedido);
    }
}