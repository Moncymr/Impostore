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
            new Word { Id = 1, Category = "Animali", Text = "Gatto", Hint = "Animale domestico" },
            new Word { Id = 2, Category = "Animali", Text = "Cane", Hint = "Animale domestico" },
            new Word { Id = 3, Category = "Animali", Text = "Elefante", Hint = "Animale grande" },
            new Word { Id = 4, Category = "Animali", Text = "Leone", Hint = "Animale selvaggio" },
            new Word { Id = 5, Category = "Animali", Text = "Tigre", Hint = "Animale selvaggio" },
            new Word { Id = 6, Category = "Animali", Text = "Delfino", Hint = "Animale marino" },
            new Word { Id = 7, Category = "Animali", Text = "Pinguino", Hint = "Animale del freddo" },
            new Word { Id = 8, Category = "Animali", Text = "Farfalla", Hint = "Insetto volante" },
            
            // Cibo
            new Word { Id = 9, Category = "Cibo", Text = "Pizza", Hint = "Piatto italiano" },
            new Word { Id = 10, Category = "Cibo", Text = "Pasta", Hint = "Piatto italiano" },
            new Word { Id = 11, Category = "Cibo", Text = "Gelato", Hint = "Dolce freddo" },
            new Word { Id = 12, Category = "Cibo", Text = "Panino", Hint = "Cibo veloce" },
            new Word { Id = 13, Category = "Cibo", Text = "Sushi", Hint = "Piatto giapponese" },
            new Word { Id = 14, Category = "Cibo", Text = "Cioccolato", Hint = "Dolce" },
            new Word { Id = 15, Category = "Cibo", Text = "Hamburger", Hint = "Cibo veloce" },
            new Word { Id = 16, Category = "Cibo", Text = "Torta", Hint = "Dolce" },
            
            // Sport
            new Word { Id = 17, Category = "Sport", Text = "Calcio", Hint = "Sport di squadra" },
            new Word { Id = 18, Category = "Sport", Text = "Tennis", Hint = "Sport con racchetta" },
            new Word { Id = 19, Category = "Sport", Text = "Basket", Hint = "Sport di squadra" },
            new Word { Id = 20, Category = "Sport", Text = "Nuoto", Hint = "Sport acquatico" },
            new Word { Id = 21, Category = "Sport", Text = "Pallavolo", Hint = "Sport di squadra" },
            new Word { Id = 22, Category = "Sport", Text = "Sci", Hint = "Sport invernale" },
            new Word { Id = 23, Category = "Sport", Text = "Boxe", Hint = "Sport da combattimento" },
            new Word { Id = 24, Category = "Sport", Text = "Yoga", Hint = "Attività di rilassamento" },
            
            // Professioni
            new Word { Id = 25, Category = "Professioni", Text = "Dottore", Hint = "Professione medica" },
            new Word { Id = 26, Category = "Professioni", Text = "Insegnante", Hint = "Professione educativa" },
            new Word { Id = 27, Category = "Professioni", Text = "Poliziotto", Hint = "Lavoro di sicurezza" },
            new Word { Id = 28, Category = "Professioni", Text = "Cuoco", Hint = "Lavoro in cucina" },
            new Word { Id = 29, Category = "Professioni", Text = "Pilota", Hint = "Lavoro di trasporto" },
            new Word { Id = 30, Category = "Professioni", Text = "Cantante", Hint = "Lavoro artistico" },
            new Word { Id = 31, Category = "Professioni", Text = "Attore", Hint = "Lavoro artistico" },
            new Word { Id = 32, Category = "Professioni", Text = "Ingegnere", Hint = "Lavoro tecnico" },
            
            // Luoghi
            new Word { Id = 33, Category = "Luoghi", Text = "Spiaggia", Hint = "Luogo di vacanza" },
            new Word { Id = 34, Category = "Luoghi", Text = "Montagna", Hint = "Luogo naturale" },
            new Word { Id = 35, Category = "Luoghi", Text = "Scuola", Hint = "Luogo educativo" },
            new Word { Id = 36, Category = "Luoghi", Text = "Cinema", Hint = "Luogo di intrattenimento" },
            new Word { Id = 37, Category = "Luoghi", Text = "Ristorante", Hint = "Luogo per mangiare" },
            new Word { Id = 38, Category = "Luoghi", Text = "Ospedale", Hint = "Luogo medico" },
            new Word { Id = 39, Category = "Luoghi", Text = "Parco", Hint = "Luogo all'aperto" },
            new Word { Id = 40, Category = "Luoghi", Text = "Biblioteca", Hint = "Luogo culturale" },
        };

        modelBuilder.Entity<Word>().HasData(words);
    }
}
