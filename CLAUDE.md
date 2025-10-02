# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

API Portal is an ASP.NET Core 8 API key management system with HTMX frontend and SQLite backend. It allows users to create, manage, and revoke API keys in a Stripe-like format (`{prefix}_{environment}_{random}`).

## Architecture

### Core Services Pattern
- **ApiKeyService** (`Services/ApiKeyService.cs`): Handles API key generation, hashing (SHA-256), and CRUD operations. Keys are 32-character cryptographically secure random strings.
- **AuthService** (`Services/AuthService.cs`): Manages user authentication using PBKDF2 password hashing with salt.

### Database Context
- **AppDbContext** (`Data/AppDbContext.cs`): EF Core context implementing `IDataProtectionKeyContext` for session key persistence across container restarts.
- Uses code-first approach with `Database.EnsureCreated()` instead of migrations.

### Security Model
- API keys are hashed before storage; full keys shown only once at creation.
- Keys auto-expire after 30 days (configurable in `ApiKeyService.cs:44`).
- Cookie-based authentication with 24-hour expiration (configurable in `Program.cs:30`).
- DataProtection keys stored in database for container restart persistence.

### Data Flow
1. User authenticates via `AuthController` → Sets authentication cookie
2. User creates key via `ApiKeysController.Create()` → `ApiKeyService.GenerateApiKey()` → Returns full key + stores hash
3. Keys displayed via HTMX partial views (`Views/ApiKeys/_KeyList.cshtml`)

## Development Commands

### Docker (Recommended)
```bash
# Start application
docker-compose up --build

# Stop and remove containers
docker-compose down

# Stop and remove containers + database file
docker-compose down -v
```

### Local .NET Development
```bash
# Run application (SQLite database created automatically)
dotnet run

# Build project
dotnet build

# Clean build artifacts
dotnet clean
```

**Note**: This project uses `Database.EnsureCreated()` instead of EF migrations. Schema changes require deleting the SQLite database file to recreate it.

### Database Setup
- **Automatic**: Database created and seeded on application startup via `Program.cs:50-53`
- **Manual reset**: Delete `apiportal.db` file (or use `docker-compose down -v` for Docker)
- Seeder (`Data/DbSeeder.cs`) creates demo users: admin/admin123, john/john123, sarah/sarah123, demo/demo123

### Connection Strings
- **Local development**: `appsettings.json` points to `Data Source=apiportal.db` (created in project root)
- **Docker**: Container uses `Data Source=/app/data/apiportal.db` via environment variable, persisted in Docker volume

## Configuration

### API Key Format
Modify in `ApiKeyService.GenerateApiKey()`:
- Key length: `KeyLength` constant (line 13)
- Expiration: `AddDays(30)` (line 44)
- Format: `{prefix}_{environment}_{random}`

### Available Prefixes
Defined in `Views/ApiKeys/Create.cshtml`: `sk` (secret), `pk` (public), `sea` (search), `api` (general)

### Scopes
Seeded in `DbSeeder.cs`: "CP Data API", "Carbon API". Add new scopes by modifying seeder.

## Important Notes

- **Database Viewer** (`Controllers/DbViewerController.cs`): Demo feature only - exposes raw database contents with auto-refresh. Remove for production.
- **Production Security**: Add certificate-based DataProtection encryption (see `Program.cs:18-19` comment) or use cloud key management (Azure Key Vault, AWS KMS).
- **No API Endpoints**: System currently has no validation endpoints for generated API keys - purely a management portal.
- Default route is `Welcome` controller (anonymous welcome page), authenticated users redirected to `Home`.
