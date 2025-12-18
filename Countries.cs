using System.Data.Common;
using System.Runtime.Intrinsics.Wasm;

namespace server;

static class Countries
{
  // Post
  public record Post_Args(string Name, int Cuisine);
  public static async Task Post(Post_Args country, Config config)
  {
    string query = """
    INSERT IGNORE INTO countries (name, cuisine) 
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



  // Get By ID
  public record Get_Data(int Id, string Name, string Cuisine);
  public static async Task<Get_Data?> Get(int id, Config config)
  {
    Get_Data? result = null;
    string query = """
    SELECT c.id, c.name, cuisines.name
    FROM countries c
    INNER JOIN cuisines 
    ON cuisines.id = c.cuisine
    WHERE c.id = @id
    """;
    var parameter = new MySqlParameter[]
    {
      new("@id", id)
    };
    using (var reader = await MySqlHelper.ExecuteReaderAsync(config.db, query, parameter))
    {
      if (reader.Read())
      {
        result = new(reader.GetInt32(0), reader.GetString(1), reader.GetString(2));
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
      DELETE FROM countries
      WHERE id = @id
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
      return result = new(true, $"Requested country with ID [{id}] is deleted!");
    }

  }



  // Put
  public record Put_response(bool Success, string Message);
  public record Put_Args(string Name, int Cuisine);
  public static async Task<Put_response> Put(int id, Put_Args country, Config config)
  {
    Put_response? result = null;
    string query = """
    UPDATE countries 
    SET name = @name, cuisine = @cuisine
    WHERE id = @id
    """;
    var parameter = new MySqlParameter[]
    {
      new("@id", id),
      new("@name", country.Name),
      new("@cuisine", country.Cuisine)
    };
    int rows_updated = await MySqlHelper.ExecuteNonQueryAsync(config.db, query, parameter);
    if (rows_updated == 0)
    {
      return result = new(false, $"Failed to update country with id [{id}]");
    }
    else
    {
      return result = new(true, $"Country with id [{id}] has been updated successfully!");
    }
  }


  // Patch
  public record Patch_Response(bool Success, string Message);
  public record Patch_Args(string? Name, int? Cuisine);
  public static async Task<Patch_Response> Patch(int id, Patch_Args country, Config config)
  {
    List<string> updates = [];
    var parameter = new List<MySqlParameter>
    {
      new("@id", id)
    };

    if (country.Name != null)
    {
      updates.Add("name = @name");
      parameter.Add(new("@name", country.Name));
    }

    if (country.Cuisine.HasValue)
    {
      updates.Add("cuisine = @cuisine");
      parameter.Add(new("@cuisine", country.Cuisine));
    }

    if (updates.Count == 0)
    {
      return new Patch_Response(false, "Nothing to patch");
    }
    string query = $"""
    UPDATE countries
    SET {string.Join(", ", updates)}
    WHERE id = @id
    """;

    int rows_updated = await MySqlHelper.ExecuteNonQueryAsync(config.db, query, parameter.ToArray());

    if (rows_updated == 0)
    {
      return new Patch_Response(false, $"Failed to patch country with id [{id}]");
    }
    else
    {
      return new Patch_Response(true, $"Country with id [{id}] has been patched successfully!");
    }
  }
}