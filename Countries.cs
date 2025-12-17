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



  // Get All
  public record GetAll_Data(int Id, string Name, string Cuisine);
  public static async Task<List<GetAll_Data>> GetAll(Config config)
  {
    List<GetAll_Data> result = new();
    string query = """
    SELECT c.id, c.name, cuisines.name
    FROM countries c
    INNER JOIN cuisines 
    ON c.cuisine = cuisines.id
    """;
    using (var reader = await MySqlHelper.ExecuteReaderAsync(config.db, query))
    {
      while (reader.Read())
      {
        result.Add(new(reader.GetInt32(0), reader.GetString(1), reader.GetString(2)));
      }
    }
    return result;
  }
}