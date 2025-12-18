using Mysqlx.Crud;
using MySqlX.XDevAPI.Common;

namespace server;


static class Booking
{
    public static Dictionary<int, List<Rooms_To_Book>> usersWithBookedRooms = new();
    public record Available_Rooms(int roomId, string name, int sleepSpots, decimal price);
    public static async Task<List<Available_Rooms>> CheckAvailability(int accommodationId, DateTime checkIn, DateTime checkOut, Config config)
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
    public record Rooms_To_Book(int roomId, DateTime checkIn, DateTime checkOut);
    public static async Task<bool> Book(Config config, HttpContext ctx)
    {
        if (ctx.Session.GetInt32("user_id") == null)
        {
            return false;
        }
        int bookingID = 0;
        string createBookingQuery =
        """
        INSERT INTO bookings (user)
        VALUES
        (@user_id);
        """;
        var createParameter = new MySqlParameter[] { new("@user_id", ctx.Session.GetInt32("user_id")) };

        await MySqlHelper.ExecuteNonQueryAsync(config.db, createBookingQuery, createParameter);

        string getBookingIdQuery =
        """
        SELECT last_insert_id() FROM bookings
        WHERE user = @user_id;
        """;
        using (var reader = await MySqlHelper.ExecuteReaderAsync(config.db, getBookingIdQuery))
        {
            while (reader.Read())
            {
                bookingID = reader.GetInt32(0);
            }
        }

        foreach ((int user, List<Rooms_To_Book> list) in usersWithBookedRooms)
        {
            if (user == ctx.Session.GetInt32("user_id"))
            {
                foreach (Rooms_To_Book room in list)
                {
                    string query =
                    """
                    INSERT INTO bookings_per_rooms (room, booking, check_in, check_out)
                    VALUES
                    (@id, @bookingid, @checkIn, @checkOut);

                    UPDATE bookings
                    SET total_price = total_price + (SELECT price
                        FROM rooms
                        WHERE id = @id)
                    WHERE id = @bookingid;
                    """;
                    var parameter = new MySqlParameter[]
                    {
                        new("@id", room.roomId),
                        new("@bookingid", bookingID),
                        new("@checkIn",room.checkIn),
                        new("@checkOut",room.checkOut)
                    };

                    await MySqlHelper.ExecuteNonQueryAsync(config.db, query, parameter);
                }
            }
        }
        var sessionID = ctx.Session.GetInt32("user_id");
        if (sessionID is int sID)
        {
            usersWithBookedRooms[sID].Clear();
            return true;
        }
        else
        {
            return false;
        }
    }
    public static async Task<bool> BookRoom(int accommodationId, int roomId, DateTime checkIn, DateTime checkOut, Config config, HttpContext ctx)
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

        var sessionID = ctx.Session.GetInt32("user_id");
        if (sessionID is int sID)
        {
            bool foundRoom = false;
            if (!usersWithBookedRooms.ContainsKey(sID))
            {
                usersWithBookedRooms[sID] = new();
            }
            foreach ((int key, List<Rooms_To_Book> list) in usersWithBookedRooms)
            {
                foreach (Rooms_To_Book room in list)
                {
                    if (room.roomId == roomId)
                    {
                        foundRoom = true;
                    }
                }
            }
            if (!foundRoom)
            {
                usersWithBookedRooms[sID].Add(new(roomId, checkIn, checkOut));
                return true;
            }
        }
        return false;
    }
}