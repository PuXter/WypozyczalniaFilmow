using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using WypozyczalniaFilmow.Models;

namespace WypozyczalniaFilmow.Data;

public class DatabaseContext : IdentityDbContext
{
    public DatabaseContext(DbContextOptions options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder){base.OnModelCreating(modelBuilder);}

    public DbSet<filmy> Filmy { get; set; }
    public DbSet<reklamacje> Reklamacje { get; set; }
    public DbSet<wypozyczenia> Wypozyczenia { get; set; }
}