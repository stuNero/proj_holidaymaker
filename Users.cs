using System.Globalization;

namespace server;

static class Users
{
  public record Post_Args(string FirstName, string LastName, string Email, string Password);
  public static async Task Post(Post_Args user, Config config)
  {
    string query =
    """
    INSERT INTO users(first_name, last_name, email, password)
    VALUES (@first_name, @last_name, @email, @password)
    """;
    var parameters = new MySqlParameter[]
    {
      new("@first_name", user.FirstName),
      new("@last_name", user.LastName),
      new("@email", user.Email),
      new("@password", user.Password)
    };
    await MySqlHelper.ExecuteNonQueryAsync(config.db, query, parameters);
  }
}