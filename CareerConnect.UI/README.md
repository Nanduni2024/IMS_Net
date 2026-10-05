# CareerConnect

CareerConnect is a React/Vite frontend connected to an ASP.NET Core API and SQL Server through Entity Framework Core. It does not use mock records or demo authentication. The first database contains only the required account roles; jobs, companies, accounts, profiles, and applications are created by users.

## Requirements

- .NET 8 SDK
- Node.js and npm
- Microsoft SQL Server or SQL Server LocalDB

The checked-in development connection string uses `(localdb)\MSSQLLocalDB`. Install the SQL Server Express LocalDB runtime or change `ConnectionStrings:DefaultConnection` to an available SQL Server instance. PostgreSQL is not used by this project.

To override the connection string without changing the tracked settings file, run from the `CareerConnect` solution folder:

```powershell
dotnet user-secrets init --project .\CareerConnect.API\CareerConnect.API.csproj
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Server=.\SQLEXPRESS;Database=CareerConnectDb;Trusted_Connection=True;TrustServerCertificate=True;Connect Timeout=3;" --project .\CareerConnect.API\CareerConnect.API.csproj
dotnet user-secrets set "Jwt:Key" "replace-with-a-random-secret-at-least-32-characters" --project .\CareerConnect.API\CareerConnect.API.csproj
```

Change `Server=.\SQLEXPRESS` to the SQL Server instance installed on the machine. Windows authentication is used by this example connection string. Generate a private random JWT key and never commit it. The API refuses to start without a configured key of at least 32 bytes.

## Run

Open two terminals from the `CareerConnect` solution folder.

Backend:

```powershell
dotnet run --project .\CareerConnect.API\CareerConnect.API.csproj --urls http://localhost:5000
```

The API applies EF Core migrations on startup. To apply them manually instead:

```powershell
dotnet ef database update --project .\CareerConnect.API\CareerConnect.API.csproj --startup-project .\CareerConnect.API\CareerConnect.API.csproj
```

Frontend:

```powershell
Set-Location .\CareerConnect.UI
npm install
npm run dev -- --host 0.0.0.0 --port 5173
```

Open `http://localhost:5173`. Swagger is available at `http://localhost:5000/swagger`.

## Real-data flows

- Register as a job seeker or recruiter. Recruiter registration creates the submitted company record.
- Recruiters can publish listings; listings and their skill tags are stored in SQL Server.
- Job seekers can update their profile and apply to active listings.
- Dashboards load applications, recruiter-owned jobs, applicant details, and admin statistics from the API.
- Passwords use ASP.NET Identity's password hasher. Existing legacy SHA-256 hashes are upgraded on successful login.
- Public signup cannot create administrator accounts. An operator can set `InitialAdmin__Email` and `InitialAdmin__Password` in the host environment before the first API start; the API creates that account once after migrations. Use a password of at least 12 characters and remove the bootstrap settings after creation.

If SQL Server is unavailable, database-backed API routes return HTTP 503 with a connection setup message; the UI does not substitute dummy records.
