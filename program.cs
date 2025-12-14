global using MySql.Data.MySqlClient;
using Org.BouncyCastle.Bcpg.Sig;
using server;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(option =>
{
  option.Cookie.HttpOnly = true;
  option.Cookie.IsEssential = true;
}
);

Config config = new("server=127.0.0.1;uid=Holidaymaker;pwd=Holidaymaker;database=Holidaymaker;");
builder.Services.AddSingleton(config);
var app = builder.Build();
app.UseSession();

app.MapPost("/users", Users.Post);
app.MapGet("/users", Users.GetAll);

app.MapPost("/login", Login.Post);
app.MapDelete("/login", Login.Delete);
app.MapGet("/login", Login.Get);

app.MapDelete("/db", db_reset_to_default);


app.Run();

async Task db_reset_to_default()
{
  await MySqlHelper.ExecuteNonQueryAsync(config.db, DBQueries.DropAllTable());
  await MySqlHelper.ExecuteNonQueryAsync(config.db, DBQueries.CreateAllTables());
  await MySqlHelper.ExecuteNonQueryAsync(config.db, DBQueries.InsertMockData());
}


