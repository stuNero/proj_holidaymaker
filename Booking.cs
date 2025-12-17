using MySqlX.XDevAPI.Common;

namespace server;


static class Booking
{
    public record Check_Args(int id, string checkIn, string checkOut);
    public static async Task<List<Check_Args>> CheckAvailability(Check_Args accommodation, Config config)
    {
        List<Check_Args> results = new();

        string query = 
        """
        SELECT id, name, price 
        FROM rooms  
        WHERE accommodation = @id AND id NOT IN(
        SELECT id FROM booked_rooms
        WHERE (@checkIn BETWEEN check_in AND check_out)
        OR (@checkOut BETWEEN check_in AND check_out));
        """;
        var parameter = new MySqlParameter[] 
        {
            new ("@id", accommodation.id),
            new ("@checkIn", accommodation.checkIn),
            new ("@checkOut", accommodation.checkOut)
        };

        using (var reader = await MySqlHelper.ExecuteReaderAsync(config.db, query, parameter))
        {
            while (reader.Read())
            {
                results.Add(new(reader.GetInt32(0), reader.GetString(1), reader.GetString(0)));
            }
        }
        
        return results;
    }
}