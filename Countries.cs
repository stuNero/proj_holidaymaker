namespace server;

static class Countries
{
  // Post
  public record Post_Args(string Name, int Cuisine);
  public static async Task Post(Post_Args country, Config config)
  {
    string query = """
    INSERT INTO countries (name, cuisine) 
    VALUES (@name, @cuisine)
    """;
    var parameter = new MySqlParameter[]
    {
      new("@name", country.Name),
      new("@cuisine", country.Cuisine)
    };
    int query_result = await MySqlHelper.ExecuteNonQueryAsync(config.db, query, parameter);
  }
}