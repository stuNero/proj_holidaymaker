using Mysqlx.Crud;
using MySqlX.XDevAPI.Common;

namespace server;


static class Booking
{
    public record Available_Rooms(int roomId, string name, int sleepSpots, decimal price);
    public static async Task<List<Available_Rooms>> CheckAvailability(int accommodationId, DateOnly checkIn, DateOnly checkOut, Config config)
    {
        List<Available_Rooms> results = new();

        string query =
        """
        SELECT id, name, sleep_spots, price 
        FROM rooms  
        WHERE accommodation = @id AND id NOT IN(
        SELECT id FROM booked_rooms
        WHERE (@checkIn BETWEEN check_in AND check_out)
        OR (@checkOut BETWEEN check_in AND check_out)
        OR (check_in BETWEEN @checkIn AND @checkOut)
        OR (check_out BETWEEN @checkIn AND @checkOut));
        """;
        var parameter = new MySqlParameter[] 
        {
            new ("@id", accommodationId),
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
    public record Success(bool success, string txt);
    public static async Task<bool> BookRoom(int accommodationId, int roomId, DateOnly checkIn, DateOnly checkOut, Config config, HttpContext ctx)
    {
        if (ctx.Session.GetInt32("user_id") == null) 
        {
            return false;
        }
            
        List<Available_Rooms> availableRooms = new();

        string findAvailabilityQuery =
        """
        SELECT id, name, sleep_spots, price 
        FROM rooms  
        WHERE accommodation = @id AND id NOT IN(
        SELECT id FROM booked_rooms
        WHERE (@checkIn BETWEEN check_in AND check_out)
        OR (@checkOut BETWEEN check_in AND check_out)
        OR (check_in BETWEEN @checkIn AND @checkOut)
        OR (check_out BETWEEN @checkIn AND @checkOut));
        """;
        var availabilityParameter = new MySqlParameter[] 
        {
            new ("@id", accommodationId),
            new ("@checkIn", checkIn),
            new ("@checkOut", checkOut)
        };

        using (var reader = await MySqlHelper.ExecuteReaderAsync(config.db, findAvailabilityQuery, availabilityParameter))
        {
            while (reader.Read())
            {
                availableRooms.Add(new(reader.GetInt32(0), reader.GetString(1), reader.GetInt32(2), reader.GetDecimal(3)));
            }
        }
        bool checkRoom = false;
        foreach (Available_Rooms room in availableRooms)
        {
            if (room.roomId == roomId)
            {
                checkRoom = true;
            }
        }
        if (!checkRoom)
        {
            return false;
        }
        string query = 
        """
        INSERT INTO bookings_per_rooms (room, check_in, check_out)
        VALUES
        (@id, @checkIn, @checkOut)
        """;
        var parameter = new MySqlParameter[] 
        {
            new("@id", roomId),
            new("@checkIn",checkIn),
            new("@checkOut",checkOut)
        };

        int check = await MySqlHelper.ExecuteNonQueryAsync(config.db, query,parameter);

        if (check == 0)
        {
            return false;
        }
        else
        {
            return true;
        }
    }
}