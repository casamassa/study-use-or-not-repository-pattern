using Microsoft.EntityFrameworkCore;

public static class GamesEndpoints
{
    const string GetGameEndpointName = "GetGame";

    public static RouteGroupBuilder MapGamesEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/games")
                        .WithParameterValidation();

        // GET /games?name=searchTerm
        group.MapGet("/", async (
            GameStoreContext dbContext,
            string? name) =>
        {
            var query = dbContext.Games
                                .Include(game => game.Genre)
                                .AsNoTracking();

            if (!string.IsNullOrWhiteSpace(name))
            {
                query = query.Where(game => game.Name.ToLower().Contains(name.ToLower()));
            }
            var games = await query.ToListAsync();

            return games.Select(game => game.ToGameSummaryDto());
        });

        // GET /games/1
        group.MapGet("/{id}", async (
            int id,
            GameStoreContext dbContext) =>
        {
            Game? game = await dbContext.Games
                .Include(game => game.Genre)
                .AsNoTracking()
                .FirstOrDefaultAsync(game => game.Id == id);

            return game is null ?
                Results.NotFound() : Results.Ok(game.ToGameSummaryDto());
        })
        .WithName(GetGameEndpointName);

        // POST /games
        group.MapPost("/", async (
            CreateGameDto newGame,
            GameStoreContext dbContext) =>
        {
            Game game = newGame.ToEntity();

            dbContext.Games.Add(game);
            await dbContext.SaveChangesAsync();

            // CORRIGIDO: Carrega explicitamente o Genre do jogo recém-criado na memória
            await dbContext.Entry(game)
                .Reference(g => g.Genre)
                .LoadAsync();

            return Results.CreatedAtRoute(
                GetGameEndpointName,
                new { id = game.Id },
                game.ToGameSummaryDto());
        });

        // PUT /games
        group.MapPut("/{id}", async (
            int id,
            UpdateGameDto updatedGame,
            GameStoreContext dbContext) =>
        {
            var existingGame = await dbContext.Games.FindAsync(id);

            if (existingGame is null)
                return Results.NotFound();

            var updatedEntity = updatedGame.ToEntity(id);
            existingGame.Name = updatedEntity.Name;
            existingGame.GenreId = updatedEntity.GenreId;
            existingGame.Price = updatedEntity.Price;
            existingGame.ReleaseDate = updatedEntity.ReleaseDate;

            await dbContext.SaveChangesAsync();

            return Results.NoContent();
        });

        // DELETE /games/1
        group.MapDelete("/{id}", async (
            int id,
            GameStoreContext dbContext) =>
        {
            var existingGame = await dbContext.Games.FindAsync(id);

            if (existingGame is null)
                return Results.NotFound();

            await dbContext.Games
                .Where(game => game.Id == id)
                .ExecuteDeleteAsync();

            return Results.NoContent();
        });

        return group;
    }
}