\set ON_ERROR_STOP on
\if :{?expected_database}
\else
\echo 'STOP: provide expected_database'
\quit 3
\endif
\if :{?runtime_role}
\else
\echo 'STOP: provide runtime_role'
\quit 3
\endif
\if :{?writer_privileges_approved}
\else
\echo 'STOP: provide writer_privileges_approved'
\quit 3
\endif

SELECT current_database() = :'expected_database' AS expected_database_ok \gset
\if :expected_database_ok
\else
\echo 'STOP: connected database does not match expected_database'
\quit 3
\endif
\if :writer_privileges_approved
\else
\echo 'STOP: writer_privileges_approved must be true'
\quit 3
\endif

SELECT set_config('ahdah.runtime_role', :'runtime_role', false);

BEGIN;
DO $runtime_writer_safety$
DECLARE
 role_name text := current_setting('ahdah.runtime_role');
 role_oid oid;
 database_owner oid;
BEGIN
 SELECT oid INTO role_oid FROM pg_roles
  WHERE rolname=role_name AND NOT rolsuper AND NOT rolcreaterole
   AND NOT rolcreatedb AND NOT rolbypassrls;
 IF role_oid IS NULL THEN
  RAISE EXCEPTION 'Runtime role absent or privileged';
 END IF;

 SELECT datdba INTO database_owner FROM pg_database WHERE datname=current_database();
 IF role_oid=database_owner OR pg_has_role(role_oid,database_owner,'MEMBER') THEN
  RAISE EXCEPTION 'Runtime owns or inherits database ownership';
 END IF;
 IF EXISTS(
  SELECT 1 FROM pg_class c JOIN pg_namespace n ON n.oid=c.relnamespace
  WHERE n.nspname='ahdah' AND (c.relowner=role_oid OR pg_has_role(role_oid,c.relowner,'MEMBER'))
 ) OR EXISTS(
  SELECT 1 FROM pg_proc p JOIN pg_namespace n ON n.oid=p.pronamespace
  WHERE n.nspname='ahdah' AND (p.proowner=role_oid OR pg_has_role(role_oid,p.proowner,'MEMBER'))
 ) THEN
  RAISE EXCEPTION 'Runtime owns or inherits ahdah object ownership';
 END IF;
END $runtime_writer_safety$;

GRANT USAGE ON SCHEMA ahdah TO :"runtime_role";
REVOKE CREATE ON SCHEMA ahdah FROM :"runtime_role";

REVOKE ALL ON TABLE
 ahdah.project_financial_controls,ahdah.project_custody_positions,
 ahdah.custody_operations,ahdah.custody_movement_entries,ahdah.custody_reservations,
 ahdah.project_settlements,ahdah.project_settlement_snapshots,
 ahdah.project_settlement_snapshot_categories,ahdah.project_settlement_events,
 ahdah.project_closure_certificates,ahdah.project_reopenings,
 ahdah.project_authorization_decisions,ahdah.project_operational_events,
 ahdah.settlement_policy_versions,ahdah.company_settlement_policy_bindings,
 ahdah.document_custody_events,ahdah.document_policy_exceptions,
 ahdah.verified_evidence_objects,ahdah.project_owner_reconciliations,
 ahdah.project_owner_reconciliation_positions,ahdah.project_owner_retention_items,
 ahdah.financial_resolution_effects,ahdah.supplier_credit_intents,
 ahdah.supplier_credit_intent_applications,ahdah.legacy_attribution_reviews,
 ahdah.legacy_attribution_review_projects
 FROM PUBLIC, :"runtime_role";

