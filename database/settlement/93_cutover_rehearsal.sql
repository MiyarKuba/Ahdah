\set ON_ERROR_STOP on
\echo 'LOCAL EMPTY REHEARSAL DATABASE ONLY — NEVER DEPLOYED AHDah — ROLLBACK AT END'
\if :{?fixture_only_approved}
\else
\quit 3
\endif
\if :fixture_only_approved
\else
\echo 'STOP: fixture_only_approved must be true'
\quit 3
\endif
-- Requires a separately prepared empty copy of baseline + reviewed schema.
-- This file creates no database/schema and is NOT called by install_reviewed.sql.
-- Structural SQL probes below do not substitute for the command/race scenarios
-- F01-F24 specified in REHEARSAL.md; no application commands exist yet.
BEGIN;
DO $guard$
BEGIN
 IF current_database() NOT LIKE 'ahdah_settlement_rehearsal_%' THEN RAISE EXCEPTION 'Dedicated rehearsal database required'; END IF;
 IF EXISTS(SELECT 1 FROM ahdah.companies) THEN RAISE EXCEPTION 'Rehearsal target must have no tenant/customer data'; END IF;
END $guard$;
-- Use deterministic, explicitly synthetic identities; never use customer actors.
INSERT INTO ahdah.companies(company_id,company_name,company_code)
 VALUES ('10000000-0000-0000-0000-000000000001','Synthetic fixture only','REHEARSAL');
INSERT INTO ahdah.app_users(user_id,company_id,full_name,phone_number,password_hash,role)
 VALUES ('10000000-0000-0000-0000-000000000002','10000000-0000-0000-0000-000000000001','Fixture Manager','+12025550101','NOT_A_LOGIN_CREDENTIAL','Manager');
INSERT INTO ahdah.project_owners(project_owner_id,company_id,owner_name,phone_number,created_by_user_id)
 VALUES ('10000000-0000-0000-0000-000000000003','10000000-0000-0000-0000-000000000001','Fixture Owner','+12025550102','10000000-0000-0000-0000-000000000002');
INSERT INTO ahdah.projects(project_id,company_id,project_owner_id,project_name,site_address,contract_value,contract_date,start_date,created_by_user_id,contract_currency_code)
 SELECT id,'10000000-0000-0000-0000-000000000001','10000000-0000-0000-0000-000000000003',label,'Synthetic site',20000,CURRENT_DATE,CURRENT_DATE,'10000000-0000-0000-0000-000000000002','LYD'
 FROM (VALUES ('10000000-0000-0000-0000-000000000004'::uuid,'A'),('10000000-0000-0000-0000-000000000005'::uuid,'B')) v(id,label);
INSERT INTO ahdah.project_financial_controls(company_id,project_id,financial_state,last_material_change_at,attribution_status)
 SELECT company_id,project_id,'Open',CURRENT_TIMESTAMP,'Reviewed' FROM ahdah.projects WHERE company_id='10000000-0000-0000-0000-000000000001';
-- Structural final F01 balance only, not a substitute for delivery/posting authority.
-- This seeds the arithmetic AFTER unassigned Opening and two approved Designate
-- commands. No project row below is an Opening. The live/legacy source and
-- action-specific command cases are specified in REHEARSAL.md and must be bound
-- to upgraded command tests before claiming full F01/F09/F10 acceptance.
INSERT INTO ahdah.advances(advance_id,company_id,advance_number,deputy_user_id,advance_amount,issue_date,purpose,created_by_user_id,currency_code)
 VALUES ('10000000-0000-0000-0000-000000000006','10000000-0000-0000-0000-000000000001','FIX-X','10000000-0000-0000-0000-000000000002',20000,CURRENT_DATE,'Synthetic structural custody','10000000-0000-0000-0000-000000000002','LYD');
INSERT INTO ahdah.user_advance_balances(user_advance_balance_id,company_id,advance_id,user_id,created_by_user_id,total_received_amount,available_amount)
 VALUES ('10000000-0000-0000-0000-000000000007','10000000-0000-0000-0000-000000000001','10000000-0000-0000-0000-000000000006','10000000-0000-0000-0000-000000000002','10000000-0000-0000-0000-000000000002',20000,20000);
