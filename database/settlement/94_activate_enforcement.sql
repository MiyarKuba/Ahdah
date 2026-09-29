\set ON_ERROR_STOP on
\if :{?expected_database}
\else
\echo 'STOP: provide expected_database'
\quit 3
\endif
SELECT current_database() = :'expected_database' AS expected_database_ok \gset
\if :expected_database_ok
\else
\echo 'STOP: connected database does not match expected_database'
\quit 3
\endif
\echo 'ENFORCEMENT ACTIVATION — SEPARATE ALL-COMPANY CUTOVER APPROVAL REQUIRED'
\if :{?activation_approved}
\else
\quit 3
\endif
\if :activation_approved
\else
\echo 'STOP: activation_approved must be true'
\quit 3
\endif
\if :{?all_companies_reviewed}
\else
\quit 3
\endif
\if :all_companies_reviewed
\else
\echo 'STOP: all_companies_reviewed must be true'
\quit 3
\endif
-- This checkpoint is global because the reviewed design has no per-company
-- activation flag. Companies may be prepared in phases, but activation waits
-- until ALL legacy rows are covered and ALL 48 writer/action entries reviewed.
-- Must run during an independently controlled financial-write maintenance window.
BEGIN;
SET LOCAL lock_timeout='5s';
LOCK TABLE ahdah.projects,ahdah.advances,ahdah.user_advance_balances,
 ahdah.project_financial_controls,ahdah.project_custody_positions,ahdah.custody_reservations
 IN SHARE ROW EXCLUSIVE MODE;
-- Reinstall the reviewed check body transactionally so an existing prepared
-- schema receives the material-project/T01 same-transaction enforcement before
-- its previously disabled projects trigger is enabled.
CREATE OR REPLACE FUNCTION ahdah.settlement_control_check() RETURNS trigger
LANGUAGE plpgsql SET search_path = pg_catalog, ahdah AS $body$
#variable_conflict error
DECLARE cid uuid; pid uuid; j jsonb; p record; c record; st record; cert record; snap record;
BEGIN
 j:=CASE WHEN TG_OP='DELETE' THEN to_jsonb(OLD) ELSE to_jsonb(NEW) END;
 cid:=(j->>'company_id')::uuid; pid:=(j->>'project_id')::uuid;
 SELECT * INTO c FROM ahdah.project_financial_controls WHERE company_id=cid AND project_id=pid FOR UPDATE;
 SELECT * INTO p FROM ahdah.projects WHERE company_id=cid AND project_id=pid;
 IF NOT FOUND THEN RETURN NULL; END IF;
 IF c.project_id IS NULL THEN RAISE EXCEPTION 'Project financial control required' USING ERRCODE='23514'; END IF;
 IF TG_TABLE_NAME='projects' AND TG_OP='UPDATE' AND
   ((to_jsonb(OLD)->>'project_owner_id') IS DISTINCT FROM (to_jsonb(NEW)->>'project_owner_id') OR
    (to_jsonb(OLD)->>'contract_date') IS DISTINCT FROM (to_jsonb(NEW)->>'contract_date') OR
    (to_jsonb(OLD)->>'start_date') IS DISTINCT FROM (to_jsonb(NEW)->>'start_date') OR
    (to_jsonb(OLD)->>'expected_end_date') IS DISTINCT FROM (to_jsonb(NEW)->>'expected_end_date') OR
    (to_jsonb(OLD)->>'status') IS DISTINCT FROM (to_jsonb(NEW)->>'status')) AND NOT EXISTS(
     SELECT 1 FROM ahdah.project_financial_controls mc
     JOIN ahdah.projects mp ON mp.company_id=mc.company_id AND mp.project_id=mc.project_id
     WHERE mc.company_id=cid AND mc.project_id=pid AND mc.xmin=mp.xmin) THEN
  RAISE EXCEPTION 'Material project change requires matching financial revision protocol' USING ERRCODE='23514';
 END IF;
 IF c.financial_state IN ('Settled','FinanciallyClosed') THEN
  SELECT * INTO st FROM ahdah.project_settlements WHERE company_id=cid AND project_id=pid AND project_settlement_id=c.active_settlement_id;
  IF NOT FOUND OR st.status<>'Approved' OR st.captured_financial_revision<>c.financial_revision THEN
   RAISE EXCEPTION 'Current approved settlement required' USING ERRCODE='23514';
  END IF;
 END IF;
 IF c.financial_state='FinanciallyClosed' THEN
  SELECT * INTO cert FROM ahdah.project_closure_certificates WHERE company_id=cid AND project_id=pid AND project_closure_certificate_id=c.active_closure_id;
  IF NOT FOUND OR cert.project_settlement_id<>c.active_settlement_id OR cert.financial_revision<>c.financial_revision OR cert.policy_version_id<>st.policy_version_id OR cert.approved_snapshot_id<>st.snapshot_id OR cert.approved_snapshot_hash<>st.snapshot_hash
    OR (cert.closure_basis='Completed' AND p.status<>'FinanciallyClosed') OR (cert.closure_basis='Cancelled' AND p.status<>'Cancelled') THEN
   RAISE EXCEPTION 'Closure certificate/control/projection mismatch' USING ERRCODE='23514';
  END IF;
  SELECT * INTO snap FROM ahdah.project_settlement_snapshots WHERE company_id=cid AND project_settlement_snapshot_id=cert.closure_snapshot_id;
  IF NOT FOUND OR snap.financial_revision<>cert.financial_revision OR snap.policy_version_id<>cert.policy_version_id OR snap.snapshot_hash<>cert.closure_snapshot_hash OR snap.blocker_count<>0 OR snap.mandatory_gap_count<>0 THEN
   RAISE EXCEPTION 'Closure snapshot mismatch' USING ERRCODE='23514';
  END IF;
 ELSIF p.status='FinanciallyClosed' THEN
  RAISE EXCEPTION 'Operational closed status requires active certificate' USING ERRCODE='23514';
 END IF;
 RETURN NULL;
