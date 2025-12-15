# Tables
## Booking
booking_id
user_id

## Users
## Payment
## Package
## Transport
id
TransportType FOREIGN KEY (transporttype.id)

## TransportType
id
name



## Region



## Cuisines
- Mediterranian
- North European
- East European
- West European

## Countries
country_id
name

## City
id
name
country_id FOREIGN KEY

## Accommodation

## HasAmenity
accommodation_id INT FOREIGN KEY
amenity_id INT FOREIGN KEY
UNIQUE (accommodation_id, amenity_id)

### Example
 ____________________
| accom_id | amen_id |
|__________|_________|
| 1        | 2
## Amenities


## RoomsByAccommodation

acc1, r1
acc1, r2
acc2, r3
acc2, r5

UPDATE availability
FROM RoomsByAccommodation
SET true WHERE room = r2;

SELECT room FROM RoomsByAccommodation WHERE availability = true ;