GRANT SELECT ON TABLE
 ahdah.project_financial_controls,ahdah.project_custody_positions,
 ahdah.custody_operations,ahdah.custody_movement_entries,ahdah.custody_reservations,
 ahdah.project_settlements,ahdah.project_settlement_snapshots,
 ahdah.project_settlement_snapshot_categories,ahdah.project_settlement_events,
 ahdah.project_closure_certificates,ahdah.project_reopenings,
 ahdah.project_authorization_decisions,ahdah.project_operational_events,
 ahdah.settlement_policy_versions,ahdah.company_settlement_policy_bindings,
 ahdah.document_custody_events,ahdah.document_policy_exceptions,
 ahdah.verified_evidence_objects,ahdah.project_owner_reconciliations,
 ahdah.project_owner_reconciliation_positions,ahdah.project_owner_retention_items,
 ahdah.financial_resolution_effects,ahdah.supplier_credit_intents,
 ahdah.supplier_credit_intent_applications,ahdah.legacy_attribution_reviews,
 ahdah.legacy_attribution_review_projects
 TO :"runtime_role";

GRANT INSERT,UPDATE ON TABLE
 ahdah.project_financial_controls,ahdah.project_custody_positions,
 ahdah.custody_operations,ahdah.custody_reservations
 TO :"runtime_role";
GRANT INSERT ON TABLE
 ahdah.custody_movement_entries,ahdah.project_authorization_decisions
 TO :"runtime_role";

DO $runtime_writer_verify$
DECLARE role_name text := current_setting('ahdah.runtime_role');
BEGIN
 IF NOT has_schema_privilege(role_name,'ahdah','USAGE')
    OR has_schema_privilege(role_name,'ahdah','CREATE') THEN
  RAISE EXCEPTION 'Runtime schema privilege mismatch';
 END IF;

 IF EXISTS(
  SELECT 1 FROM (VALUES
   ('project_financial_controls',true,true),
   ('project_custody_positions',true,true),
   ('custody_operations',true,true),
   ('custody_movement_entries',true,false),
   ('custody_reservations',true,true),
   ('project_authorization_decisions',true,false),
   ('project_settlements',false,false),
   ('project_settlement_snapshots',false,false),
   ('project_settlement_snapshot_categories',false,false),
   ('project_settlement_events',false,false),
   ('project_closure_certificates',false,false),
   ('project_reopenings',false,false),
   ('project_operational_events',false,false),
   ('settlement_policy_versions',false,false),
   ('company_settlement_policy_bindings',false,false),
   ('document_custody_events',false,false),
   ('document_policy_exceptions',false,false),
   ('verified_evidence_objects',false,false),
   ('project_owner_reconciliations',false,false),
   ('project_owner_reconciliation_positions',false,false),
   ('project_owner_retention_items',false,false),
   ('financial_resolution_effects',false,false),
   ('supplier_credit_intents',false,false),
   ('supplier_credit_intent_applications',false,false),
   ('legacy_attribution_reviews',false,false),
   ('legacy_attribution_review_projects',false,false)
  ) AS expected(table_name,can_insert,can_update)
  WHERE NOT has_table_privilege(role_name,'ahdah.'||table_name,'SELECT')
     OR has_table_privilege(role_name,'ahdah.'||table_name,'INSERT')<>can_insert
     OR has_table_privilege(role_name,'ahdah.'||table_name,'UPDATE')<>can_update
     OR has_table_privilege(role_name,'ahdah.'||table_name,'DELETE')
     OR has_table_privilege(role_name,'ahdah.'||table_name,'TRUNCATE')
     OR has_table_privilege(role_name,'ahdah.'||table_name,'TRIGGER')
     OR has_table_privilege(role_name,'ahdah.'||table_name,'REFERENCES')
 ) THEN
  RAISE EXCEPTION 'Runtime settlement writer privilege mismatch';
 END IF;
 IF EXISTS(
  SELECT 1 FROM information_schema.role_table_grants
  WHERE grantee=role_name AND table_schema='ahdah' AND is_grantable='YES'
 ) THEN
  RAISE EXCEPTION 'Runtime must not receive grant options';
 END IF;
END $runtime_writer_verify$;
COMMIT;

SELECT table_name,privilege_type
FROM information_schema.role_table_grants
WHERE grantee=:'runtime_role' AND table_schema='ahdah'
 AND table_name IN (
  'project_financial_controls','project_custody_positions','custody_operations',
  'custody_movement_entries','custody_reservations','project_authorization_decisions')
ORDER BY table_name,privilege_type;
