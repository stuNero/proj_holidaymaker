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
        using (var reader = await MySqlHelper.ExecuteReaderAsync(config.db, getBookingIdQuery, createParameter))
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
                    if (room.roomId == roomId) // add date check 
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
    public record RoomOverview(
        string roomName, DateOnly checkIn, DateOnly checkOut, int price,
        string accName, string cityName, string countryName);
    public record Booking_Data(int id, decimal totalPrice);
    public record Booking_X_Room(int bookingid, List<RoomOverview> rooms, decimal totalPrice);
    public static async Task<List<Booking_X_Room>> Overview(Config config, HttpContext ctx)
    {
        Dictionary<Booking_Data, List<RoomOverview>> roomsPerBooking = new();
        List<Booking_X_Room> result = new();
        string query =
        """
        SELECT b.id, b.total_price, r.name, bxr.check_in, bxr.check_out,
            r.price, a.name, ci.name, co.name
            FROM bookings_per_rooms bxr
            JOIN bookings b ON bxr.booking = b.id
            JOIN rooms r ON bxr.room = r.id
            JOIN accommodations a ON r.accommodation = a.id
            JOIN cities ci ON a.city = ci.id
            JOIN countries co ON ci.country = co.id
        WHERE b.user = @user_id;
        """;
        var parameters = new MySqlParameter[]
        {
            new("@user_id", ctx.Session.GetInt32("user_id")),
        };
        using (var reader = await MySqlHelper.ExecuteReaderAsync(config.db, query, parameters))
        {
            Booking_Data? bookingData = null;

            while (reader.Read())
            {
                // Extracts booking id and totalprice as dictionary key
                bookingData = new(reader.GetInt32(0), reader.GetDecimal(1));

                if (!roomsPerBooking.ContainsKey(bookingData))
                {
                    roomsPerBooking[bookingData] = new();
                }
                // Extracts room, name, checkin, checkout, roomprice, accommodation name, city name and country name as key's value
                RoomOverview roomOverview = new(reader.GetString(2), DateOnly.FromDateTime(reader.GetDateTime(3)), DateOnly.FromDateTime(reader.GetDateTime(4)),
                reader.GetInt32(5), reader.GetString(6), reader.GetString(7), reader.GetString(8));

                // Merges key and value in dictionary
                roomsPerBooking[bookingData].Add(roomOverview);
            }
        }
        // Unzips dictionary 
        foreach ((Booking_Data booking, List<RoomOverview> roomList) in roomsPerBooking)
        {
            // zips booking id, rooms per booking and totalprice into one object
            Booking_X_Room temp = new(booking.id, roomList, booking.totalPrice);
            // Puts that object into list
            result.Add(temp);
        }
        return result;
    }
}