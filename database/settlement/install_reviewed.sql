\set ON_ERROR_STOP on
\echo 'INSTALLATION REQUIRES SEPARATE EXECUTION APPROVAL. PREPARATION ONLY.'
\if :{?settlement_ddl_approved}
\else
\quit 3
\endif
\if :settlement_ddl_approved
\else
\echo 'STOP: settlement_ddl_approved must be true'
\quit 3
\endif
\ir 00_preflight.sql
BEGIN;
SET LOCAL lock_timeout='5s';
SET LOCAL statement_timeout='10min';
SET LOCAL search_path=ahdah,pg_catalog;
\ir 01_core_financial_control.sql
\ir 02_custody.sql
\ir 03_settlement_certification.sql
\ir 04_policy_evidence.sql
\ir 05_owner_reconciliation.sql
\ir 06_resolution_credit_intent.sql
\ir 07_legacy_cutover_schema.sql
\ir 08_existing_table_alterations.sql
\ir 09_indexes_constraints_triggers.sql
\ir 10_runtime_privileges.sql
COMMIT;
-- No cutover population, activation or fixture execution is included.
