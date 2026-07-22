# API conventions

These conventions establish the initial direction for future endpoints. The health endpoint is intentionally small and database independent.

## Routes and versioning

- Public application routes begin with `/api`.
- Resource names use lowercase, stable nouns and clear nested relationships only when ownership is real.
- The initial health route is `GET /api/system/health`.
- API versioning will be introduced deliberately before incompatible business endpoints are published. URL-segment versioning such as `/api/v1/...` is the current direction, subject to an architecture decision record.

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

## Idempotency for financial commands

- Commands that can move money, create liabilities, record payments, or close settlements require a client-supplied idempotency key when introduced.
- A key is scoped to the authenticated tenant and operation, stored with an integrity-safe request fingerprint, and returns the original outcome for safe retries.
- Reusing a key with a different payload is rejected.
- Idempotency is not a substitute for database transactions, concurrency control, authorization, or audit records.
