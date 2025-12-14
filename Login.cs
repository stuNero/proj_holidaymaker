using Microsoft.AspNetCore.SignalR;
using MySqlX.XDevAPI.Common;
using ZstdSharp.Unsafe;

namespace server;

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


}