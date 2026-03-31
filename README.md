# EventHub API

EventHub is a production-ready Event Management System backend built with ASP.NET Core 8 Web API, Entity Framework Core, SQL Server, and JWT Authentication.

## Features

- **Entity Relationships**:
  - One-to-One: Organizer ↔ OrganizerProfile
  - One-to-Many: Organizer → Events
  - Many-to-Many: Attendee ↔ Event via Registration
- **JWT Authentication**: Secure login for Organizers and Attendees with role-based access control.
- **DTO Pattern**: Clear separation between database entities and API responses.
- **Validation**: Strict validation on all incoming data using Data Annotations.
- **Docker Support**: Full containerization with SQL Server and API orchestration.
- **Auto-Migrations**: Database migrations are applied automatically on startup.

## How to Run

### Option A — Docker (Recommended)
```bash
# From the project root:
docker-compose up --build

# API will be available at:
# http://localhost:8080/swagger  ← Swagger UI
# http://localhost:8080/api      ← API base URL
```

To stop:
`docker-compose down`

To wipe database volume and start fresh:
`docker-compose down -v`

### Option B — Local Development
```bash
dotnet restore
dotnet ef migrations add InitialCreate
dotnet ef database update
dotnet run
# Open: https://localhost:5001/swagger
```

## API Testing with Swagger

Follow these steps to test the full system:

1.  **Register an Organizer**:
    `POST /api/organizers`
    Body: `{ "name": "Alice", "email": "alice@test.com", "password": "pass123" }`
2.  **Login as Organizer**:
    `POST /api/auth/organizer/login`
    Body: `{ "email": "alice@test.com", "password": "pass123" }`
    → Copy the token from the response.
3.  **Authorize in Swagger**:
    Click the 🔒 **Authorize** button (top right).
    Enter: `Bearer <paste_your_token_here>`
    Click **Authorize** → **Close**.
4.  **Create an Event**:
    `POST /api/events`
    Body: `{ "title": "Tech Summit 2025", "description": "Annual tech conference", "date": "2025-09-01T09:00:00", "capacity": 200, "location": "Cairo Convention Center" }`
5.  **Register an Attendee**:
    `POST /api/attendees`
    Body: `{ "name": "Bob", "email": "bob@test.com", "password": "pass123" }`
6.  **Login as Attendee**:
    `POST /api/auth/attendee/login`
    → Copy token → Re-authorize in Swagger with Attendee token.
7.  **Attendee Registers for Event**:
    `POST /api/events/{id}/register`
8.  **Verify**:
    - `GET /api/events/{id}` → Check `RegisteredCount` increased.
    - `GET /api/attendees/{id}` → Check `RegisteredEvents` list.

## Validation Testing

| Test | Request | Expected Response |
|---|---|---|
| Missing required field | POST /api/events with no Title | 400 Bad Request |
| Capacity out of range | Capacity: 0 or 99999 | 400 Bad Request |
| Invalid email | email: "notanemail" | 400 Bad Request |
| Short password | password: "abc" | 400 Bad Request |
| Duplicate registration | Register same attendee twice | 400 Bad Request |
| Event at capacity | Register when Capacity reached | 400 Bad Request |

## Why HTTP-Only Cookies Are the Industry Standard for Auth

JWT tokens stored in `localStorage` are readable by any JavaScript running on the page. If the site is vulnerable to an XSS (Cross-Site Scripting) attack — where malicious script gets injected — the attacker can steal the token and impersonate the user completely.

HTTP-only cookies cannot be read by JavaScript at all. The browser automatically attaches them to requests and automatically keeps them hidden. Combined with two additional flags:

- `Secure` — cookie only travels over HTTPS, never plain HTTP
- `SameSite=Strict` — cookie is not sent on cross-site requests, blocking CSRF

This combination makes HTTP-only cookies significantly more secure than storing tokens in browser storage, which is why they are the preferred approach in production systems.
