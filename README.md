# TaskTrack Backend

ASP.NET Core Web API for QE190107 SE19B.NET Assignment 1.

## Architecture

- `TaskTrack.API` — controllers, HTTP middleware, dependency injection, CORS and Swagger.
- `TaskTrack.Service` — DTOs, validation and business rules.
- `TaskTrack.Repo` — Entity Framework Core models, `DbContext` and repositories.

All database access is isolated in the repository project. Controllers communicate only with service interfaces.

## Build

```powershell
dotnet restore QE190107_SE19B.NET_Ass1_BE.sln
dotnet build QE190107_SE19B.NET_Ass1_BE.sln
```

Set the database connection in `TaskTrack.API/appsettings.json` or provide `DATABASE_URL`. Render-style `postgres://` URLs and standard Npgsql connection strings are both supported.

## API groups

- `/api/departments`
- `/api/projects`
- `/api/tasks`
- `/api/tags`
- `/health`
- `/swagger`
