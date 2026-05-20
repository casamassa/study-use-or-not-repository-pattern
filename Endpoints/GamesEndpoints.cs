public static class GamesEndpoints
{
    const string GetGameEndpointName = "GetGame";

    public static RouteGroupBuilder MapGamesEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/games")
                        .WithParameterValidation();

        // GET /games?name=searchTerm
        group.MapGet("/", async (
            IGamesRepository repository,
            string? name) =>
        {
            var games = string.IsNullOrWhiteSpace(name)
                ? await repository.GetAllWithGenresAsync()
                : await repository.SearchByNameAsync(name);

            return games.Select(game => game.ToGameSummaryDto());
        });

        // GET /games/1
        group.MapGet("/{id}", async (
            int id,
            IGamesRepository gamesRepository) =>
        {
            Game? game = await gamesRepository.GetByIdAsync(id);

            return game is null ?
                Results.NotFound() : Results.Ok(game.ToGameSummaryDto());
        })
        .WithName(GetGameEndpointName);

        // POST /games
        group.MapPost("/", async (
            CreateGameDto newGame,
            IGamesRepository gamesRepository) =>
        {
            Game game = newGame.ToEntity();

            var createdGame = await gamesRepository.AddAsync(game);
            await gamesRepository.SaveChangesAsync();

            return Results.CreatedAtRoute(
                GetGameEndpointName,
                new { id = createdGame.Id },
                createdGame.ToGameSummaryDto());
        });

        // PUT /games
        group.MapPut("/{id}", async (
            int id,
            UpdateGameDto updatedGame,
            IGamesRepository gamesRepository) =>
        {
            var existingGame = await gamesRepository.GetByIdAsync(id);

            if (existingGame is null)
                return Results.NotFound();

            var updatedEntity = updatedGame.ToEntity(id);
            existingGame.Name = updatedEntity.Name;
            existingGame.GenreId = updatedEntity.GenreId;
            existingGame.Price = updatedEntity.Price;
            existingGame.ReleaseDate = updatedEntity.ReleaseDate;

            await gamesRepository.UpdateAsync(existingGame);
            await gamesRepository.SaveChangesAsync();

            return Results.NoContent();
        });

        // DELETE /games/1
        group.MapDelete("/{id}", async (
            int id,
            IGamesRepository gamesRepository) =>
        {
            var deleted = await gamesRepository.DeleteAsync(id);
            if (!deleted)
                return Results.NotFound();

            await gamesRepository.SaveChangesAsync();
            return Results.NoContent();
        });

        return group;
    }
}