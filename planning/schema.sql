```sql

CREATE DATABASE IF NOT EXISTS holidaymaker;

USE holidaymaker;

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
``` 
