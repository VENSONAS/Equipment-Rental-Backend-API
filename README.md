How to Run the Project Locally
1. Start the Database (Docker)

Open a terminal inside the project root folder:

Equipment Rental Backend API/


Then run:

docker compose up -d


This starts the PostgreSQL container in the background.

2. Run the API

Run the application with:

dotnet run

(If migrations didn't apply then:
dotnet ef migrations add InitialCreate --project "../Data" --startup-project .

dotnet ef database update --startup-project "../Equipment Rental Backend API")


The API will launch and automatically connect to the database container.

Swagger UI is available at:

http://localhost:<your_port>/swagger


(Usually shown in the console output.)




Overview

This project is a backend API for an Equipment Rental system.
It provides CRUD operations for Users, Items, and Rental Bookings, implements core business logic, and integrates with an external currency API for price conversion.

The application follows a clean layered architecture:

Controller layer – HTTP endpoints

Service layer – business logic

Repository layer – database access

Domain layer – core entities

Integration layer – external currency API client

DTO layer – request/response models

A PostgreSQL database runs locally via Docker, and the API connects to it automatically using environment variables.




Key Features:

CRUD for Users, Items, RentalBookings

Rental booking business rules:

Date validation

Availability checks against overlapping bookings

Price calculation

Status transitions (Pending → Confirmed → Completed / Cancelled)

External currency service integration

Fully separated architecture layers

DTO mappings (Mapster)

Unit tests for service logic

Dockerized PostgreSQL

EF Core migrations
