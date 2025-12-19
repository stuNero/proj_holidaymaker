using MySqlX.XDevAPI.CRUD;

namespace server;

static class DBQueries
{
    static public string DropAllTable()
    {
        string query =
        """
        DROP VIEW IF EXISTS booked_rooms;
        DROP TABLE IF EXISTS bookings_per_rooms;
        DROP TABLE IF EXISTS properties_per_room;
        DROP TABLE IF EXISTS rooms;
        DROP TABLE IF EXISTS amenities_per_accommodation;
        DROP TABLE IF EXISTS accommodations;
        DROP TABLE IF EXISTS cities;
        DROP TABLE IF EXISTS countries;
        DROP TABLE IF EXISTS cuisines;
        DROP TABLE IF EXISTS bookings;
        DROP TABLE IF EXISTS users;
        DROP TABLE IF EXISTS room_properties;
        DROP TABLE IF EXISTS amenities;
        """;
        return query;
    }
    static public string CreateAllTables()
    {
        string createQueries =
        """
           CREATE TABLE IF NOT EXISTS users
            (
                id          INT PRIMARY KEY AUTO_INCREMENT,
                first_name  VARCHAR(255) NOT NULL,
                last_name   VARCHAR(255) NOT NULL,
                email       VARCHAR(254) NOT NULL UNIQUE,
                password    VARCHAR(128),
                role        ENUM('admin','customer') DEFAULT 'customer'
            );

            CREATE TABLE IF NOT EXISTS cuisines
            (
                id      INT PRIMARY KEY AUTO_INCREMENT,
                name    VARCHAR(255) UNIQUE
            );

            CREATE TABLE IF NOT EXISTS countries
            (
                id      INT PRIMARY KEY AUTO_INCREMENT,
                name    VARCHAR(255) UNIQUE,
                cuisine INT NOT NULL,
                FOREIGN KEY (cuisine) REFERENCES cuisines(id) ON DELETE RESTRICT ON UPDATE CASCADE
            );

            CREATE TABLE IF NOT EXISTS cities
            (
                id      INT PRIMARY KEY AUTO_INCREMENT,
                name    VARCHAR(255),
                country INT NOT NULL,
                FOREIGN KEY (country) REFERENCES countries(id) ON DELETE RESTRICT ON UPDATE CASCADE
            );

            CREATE TABLE IF NOT EXISTS accommodations
            (
                id      INT PRIMARY KEY AUTO_INCREMENT,
                name    VARCHAR(255),
                city    INT NOT NULL,
                type    ENUM('hotel', 'motel', 'hostel') DEFAULT 'hotel',
                FOREIGN KEY (city) REFERENCES cities(id) ON DELETE RESTRICT ON UPDATE CASCADE,
                UNIQUE (city, name)
            );

            CREATE TABLE IF NOT EXISTS rooms
            (
                id            INT PRIMARY KEY AUTO_INCREMENT,
                name          VARCHAR(255) NOT NULL,
                sleep_spots   INT NOT NULL,
                accommodation INT NOT NULL,
                price         DECIMAL(10,2),
                FOREIGN KEY (accommodation) REFERENCES accommodations(id) ON DELETE CASCADE ON UPDATE CASCADE
            );
            CREATE TABLE IF NOT EXISTS bookings
            (
                id          INT PRIMARY KEY AUTO_INCREMENT,
                user        INT NOT NULL,
                total_price DECIMAL(10,2) DEFAULT(0.00),
                FOREIGN KEY (user) REFERENCES users(id) ON DELETE CASCADE ON UPDATE CASCADE,
                UNIQUE (id, user)
            );
            CREATE TABLE IF NOT EXISTS bookings_per_rooms
            (
                id          INT PRIMARY KEY AUTO_INCREMENT,
                room        INT NOT NULL,
                booking     INT NOT NULL,
                check_in    DATE,
                check_out   DATE,
                FOREIGN KEY (room) REFERENCES rooms(id) ON DELETE RESTRICT ON UPDATE CASCADE,
                FOREIGN KEY (booking) REFERENCES bookings(id),
                UNIQUE (room, booking, check_in, check_out)
            );

            CREATE TABLE IF NOT EXISTS room_properties
            (
                id   INT PRIMARY KEY AUTO_INCREMENT,
                name VARCHAR(255) UNIQUE
            );

            CREATE TABLE IF NOT EXISTS properties_per_room
            (
                id       INT PRIMARY KEY AUTO_INCREMENT,
                room     INT NOT NULL,
                property INT NOT NULL,
                FOREIGN KEY (room) REFERENCES rooms(id) ON DELETE CASCADE ON UPDATE CASCADE,
                FOREIGN KEY (property) REFERENCES room_properties(id) ON DELETE CASCADE ON UPDATE CASCADE,
                UNIQUE (room, property)
            );

            CREATE TABLE IF NOT EXISTS amenities
            (
                id   INT PRIMARY KEY AUTO_INCREMENT,
                name VARCHAR(255) UNIQUE
            );

            CREATE TABLE IF NOT EXISTS amenities_per_accommodation
            (
                id            INT PRIMARY KEY AUTO_INCREMENT,
                amenity       INT NOT NULL,
                accommodation INT NOT NULL,
                FOREIGN KEY (amenity) REFERENCES amenities(id) ON DELETE CASCADE ON UPDATE CASCADE,
                FOREIGN KEY (accommodation) REFERENCES accommodations(id) ON DELETE CASCADE ON UPDATE CASCADE,
                UNIQUE (amenity, accommodation)
            );
            
            CREATE OR REPLACE VIEW booked_rooms AS
            SELECT r.id, name, sleep_spots, price, accommodation, check_in, check_out
            FROM bookings_per_rooms x
            JOIN rooms r ON x.room = r.id;
       """;
        return createQueries;
    }
    static public string InsertMockData()
    {
        string insertQueries =
        """
        
            INSERT IGNORE INTO cuisines (name)
            VALUES
            ('Mediterranian'),
            ('Asian'),
            ('Nordic');

            INSERT IGNORE INTO countries (name, cuisine)
            VALUES
            ('Italy', 1),
            ('Spain', 1),
            ('Greece',1),
            ('Japan', 2),
            ('Sweden', 3);

            INSERT IGNORE INTO cities (name, country)
            VALUES
            ('Rome', 1),
            ('Barcelona', 2),
            ('Athens', 3),
            ('Milan', 1),
            ('Tokyo', 4),
            ('Osaka', 4),
            ('Göteborg', 5);

            INSERT IGNORE INTO accommodations (name, city, type)
            VALUES
            ('Best Western Barcelona', (select id from cities where name = "Barcelona"), 'hotel'),
            ('Best Western Rome', (select id from cities where name = "Rome"), 'hotel'),
            ('Best Western Athens', (select id from cities where name = "Athens"), 'hotel'),
            ('Roma Central Hotel', (select id from cities where name = "Rome"), 'hotel'),
            ('Milan Budget Hostel', (select id from cities where name = "Milan"), 'hostel'),
            ('Tokyo Garden Motel', (select id from cities where name = "Tokyo"), 'motel'),
            ('Osaka Riverside Hotel', (select id from cities where name = "Osaka"), 'hotel');

            INSERT IGNORE INTO rooms (name, sleep_spots, accommodation, price)
            VALUES
            ('Single Room Classic', 1, (select id from accommodations where name = "Best Western Barcelona"), 120.00),
            ('Double Room Classic', 2, (select id from accommodations where name = "Best Western Barcelona"), 160.00),
            ('Triple Room Classic', 3, (select id from accommodations where name = "Best Western Barcelona"), 180.00),

            ('Single Room Classic', 1, (select id from accommodations where name = "Best Western Rome"), 120.00),
            ('Double Room Classic', 2, (select id from accommodations where name = "Best Western Rome"), 160.00),
            ('Triple Room Classic', 3, (select id from accommodations where name = "Best Western Rome"), 180.00),

            ('Single Room Classic', 1, (select id from accommodations where name = "Best Western Athens"), 120.00),
            ('Double Room Classic', 2, (select id from accommodations where name = "Best Western Athens"), 160.00),
            ('Triple Room Classic', 3, (select id from accommodations where name = "Best Western Athens"), 180.00),

            ('Double Room Classic', 2, (select id from accommodations where name = "Roma Central Hotel"), 120.00),
            ('Suite Panoramica',    3, (select id from accommodations where name = "Milan Budget Hostel"), 210.00),
            ('Shared Dorm 6-bed',   6, (select id from accommodations where name = "Tokyo Garden Motel"), 30.00),
            ('Standard Twin',       2, (select id from accommodations where name = "Osaka Riverside Hotel"), 95.00),
            ('Deluxe King',         2, (select id from accommodations where name = "Osaka Riverside Hotel"), 150.00);

            INSERT IGNORE INTO bookings_per_rooms (room, check_in,check_out)
            VALUES
            (1, "2025-11-27","2025-11-30");

            INSERT IGNORE INTO room_properties (name)
            VALUES
            ('Sea View'),
            ('Balcony'),
            ('Private Bathroom'),
            ('Air Conditioning');

            INSERT IGNORE INTO properties_per_room (room, property)
            VALUES
            (1, 2),
            (1, 3),
            (2, 1),
            (2, 4),
            (3, 3),
            (4, 4),
            (5, 1),
            (5, 3),
            (6, 2),
            (7, 3),
            (8, 1),
            (9, 4),
            (10, 3),
            (11, 4),
            (12, 1),
            (13, 3),
            (14, 2),
            (6, 3),
            (7, 1),
            (8, 4),
            (9, 3),
            (10, 4),
            (11, 1),
            (12, 3);

            INSERT IGNORE INTO amenities (name)
            VALUES
            ('WiFi'),
            ('Breakfast Included'),
            ('Parking'),
            ('Airport Shuttle');

            INSERT IGNORE INTO amenities_per_accommodation (amenity, accommodation)
            VALUES
            (1, 1),
            (2, 1),
            (3, 1),
            (1, 2),
            (1, 3),
            (4, 3),
            (1, 4),
            (2, 4),
            (1, 5),
            (2, 5),
            (3, 5),
            (1, 6),
            (1, 6),
            (4, 6),
            (1, 7),
            (2, 7);
        """;
        return insertQueries;
    }
}
