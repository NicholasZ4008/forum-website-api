# Forum API Server

A lightweight, efficient ASP.NET Core API for managing email subscriptions for The Forum University.

## Overview

The Forum API is a RESTful web service that provides endpoints for managing email subscriptions. It uses an in-memory database for development and features a clean, minimal API design using ASP.NET Core's minimal API approach.

## Features

- Email subscription management (CRUD operations)
- CORS support for cross-origin requests
- In-memory database for quick development
- Swagger UI for API documentation and testing
- JSON response formatting

## Prerequisites

- [.NET 7.0 SDK](https://dotnet.microsoft.com/download/dotnet/7.0) or later
- Any code editor (recommended: Visual Studio Code, Visual Studio)

## Setup

1. Clone the repository
   ```
   git clone https://github.com/yourusername/forum-api.git
   cd forum-api
   ```

2. Install the required packages
   ```
   dotnet restore
   ```
   
3. If Swagger is not already included, add it with:
   ```
   dotnet add package NSwag.AspNetCore
   ```

## Running the API

Start the API with:

```
dotnet run
```

The API will be available at:
- https://localhost:5246 (or the port specified in your environment)

## API Testing with Swagger

Swagger UI is integrated for easy API testing and documentation:

1. Start the API server with `dotnet run`
2. Navigate to `https://localhost:5246/swagger` in your browser
3. Use the interactive UI to test the available endpoints

## API Endpoints

| Method | Endpoint | Description |
|--------|----------|-------------|
| GET    | /emails  | Get all email subscriptions |
| GET    | /emails/{id} | Get a specific email subscription by ID |
| POST   | /emails  | Add a new email subscription |
| PUT    | /emails/{id} | Update an existing email subscription |
| DELETE | /emails/{id} | Remove an email subscription |

## Request & Response Examples

### POST /emails

Request:
```json
{
  "id": 0,
  "EmailName": "user@example.com"
}
```

Response:
```json
{
  "id": 1,
  "EmailName": "user@example.com"
}
```

## CORS Configuration

The API is configured to accept requests from:
- http://localhost:3000 (development frontend)
- http://theforumuniversity.com (production frontend)

## Development Notes

- The API uses an in-memory database, so data will be reset when the server restarts
- For production, you would need to replace the in-memory database with a persistent option like SQL Server or PostgreSQL
- The Email model requires the `EmailName` property to be non-null (required)

## Email Model

```csharp
public class Email
{
    public int Id { get; set; }
    public required string EmailName { get; set; }
}
```

## Contributing

1. Fork the repository
2. Create a feature branch: `git checkout -b feature-name`
3. Commit your changes: `git commit -m 'Add some feature'`
4. Push to the branch: `git push origin feature-name`
5. Submit a pull request

## License

[MIT License](LICENSE)