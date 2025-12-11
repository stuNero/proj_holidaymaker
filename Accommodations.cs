using Microsoft.VisualBasic;
using Mysqlx.Crud;

namespace server;

class Accommodations
{
    public record DeleteResponse(bool success,string msg);
    public static async Task<DeleteResponse?> Delete(int id,Config config)
    {
        string query = 
        """
        DELETE FROM accommodations
        WHERE id = @id
        """;
        var parameters = new MySqlParameter [] {new ("@id",id)};
        int deleted = await MySqlHelper.ExecuteNonQueryAsync(config.db,query,parameters);

        if (deleted == 0)
            return new DeleteResponse(false, $"Row with id {id} not found.");

        return new DeleteResponse(true, $"Row with id {id} deleted.");
    }
    public record Get_AllData(int id, string name, string city, string type);
    public static async Task<List<Get_AllData?>> GetAll(Config config)
    {
        List<Get_AllData?> result = new();
        string query = 
        """
        SELECT a.id, a.name, c.name, a.type 
        FROM accommodations a
        JOIN cities c ON a.city = c.id;
        """;

        using (var reader = await MySqlHelper.ExecuteReaderAsync(config.db, query))
        {
            while (reader.Read())
            {
                result.Add(new(reader.GetInt32(0), reader.GetString(1), reader.GetString(2), reader.GetString(3)));
            }
        }
        return result;
    }

    public record Get_Data(int id, string name, string city, string type);
    public static async Task<Get_Data?> Get(int id, Config config)
    {
        Get_Data? result = null;
        string query =
        """
        SELECT a.id, a.name, c.name, a.type 
        FROM accommodations a
        JOIN cities c ON a.city = c.id
        WHERE a.id = @id;
        """;
        var parameters = new MySqlParameter[] { new ("@id", id)};
        using (var reader = await MySqlHelper.ExecuteReaderAsync(config.db, query, parameters))
        {
            if(reader.Read())
            {
                result = new(reader.GetInt32(0), reader.GetString(1), reader.GetString(2), reader.GetString(3));
            }
        }
        return result;
    }

    // public record Post_Args(string name, int city, string type);
    // public static async Task Post(Post_Args Accommodations, Config config)
    // {
    //     string query = """
    //     INSERT INTO accommodations (name, city, type)
    //     VALUES(@name, @city, @type)
    //     """;
    //     var parameters = new MySqlParameter[]
    //     {
    //         new("@name", Accommodations.name),
    //         new("@city", Accommodations.city),
    //         new("@type", Accommodations.type)

    //     };
        
    //     await MySqlHelper.ExecuteNonQuery(config.db, query, parameters);
    // }
    
}