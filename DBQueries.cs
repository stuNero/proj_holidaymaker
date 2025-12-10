namespace server;

static class DBQueries
{
    static public string CreateUsersTable()
    {
        string query = 
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
        """;
        return query;
    }
    static public string CreateCuisinesTable()
    {
        string query = 
        """
            CREATE TABLE IF NOT EXISTS cuisines
            (
                id      INT PRIMARY KEY AUTO_INCREMENT,
                name    VARCHAR(255) UNIQUE
            );
        """;
        return query;
    }
    static public string CreateCitiesTable()
    {
        string query = 
        """
            CREATE TABLE IF NOT EXISTS cities
            (
                id      INT PRIMARY KEY AUTO_INCREMENT,
                name    VARCHAR(255),
                country INT NOT NULL REFERENCES countries(id)
            );
        """;
        return query;
    }
    static public string CreateCountriesTable()
    {
        string query = 
        """
            CREATE TABLE IF NOT EXISTS countries
            (
                id      INT PRIMARY KEY AUTO_INCREMENT,
                name    VARCHAR(255) UNIQUE,
                cuisine INT NOT NULL REFERENCES cuisines(id)
            );
        """;
        return query;
    }
    static public string CreateAccommodationsTable()
    {
        string query = 
        """
            CREATE TABLE IF NOT EXISTS accommodations
            (
                id      INT PRIMARY KEY AUTO_INCREMENT,
                name    VARCHAR(255),
                city    INT NOT NULL REFERENCES cities(id),
                type ENUM('hotel', 'motel', 'hostel') DEFAULT ('hotel'),
                UNIQUE (city, name)
            );
        """;
        return query;
    }
    static public string CreatePackagesTable()
    {
        string query = 
        """
            CREATE TABLE IF NOT EXISTS packages
            (
                id      INT PRIMARY KEY AUTO_INCREMENT,
                name    VARCHAR(255) NOT NULL,
                description TEXT NOT NULL,
                price DECIMAL(10,2) NOT NULL
            );
        """;
        return query;
    }
    static public string CreateAccommodations_Per_PackageTable()
    {
        string query = 
        """
            CREATE TABLE IF NOT EXISTS accommodation_per_package
            (
                accommodation INT NOT NULL REFERENCES accommodations(id),
                package INT NOT NULL REFERENCES packages(id)
            );
        """;
        return query;
    }
    static public string CreateTransport_TypesTable()
    {
        string query = 
        """
            CREATE TABLE IF NOT EXISTS transport_types
            (
                id INT PRIMARY KEY AUTO_INCREMENT,
                name VARCHAR(255) UNIQUE
            );
        """;
        return query;
    }
    static public string CreateTransportsTable()
    {
        string query = 
        """
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
        """;
        return query;
    }
    static public string CreateTransports_Per_PackageTable()
    {
        string query = 
        """
            CREATE TABLE IF NOT EXISTS transport_per_package
            (
                transport INT NOT NULL REFERENCES transports(id),
                package   INT NOT NULL REFERENCES packages(id),
                UNIQUE (transport, package)
            );
        """;
        return query;
    }
    static public string CreatePaymentTable()
    {
        string query = 
        """
            CREATE TABLE IF NOT EXISTS payments
            (
                id INT PRIMARY KEY AUTO_INCREMENT,
                amount DECIMAL(10,2) NOT NULL,
                payment_time TIMESTAMP
            ); 
        """;
        return query;
    }
    static public string CreateBookingsTable()
    {
        string query = 
        """
        CREATE TABLE IF NOT EXISTS bookings
        (
            id INT PRIMARY KEY AUTO_INCREMENT,
            payment INT NOT NULL REFERENCES payments(id),
            user INT NOT NULL REFERENCES users(id),
            package INT NOT NULL REFERENCES packages(id),
            UNIQUE (id, user)
        );
        """;
        return query;
    }
    static public string CreateRoomsTable()
    {
        string query = 
        """
            CREATE TABLE IF NOT EXISTS rooms
            (
                id INT PRIMARY KEY AUTO_INCREMENT,
                name VARCHAR(255) NOT NULL,
                sleep_spots INT NOT NULL,
                accommodation INT NOT NULL REFERENCES accommodations(id)
            );
        """;
        return query;
    }
    static public string CreateBooked_RoomsTable()
    {
        string query = 
        """
            CREATE TABLE IF NOT EXISTS booked_rooms
            (
                rooms int NOT NULL REFERENCES rooms(id),
                start_datetime DATETIME,
                end_datetime DATETIME,
                UNIQUE(rooms, start_datetime, end_datetime)
            );
        """;
        return query;
    }
    static public string CreateRoom_PropertiesTable()
    {
        string query = 
        """
            CREATE TABLE IF NOT EXISTS room_properties
            (
            id INT PRIMARY KEY AUTO_INCREMENT,
            name VARCHAR(255) UNIQUE
            );
        """;
        return query;
    }
    static public string CreateProperties_Per_RoomTable()
    {
        string query = 
        """
            CREATE TABLE IF NOT EXISTS properties_per_room
            (
                room INT NOT NULL REFERENCES rooms(id),
                property INT NOT NULL REFERENCES rooms_properties(id),
                UNIQUE (room,property)
            );
        """;
        return query;
    }
    static public string CreateAmenitiesTable()
    {
        string query = 
        """
            CREATE TABLE IF NOT EXISTS amenities
            (
                id INT PRIMARY KEY AUTO_INCREMENT,
                name VARCHAR(255) UNIQUE
            );
        """;
        return query;
    }
    static public string CreateAmenities_Per_AccommodationTable()
    {
        string query = 
        """
            CREATE TABLE IF NOT EXISTS amenities_per_accommodation
            (
                amenity INT NOT NULL REFERENCES amenities(id),
                accommodation INT NOT NULL REFERENCES accommodations(id),
                UNIQUE (amenity,accommodation)
            )
        """;
        return query;
    }
}