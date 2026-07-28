# API conventions

These conventions establish the initial direction for future endpoints. The health endpoint is intentionally small and database independent.

## Routes and versioning

- Public application routes begin with `/api`.
- Resource names use lowercase, stable nouns and clear nested relationships only when ownership is real.
- The initial health route is `GET /api/system/health`.
- API versioning will be introduced deliberately before incompatible business endpoints are published. URL-segment versioning such as `/api/v1/...` is the current direction, subject to an architecture decision record.

Identity Phase 1 uses the versioned routes `POST /api/v1/auth/register-company`, `POST /api/v1/auth/login`, and `GET /api/v1/auth/me`. The health route remains anonymous. No refresh or server logout route is published until secure server-side session storage exists.

## JSON

- Requests and responses use `application/json` unless an endpoint explicitly handles files.
- JSON properties use `camelCase`.
- Contracts are explicit; persistence models are never returned directly.
- Optional, absent, empty, and null values must have documented meanings.

## Time

- API timestamps represent UTC and use ISO 8601 values with an explicit `Z` or UTC offset.
- Server code uses `DateTimeOffset` for instants unless a reviewed domain concept requires another type.
- Local dates and time zones are modeled explicitly; client locale must not silently change stored instants.

## Errors

- Non-success responses use a consistent Problem Details-compatible JSON shape.
- Errors include a stable machine-readable code, a safe human-readable message, the HTTP status, and the request correlation ID.
- Internal exceptions, SQL details, credentials, and sensitive tenant data are never exposed.
- Authorization failures must not disclose whether a cross-tenant record exists.
- Registration uniqueness conflicts return a non-disclosing 409 Problem Details response. Login failures use the same generic 401 response for unknown identifiers, invalid passwords, inactive or suspended users, inactive companies, and active lock timestamps.

## Validation

- Structural and domain input validation occurs server side even when Flutter validates the same input.
- Validation errors identify safe field paths and stable codes.
- Invalid financial commands fail as a unit; partial state changes are not accepted.

## Pagination

- Collection endpoints are paginated by default.
- Requests use bounded page sizes with a documented default and maximum.
- Responses include items and pagination metadata or an opaque continuation token.
- Sort order must be deterministic, and filters must be explicitly allow-listed.

## Correlation IDs

- The API accepts a valid incoming correlation ID or generates one.
- The correlation ID is returned in response headers and included in structured server logs and safe error payloads.
- Background work originating from a request carries the originating correlation context when appropriate.

## Tenant isolation

- Authenticated tenant context is resolved server side.
- Every tenant-owned query and command enforces `company_id`.
- Route or payload identifiers never override the authenticated tenant context.
- Cross-tenant identifiers produce a non-disclosing authorization or not-found response according to the reviewed security policy.
- JWT access tokens carry only `sub`, `company_id`, `role`, `jti`, and optional safe display `name`. Tenant context is never taken from client headers, query strings, route values, or bodies.
- `GET /api/v1/auth/me` resolves both identifiers from validated claims and filters by both `company_id` and `user_id` before returning explicit safe DTOs.

## Authentication and authorization

- JWT Bearer validation requires issuer, audience, expiration, signature, and the configured signing key. Signed tokens and a small explicit clock skew are required.
- The non-secret defaults are issuer `Ahdah.Api`, audience `Ahdah.Clients`, and a 15-minute access-token lifetime. Signing keys are secret configuration and never belong in repository files.
- Authorization policies are `AuthenticatedUser`, `CompanyMember`, and `ManagerOnly`. `CompanyMember` requires a valid GUID `company_id` claim. `ManagerOnly` additionally requires the exact `Manager` role.
- Access-token responses use token type `Bearer` and an explicit UTC expiry. Password hashes and generated persistence entities never appear in API schemas or responses.

## Idempotency for financial commands

- Commands that can move money, create liabilities, record payments, or close settlements require a client-supplied idempotency key when introduced.
- A key is scoped to the authenticated tenant and operation, stored with an integrity-safe request fingerprint, and returns the original outcome for safe retries.
- Reusing a key with a different payload is rejected.
- Idempotency is not a substitute for database transactions, concurrency control, authorization, or audit records.
