# Database-First persistence model

Files under `Generated` are produced by EF Core Database-First scaffolding from the existing `ahdah` PostgreSQL schema. They are persistence models, not domain models, and must not be manually edited.

Custom behavior and extensions belong in partial classes outside `Generated`. Database changes require explicit approval. Re-scaffolding must occur on a dedicated branch and be reviewed, including all generated mapping changes and provider warnings.
