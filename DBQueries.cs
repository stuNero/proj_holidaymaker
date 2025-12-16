using MySqlX.XDevAPI.CRUD;

namespace server;

static class DBQueries
{
    static public string DropAllTable()
    {
        string query =
        """
        DROP TABLE IF EXISTS booked_rooms;
        DROP TABLE IF EXISTS properties_per_room;
        DROP TABLE IF EXISTS rooms;
        DROP TABLE IF EXISTS accommodation_per_package;
        DROP TABLE IF EXISTS amenities_per_accommodation;
        DROP TABLE IF EXISTS accommodations;
        DROP TABLE IF EXISTS cities;
        DROP TABLE IF EXISTS countries;
        DROP TABLE IF EXISTS cuisines;
        DROP TABLE IF EXISTS orders;
        DROP TABLE IF EXISTS packages;
        DROP TABLE IF EXISTS users;
        DROP TABLE IF EXISTS room_properties;
        DROP TABLE IF EXISTS amenities;
        """;
        return query;
    }
    static public string InsertMockData()
    {
        string insertQueries =
        """
            INSERT IGNORE INTO users (first_name, last_name, email, password, role)
            VALUES
            ('Alice', 'Walker', 'alice.walker@example.com', '123', 'customer'),
            ('Bob', 'Anderson', 'bob.anderson@example.com', '123', 'admin');

            INSERT IGNORE INTO cuisines (name)
            VALUES
            ('Mediterranian'),
            ('Asian');

            INSERT IGNORE INTO countries (name, cuisine)
            VALUES
            ('Italy', 1),
            ('Spain', 1),
            ('Greece',1),
            ('Japan', 2);

            INSERT IGNORE INTO cities (name, country)
            VALUES
            ('Rome', 1),
            ('Barcelona', 2),
            ('Athens', 3),
            ('Milan', 1),
            ('Tokyo', 4),
            ('Osaka', 4);

            INSERT IGNORE INTO accommodations (name, city, type)
            VALUES
            ('Roma Central Hotel', 1, 'hotel'),
            ('Milan Budget Hostel', 2, 'hostel'),
            ('Tokyo Garden Motel', 3, 'motel'),
            ('Osaka Riverside Hotel', 4, 'hotel');

            INSERT IGNORE INTO packages (name, description, discount)
            VALUES
            ('Romantic Getaway Italy', '4 nights in Rome and Milan with breakfast included.', 10.00),
            ('Japan Explorer', '7-day trip including Tokyo and Osaka stays.', 12.50);

            INSERT IGNORE INTO accommodation_per_package (accommodation, package)
            VALUES
            (1, 1),
            (2, 1),
            (3, 2),
            (4, 2);

            INSERT IGNORE INTO orders (user, package, total_price)
            VALUES
            (1, 1, 899.99);

            INSERT IGNORE INTO rooms (name, sleep_spots, accommodation, price)
            VALUES
            ('Double Room Classic', 2, 1, 120.00),
            ('Suite Panoramica', 3, 1, 210.00),
            ('Shared Dorm 6-bed', 6, 2, 30.00),
            ('Standard Twin', 2, 3, 95.00),
            ('Deluxe King', 2, 4, 150.00);

            INSERT IGNORE INTO booked_rooms (room_id, order_id, start_datetime, end_datetime)
            VALUES
            (1, 1, '2026-06-10 14:00:00', '2026-06-13 10:00:00'),
            (3, 1, '2026-06-13 15:00:00', '2026-06-15 11:00:00');

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
            (5, 3);

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
            (2, 4);
        """;
        return insertQueries;
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

            CREATE TABLE IF NOT EXISTS packages
            (
                id          INT PRIMARY KEY AUTO_INCREMENT,
                name        VARCHAR(255) NOT NULL,
                description TEXT NOT NULL,
                discount    DECIMAL(5,2) NOT NULL
            );

            CREATE TABLE IF NOT EXISTS accommodation_per_package
            (
                accommodation INT NOT NULL,
                package       INT NOT NULL,
                FOREIGN KEY (accommodation) REFERENCES accommodations(id) ON DELETE CASCADE ON UPDATE CASCADE,
                FOREIGN KEY (package) REFERENCES packages(id) ON DELETE CASCADE ON UPDATE CASCADE
            );

            CREATE TABLE IF NOT EXISTS orders
            (
                id          INT PRIMARY KEY AUTO_INCREMENT,
                user        INT NOT NULL,
                package     INT,
                total_price DECIMAL(10,2),
                FOREIGN KEY (user) REFERENCES users(id) ON DELETE RESTRICT ON UPDATE CASCADE,
                FOREIGN KEY (package) REFERENCES packages(id) ON DELETE SET NULL ON UPDATE CASCADE,
                UNIQUE (id, user)
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

            CREATE TABLE IF NOT EXISTS booked_rooms
            (
                id             INT PRIMARY KEY AUTO_INCREMENT,
                room_id        INT NOT NULL,
                order_id       INT NOT NULL,
                start_datetime DATETIME,
                end_datetime   DATETIME,
                FOREIGN KEY (room_id) REFERENCES rooms(id) ON DELETE RESTRICT ON UPDATE CASCADE,
                FOREIGN KEY (order_id) REFERENCES orders(id) ON DELETE CASCADE ON UPDATE CASCADE,
                UNIQUE (room_id, start_datetime, end_datetime)
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
       """;
        return createQueries;
    }
}