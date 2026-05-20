public interface IGamesRepository : IRepository<Game>
{
    Task<IEnumerable<Game>> GetAllWithGenresAsync();
    Task<IEnumerable<Game>> SearchByNameAsync(string name);
    Task<Game?> GetByIdWithGenreAsync(int id);
}