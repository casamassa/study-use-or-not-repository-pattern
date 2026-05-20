using Microsoft.EntityFrameworkCore;

public class SqliteGamesRepository(GameStoreContext gameStoreContext)
    : Repository<Game>(gameStoreContext), IGamesRepository
{
    public async Task<IEnumerable<Game>> GetAllWithGenresAsync()
    {
        return await dbSet
                        .Include(game => game.Genre)
                        .AsNoTracking()
                        .ToListAsync();
    }

    public async Task<IEnumerable<Game>> SearchByNameAsync(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            return [];

        return await dbSet
                    .Where(game => game.Name.ToLower().Contains(name.ToLower()))
                    .Include(game => game.Genre)
                    .AsNoTracking()
                    .ToListAsync();
    }
}