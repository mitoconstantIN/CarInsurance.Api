**Task A — Policy EndDate Requirement
**
Made EndDate non-nullable in InsurancePolicy.

Added EF Core configuration to enforce EndDate as required.

Fixed seed data (2099-12-31 for open-ended policies).

Applied EF Core migrations with a custom SQLite table rebuild (since SQLite doesn’t support ALTER COLUMN).

Ensures all policies have a clear [StartDate, EndDate] interval.

**Task B1 — Insurance Claims Management
**
Added insurance claims per car with a REST endpoint:
POST /api/cars/{carId}/claims (date, description, amount).

Introduced model Claim, DTOs, ClaimsService, validation logic, and controller integration.

Full REST semantics: returns 201 Created with Location header, 400/404 for invalid cases.

**Task C — Enhanced Validation
**
Strengthened GET /api/cars/{carId}/insurance-valid?date=YYYY-MM-DD.

Features:

404 for missing car.

400 for invalid/out-of-range dates (via DateOnly).

Validation against policy intervals [StartDate, EndDate].

Propagation of CancellationToken for clean request aborts.

Integration-tested with xUnit + WebApplicationFactory.

**Task D — Scheduled Background Task
**
Implemented PolicyExpirationMonitor using BackgroundService.

Runs every 30 minutes, checks for policies expired in the last hour.

Prevents duplicate logs by only targeting recently expired policies.

Logs detailed warnings with policy, owner, car info, and elapsed time since expiration.

Demonstrates use of background processing in ASP.NET Core with DI and structured logging.
