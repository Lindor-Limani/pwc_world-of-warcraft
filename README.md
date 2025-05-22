# PWC RPG API

This project is a .NET 8 Web API for managing a simple RPG world, including **Monsters**, **Charakters**, and **Items**. It uses Entity Framework Core with PostgreSQL, MediatR for CQRS, and AutoMapper for DTO mapping. The API is designed for extensibility and easy local development, including database seeding for demo/test data.

---

## Features

- CRUD operations for Monsters, Charakters, and Items
- Equip/unequip items for Charakters
- Assign item drops to Monsters
- Query endpoints for filtering by name, category, or relationships
- Automatic database migration and seeding on startup (in development)
- OpenAPI/Swagger documentation

---

## Getting Started

### 1. Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download)
- [PostgreSQL](https://www.postgresql.org/) (local or via Docker)
- [Docker & Docker Compose](https://docs.docker.com/get-docker/) (recommended for easy setup)
- [Node.js](https://nodejs.org/) (optional, for tools like Postman)

### 2. Configuration

- The connection string is set in `pwc.API/appsettings.json` under `DefaultConnection`.
- By default, the API expects a PostgreSQL instance named `pwc-db` with user `postgres` and password `postgres`.

### 3. Database Migration & Seeding

- On startup (in development), the API will **automatically apply migrations and seed the database** with demo data (see `AppDbContext.Seed()`).
- The seed uses [Bogus](https://github.com/bchavez/Bogus) to generate random Items, Monsters, and Charakters.

### 4. Running with Docker Compose

- Use the provided `docker-compose.yml` to start both the API and the database:
```
docker compose up --build
```

- The API will wait for the database, apply migrations, and seed data on first run.

### 5. API Documentation

- After starting, navigate to [https://localhost:8081/swagger/index.html](https://localhost:8081/swagger/index.html) (or the port in your config) for interactive API docs.

---

## Project Structure

- **pwc.API**: ASP.NET Core Web API entry point and controllers
- **pwc.Application**: CQRS command/query handlers, DTOs, and mapping
- **pwc.Domain**: Domain models, enums, interfaces, and exceptions
- **pwc.Infrastructure**: EF Core `AppDbContext`, migrations, and seeding
- **pwc.Repository**: Repository implementations
- **pwc.Extensions**: Extension methods (e.g., for migrations)
- **pwc.API.Test / pwc.Application.Test / pwc.Repository.Test**: Unit and integration tests

---

## Useful Endpoints

- `GET /api/monsters` — List all monsters
- `GET /api/monsters/{id}` — Get monster by ID
- `POST /api/monsters` — Create a new monster
- `POST /api/monsters/{monsterId}/drops` — Add item drop to monster
- `GET /api/charakters` — List all charakters
- `POST /api/charakters` — Create a new charakter
- `POST /api/charakters/{charakterId}/equip` — Equip item to charakter
- `GET /api/items` — List all items
- `POST /api/items` — Create a new item

See Swagger UI for the full list and request/response schemas.

---

## First Steps Checklist

1. **Clone the repository**
2. **Configure your connection string** in `appsettings.json` if needed
3. **Run with Docker Compose**: `docker compose up --build`
4. **Check the logs**: Ensure migrations and seeding run without errors
5. **Open Swagger UI**: Try out the API and inspect the seeded data
6. **(Optional) Import the provided Postman collection** for quick API testing

---

## Troubleshooting

- **Database not seeded?**  
  Ensure the `Seed()` method is called after migrations (see `MigrationsExtensions.cs`).
- **Connection issues?**  
  Check that the database container is healthy and the connection string matches.
- **Port conflicts?**  
  Adjust the ports in `docker-compose.yml` or `launchSettings.json`.

---

## Test Results


| Test Suite | Passed | Failed |
|------------|--------|--------|
| Total      | 144    | 0      |



