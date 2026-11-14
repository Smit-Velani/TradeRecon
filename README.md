# TradeRecon

TradeRecon is a financial reconciliation and data governance platform built with C# and .NET 8.

It compares trades from internal systems and broker feeds, detects reconciliation breaks, persists results, records audit activity, and exposes operational metrics through a Blazor dashboard.

## Architecture

Blazor Dashboard
-> ASP.NET Core REST API
-> Application Services
-> Repository Interfaces
-> Entity Framework Core
-> SQLite

## Projects

TradeRecon.Core
Domain models, enums, DTOs, and interfaces.

TradeRecon.Application
Reconciliation logic, validation, audit services, and business rules.

TradeRecon.Infrastructure
Entity Framework Core, SQLite persistence, repositories, and CSV ingestion.

TradeRecon.Api
REST APIs, middleware, rate limiting, health checks, and background reconciliation.

TradeRecon.Web
Interactive Blazor operations dashboard.

TradeRecon.Tests
xUnit unit tests and ASP.NET Core integration tests.

## Reconciliation Rules

TradeRecon detects:

- Matched trades
- Quantity mismatches
- Price mismatches
- Missing broker trades
- Missing internal trades
- Duplicate trade IDs
- Invalid trade data

## Example

Internal:
T001 AAPL 100 278.40
T002 MSFT 50 492.15
T003 NVDA 75 188.20

Broker:
T001 AAPL 100 278.40
T002 MSFT 55 492.15
T004 AMZN 20 244.10

Result:
T001 Matched
T002 QuantityMismatch
T003 MissingFromBroker
T004 MissingFromInternal

## Technology

- C#
- .NET 8
- ASP.NET Core
- Blazor
- Entity Framework Core
- SQLite
- REST APIs
- LINQ
- async and await
- Dependency Injection
- Repository Pattern
- BackgroundService
- Structured Logging
- Middleware
- Rate Limiting
- Health Checks
- EF Core Migrations
- xUnit
- Moq
- Integration Testing
- Docker
- GitHub Actions

## APIs

GET /api/trades

GET /api/trades/search

POST /api/import?source=Internal

POST /api/import?source=Broker

POST /api/reconciliation/run

GET /api/reconciliation

GET /api/reconciliation/{id}

GET /api/dashboard/summary

GET /api/audit

GET /health

## Pipeline

CSV file
-> API
-> CSV parser
-> Validation
-> Idempotency check
-> Entity Framework Core
-> SQLite
-> Reconciliation engine
-> Audit trail
-> Blazor dashboard

## Reliability

- Input validation
- Idempotent imports
- Duplicate detection
- Global exception handling
- Correlation IDs
- Structured logging
- Rate limiting
- Database indexes
- Background reconciliation
- Health checks
- Audit logging
- Unit tests
- Integration tests
- CI/CD

## Run Locally

API:

dotnet run --project TradeRecon.Api --urls http://localhost:5055

Dashboard:

dotnet run --project TradeRecon.Web --urls http://localhost:5056

Open:

http://localhost:5056

## Testing

dotnet test

## Docker

docker compose up --build

## Engineering Concepts Demonstrated

- Object oriented programming
- Classes and objects
- Interfaces and abstraction
- Encapsulation
- Inheritance
- Generics
- LINQ
- async and await
- Exception handling
- Dependency injection
- Repository pattern
- Separation of concerns
- REST API design
- Database persistence
- ORM usage
- EF Core migrations
- Background processing
- Middleware
- Configuration
- Containerization
- Automated testing
- CI/CD
- Financial reconciliation
- Data governance
- Auditability
