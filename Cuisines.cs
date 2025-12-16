namespace server;

static class Cuisines
{
  // Post
  public record Post_Args(string Name);
  public static async Task Post(Post_Args cuisine, Config config)
  {
    string query = "INSERT INTO cuisines (name) VALUES (@name)";
    var parameter = new MySqlParameter[]
    {
      new("@name", cuisine.Name)
    };
    int query_result = await MySqlHelper.ExecuteNonQueryAsync(config.db, query, parameter);
  }


  // Get All
  public record GetAll_Data(int Id, string Name);
  public static async Task<List<GetAll_Data>> GetAll(Config config)
  {
    List<GetAll_Data> result = new();
    string query = "SELECT id, name FROM cuisines";
    using (var reader = await MySqlHelper.ExecuteReaderAsync(config.db, query))
    {
      while (reader.Read())
      {
        result.Add(new(reader.GetInt32(0), reader.GetString(1)));
      }
    }
    return result;
  }


  // Get By ID
  public record Get_Data(int Id, string Name);
  public static async Task<Get_Data?> Get(int id, Config config)
  {
    Get_Data? result = null;
    string query = "SELECT id, name FROM cuisines WHERE id = @id";
    var parameter = new MySqlParameter[]
    {
      new("@id", id)
    };
    using (var reader = await MySqlHelper.ExecuteReaderAsync(config.db, query, parameter))
    {
      if (reader.Read())
      {
        result = new(reader.GetInt32(0), reader.GetString(1));
      }
    }
    return result;
  }


  // Delete 
  public record Delete_Response(bool Success, string Message);
  public static async Task<Delete_Response> Delete(int id, Config config)
  {
    Delete_Response? result = null;
    string query = """
      DELETE FROM cuisines
      WHERE id = @id
      ORDER BY id
      """;
    var parameter = new MySqlParameter[]
    {
      new("@id", id)
    };

    int rows_deleted = await MySqlHelper.ExecuteNonQueryAsync(config.db, query, parameter);
    if (rows_deleted == 0)
    {
      return result = new(false, $"Requested ID [{id}] was not found!");
    }
    else
    {
      return result = new(true, $"Requested cuisine with ID [{id}] is deleted!");
    }

  }



  // Put
  public record Put_response(bool Success, string Message);
  public record Put_Args(string Name);
  public static async Task<Put_response> Put(int id, Put_Args cuisine, Config config)
  {
    Put_response? result = null;
    string query = """
    UPDATE cuisines 
    SET name = @name
    WHERE id = @id
    """;
    var parameter = new MySqlParameter[]
    {
      new("@id", id),
      new("@name", cuisine.Name)
    };
    int rows_updated = await MySqlHelper.ExecuteNonQueryAsync(config.db, query, parameter);
    if (rows_updated == 0)
    {
      return result = new(false, $"Failed to update cuisine with id [{id}]");
    }
    else
    {
      return result = new(true, $"Cuisine with id [{id}] has been updated successfully!");
    }
  }


  // Patch
  public record Patch_Response(bool Success, string Message);
  public record Patch_Args(string? Name);
  public static async Task<Patch_Response> Patch(int id, Patch_Args cuisine, Config config)
  {

    List<string> updates = [];
    var parameter = new List<MySqlParameter>
    {
      new("@id", id)
    };

    if (cuisine.Name != null)
    {
      updates.Add("name = @name");
      parameter.Add(new("@name", cuisine.Name));
    }
    string query = $"""
    UPDATE cuisines 
    SET {string.Join(", ", updates)}
    WHERE id = @id
    """;
    if (updates.Count == 0)
    {
      return new Patch_Response(false, "Nothing to update!");
    }
    int rows_updated = await MySqlHelper.ExecuteNonQueryAsync(config.db, query, parameter.ToArray());
    if (rows_updated == 0)
    {
      return new Patch_Response(false, $"Failed to update cuisine with id [{id}]");
    }
    else
    {
      return new Patch_Response(true, $"Cuisine with id [{id}] has been updated successfully!");
    }
  }
}