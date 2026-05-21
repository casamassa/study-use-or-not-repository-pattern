var builder = WebApplication.CreateBuilder(args);

var connString = builder.Configuration.GetConnectionString("GameStore");
builder.Services.AddSqlite<GameStoreContext>(connString);

builder.Services.AddValidation();

var app = builder.Build();

app.MapGamesEndpoints();

app.Run();
