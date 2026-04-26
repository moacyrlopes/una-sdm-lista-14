using CacauShowApi.Models;
using Microsoft.EntityFrameworkCore;

namespace CacauShowApi.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<Produto> Produtos { get; set; }
    public DbSet<Franquia> Franquias { get; set; }
    public DbSet<LoteProducao> LotesProducao { get; set; }
    public DbSet<Pedido> Pedidos { get; set; }
}