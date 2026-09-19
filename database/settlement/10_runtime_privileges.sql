\set ON_ERROR_STOP on
-- Included only by install_reviewed.sql. Any prerequisite failure below is a
-- SQL exception so ON_ERROR_STOP aborts the parent-owned transaction.
DO $runtime_safety$
DECLARE role_name text:=current_setting('ahdah.runtime_role');
BEGIN
 IF NOT EXISTS(SELECT 1 FROM pg_roles WHERE rolname=role_name AND NOT rolsuper AND NOT rolcreaterole AND NOT rolcreatedb AND NOT rolbypassrls) THEN
  RAISE EXCEPTION 'Runtime role absent or privileged';
 END IF;
 IF EXISTS(SELECT 1 FROM pg_class c JOIN pg_namespace n ON n.oid=c.relnamespace
  WHERE n.nspname='ahdah' AND c.relname IN ('project_financial_controls','project_custody_positions','custody_operations','custody_movement_entries','custody_reservations','project_settlements','project_settlement_snapshots','project_settlement_snapshot_categories','project_settlement_events','project_closure_certificates','project_reopenings','project_authorization_decisions','project_operational_events','settlement_policy_versions','company_settlement_policy_bindings','document_custody_events','document_policy_exceptions','verified_evidence_objects','project_owner_reconciliations','project_owner_reconciliation_positions','project_owner_retention_items','financial_resolution_effects','supplier_credit_intents','supplier_credit_intent_applications','legacy_attribution_reviews','legacy_attribution_review_projects') AND pg_has_role(role_name,c.relowner,'MEMBER')) THEN
  RAISE EXCEPTION 'Runtime owns or inherits settlement object ownership';
 END IF;
END $runtime_safety$;

GRANT USAGE ON SCHEMA ahdah TO :"runtime_role";
REVOKE CREATE ON SCHEMA ahdah FROM :"runtime_role";
REVOKE ALL ON TABLE ahdah.project_financial_controls,ahdah.project_custody_positions,ahdah.custody_operations,ahdah.custody_movement_entries,ahdah.custody_reservations,ahdah.project_settlements,ahdah.project_settlement_snapshots,ahdah.project_settlement_snapshot_categories,ahdah.project_settlement_events,ahdah.project_closure_certificates,ahdah.project_reopenings,ahdah.project_authorization_decisions,ahdah.project_operational_events,ahdah.settlement_policy_versions,ahdah.company_settlement_policy_bindings,ahdah.document_custody_events,ahdah.document_policy_exceptions,ahdah.verified_evidence_objects,ahdah.project_owner_reconciliations,ahdah.project_owner_reconciliation_positions,ahdah.project_owner_retention_items,ahdah.financial_resolution_effects,ahdah.supplier_credit_intents,ahdah.supplier_credit_intent_applications,ahdah.legacy_attribution_reviews,ahdah.legacy_attribution_review_projects FROM PUBLIC, :"runtime_role";
GRANT SELECT ON TABLE ahdah.project_financial_controls,ahdah.project_custody_positions,ahdah.custody_operations,ahdah.custody_movement_entries,ahdah.custody_reservations,ahdah.project_settlements,ahdah.project_settlement_snapshots,ahdah.project_settlement_snapshot_categories,ahdah.project_settlement_events,ahdah.project_closure_certificates,ahdah.project_reopenings,ahdah.project_authorization_decisions,ahdah.project_operational_events,ahdah.settlement_policy_versions,ahdah.company_settlement_policy_bindings,ahdah.document_custody_events,ahdah.document_policy_exceptions,ahdah.verified_evidence_objects,ahdah.project_owner_reconciliations,ahdah.project_owner_reconciliation_positions,ahdah.project_owner_retention_items,ahdah.financial_resolution_effects,ahdah.supplier_credit_intents,ahdah.supplier_credit_intent_applications,ahdah.legacy_attribution_reviews,ahdah.legacy_attribution_review_projects TO :"runtime_role";
REVOKE ALL ON FUNCTION ahdah.settlement_advance_currency(),ahdah.settlement_category_insert(),ahdah.settlement_check_custody(),ahdah.settlement_child_freeze(),ahdah.settlement_control_check(),ahdah.settlement_fixed_keys(),ahdah.settlement_freeze_status(),ahdah.settlement_owner_approval(),ahdah.settlement_packet_check(),ahdah.settlement_approval_packet(),ahdah.settlement_custody_authority() FROM PUBLIC, :"runtime_role";

DO $runtime_verify$
DECLARE role_name text:=current_setting('ahdah.runtime_role');
BEGIN
 IF NOT has_schema_privilege(role_name,'ahdah','USAGE') OR has_schema_privilege(role_name,'ahdah','CREATE') THEN
  RAISE EXCEPTION 'Runtime schema privilege mismatch';
 END IF;
 IF EXISTS(SELECT 1 FROM pg_class c JOIN pg_namespace n ON n.oid=c.relnamespace
  WHERE n.nspname='ahdah' AND c.relname IN ('project_financial_controls','project_custody_positions','custody_operations','custody_movement_entries','custody_reservations','project_settlements','project_settlement_snapshots','project_settlement_snapshot_categories','project_settlement_events','project_closure_certificates','project_reopenings','project_authorization_decisions','project_operational_events','settlement_policy_versions','company_settlement_policy_bindings','document_custody_events','document_policy_exceptions','verified_evidence_objects','project_owner_reconciliations','project_owner_reconciliation_positions','project_owner_retention_items','financial_resolution_effects','supplier_credit_intents','supplier_credit_intent_applications','legacy_attribution_reviews','legacy_attribution_review_projects') AND
  (NOT has_table_privilege(role_name,c.oid,'SELECT') OR has_table_privilege(role_name,c.oid,'INSERT,UPDATE,DELETE,TRUNCATE,TRIGGER') OR pg_has_role(role_name,c.relowner,'MEMBER'))) THEN
  RAISE EXCEPTION 'Runtime settlement table privilege mismatch';
 END IF;
END $runtime_verify$;
-- Baseline-table DML grants remain unchanged. No settlement write authority is granted.
