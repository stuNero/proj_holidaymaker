CREATE DATABASE IF NOT EXISTS holidaymaker;

USE holidaymaker;

CREATE TABLE IF NOT EXISTS Users
(
    id          INT PRIMARY KEY AUTO_INCREMENT,
    first_name  VARCHAR(255),
    last_name   VARCHAR(255),
    email       VARCHAR(255),
    password    VARCHAR(255),
    role        VARCHAR(255)
);
CREATE TABLE IF NOT EXISTS Cuisine
(
    id      INT PRIMARY KEY AUTO_INCREMENT,
    name    VARCHAR(255)
);
CREATE TABLE IF NOT EXISTS Country
(
    id      INT PRIMARY KEY AUTO_INCREMENT,
    name    VARCHAR(255),
    cuisine INT,
    FOREIGN KEY (cuisine) REFERENCES Cuisine(id)
);
CREATE TABLE IF NOT EXISTS City
(
    id      INT PRIMARY KEY AUTO_INCREMENT,
    name    VARCHAR(255),
    country INT,
    FOREIGN KEY (country) REFERENCES Country(id)
);
CREATE TABLE IF NOT EXISTS Accommodation
(
    id      INT PRIMARY KEY AUTO_INCREMENT,
    name    VARCHAR(255),
    city    INT,
    FOREIGN KEY (city) REFERENCES City(id)
);
CREATE TABLE IF NOT EXISTS Package
(
    id      INT PRIMARY KEY AUTO_INCREMENT,
    name    VARCHAR(255),
    description TEXT,
    price DECIMAL(10,2)
);
CREATE TABLE IF NOT EXISTS AccommodationPerPackage
(
    accommodation INT,
    package INT,
    FOREIGN KEY (accommodation) REFERENCES Accommodation(id),
    FOREIGN KEY (package) REFERENCES Package(id)
);
CREATE TABLE IF NOT EXISTS TransportType
(
  id INT PRIMARY KEY AUTO_INCREMENT,
  name VARCHAR(255) UNIQUE
);
CREATE TABLE IF NOT EXISTS Transport
(
    id INT PRIMARY KEY AUTO_INCREMENT,
    type INT,
    start_city INT,
    end_city INT,
    company VARCHAR(255),
    price decimal(10,2),
    FOREIGN KEY (type) REFERENCES TransportType(id),
    FOREIGN KEY (start_city) REFERENCES City(id),
    FOREIGN KEY (end_city) REFERENCES City(id)
);
CREATE TABLE IF NOT EXISTS TransportPerPackage
(
    transport INT,
    package   INT,
    FOREIGN KEY (transport) REFERENCES Transport(id),
    FOREIGN KEY (package) REFERENCES Package(id)
);
CREATE TABLE IF NOT EXISTS Payment
(
    id INT PRIMARY KEY AUTO_INCREMENT,
    amount DECIMAL(10,2),
    method VARCHAR(255),
    payment_time DATETIME
);
CREATE TABLE IF NOT EXISTS Booking
(
    id INT PRIMARY KEY AUTO_INCREMENT,
    payment INT,
    user INT,
    package INT,
    start_time DATETIME,
    end_time DATETIME,
    total_cost DECIMAL(10,2),
    UNIQUE (id, user),
    FOREIGN KEY (payment) REFERENCES Payment(id),
    FOREIGN KEY (user) REFERENCES Users(id),
    FOREIGN KEY (package) REFERENCES Package(id)
);
CREATE TABLE IF NOT EXISTS Rooms
(
    id INT PRIMARY KEY AUTO_INCREMENT,
    name VARCHAR(255),
    sleep_spots INT,
    accommodation INT,
    FOREIGN KEY (accommodation) REFERENCES Accommodation(id)
);
CREATE TABLE IF NOT EXISTS RoomProperties
(
  id INT PRIMARY KEY AUTO_INCREMENT,
  name VARCHAR(255) UNIQUE
);
CREATE TABLE IF NOT EXISTS PropertiesPerRoom
(
    room INT,
    property INT,
    UNIQUE (room,property),
    FOREIGN KEY (room) REFERENCES Rooms(id),
    FOREIGN KEY (property) REFERENCES RoomProperties(id)
);
CREATE TABLE IF NOT EXISTS Amenities
(
    id INT PRIMARY KEY AUTO_INCREMENT,
    name VARCHAR(255) UNIQUE
);
CREATE TABLE IF NOT EXISTS AmenitiesPerAccommodation
(
    amenity INT,
    accommodation INT,
    UNIQUE (amenity,accommodation),
    FOREIGN KEY (amenity) REFERENCES Amenities(id),
    FOREIGN KEY (accommodation) REFERENCES Accommodation(id)
)