using System.Diagnostics;
using MySqlX.XDevAPI.Common;

namespace server;

// Login 
static class Login
{
  public record Post_Args(string Email, string Password);
  public static async Task<bool> Post(Post_Args credentials, Config config, HttpContext ctx)
  {
    bool result = false;
    if (ctx.Session.IsAvailable)
    {
      string query = "SELECT id FROM users WHERE email = @email AND password = @Password";
      var parameters = new MySqlParameter[]
      {
        new("@email", credentials.Email),
        new("@password", credentials.Password)
      };
      object query_result = await MySqlHelper.ExecuteScalarAsync(config.db, query, parameters);
      if (query_result is int id)
      {
        ctx.Session.SetInt32("user_id", id);
        result = true;
      }
    }
    return result;
  }

  // Logout
  public static async Task<string> Delete(Config config, HttpContext ctx)
  {
    ctx.Session.Remove("user_id");
    return "You are loged out.";
  }


  // Hämta data som login user
  public record Get_Data(string FirstName, string LastName, string Email, string? Role);
  public static async Task<Get_Data> Get(Config config, HttpContext ctx)
  {
    Get_Data? result = null;
    if (ctx.Session.IsAvailable)
    {
      if (ctx.Session.Keys.Contains("user_id"))
      {
        string query = "SELECT first_name, last_name, email, role FROM users WHERE id = @id";
        var parameters = new MySqlParameter[]
        {
          new("@id", ctx.Session.GetInt32("user_id"))
        };

        using (var reader = await MySqlHelper.ExecuteReaderAsync(config.db, query, parameters))
        {
          if (reader.Read())
          {
            result = new(reader.GetString(0), reader.GetString(1), reader.GetString(2), reader[3] as string);
          }
        }
      }
    }
    return result!;
  }
}