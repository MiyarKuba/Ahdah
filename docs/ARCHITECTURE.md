# Architecture

## System shape

Ahdah consists of a Flutter client, an ASP.NET Core Web API, and an existing PostgreSQL database. The Flutter application communicates with the API over HTTPS using JSON REST endpoints. Only the API and approved server-side processes may communicate with PostgreSQL.

```text
Flutter (Android / iOS / Web)
              |
         HTTPS + JSON
              |
ASP.NET Core controller-based API
              |
   Modular Monolith internals
              |
PostgreSQL 18 / ahdah_db / ahdah
```

The initialization phase contains no database connection and no business modules. The boundaries below are directions for later reviewed work, not implemented features.

## Flutter client

Flutter provides one shared product experience across Android, iOS, and Web. Presentation, navigation, client-side input feedback, and API transport belong in the client. Platform-specific behavior must be isolated behind a shared interface and must have an iOS-compatible path whenever Android behavior is introduced.

The client is not a trusted enforcement boundary. Authorization, tenant isolation, financial invariants, and audit behavior must be enforced by the API even if the client also provides contextual validation.

## ASP.NET Core Web API

The .NET 10 API is controller based and owns the public HTTP contract. It is responsible for authentication and authorization when introduced, tenant context resolution, input validation, application orchestration, transaction boundaries, persistence access, audit production, and safe integration with external services.

The initial project dependency direction is:

```text
Ahdah.Api            -> Ahdah.Application, Ahdah.Infrastructure
Ahdah.Infrastructure -> Ahdah.Application, Ahdah.Domain
Ahdah.Application    -> Ahdah.Domain
Ahdah.Domain         -> no other Ahdah project
```

Dependencies must continue to point inward toward business policy. Infrastructure details must not leak into the domain.

## Modular Monolith

A modular monolith provides one deployable backend while keeping high-cohesion business areas explicit. Modules will share process hosting and operational tooling but should communicate through defined application contracts rather than reaching into one another's internals.

Candidate future module boundaries are:

- Companies and tenant administration
- Users, roles, and access control
- Projects and construction operations
- Funding sources and custody advances
- Transfers and custody balances
- Expenses, receipts, and reimbursements
- Suppliers, debts, and payments
- Worker claims
- Advance settlement and closure
- Documents and attachments
- Notifications
- Audit
- Background jobs, scheduled reports, generation, and delivery

These boundaries require validation against the existing schema and approved business rules before implementation. They are not permission to restructure the database.

## PostgreSQL and database-first integration

The existing `ahdah_db` database, `ahdah` schema, and 91 tables are the current source of truth. A later task will add EF Core and Npgsql through a controlled database-first workflow. The application must not call `EnsureCreated`, apply migrations automatically, or make destructive schema changes.

Financial values use PostgreSQL `NUMERIC` and C# `decimal`. Financial ledger changes require explicit transactions and auditable behavior.

## Multi-tenancy

Tenant ownership is based on `company_id`. Every tenant-owned query and command must scope and verify `company_id` at the server boundary and persistence layer. An identifier supplied by a client is never sufficient proof that the active tenant owns a record.

Cross-tenant access must be denied by default. Future background jobs and reports must carry an explicit tenant context and apply the same rules as interactive requests.

## REST communication

The Flutter client uses versionable `/api` REST endpoints with JSON payloads, UTC timestamps, consistent errors, correlation IDs, pagination, validation, and idempotency controls for financial commands. Detailed conventions are in [API_CONVENTIONS.md](API_CONVENTIONS.md).

## Why Flutter never connects directly to PostgreSQL

A direct database connection from a mobile or browser client would expose credentials, bypass authorization and tenant enforcement, couple releases to physical schema details, and make financial auditing and transaction policy unreliable. Browser environments also cannot safely hold database secrets. All database access therefore remains behind the API, where credentials can be protected and server-side policy can be enforced consistently.
