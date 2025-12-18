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

// DB functions
app.MapDelete("/db", db_reset_to_default);

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

// Login Functions
app.MapPost("/login", Login.Post);
app.MapDelete("/login", Login.Delete);
app.MapGet("/login", Login.Get);

// Cusinies Functions
app.MapPost("/cuisines", Cuisines.Post);
app.MapGet("/cuisines", Cuisines.GetAll);
app.MapGet("/cuisines/{id}", Cuisines.Get);
app.MapDelete("/cuisines/{id}", Cuisines.Delete);
app.MapPut("/cuisines/{id}", Cuisines.Put);
app.MapPatch("/cuisines/{id}", Cuisines.Patch);

// Countries Functions
app.MapPost("/countries", Countries.Post);
app.MapGet("/countries", Countries.GetAll);
app.MapGet("/countries/{id}", Countries.Get);
app.MapDelete("/countries/{id}", Countries.Delete);
app.MapPut("/countries/{id}", Countries.Put);
app.MapPatch("/countries/{id}", Countries.Patch);

// Booking functions
app.MapGet("/bookings/availability", Booking.CheckAvailability);
app.MapPost("/bookings/bookroom", Booking.BookRoom);
app.MapPost("/bookings/book", Booking.Book);
app.MapGet("/bookings/overview", Booking.Overview);

async Task db_reset_to_default()
{
  await MySqlHelper.ExecuteNonQueryAsync(config.db, DBQueries.DropAllTable());
  await MySqlHelper.ExecuteNonQueryAsync(config.db, DBQueries.CreateAllTables());
  await MySqlHelper.ExecuteNonQueryAsync(config.db, DBQueries.InsertMockData());
}

app.Run();