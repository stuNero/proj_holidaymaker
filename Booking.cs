using Mysqlx.Crud;
using MySqlX.XDevAPI.Common;

namespace server;


static class Booking
{
    public static Dictionary<int, List<Rooms_To_Book>> usersWithBookedRooms = new();
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
    public record Rooms_To_Book(int roomId, DateOnly checkIn, DateOnly checkOut);
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
        var createParameter = new MySqlParameter[] {new("@user_id",ctx.Session.GetInt32("user_id"))};

        await MySqlHelper.ExecuteNonQueryAsync(config.db, createBookingQuery, createParameter);

        string getBookingIdQuery = 
        """
        SELECT last_insert_id() FROM bookings;
        """;
        using (var reader = await MySqlHelper.ExecuteReaderAsync(config.db, getBookingIdQuery))
        {
            while(reader.Read()) // löser problemet
            {
                bookingID = reader.GetInt32(0);
            }
            /*
            MySql.Data.MySqlClient.MySqlException (0x80004005): Invalid attempt to access a field before calling Read()
            at MySql.Data.MySqlClient.ResultSet.get_Item(Int32 index)
            at MySql.Data.MySqlClient.MySqlDataReader.GetFieldValue(Int32 index, Boolean checkNull)
            at MySql.Data.MySqlClient.MySqlDataReader.GetInt32(Int32 i)
            at server.Booking.Book(Config config, HttpContext ctx) in C:\Users\maxve\repos\proj_holidaymaker\Booking.cs:line 66
            at Microsoft.AspNetCore.Http.RequestDelegateFactory.<ExecuteTaskOfTFast>g__ExecuteAwaited|132_0[T](Task`1 task, HttpContext httpContext, JsonTypeInfo`1 jsonTypeInfo)
            at Microsoft.AspNetCore.Session.SessionMiddleware.Invoke(HttpContext context)
            at Microsoft.AspNetCore.Session.SessionMiddleware.Invoke(HttpContext context)
            at Microsoft.AspNetCore.Diagnostics.DeveloperExceptionPageMiddlewareImpl.Invoke(HttpContext context)
            */
        }
        /*
        1. Skapa ett bookingID i kod
        2. Kolla om det finns i tabellen
        3. om det finns byt ut mot nytt
        4. gå vidare
        */

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
                    (@id, @bookingid, @checkIn, @checkOut)
                    """;
                    var parameter = new MySqlParameter[] 
                    {
                        new("@id", room.roomId),
                        new("@bookingid", bookingID),
                        new("@checkIn",room.checkIn),
                        new("@checkOut",room.checkOut)
                    };

                    await MySqlHelper.ExecuteNonQueryAsync(config.db, query,parameter);
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

        var sessionID = ctx.Session.GetInt32("user_id");
        if (sessionID is int sID)
        {
            if (!usersWithBookedRooms.ContainsKey(sID))
            {
                usersWithBookedRooms[sID] = new();
            }
            usersWithBookedRooms[sID].Add(new(roomId, checkIn,checkOut));
            return true;
        }
        // check if same room exists in dictionary
        return false;
    }
}