INSERT INTO ahdah.project_custody_positions(company_id,user_advance_balance_id,project_id,available_amount)
 VALUES ('10000000-0000-0000-0000-000000000001','10000000-0000-0000-0000-000000000007','10000000-0000-0000-0000-000000000004',10000),
 ('10000000-0000-0000-0000-000000000001','10000000-0000-0000-0000-000000000007','10000000-0000-0000-0000-000000000005',5000),
 ('10000000-0000-0000-0000-000000000001','10000000-0000-0000-0000-000000000007',NULL,5000);
SET CONSTRAINTS ALL IMMEDIATE;
SET CONSTRAINTS ALL DEFERRED;
-- F01 structural baseline and F24 rejection probes. Each expected failure is
-- contained in its own subtransaction; absence of failure raises an exception.
DO $probes$
DECLARE rejected boolean;
BEGIN
 rejected:=false;
 BEGIN
  INSERT INTO ahdah.project_custody_positions(company_id,user_advance_balance_id,available_amount)
   VALUES ('10000000-0000-0000-0000-000000000001','10000000-0000-0000-0000-000000000007',0);
 EXCEPTION WHEN unique_violation THEN rejected:=true;
 END;
 IF NOT rejected THEN RAISE EXCEPTION 'F24 duplicate unassigned accepted'; END IF;
 rejected:=false;
 BEGIN
  UPDATE ahdah.project_custody_positions SET available_amount=-1 WHERE company_id='10000000-0000-0000-0000-000000000001' AND project_id='10000000-0000-0000-0000-000000000004';
 EXCEPTION WHEN check_violation THEN rejected:=true;
 END;
 IF NOT rejected THEN RAISE EXCEPTION 'F24 negative custody accepted'; END IF;
 rejected:=false;
 BEGIN
  UPDATE ahdah.project_custody_positions SET available_amount=9999 WHERE company_id='10000000-0000-0000-0000-000000000001' AND project_id='10000000-0000-0000-0000-000000000004';
  SET CONSTRAINTS ALL IMMEDIATE;
 EXCEPTION WHEN check_violation THEN rejected:=true;
 END;
 IF NOT rejected THEN RAISE EXCEPTION 'F24 conservation drift accepted'; END IF;
END $probes$;
-- F02 posted-consumption arithmetic probe only; future command test must also
-- prove expense reservation, allocation, movement, audit and authorization.
UPDATE ahdah.user_advance_balances SET total_expensed_amount=6000,available_amount=14000,version_number=version_number+1
 WHERE company_id='10000000-0000-0000-0000-000000000001' AND user_advance_balance_id='10000000-0000-0000-0000-000000000007';
UPDATE ahdah.project_custody_positions SET available_amount=4000,version_number=version_number+1
 WHERE company_id='10000000-0000-0000-0000-000000000001' AND project_id='10000000-0000-0000-0000-000000000004';
SET CONSTRAINTS ALL IMMEDIATE;
SET CONSTRAINTS ALL DEFERRED;
SAVEPOINT before_release;
-- F04 structural pair: available balance remains 14000.
UPDATE ahdah.project_custody_positions SET available_amount=0,version_number=version_number+1 WHERE company_id='10000000-0000-0000-0000-000000000001' AND project_id='10000000-0000-0000-0000-000000000004';
UPDATE ahdah.project_custody_positions SET available_amount=9000,version_number=version_number+1 WHERE company_id='10000000-0000-0000-0000-000000000001' AND project_id IS NULL;
SET CONSTRAINTS ALL IMMEDIATE;
ROLLBACK TO before_release;
SET CONSTRAINTS ALL DEFERRED;
-- F05 structural pair, request order never determines gate order.
SELECT project_id FROM ahdah.project_financial_controls WHERE company_id='10000000-0000-0000-0000-000000000001' ORDER BY project_id FOR UPDATE;
UPDATE ahdah.project_custody_positions SET available_amount=CASE WHEN project_id='10000000-0000-0000-0000-000000000004' THEN 3000 ELSE 6000 END,version_number=version_number+1
 WHERE company_id='10000000-0000-0000-0000-000000000001' AND project_id IS NOT NULL;
