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
``` 
