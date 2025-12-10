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

  public record GetAll_Data(int Id, string FirstName, string LastName, string Email, string Role);

  public static async Task<List<GetAll_Data>> GetAll(Config config)
  {
    List<GetAll_Data> result = new();
    string query = "SELECT id, first_name, last_name, email, role FROM users";
    using (var reader = await MySqlHelper.ExecuteReaderAsync(config.db, query))
    {
      while (reader.Read())
      {
        result.Add(new(reader.GetInt32(0), reader.GetString(1), reader.GetString(2), reader.GetString(3), reader.GetString(4)));
      }
    }
    return result;
  }
}