END $body$;
DO $activation$
BEGIN
 IF EXISTS(SELECT 1 FROM ahdah.projects p LEFT JOIN ahdah.project_financial_controls c ON c.company_id=p.company_id AND c.project_id=p.project_id WHERE c.project_id IS NULL) THEN RAISE EXCEPTION 'Missing legacy control'; END IF;
 IF EXISTS(SELECT 1 FROM ahdah.projects p JOIN ahdah.project_financial_controls c ON c.company_id=p.company_id AND c.project_id=p.project_id WHERE p.status='FinanciallyClosed' AND c.active_closure_id IS NULL) THEN RAISE EXCEPTION 'Uncertified legacy Closed status requires reviewed remediation'; END IF;
 IF EXISTS(SELECT 1 FROM ahdah.user_advance_balances b WHERE
  (SELECT count(*) FROM ahdah.project_custody_positions p WHERE p.company_id=b.company_id AND p.user_advance_balance_id=b.user_advance_balance_id AND p.project_id IS NULL)<>1 OR
  b.available_amount<>(SELECT coalesce(sum(p.available_amount),0) FROM ahdah.project_custody_positions p WHERE p.company_id=b.company_id AND p.user_advance_balance_id=b.user_advance_balance_id) OR
  b.reserved_amount<>(SELECT coalesce(sum(p.reserved_amount),0) FROM ahdah.project_custody_positions p WHERE p.company_id=b.company_id AND p.user_advance_balance_id=b.user_advance_balance_id)) THEN RAISE EXCEPTION 'Opening balance conservation fails'; END IF;
 IF EXISTS(SELECT 1 FROM ahdah.project_custody_positions p WHERE p.reserved_amount<>(SELECT coalesce(sum(r.amount),0) FROM ahdah.custody_reservations r WHERE r.company_id=p.company_id AND r.project_custody_position_id=p.project_custody_position_id AND r.status='Reserved')) THEN RAISE EXCEPTION 'Opening reservation conservation fails'; END IF;
END $activation$;
ALTER TABLE ahdah.projects ENABLE TRIGGER trg_settlement_project_control;
ALTER TABLE ahdah.user_advance_balances ENABLE TRIGGER trg_settlement_balance_conservation;
ALTER TABLE ahdah.advances ENABLE TRIGGER trg_settlement_advance_currency;
COMMIT;
-- This activates structural enforcement ONLY. It grants no runtime write rights
-- and does not enable closure. D16, typed evaluator, reopening, authorization,
-- retries, fixtures/races and separately reviewed command privileges still gate it.
