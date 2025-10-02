# API Portal

A simple API key management system built with ASP.NET Core 8, HTMX, and SQLite.

## Features

- **User Authentication**: Simple cookie-based authentication
- **API Key Management**: Create, list, and revoke API keys
- **Stripe-like Format**: Keys follow the format `{prefix}_{environment}_{random}` (e.g., `sk_live_7Xh2kP9vN3mQ...`)
- **Configurable Prefixes**: Support for different key types (sk, pk, sea, api)
- **Scopes**: Basic scope system for access control
- **Auto-Expiration**: Keys automatically expire after 30 days
- **Secure Storage**: Keys are hashed using SHA-256 before storage
- **HTMX Integration**: Dynamic UI without page reloads
- **Database Viewer**: Live demo page showing real-time database contents (auto-refreshes every 5 seconds)

## Prerequisites

- Docker and Docker Compose
- .NET 8 SDK (for local development)

## Quick Start

### Using Docker (Recommended)

1. **Start the application:**
   ```bash
   docker-compose up --build
   ```

2. **Access the application:**
   - Open your browser to: http://localhost:5000

3. **Login with demo accounts:**
   - Username: `admin` | Password: `admin123`
   - Username: `john` | Password: `john123`
   - Username: `sarah` | Password: `sarah123`
   - Username: `demo` | Password: `demo123`

### Local Development

1. **Run the application:**
   ```bash
   dotnet run
   ```

2. **Access at:** http://localhost:5000

Note: The SQLite database file will be created automatically in the project directory on first run.

## Using the Application

### Main Portal
- Navigate to http://localhost:5000 and log in
- Create API keys with custom prefixes and scopes
- View all your API keys (masked by default)
- Revoke keys when needed
- Keys are shown in full only once at creation

### Database Viewer (Demo Feature)
- Click "Database Viewer" in the navigation bar
- **⚠️ This is a demonstration feature only** - never expose database contents in production!
- View live database contents that auto-refresh every 5 seconds
- See how API keys are hashed and stored securely
- Monitor changes in real-time as you create/revoke keys
- Tables shown:
  - **API Keys**: Full details including hashed keys, status, expiration
  - **Users**: User accounts with hashed passwords
  - **Scopes**: Available permission scopes
  - **DataProtection Keys**: Session encryption keys
- Pause/resume auto-refresh as needed

## Project Structure

```
APIportal/
├── Controllers/          # MVC Controllers
│   ├── AuthController.cs
│   ├── HomeController.cs
│   ├── ApiKeysController.cs
│   └── DbViewerController.cs (Demo only)
├── Data/                # Database context and seeding
│   ├── AppDbContext.cs
│   └── DbSeeder.cs
├── Models/              # Domain models
│   ├── User.cs
│   ├── ApiKey.cs
│   └── Scope.cs
├── Services/            # Business logic
│   ├── ApiKeyService.cs
│   └── AuthService.cs
├── Views/               # Razor views
│   ├── Auth/
│   ├── Home/
│   ├── ApiKeys/
│   ├── DbViewer/        (Demo only)
│   └── Shared/
└── wwwroot/             # Static files
    └── css/
```

## API Key Format

Keys follow the Stripe-like format:

```
{prefix}_{environment}_{random}
```

## Security Features

- Passwords hashed using PBKDF2 with SHA-256
- API keys hashed using SHA-256 before storage
- Keys shown only once at creation
- Cookie-based authentication with 24-hour expiration
- Keys auto-expire after 30 days
- Manual revocation support
- DataProtection keys stored in database (persisted across container restarts)
- ⚠️ **Production Note**: Add certificate-based encryption or use Azure Key Vault/AWS KMS for additional key protection

## Database Schema

### Users
- Id, Username, PasswordHash, Salt, CreatedAt

### ApiKeys
- Id, UserId, KeyPrefix, KeyHash, KeyPreview (last 4 chars), Scopes, CreatedAt, ExpiresAt, RevokedAt, LastUsedAt

### Scopes
- Id, Name, Description

### DataProtectionKeys
- Id, FriendlyName, Xml (encrypted key material)

## Available Scopes

- **CP Data API**: Access to CP Data API endpoints
- **Carbon API**: Access to Carbon API endpoints

## Technologies

- **Backend**: ASP.NET Core 8 MVC
- **Database**: SQLite
- **ORM**: Entity Framework Core 8
- **Frontend**: HTMX 1.9, Bootstrap 5
- **Containerization**: Docker & Docker Compose

## Stopping the Application

```bash
docker-compose down
```

To remove volumes (SQLite database file):
```bash
docker-compose down -v
```

## Future Enhancements

- API endpoint for key validation
- Usage tracking and analytics
- Rate limiting per key
- More granular scope management
- API key rotation
- Webhook support
- Email notifications
- Two-factor authentication
