# CarInsurance API – Practical Task (.NET 8, EF Core)

This project implements a simplified car insurance management system as part of a practical assignment.  
It demonstrates clean architecture, REST API design, Entity Framework Core usage, validation, and background processing in ASP.NET Core.  

---

Task A — Policy EndDate Requirement
- Refactored `InsurancePolicy.EndDate` to be **non-nullable**.  
- Added EF Core configuration to enforce `EndDate` as required.  
- Updated seed data with `2099-12-31` for open-ended policies.  
- Applied EF Core migrations using a **custom SQLite table rebuild** (since SQLite doesn’t support `ALTER COLUMN`).  
- Ensured all policies have a well-defined validity interval `[StartDate, EndDate]`.  

---

Task B1 — Insurance Claims Management
- Added insurance claims per car with a new REST endpoint:  
  `POST /api/cars/{carId}/claims`  
- Request includes **date, description, and amount**.  
- Introduced `Claim` model, DTOs, `ClaimsService`, validation logic, and controller integration.  
- Implemented **RESTful semantics**:  
  - `201 Created` + `Location` header for success.  
  - `400 / 404` for invalid cases.  

---

Task C — Enhanced Validation
- Strengthened:  
  `GET /api/cars/{carId}/insurance-valid?date=YYYY-MM-DD`  
- Features:  
  - `404` if car doesn’t exist.  
  - `400` for invalid or out-of-range dates (via `DateOnly`).  
  - Validation against policy intervals `[StartDate, EndDate]`.  
  - Propagated `CancellationToken` for clean request aborts.  
- Integration tested with **xUnit + WebApplicationFactory**.  

---

Task D — Scheduled Background Task
- Implemented `PolicyExpirationMonitor` using `BackgroundService`.  
- Runs every **30 minutes**, checks for policies expired in the last hour.  
- Prevents duplicate logs by only targeting recently expired policies.  
- Logs detailed structured warnings:  
  - Policy details  
  - Owner information  
  - Car details (Make, Model, Year, VIN)  
  - Time elapsed since expiration  
- Demonstrates **background processing in ASP.NET Core** with DI and structured logging.  

---

Key Highlights
- Clean separation of **Models, DTOs, Services, Controllers**.  
- EF Core **migrations with SQLite custom handling**.  
- REST API following proper **HTTP semantics**.  
- Robust **validation & background tasks**.  
- Automated **integration tests**.  
