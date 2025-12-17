global using MySql.Data.MySqlClient;
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

// User Functions
app.MapPost("/users", Users.Post);
app.MapGet("/users", Users.GetAll);

// Accommodations Functions
app.MapGet("/accommodations", Accommodations.GetAll);
app.MapGet("/accommodations/{id}", Accommodations.Get);
app.MapPost("/accommodations", Accommodations.Post);
app.MapPut("/accommodations/{id}", Accommodations.Put);
app.MapPatch("/accommodations/{id}/{column}/{value}", Accommodations.Patch);
app.MapDelete("/accommodations/{id}", Accommodations.Delete);
app.MapGet("/accommodations/{id}/rooms", Accommodations.GetRooms);
app.MapGet("/accommodations/{id}/amenities", Accommodations.GetAmenities);

// Rooms functions
app.MapGet("/rooms/{id}", Rooms.Get);
app.MapGet("/properties/{id}", Rooms.GetProperties);

// Login functions
app.MapPost("/login", Login.Post);
app.MapDelete("/login", Login.Delete);
app.MapGet("/login", Login.Get);

// DB functions
app.MapDelete("/db", db_reset_to_default);
async Task db_reset_to_default()
{
  await MySqlHelper.ExecuteNonQueryAsync(config.db, DBQueries.DropAllTable());
  await MySqlHelper.ExecuteNonQueryAsync(config.db, DBQueries.CreateAllTables());
  await MySqlHelper.ExecuteNonQueryAsync(config.db, DBQueries.InsertMockData());
}

app.Run();