# AcxiomCRM — Smart Customer & Sales Management System

A student-ready ASP.NET Core MVC CRM starter aligned with the uploaded AcxiomCRM project assignment.

## Included
- ASP.NET Core MVC and SQL Server / Entity Framework Core
- Custom login/register/logout pages backed by ASP.NET Core Identity for password hashing, password policy and lockout
- Admin, Manager and SalesExecutive roles
- Dashboard KPI cards
- Customer create, list, search, edit and deactivate
- Lead create, list and search
- Opportunity create/list with amount, probability and expected-close-date business rules
- Follow-up create/list with date validation
- Admin-only audit log
- Authenticated customer REST API (`GET /api/customers`, `GET /api/customers/{id}`, `POST /api/customers`)
- Bootstrap responsive layout and validation messages

## Requirements
- .NET 8 SDK
- SQL Server LocalDB (Visual Studio) or SQL Server instance
- Visual Studio 2022 or VS Code

## Run locally
1. Extract the ZIP and open `AcxiomCRM.csproj`.
2. Confirm the SQL Server connection string in `appsettings.json`.
3. From the project directory run:

   ```bash
   dotnet restore
   dotnet run
   ```

4. Open the HTTPS URL shown in the terminal.

The project creates the database schema on first launch using `EnsureCreated`. For production or ongoing schema evolution, switch to EF Core migrations (`dotnet ef migrations add InitialCreate` and `dotnet ef database update`).

## Create the first Admin
For safety, a default admin password is **not** hard-coded. Set environment variables before first launch:

**PowerShell**
```powershell
$env:ACXIOM_ADMIN_EMAIL="admin@example.com"
$env:ACXIOM_ADMIN_PASSWORD="Choose-A-Strong-Password-123"
dotnet run
```

**macOS/Linux**
```bash
export ACXIOM_ADMIN_EMAIL="admin@example.com"
export ACXIOM_ADMIN_PASSWORD="Choose-A-Strong-Password-123"
dotnet run
```

The first launch creates the three roles. If both environment variables are set and that email does not exist, it creates an Admin account. Use the Register page for normal user accounts. New public registrations are assigned the SalesExecutive role by default; this starter intentionally does not expose public role assignment.

## Important notes before submission
This is a functional starter, not a certified production deployment. It should be built, run, and tested in your own environment before submission. The assignment's full scope also mentions user/role administration UI, lead conversion, opportunity editing, full activity workflows, reporting/export, richer role/record ownership filters, and complete CRUD APIs. These are not all implemented in this starter and should be added if your evaluator expects every item.

Recommended next improvements:
1. Add record ownership enforcement for SalesExecutive and team-scope filtering for Manager.
2. Add user/role administration UI and secure role assignment.
3. Add edit/status-transition workflows for leads and opportunities.
4. Add API update/delete endpoints and integration tests.
5. Replace `EnsureCreated` with EF Core migrations and add deployment secrets/configuration.
6. Add Chart.js charts for lead status, opportunity pipeline and monthly sales.
7. Add automated tests for authorization, validation, lockout, APIs and audit behavior.

## Main routes
- `/Dashboard`
- `/Customers`
- `/Leads`
- `/Opportunities`
- `/FollowUps`
- `/Audit` (Admin role only)
- `/api/customers` (authenticated)
