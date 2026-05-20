using Microsoft.EntityFrameworkCore;

public class Repository<T> : IRepository<T> where T : BaseEntity
{
    protected readonly DbContext context;
    protected readonly DbSet<T> dbSet;

    protected Repository(DbContext context)
    {
        this.context = context
            ?? throw new ArgumentNullException(nameof(context));
        dbSet = this.context.Set<T>();
    }

    public virtual async Task<IEnumerable<T>> GetAllAsync()
    {
        return await dbSet
                    .AsNoTracking()
                    .ToListAsync();
    }

    public virtual async Task<T?> GetByIdAsync(int id)
    {
        return await dbSet.FindAsync(id);
    }

    public virtual async Task<T> AddAsync(T entity)
    {
        ArgumentNullException.ThrowIfNull(entity);

        var entry = await dbSet.AddAsync(entity);
        return entry.Entity;
    }

    public virtual Task<T> UpdateAsync(T entity)
    {
        ArgumentNullException.ThrowIfNull(entity);

        dbSet.Update(entity);
        return Task.FromResult(entity);
    }

    public virtual async Task<bool> DeleteAsync(int id)
    {
        var deletedCount = await dbSet
                                    .Where(entity => entity.Id == id)
                                    .ExecuteDeleteAsync();
        return deletedCount > 0;
    }

    public virtual async Task<int> SaveChangesAsync()
    {
        return await context.SaveChangesAsync();
    }
}