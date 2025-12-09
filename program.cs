global using MySql.Data.MySqlClient;
using server;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.MapDelete("/db",db_reset_to_default);



async Task db_reset_to_default()
{
    
}

app.Run();