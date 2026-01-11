using Microsoft.EntityFrameworkCore;
using ImpostoreGame.Models;

namespace ImpostoreGame.Data;

public class GameDbContext : DbContext
{
    public GameDbContext(DbContextOptions<GameDbContext> options) : base(options)
    {
    }

    public DbSet<Game> Games { get; set; }
    public DbSet<Player> Players { get; set; }
    public DbSet<Vote> Votes { get; set; }
    public DbSet<ChatMessage> ChatMessages { get; set; }
    public DbSet<Word> Words { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configure relationships
        modelBuilder.Entity<Game>()
            .HasMany(g => g.Players)
            .WithOne()
            .HasForeignKey(p => p.GameId);

        modelBuilder.Entity<Game>()
            .HasMany(g => g.Votes)
            .WithOne()
            .HasForeignKey(v => v.GameId);

        modelBuilder.Entity<Game>()
            .HasMany(g => g.Messages)
            .WithOne()
            .HasForeignKey(m => m.GameId);

        // Seed words
        SeedWords(modelBuilder);
    }

    private void SeedWords(ModelBuilder modelBuilder)
    {
        var words = new List<Word>
        {
            // Animali
            new Word { Id = 1, Category = "Animali", Text = "Gatto" },
            new Word { Id = 2, Category = "Animali", Text = "Cane" },
            new Word { Id = 3, Category = "Animali", Text = "Elefante" },
            new Word { Id = 4, Category = "Animali", Text = "Leone" },
            new Word { Id = 5, Category = "Animali", Text = "Tigre" },
            new Word { Id = 6, Category = "Animali", Text = "Delfino" },
            new Word { Id = 7, Category = "Animali", Text = "Pinguino" },
            new Word { Id = 8, Category = "Animali", Text = "Farfalla" },
            
            // Cibo
            new Word { Id = 9, Category = "Cibo", Text = "Pizza" },
            new Word { Id = 10, Category = "Cibo", Text = "Pasta" },
            new Word { Id = 11, Category = "Cibo", Text = "Gelato" },
            new Word { Id = 12, Category = "Cibo", Text = "Panino" },
            new Word { Id = 13, Category = "Cibo", Text = "Sushi" },
            new Word { Id = 14, Category = "Cibo", Text = "Cioccolato" },
            new Word { Id = 15, Category = "Cibo", Text = "Hamburger" },
            new Word { Id = 16, Category = "Cibo", Text = "Torta" },
            
            // Sport
            new Word { Id = 17, Category = "Sport", Text = "Calcio" },
            new Word { Id = 18, Category = "Sport", Text = "Tennis" },
            new Word { Id = 19, Category = "Sport", Text = "Basket" },
            new Word { Id = 20, Category = "Sport", Text = "Nuoto" },
            new Word { Id = 21, Category = "Sport", Text = "Pallavolo" },
            new Word { Id = 22, Category = "Sport", Text = "Sci" },
            new Word { Id = 23, Category = "Sport", Text = "Boxe" },
            new Word { Id = 24, Category = "Sport", Text = "Yoga" },
            
            // Professioni
            new Word { Id = 25, Category = "Professioni", Text = "Dottore" },
            new Word { Id = 26, Category = "Professioni", Text = "Insegnante" },
            new Word { Id = 27, Category = "Professioni", Text = "Poliziotto" },
            new Word { Id = 28, Category = "Professioni", Text = "Cuoco" },
            new Word { Id = 29, Category = "Professioni", Text = "Pilota" },
            new Word { Id = 30, Category = "Professioni", Text = "Cantante" },
            new Word { Id = 31, Category = "Professioni", Text = "Attore" },
            new Word { Id = 32, Category = "Professioni", Text = "Ingegnere" },
            
            // Luoghi
            new Word { Id = 33, Category = "Luoghi", Text = "Spiaggia" },
            new Word { Id = 34, Category = "Luoghi", Text = "Montagna" },
            new Word { Id = 35, Category = "Luoghi", Text = "Scuola" },
            new Word { Id = 36, Category = "Luoghi", Text = "Cinema" },
            new Word { Id = 37, Category = "Luoghi", Text = "Ristorante" },
            new Word { Id = 38, Category = "Luoghi", Text = "Ospedale" },
            new Word { Id = 39, Category = "Luoghi", Text = "Parco" },
            new Word { Id = 40, Category = "Luoghi", Text = "Biblioteca" },
        };

        modelBuilder.Entity<Word>().HasData(words);
    }
}