SET CONSTRAINTS ALL IMMEDIATE;
-- Packet-construction probe (F14/F24): preallocated IDs, category before header,
-- deferred references, same transaction, late insertion and mutation refused.
SET CONSTRAINTS ALL DEFERRED;
INSERT INTO ahdah.settlement_policy_versions(settlement_policy_version_id,company_id,policy_code,major_version,configuration_version,policy_text,canonical_rules,rules,content_hash,approved_by_user_id,approved_at,effective_from)
 VALUES ('10000000-0000-0000-0000-000000000008','10000000-0000-0000-0000-000000000001','FixtureOnly',1,1,'Synthetic structural policy; never certification authority',convert_to('{}','UTF8'),'{}',sha256(convert_to('{}','UTF8')),'10000000-0000-0000-0000-000000000002',CURRENT_TIMESTAMP,CURRENT_TIMESTAMP);
INSERT INTO ahdah.project_settlements(project_settlement_id,company_id,project_id,cycle_number,status,operational_basis,captured_project_version,captured_financial_revision,policy_version_id,evaluator_version,snapshot_schema_version,prepared_by_user_id)
 VALUES ('10000000-0000-0000-0000-000000000009','10000000-0000-0000-0000-000000000001','10000000-0000-0000-0000-000000000004',1,'Draft','Completed',1,1,'10000000-0000-0000-0000-000000000008','fixture',1,'10000000-0000-0000-0000-000000000002');
INSERT INTO ahdah.project_settlement_snapshot_categories(company_id,project_id,snapshot_id,category_code,evaluation_status,currency_code,amount_meaning,exact_amount,blocker_count,gap_count,details)
 VALUES ('10000000-0000-0000-0000-000000000001','10000000-0000-0000-0000-000000000004','10000000-0000-0000-0000-000000000010','Custody','Evaluated','LYD','CurrentAvailable',3000,0,0,'[]');
INSERT INTO ahdah.project_settlement_snapshots(project_settlement_snapshot_id,company_id,project_id,project_settlement_id,snapshot_number,purpose,project_version,financial_revision,policy_version_id,evaluator_version,schema_version,evaluated_at,canonical_payload,payload,hash_algorithm,snapshot_hash,evaluated_by_user_id,blocker_count,mandatory_gap_count)
 VALUES ('10000000-0000-0000-0000-000000000010','10000000-0000-0000-0000-000000000001','10000000-0000-0000-0000-000000000004','10000000-0000-0000-0000-000000000009',1,'Evaluation',1,1,'10000000-0000-0000-0000-000000000008','fixture',1,CURRENT_TIMESTAMP,convert_to('{}','UTF8'),'{}','SHA256',sha256(convert_to('{}','UTF8')),'10000000-0000-0000-0000-000000000002',0,0);
SET CONSTRAINTS ALL IMMEDIATE;
DO $packet_probe$
DECLARE rejected boolean:=false;
BEGIN
 BEGIN
  INSERT INTO ahdah.project_settlement_snapshot_categories(company_id,project_id,snapshot_id,category_code,evaluation_status,currency_code,amount_meaning,exact_amount,blocker_count,gap_count,details)
   VALUES ('10000000-0000-0000-0000-000000000001','10000000-0000-0000-0000-000000000004','10000000-0000-0000-0000-000000000010','LateCategory','Evaluated','LYD','CurrentAvailable',0,0,0,'[]');
 EXCEPTION WHEN check_violation THEN rejected:=true;
 END;
 IF NOT rejected THEN RAISE EXCEPTION 'Late category insertion accepted'; END IF;
 rejected:=false;
 BEGIN
  UPDATE ahdah.project_settlement_snapshots SET payload='{"changed":true}' WHERE company_id='10000000-0000-0000-0000-000000000001';
 EXCEPTION WHEN raise_exception THEN rejected:=true;
 END;
 IF NOT rejected THEN RAISE EXCEPTION 'Immutable snapshot changed'; END IF;
END $packet_probe$;
ROLLBACK;
\echo 'Structural probes finished and rolled back. This is NOT a passing F01-F24 application/race suite.'
