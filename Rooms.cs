namespace server;

class Rooms
{
  public record Get_Args(int id, string name, int sleep_spots, string accommodation, decimal price, List<Get_PropertiesData> properties);
  public record Get_PropertiesData(string name);

  public static async Task<Get_Args?> Get(int id, Config config)
  {
    Get_Args? result = null;

    string query = """
        SELECT r.id, r.name, r.sleep_spots, a.name, r.price 
        FROM rooms r
        JOIN accommodations a ON r.accommodation = a.id
        WHERE r.id = @id;
        """;

    var parameter = new MySqlParameter[]
    {
            new("@id", id)
    };

    using (var reader = await MySqlHelper.ExecuteReaderAsync(config.db, query, parameter))
    {
      if (reader.Read())
      {
        var properties = await GetProperties(id, config);
        result = new(reader.GetInt32(0), reader.GetString(1), reader.GetInt32(2), reader.GetString(3), reader.GetDecimal(4), properties);
      }
    }
    return result;
  }

  public static async Task<List<Get_PropertiesData>> GetProperties(int id, Config config)
  {
    List<Get_PropertiesData> result = new();
    string query =
    """
    SELECT rp.name 
    FROM room_properties rp
    JOIN properties_per_room ppr ON ppr.property = rp.id
    JOIN rooms r ON r.id = ppr.room
    WHERE r.id = @id
    """;

    var parameters = new MySqlParameter[] { new("@id", id) };
    using (var reader = await MySqlHelper.ExecuteReaderAsync(config.db, query, parameters))
    {
      while (reader.Read())
      {
        result.Add(new(reader.GetString(0)));
      }
    }
    return result;
  }
}