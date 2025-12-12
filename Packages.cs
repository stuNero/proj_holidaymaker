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
  public record Package_Data(string City, string Accommodation, string Transport, DateTime Departure, DateTime Arrival);

  public static async Task<List<Package_Data>> GetPackageDetails(int id, Config config)
  {
    List<Package_Data> result = new();
    string query =
    """ 
    SELECT c.name, a.name, t.company, t.start_datetime, t.end_datetime
    FROM accommodation_per_package app
    JOIN accommodations a ON app.accommodation = a.id
    JOIN transport_per_package tpp ON tpp.package = app.package
    JOIN transports t ON t.id = tpp.transport
    JOIN cities c ON a.city = c.id
    WHERE app.package = @id;
    """;
    var parameters = new MySqlParameter[] { new("@id", id) };
    using (var reader = await MySqlHelper.ExecuteReaderAsync(config.db, query, parameters))
    {
      while (reader.Read())
      {
        result.Add(new(reader.GetString(0), reader.GetString(1), reader.GetString(2), reader.GetDateTime(3), reader.GetDateTime(4)));
      }
    }
    return result;
  }
}