global using MySql.Data.MySqlClient;
using server;

var builder = WebApplication.CreateBuilder(args);
Config config = new("server=127.0.0.1;uid=Holidaymaker;pwd=Holidaymaker;database=Holidaymaker;");
builder.Services.AddSingleton(config);
var app = builder.Build();

// User Functions
app.MapPost("/users", Users.Post);
app.MapGet("/users", Users.GetAll);

// Accommodations Functions
app.MapGet("/accommodations",Accommodations.GetAll);
app.MapGet("/accommodations/{id}",Accommodations.Get);
// app.MapPost("/accommodations",Accommodations.Post);
// app.MapPut("/accommodations",Accommodations.Put);
// app.MapPatch("/accommodations",Accommodations.Patch);
// app.MapDelete("/accommodations",Accommodations.Delete);

// DB functions
app.MapDelete("/db", db_reset_to_default);
async Task db_reset_to_default()
{
  await MySqlHelper.ExecuteNonQueryAsync(config.db, DBQueries.DropAllTable());
  await MySqlHelper.ExecuteNonQueryAsync(config.db, DBQueries.CreateAllTables());
  await MySqlHelper.ExecuteNonQueryAsync(config.db, DBQueries.InsertMockData2());
}

app.Run();