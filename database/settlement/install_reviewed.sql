\set ON_ERROR_STOP on
\if :{?expected_database}
\else
DO $$ BEGIN RAISE EXCEPTION 'provide expected_database'; END $$;
\endif
\if :{?expected_catalog_fingerprint}
\else
DO $$ BEGIN RAISE EXCEPTION 'provide expected_catalog_fingerprint'; END $$;
\endif
\if :{?runtime_role}
\else
DO $$ BEGIN RAISE EXCEPTION 'provide runtime_role'; END $$;
\endif
\if :{?settlement_ddl_approved}
\else
DO $$ BEGIN RAISE EXCEPTION 'provide settlement_ddl_approved'; END $$;
\endif
\if :settlement_ddl_approved
\else
DO $$ BEGIN RAISE EXCEPTION 'settlement_ddl_approved must be true'; END $$;
\endif
SELECT current_database() = :'expected_database' AS expected_database_ok \gset
\if :expected_database_ok
\else
DO $$ BEGIN RAISE EXCEPTION 'connected database does not match expected_database'; END $$;
\endif
SELECT set_config('ahdah.runtime_role', :'runtime_role', false);
DO $top_role$
DECLARE role_name text:=current_setting('ahdah.runtime_role');
BEGIN
 IF NOT EXISTS(SELECT 1 FROM pg_roles WHERE rolname=role_name AND NOT rolsuper AND NOT rolcreaterole AND NOT rolcreatedb AND NOT rolbypassrls) THEN
  RAISE EXCEPTION 'Runtime role absent or privileged';
 END IF;
END $top_role$;
\echo 'INSTALLATION REQUIRES SEPARATE EXECUTION APPROVAL.'
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
