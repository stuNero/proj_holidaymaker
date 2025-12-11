namespace server;

static class Packages
{
  public record Get_Data(int Id, string Name, string Description, decimal Price);
  public static async Task<List<Get_Data>> Get(Config config)
  {
    List<Get_Data> result = new();

    string query = "SELECT id, name, description, price FROM packages";
    using (var reader = await MySqlHelper.ExecuteReaderAsync(config.db, query))
    {
      while (reader.Read())
      {
        result.Add(new(reader.GetInt32(0), reader.GetString(1), reader.GetString(2), reader.GetDecimal(3)));
      }
    }
    return result;
  }
}