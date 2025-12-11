using MySqlX.XDevAPI.CRUD;

namespace server;

static class DBQueries
{
    static public string DropAllTable()
    {
        string query =
        """
        DROP TABLE IF EXISTS users;
        DROP TABLE IF EXISTS cuisines;
        DROP TABLE IF EXISTS countries;
        DROP TABLE IF EXISTS cities;
        DROP TABLE IF EXISTS accommodations;
        DROP TABLE IF EXISTS packages;
        DROP TABLE IF EXISTS accommodation_per_package;
        DROP TABLE IF EXISTS transport_types;
        DROP TABLE IF EXISTS transports;
        DROP TABLE IF EXISTS transport_per_package;
        DROP TABLE IF EXISTS payments;
        DROP TABLE IF EXISTS bookings;
        DROP TABLE IF EXISTS rooms;
        DROP TABLE IF EXISTS booked_rooms;
        DROP TABLE IF EXISTS room_properties;
        DROP TABLE IF EXISTS properties_per_room;
        DROP TABLE IF EXISTS amenities;
        DROP TABLE IF EXISTS amenities_per_accommodation;
        """;
        return query;
    }
    static public string InsertMockData()
    {
        string insertQueries =
        """
            INSERT IGNORE INTO cuisines (name) VALUES ('Mediterranean');

            -- Countries
            INSERT IGNORE INTO countries (name, cuisine) VALUES ('Greece', 1),
            ('Italy', 1),('Spain', 1);

            -- Cities
            INSERT IGNORE INTO cities (name, country) VALUES ('Athens', 1),('Thessaloniki', 1),
            ('Rome', 2), ('Milan', 2),
            ('Madrid', 3),('Barcelona', 3);

            -- Accommodations
            INSERT IGNORE INTO accommodations (name, city) VALUES ('Athens Grand Hotel', 1),('Athens Resort', 1),('Athens Boutique Hotel', 1),
            ('Thessaloniki Inn', 2),('Thessaloniki Palace', 2),('Thessaloniki Plaza', 2),('Rome Luxury Suites', 3),
            ('Rome Hotel Deluxe', 3),('Rome City Center', 3),('Milan Sky Hotel', 4),('Milan Luxury Inn', 4),
            ('Milan Central Suites', 4),('Madrid Palace Hotel', 5),('Madrid Boutique', 5),('Madrid Downtown', 5),
            ('Barcelona Beach Resort', 6),('Barcelona City View', 6),('Barcelona Plaza Hotel', 6);

            -- Rooms
            INSERT IGNORE INTO rooms (name, sleep_spots, accommodation) VALUES ('Room 101', 2, 1),
            ('Room 102', 2, 1),('Room 103', 2, 1),
            ('Room 104', 2, 1),('Room 105', 2, 1),
            ('Room 201', 2, 2),('Room 202', 2, 2),
            ('Room 203', 2, 2),('Room 204', 2, 2),
            ('Room 205', 2, 2),('Room 301', 2, 3),
            ('Room 302', 2, 3),('Room 303', 2, 3),
            ('Room 304', 2, 3),('Room 305', 2, 3),
            ('Room 401', 2, 4),('Room 402', 2, 4),
            ('Room 403', 2, 4),('Room 404', 2, 4),
            ('Room 405', 2, 4),('Room 501', 2, 5),
            ('Room 502', 2, 5),('Room 503', 2, 5),
            ('Room 504', 2, 5),('Room 505', 2, 5),
            ('Room 601', 2, 6),('Room 602', 2, 6),
            ('Room 603', 2, 6),('Room 604', 2, 6),
            ('Room 605', 2, 6);

            -- Transport types
            INSERT IGNORE INTO transport_types (name) VALUES ('Flight'),('Train'),('Bus');

            -- Transports
            INSERT IGNORE INTO transports (type, start_city, end_city, company, price, start_datetime, end_datetime)
            VALUES (1, 1, 3, 'GreekAir', 199.99, '2026-01-10 08:00:00', '2026-01-10 10:30:00'),
            (2, 2, 4, 'EuroTrain', 129.99, '2026-01-12 09:00:00', '2026-01-12 17:00:00'),
            (3, 1, 3, 'MediterraneanBus', 49.99, '2026-01-15 07:00:00', '2026-01-15 18:30:00'),
            (1, 3, 5, 'ItalyFly', 159.99, '2026-01-20 10:00:00', '2026-01-20 12:00:00'),
            (2, 4, 6, 'MediterraneanRail', 119.99, '2026-01-22 08:00:00', '2026-01-22 15:30:00'),
            (3, 4, 5, 'MediterraneanBus', 69.99, '2026-01-25 06:00:00', '2026-01-25 18:00:00'),
            (1, 6, 1, 'AirMediterranean', 189.99, '2026-02-02 09:00:00', '2026-02-02 11:30:00'),
            (2, 5, 2, 'EuroTrain', 139.99, '2026-02-05 08:00:00', '2026-02-05 18:00:00'),
            (3, 6, 3, 'MediterraneanBus', 79.99, '2026-02-07 07:00:00', '2026-02-07 19:00:00');

            -- Package
            INSERT IGNORE INTO packages (name, description, price)
            VALUES ('Mediterranean Delight', 'Explore the Mediterranean: Greece, Italy, and Spain with beautiful cities and accommodations.', 1999.99);

            -- Transport per package
            INSERT IGNORE INTO transport_per_package (transport, package) VALUES (1, 1),(2, 1),(3, 1),
            (4, 1),(5, 1),(6, 1),(7, 1),(8, 1),(9, 1);

            -- Accommodation per package
            INSERT IGNORE INTO accommodation_per_package (accommodation, package) VALUES (1, 1),(2, 1),(3, 1),
            (4, 1),(5, 1),(6, 1),(7, 1),(8, 1),(9, 1),
            (10, 1),(11, 1),(12, 1),(13, 1),(14, 1),(15, 1),
            (16, 1),(17, 1),(18, 1);

            -- Amenities
            INSERT IGNORE INTO amenities (name) VALUES
            ('Free breakfast'),('Swimming pool'),('Airport shuttle'),
            ('Gym'),('Parking');

            -- Amenities per accommodation
            INSERT IGNORE INTO amenities_per_accommodation (amenity, accommodation) VALUES
            (1, 1),(2, 1),(3, 1),
            (4, 1),(5, 1),(1, 2),
            (2, 2),(4, 2),(3, 3),
            (5, 3),(1, 4),(3, 4),
            (4, 4),(2, 5),(5, 5),
            (1, 6),(3, 6),(4, 6);

            -- Room properties
            INSERT IGNORE INTO room_properties (name) VALUES
            ('Sea view'),('Balcony'),
            ('Air conditioning'),('Private bathroom'),('Wi-Fi');

            -- Properties per room
            INSERT IGNORE INTO properties_per_room (room, property) VALUES
            (1, 1),(1, 2),(1, 3),
            (1, 4),(1, 5),(2, 1),
            (2, 2),(2, 3),(2, 4),
            (3, 1),(3, 2),(3, 3),
            (4, 1),(4, 5),(5, 2),
            (5, 3),(5, 4),(6, 3),
            (6, 5);
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
            role        ENUM('admin','customer') DEFAULT('customer')
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
            cuisine INT NOT NULL REFERENCES cuisines(id)
        );
        CREATE TABLE IF NOT EXISTS cities
        (
            id      INT PRIMARY KEY AUTO_INCREMENT,
            name    VARCHAR(255),
            country INT NOT NULL REFERENCES countries(id)
        );
        CREATE TABLE IF NOT EXISTS accommodations
        (
            id      INT PRIMARY KEY AUTO_INCREMENT,
            name    VARCHAR(255),
            city    INT NOT NULL REFERENCES cities(id),
            type ENUM('hotel', 'motel', 'hostel') DEFAULT ('hotel'),
            UNIQUE (city, name)
        );
        CREATE TABLE IF NOT EXISTS packages
        (
            id      INT PRIMARY KEY AUTO_INCREMENT,
            name    VARCHAR(255) NOT NULL,
            description TEXT NOT NULL,
            price DECIMAL(10,2) NOT NULL
        );
        CREATE TABLE IF NOT EXISTS accommodation_per_package
        (
            accommodation INT NOT NULL REFERENCES accommodations(id),
            package INT NOT NULL REFERENCES packages(id)
        );
        CREATE TABLE IF NOT EXISTS transport_types
        (
        id INT PRIMARY KEY AUTO_INCREMENT,
        name VARCHAR(255) UNIQUE
        );
        CREATE TABLE IF NOT EXISTS transports
        (
            id INT PRIMARY KEY AUTO_INCREMENT,
            type INT NOT NULL REFERENCES transport_types(id),
            start_city INT NOT NULL REFERENCES cities(id),
            end_city INT NOT NULL REFERENCES cities(id),
            company VARCHAR(255),
            price decimal(10,2) NOT NULL,
            start_datetime DATETIME NOT NULL,
            end_datetime DATETIME NOT NULL
        );
        CREATE TABLE IF NOT EXISTS transport_per_package
        (
            transport INT NOT NULL REFERENCES transports(id),
            package   INT NOT NULL REFERENCES packages(id),
            UNIQUE (transport, package)
        );
        CREATE TABLE IF NOT EXISTS payments
        (
            id INT PRIMARY KEY AUTO_INCREMENT,
            amount DECIMAL(10,2) NOT NULL,
            payment_time TIMESTAMP
        );
        CREATE TABLE IF NOT EXISTS bookings
        (
            id INT PRIMARY KEY AUTO_INCREMENT,
            payment INT NOT NULL REFERENCES payments(id),
            user INT NOT NULL REFERENCES users(id),
            package INT NOT NULL REFERENCES packages(id),
            UNIQUE (id, user)
        );
        CREATE TABLE IF NOT EXISTS rooms
        (
            id INT PRIMARY KEY AUTO_INCREMENT,
            name VARCHAR(255) NOT NULL,
            sleep_spots INT NOT NULL,
            accommodation INT NOT NULL REFERENCES accommodations(id)
        );
        CREATE TABLE IF NOT EXISTS booked_rooms
        (
            rooms int NOT NULL REFERENCES rooms(id),
            start_datetime DATETIME,
            end_datetime DATETIME,
            UNIQUE(rooms, start_datetime, end_datetime)
        );

        CREATE TABLE IF NOT EXISTS room_properties
        (
        id INT PRIMARY KEY AUTO_INCREMENT,
        name VARCHAR(255) UNIQUE
        );
        CREATE TABLE IF NOT EXISTS properties_per_room
        (
            room INT NOT NULL REFERENCES rooms(id),
            property INT NOT NULL REFERENCES room_properties(id),
            UNIQUE (room,property)
        );
        CREATE TABLE IF NOT EXISTS amenities
        (
            id INT PRIMARY KEY AUTO_INCREMENT,
            name VARCHAR(255) UNIQUE
        );
        CREATE TABLE IF NOT EXISTS amenities_per_accommodation
        (
            amenity INT NOT NULL REFERENCES amenities(id),
            accommodation INT NOT NULL REFERENCES accommodations(id),
            UNIQUE (amenity,accommodation)
        )
       """;
        return createQueries;
    }
}