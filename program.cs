global using MySql.Data.MySqlClient;
using server;

var builder = WebApplication.CreateBuilder(args);

Config config = new("server=127.0.0.1;uid=Holidaymaker;pwd=Holidaymaker;database=Holidaymaker;");
builder.Services.AddSingleton(config);
var app = builder.Build();

app.MapPost("/users", Users.Post);
app.MapGet("/users", Users.GetAll);


app.MapDelete("/db", db_reset_to_default);


app.Run();

async Task db_reset_to_default()
{
  await MySqlHelper.ExecuteNonQueryAsync(config.db, DBQueries.DropAllTable());
  await MySqlHelper.ExecuteNonQueryAsync(config.db, DBQueries.CreateAllTables());
}


