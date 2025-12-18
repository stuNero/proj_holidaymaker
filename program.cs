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



// Admin access logic
app.Use(async (context, next) =>
{
  PathString path = context.Request.Path;
  string method = context.Request.Method;
  // paths that need admin protection
  bool isAdminPath = path.StartsWithSegments("/users") ||
                     path.StartsWithSegments("/accommodations") ||
                     path.StartsWithSegments("/cuisines") ||
                     path.StartsWithSegments("/countries");

  // paths that are public
  bool isPublicMethod = (method == "GET" && (path == "/accommodations" || path == "/cuisines" || path == "/countries")) ||
                        (method == "GET" && (path == "/accommodations/" || path == "/cuisines/" || path == "/countries/")) ||
                        (method == "POST" && (path == "/login" || path == "/login/")) ||
                        (method == "POST" && (path == "/users" || path == "/users/"));


  if (isAdminPath && !isPublicMethod)
  {
    if (!context.Session.IsAvailable || context.Session.GetInt32("user_id") is not int userId)
    {
      context.Response.StatusCode = 401;
      await context.Response.WriteAsJsonAsync(new { message = "Login required" });
      return;
    }
    string query = "SELECT role FROM users WHERE id = @id";
    var parameter = new MySqlParameter[]
    {
      new("@id", userId)
    };

    object role = await MySqlHelper.ExecuteScalarAsync(config.db, query, parameter);
    if (role is not string s || s != "admin")
    {
      context.Response.StatusCode = 401;
      await context.Response.WriteAsJsonAsync(new { Message = "You are not authorized!" });
      return;
    }

  }
  await next();
}
);




// User Functions
app.MapGet("/users", Users.GetAll);
app.MapPost("/users", Users.Post);


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



// DB functions
app.MapDelete("/db", db_reset_to_default);
async Task db_reset_to_default()
{
  await MySqlHelper.ExecuteNonQueryAsync(config.db, DBQueries.DropAllTable());
  await MySqlHelper.ExecuteNonQueryAsync(config.db, DBQueries.CreateAllTables());
  await MySqlHelper.ExecuteNonQueryAsync(config.db, DBQueries.InsertMockData());
}

app.Run();