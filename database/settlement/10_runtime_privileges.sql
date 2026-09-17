\set ON_ERROR_STOP on
-- Runtime remains READ ONLY on the new authority until all writers are upgraded.
\if :{?runtime_role}
\else
\echo 'STOP: provide reviewed runtime_role'
\quit 3
\endif
SELECT EXISTS(SELECT 1 FROM pg_roles WHERE rolname=:'runtime_role' AND NOT rolsuper AND NOT rolcreaterole AND NOT rolcreatedb AND NOT rolbypassrls) AS safe_role \gset
\if :safe_role
\else
\echo 'STOP: runtime role absent or privileged'
\quit 3
\endif
SELECT NOT pg_has_role(:'runtime_role',relowner,'MEMBER') AS not_owner FROM pg_class WHERE oid='ahdah.project_financial_controls'::regclass \gset
\if :not_owner
\else
\echo 'STOP: runtime owns/inherits ownership'
\quit 3
\endif
REVOKE ALL ON TABLE ahdah.project_financial_controls FROM PUBLIC, :"runtime_role";
GRANT SELECT ON TABLE ahdah.project_financial_controls TO :"runtime_role";
SELECT NOT pg_has_role(:'runtime_role',relowner,'MEMBER') AS not_owner FROM pg_class WHERE oid='ahdah.project_custody_positions'::regclass \gset
\if :not_owner
\else
\echo 'STOP: runtime owns/inherits ownership'
\quit 3
\endif
REVOKE ALL ON TABLE ahdah.project_custody_positions FROM PUBLIC, :"runtime_role";
GRANT SELECT ON TABLE ahdah.project_custody_positions TO :"runtime_role";
SELECT NOT pg_has_role(:'runtime_role',relowner,'MEMBER') AS not_owner FROM pg_class WHERE oid='ahdah.custody_operations'::regclass \gset
\if :not_owner
\else
\echo 'STOP: runtime owns/inherits ownership'
\quit 3
\endif
REVOKE ALL ON TABLE ahdah.custody_operations FROM PUBLIC, :"runtime_role";
GRANT SELECT ON TABLE ahdah.custody_operations TO :"runtime_role";
SELECT NOT pg_has_role(:'runtime_role',relowner,'MEMBER') AS not_owner FROM pg_class WHERE oid='ahdah.custody_movement_entries'::regclass \gset
\if :not_owner
\else
\echo 'STOP: runtime owns/inherits ownership'
\quit 3
\endif
REVOKE ALL ON TABLE ahdah.custody_movement_entries FROM PUBLIC, :"runtime_role";
GRANT SELECT ON TABLE ahdah.custody_movement_entries TO :"runtime_role";
SELECT NOT pg_has_role(:'runtime_role',relowner,'MEMBER') AS not_owner FROM pg_class WHERE oid='ahdah.custody_reservations'::regclass \gset
\if :not_owner
\else
\echo 'STOP: runtime owns/inherits ownership'
\quit 3
\endif
REVOKE ALL ON TABLE ahdah.custody_reservations FROM PUBLIC, :"runtime_role";
GRANT SELECT ON TABLE ahdah.custody_reservations TO :"runtime_role";
SELECT NOT pg_has_role(:'runtime_role',relowner,'MEMBER') AS not_owner FROM pg_class WHERE oid='ahdah.project_settlements'::regclass \gset
\if :not_owner
\else
\echo 'STOP: runtime owns/inherits ownership'
\quit 3
\endif
REVOKE ALL ON TABLE ahdah.project_settlements FROM PUBLIC, :"runtime_role";
GRANT SELECT ON TABLE ahdah.project_settlements TO :"runtime_role";
SELECT NOT pg_has_role(:'runtime_role',relowner,'MEMBER') AS not_owner FROM pg_class WHERE oid='ahdah.project_settlement_snapshots'::regclass \gset
\if :not_owner
\else
\echo 'STOP: runtime owns/inherits ownership'
\quit 3
\endif
REVOKE ALL ON TABLE ahdah.project_settlement_snapshots FROM PUBLIC, :"runtime_role";
GRANT SELECT ON TABLE ahdah.project_settlement_snapshots TO :"runtime_role";
SELECT NOT pg_has_role(:'runtime_role',relowner,'MEMBER') AS not_owner FROM pg_class WHERE oid='ahdah.project_settlement_snapshot_categories'::regclass \gset
\if :not_owner
\else
\echo 'STOP: runtime owns/inherits ownership'
\quit 3
\endif
REVOKE ALL ON TABLE ahdah.project_settlement_snapshot_categories FROM PUBLIC, :"runtime_role";
GRANT SELECT ON TABLE ahdah.project_settlement_snapshot_categories TO :"runtime_role";
SELECT NOT pg_has_role(:'runtime_role',relowner,'MEMBER') AS not_owner FROM pg_class WHERE oid='ahdah.project_settlement_events'::regclass \gset
\if :not_owner
\else
\echo 'STOP: runtime owns/inherits ownership'
\quit 3
\endif
REVOKE ALL ON TABLE ahdah.project_settlement_events FROM PUBLIC, :"runtime_role";
GRANT SELECT ON TABLE ahdah.project_settlement_events TO :"runtime_role";
SELECT NOT pg_has_role(:'runtime_role',relowner,'MEMBER') AS not_owner FROM pg_class WHERE oid='ahdah.project_closure_certificates'::regclass \gset
\if :not_owner
\else
\echo 'STOP: runtime owns/inherits ownership'
\quit 3
\endif
REVOKE ALL ON TABLE ahdah.project_closure_certificates FROM PUBLIC, :"runtime_role";
GRANT SELECT ON TABLE ahdah.project_closure_certificates TO :"runtime_role";
SELECT NOT pg_has_role(:'runtime_role',relowner,'MEMBER') AS not_owner FROM pg_class WHERE oid='ahdah.project_reopenings'::regclass \gset
\if :not_owner
\else
\echo 'STOP: runtime owns/inherits ownership'
\quit 3
\endif
REVOKE ALL ON TABLE ahdah.project_reopenings FROM PUBLIC, :"runtime_role";
GRANT SELECT ON TABLE ahdah.project_reopenings TO :"runtime_role";
SELECT NOT pg_has_role(:'runtime_role',relowner,'MEMBER') AS not_owner FROM pg_class WHERE oid='ahdah.project_authorization_decisions'::regclass \gset
\if :not_owner
\else
\echo 'STOP: runtime owns/inherits ownership'
\quit 3
\endif
REVOKE ALL ON TABLE ahdah.project_authorization_decisions FROM PUBLIC, :"runtime_role";
GRANT SELECT ON TABLE ahdah.project_authorization_decisions TO :"runtime_role";
SELECT NOT pg_has_role(:'runtime_role',relowner,'MEMBER') AS not_owner FROM pg_class WHERE oid='ahdah.project_operational_events'::regclass \gset
\if :not_owner
\else
\echo 'STOP: runtime owns/inherits ownership'
\quit 3
\endif
REVOKE ALL ON TABLE ahdah.project_operational_events FROM PUBLIC, :"runtime_role";
GRANT SELECT ON TABLE ahdah.project_operational_events TO :"runtime_role";
SELECT NOT pg_has_role(:'runtime_role',relowner,'MEMBER') AS not_owner FROM pg_class WHERE oid='ahdah.settlement_policy_versions'::regclass \gset
\if :not_owner
\else
\echo 'STOP: runtime owns/inherits ownership'
\quit 3
\endif
REVOKE ALL ON TABLE ahdah.settlement_policy_versions FROM PUBLIC, :"runtime_role";
GRANT SELECT ON TABLE ahdah.settlement_policy_versions TO :"runtime_role";
SELECT NOT pg_has_role(:'runtime_role',relowner,'MEMBER') AS not_owner FROM pg_class WHERE oid='ahdah.company_settlement_policy_bindings'::regclass \gset
\if :not_owner
\else
\echo 'STOP: runtime owns/inherits ownership'
\quit 3
\endif
REVOKE ALL ON TABLE ahdah.company_settlement_policy_bindings FROM PUBLIC, :"runtime_role";
GRANT SELECT ON TABLE ahdah.company_settlement_policy_bindings TO :"runtime_role";
SELECT NOT pg_has_role(:'runtime_role',relowner,'MEMBER') AS not_owner FROM pg_class WHERE oid='ahdah.document_custody_events'::regclass \gset
\if :not_owner
\else
\echo 'STOP: runtime owns/inherits ownership'
\quit 3
\endif
REVOKE ALL ON TABLE ahdah.document_custody_events FROM PUBLIC, :"runtime_role";
GRANT SELECT ON TABLE ahdah.document_custody_events TO :"runtime_role";
SELECT NOT pg_has_role(:'runtime_role',relowner,'MEMBER') AS not_owner FROM pg_class WHERE oid='ahdah.document_policy_exceptions'::regclass \gset
\if :not_owner
\else
\echo 'STOP: runtime owns/inherits ownership'
\quit 3
\endif
REVOKE ALL ON TABLE ahdah.document_policy_exceptions FROM PUBLIC, :"runtime_role";
GRANT SELECT ON TABLE ahdah.document_policy_exceptions TO :"runtime_role";
SELECT NOT pg_has_role(:'runtime_role',relowner,'MEMBER') AS not_owner FROM pg_class WHERE oid='ahdah.verified_evidence_objects'::regclass \gset
\if :not_owner
\else
\echo 'STOP: runtime owns/inherits ownership'
\quit 3
\endif
REVOKE ALL ON TABLE ahdah.verified_evidence_objects FROM PUBLIC, :"runtime_role";
GRANT SELECT ON TABLE ahdah.verified_evidence_objects TO :"runtime_role";
SELECT NOT pg_has_role(:'runtime_role',relowner,'MEMBER') AS not_owner FROM pg_class WHERE oid='ahdah.project_owner_reconciliations'::regclass \gset
\if :not_owner
\else
\echo 'STOP: runtime owns/inherits ownership'
\quit 3
\endif
REVOKE ALL ON TABLE ahdah.project_owner_reconciliations FROM PUBLIC, :"runtime_role";
GRANT SELECT ON TABLE ahdah.project_owner_reconciliations TO :"runtime_role";
SELECT NOT pg_has_role(:'runtime_role',relowner,'MEMBER') AS not_owner FROM pg_class WHERE oid='ahdah.project_owner_reconciliation_positions'::regclass \gset
\if :not_owner
\else
\echo 'STOP: runtime owns/inherits ownership'
\quit 3
\endif
REVOKE ALL ON TABLE ahdah.project_owner_reconciliation_positions FROM PUBLIC, :"runtime_role";
GRANT SELECT ON TABLE ahdah.project_owner_reconciliation_positions TO :"runtime_role";
SELECT NOT pg_has_role(:'runtime_role',relowner,'MEMBER') AS not_owner FROM pg_class WHERE oid='ahdah.project_owner_retention_items'::regclass \gset
\if :not_owner
\else
\echo 'STOP: runtime owns/inherits ownership'
\quit 3
\endif
REVOKE ALL ON TABLE ahdah.project_owner_retention_items FROM PUBLIC, :"runtime_role";
GRANT SELECT ON TABLE ahdah.project_owner_retention_items TO :"runtime_role";
SELECT NOT pg_has_role(:'runtime_role',relowner,'MEMBER') AS not_owner FROM pg_class WHERE oid='ahdah.financial_resolution_effects'::regclass \gset
\if :not_owner
\else
\echo 'STOP: runtime owns/inherits ownership'
\quit 3
\endif
REVOKE ALL ON TABLE ahdah.financial_resolution_effects FROM PUBLIC, :"runtime_role";
GRANT SELECT ON TABLE ahdah.financial_resolution_effects TO :"runtime_role";
SELECT NOT pg_has_role(:'runtime_role',relowner,'MEMBER') AS not_owner FROM pg_class WHERE oid='ahdah.supplier_credit_intents'::regclass \gset
\if :not_owner
\else
\echo 'STOP: runtime owns/inherits ownership'
\quit 3
\endif
REVOKE ALL ON TABLE ahdah.supplier_credit_intents FROM PUBLIC, :"runtime_role";
GRANT SELECT ON TABLE ahdah.supplier_credit_intents TO :"runtime_role";
SELECT NOT pg_has_role(:'runtime_role',relowner,'MEMBER') AS not_owner FROM pg_class WHERE oid='ahdah.supplier_credit_intent_applications'::regclass \gset
\if :not_owner
\else
\echo 'STOP: runtime owns/inherits ownership'
\quit 3
\endif
REVOKE ALL ON TABLE ahdah.supplier_credit_intent_applications FROM PUBLIC, :"runtime_role";
GRANT SELECT ON TABLE ahdah.supplier_credit_intent_applications TO :"runtime_role";
SELECT NOT pg_has_role(:'runtime_role',relowner,'MEMBER') AS not_owner FROM pg_class WHERE oid='ahdah.legacy_attribution_reviews'::regclass \gset
\if :not_owner
\else
\echo 'STOP: runtime owns/inherits ownership'
\quit 3
\endif
REVOKE ALL ON TABLE ahdah.legacy_attribution_reviews FROM PUBLIC, :"runtime_role";
GRANT SELECT ON TABLE ahdah.legacy_attribution_reviews TO :"runtime_role";
SELECT NOT pg_has_role(:'runtime_role',relowner,'MEMBER') AS not_owner FROM pg_class WHERE oid='ahdah.legacy_attribution_review_projects'::regclass \gset
\if :not_owner
\else
\echo 'STOP: runtime owns/inherits ownership'
\quit 3
\endif
REVOKE ALL ON TABLE ahdah.legacy_attribution_review_projects FROM PUBLIC, :"runtime_role";
GRANT SELECT ON TABLE ahdah.legacy_attribution_review_projects TO :"runtime_role";
SELECT has_schema_privilege(:'runtime_role','ahdah','USAGE') AS has_usage \gset
\if :has_usage
\else
\echo 'STOP: existing schema USAGE grant must be reviewed separately'
\quit 3
\endif
REVOKE ALL ON FUNCTION ahdah.settlement_advance_currency() FROM PUBLIC, :"runtime_role";
REVOKE ALL ON FUNCTION ahdah.settlement_category_insert() FROM PUBLIC, :"runtime_role";
REVOKE ALL ON FUNCTION ahdah.settlement_check_custody() FROM PUBLIC, :"runtime_role";
REVOKE ALL ON FUNCTION ahdah.settlement_child_freeze() FROM PUBLIC, :"runtime_role";
REVOKE ALL ON FUNCTION ahdah.settlement_control_check() FROM PUBLIC, :"runtime_role";
REVOKE ALL ON FUNCTION ahdah.settlement_fixed_keys() FROM PUBLIC, :"runtime_role";
REVOKE ALL ON FUNCTION ahdah.settlement_freeze_status() FROM PUBLIC, :"runtime_role";
REVOKE ALL ON FUNCTION ahdah.settlement_owner_approval() FROM PUBLIC, :"runtime_role";
REVOKE ALL ON FUNCTION ahdah.settlement_packet_check() FROM PUBLIC, :"runtime_role";
-- Existing DML grants on baseline tables are not modified here. No new authority
-- write privilege is granted. A separately reviewed activation privilege patch
-- must enroll every writer, reject inherited/owner bypass and retain no TRUNCATE.

REVOKE ALL ON FUNCTION ahdah.settlement_approval_packet() FROM PUBLIC, :"runtime_role";

REVOKE ALL ON FUNCTION ahdah.settlement_custody_authority() FROM PUBLIC, :"runtime_role";
