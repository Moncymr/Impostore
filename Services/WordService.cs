using ImpostoreGame.Models;
using ImpostoreGame.Data;
using Microsoft.EntityFrameworkCore;

namespace ImpostoreGame.Services;

public class WordService
{
    private readonly GameDbContext _context;

    public WordService(GameDbContext context)
    {
        _context = context;
    }

    public async Task<Word?> GetRandomWordAsync()
    {
        var words = await _context.Words.ToListAsync();
        if (words.Count == 0)
            return null;

        var word = words[Random.Shared.Next(words.Count)];
        return word;
    }

    public async Task<List<string>> GetCategoriesAsync()
    {
        return await _context.Words
            .Select(w => w.Category)
            .Distinct()
            .ToListAsync();
    }

    public async Task<Word?> GetRandomWordByCategoryAsync(string category)
    {
        var words = await _context.Words
            .Where(w => w.Category == category)
            .ToListAsync();

        if (words.Count == 0)
            return null;

        var word = words[Random.Shared.Next(words.Count)];
        return word;
    }
}
