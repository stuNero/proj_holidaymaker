using MySqlX.XDevAPI.Common;

namespace server;


static class Booking
{
    public record Available_Rooms(int id, string name, int sleepSpots, decimal price);
    public static async Task<List<Available_Rooms>> CheckAvailability(int id, DateOnly checkIn, DateOnly checkOut, Config config)
    {
        List<Available_Rooms> results = new();

        string query =
        """
        SELECT id, name, sleep_spots, price 
        FROM rooms  
        WHERE accommodation = @id AND id NOT IN(
        SELECT id FROM booked_rooms
        WHERE (@checkIn BETWEEN check_in AND check_out)
        OR (@checkOut BETWEEN check_in AND check_out));
        """;
        var parameter = new MySqlParameter[] 
        {
            new ("@id", id),
            new ("@checkIn", checkIn),
            new ("@checkOut", checkOut)
        };

        using (var reader = await MySqlHelper.ExecuteReaderAsync(config.db, query, parameter))
        {
            while (reader.Read())
            {
                results.Add(new(reader.GetInt32(0), reader.GetString(1), reader.GetInt32(2), reader.GetDecimal(3)));
            }
        }    
        return results;
    }
}