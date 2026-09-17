\set ON_ERROR_STOP on
-- PREPARATION ARTIFACT ONLY. NOT EXECUTED. FKs and indexes; trigger definitions follow in this module.
\if :{?settlement_ddl_approved}
\else
\echo 'STOP: future execution requires separate approval and settlement_ddl_approved variable.'
\quit 3
\endif
\if :settlement_ddl_approved
\else
\echo 'STOP: settlement_ddl_approved must be true'
\quit 3
\endif
ALTER TABLE ahdah.project_financial_controls ADD CONSTRAINT fk_t01_company_id FOREIGN KEY (company_id) REFERENCES ahdah.companies (company_id) ON UPDATE NO ACTION ON DELETE NO ACTION;
ALTER TABLE ahdah.project_financial_controls ADD CONSTRAINT fk_t01_project_id FOREIGN KEY (company_id, project_id) REFERENCES ahdah.projects (company_id, project_id) ON UPDATE NO ACTION ON DELETE NO ACTION;
ALTER TABLE ahdah.project_financial_controls ADD CONSTRAINT fk_t01_active_settlement_id FOREIGN KEY (company_id, project_id, active_settlement_id) REFERENCES ahdah.project_settlements (company_id, project_id, project_settlement_id) ON UPDATE NO ACTION ON DELETE NO ACTION DEFERRABLE INITIALLY DEFERRED;
ALTER TABLE ahdah.project_financial_controls ADD CONSTRAINT fk_t01_active_closure_id FOREIGN KEY (company_id, project_id, active_closure_id) REFERENCES ahdah.project_closure_certificates (company_id, project_id, project_closure_certificate_id) ON UPDATE NO ACTION ON DELETE NO ACTION DEFERRABLE INITIALLY DEFERRED;
ALTER TABLE ahdah.project_custody_positions ADD CONSTRAINT fk_t02_company_id FOREIGN KEY (company_id) REFERENCES ahdah.companies (company_id) ON UPDATE NO ACTION ON DELETE NO ACTION;
ALTER TABLE ahdah.project_custody_positions ADD CONSTRAINT fk_t02_user_advance_balance_id FOREIGN KEY (company_id, user_advance_balance_id) REFERENCES ahdah.user_advance_balances (company_id, user_advance_balance_id) ON UPDATE NO ACTION ON DELETE NO ACTION;
ALTER TABLE ahdah.project_custody_positions ADD CONSTRAINT fk_t02_project_id FOREIGN KEY (company_id, project_id) REFERENCES ahdah.projects (company_id, project_id) ON UPDATE NO ACTION ON DELETE NO ACTION;
ALTER TABLE ahdah.custody_operations ADD CONSTRAINT fk_t03_company_id FOREIGN KEY (company_id) REFERENCES ahdah.companies (company_id) ON UPDATE NO ACTION ON DELETE NO ACTION;
ALTER TABLE ahdah.custody_operations ADD CONSTRAINT fk_t03_requested_by_user_id FOREIGN KEY (company_id, requested_by_user_id) REFERENCES ahdah.app_users (company_id, user_id) ON UPDATE NO ACTION ON DELETE NO ACTION;
ALTER TABLE ahdah.custody_operations ADD CONSTRAINT fk_t03_approved_by_user_id FOREIGN KEY (company_id, approved_by_user_id) REFERENCES ahdah.app_users (company_id, user_id) ON UPDATE NO ACTION ON DELETE NO ACTION;
ALTER TABLE ahdah.custody_operations ADD CONSTRAINT fk_t03_authorization_decision_id FOREIGN KEY (company_id, authorization_decision_id) REFERENCES ahdah.project_authorization_decisions (company_id, project_authorization_decision_id) ON UPDATE NO ACTION ON DELETE NO ACTION DEFERRABLE INITIALLY DEFERRED;
ALTER TABLE ahdah.custody_operations ADD CONSTRAINT fk_t03_idempotency_record_id FOREIGN KEY (company_id, idempotency_record_id) REFERENCES ahdah.idempotency_records (company_id, idempotency_record_id) ON UPDATE NO ACTION ON DELETE NO ACTION;
ALTER TABLE ahdah.custody_operations ADD CONSTRAINT fk_t03_source_operation_id FOREIGN KEY (company_id, source_operation_id) REFERENCES ahdah.custody_operations (company_id, custody_operation_id) ON UPDATE NO ACTION ON DELETE NO ACTION DEFERRABLE INITIALLY DEFERRED;
ALTER TABLE ahdah.custody_operations ADD CONSTRAINT fk_t03_money_transfer_id FOREIGN KEY (company_id, money_transfer_id) REFERENCES ahdah.money_transfers (company_id, money_transfer_id) ON UPDATE NO ACTION ON DELETE NO ACTION;
ALTER TABLE ahdah.custody_operations ADD CONSTRAINT fk_t03_expense_id FOREIGN KEY (company_id, expense_id) REFERENCES ahdah.expenses (company_id, expense_id) ON UPDATE NO ACTION ON DELETE NO ACTION;
ALTER TABLE ahdah.custody_operations ADD CONSTRAINT fk_t03_expense_return_id FOREIGN KEY (company_id, expense_return_id) REFERENCES ahdah.expense_returns (company_id, expense_return_id) ON UPDATE NO ACTION ON DELETE NO ACTION;
ALTER TABLE ahdah.custody_operations ADD CONSTRAINT fk_t03_legacy_attribution_review_id FOREIGN KEY (company_id, legacy_attribution_review_id) REFERENCES ahdah.legacy_attribution_reviews (company_id, legacy_attribution_review_id) ON UPDATE NO ACTION ON DELETE NO ACTION DEFERRABLE INITIALLY DEFERRED;
ALTER TABLE ahdah.custody_movement_entries ADD CONSTRAINT fk_t04_company_id FOREIGN KEY (company_id) REFERENCES ahdah.companies (company_id) ON UPDATE NO ACTION ON DELETE NO ACTION;
ALTER TABLE ahdah.custody_movement_entries ADD CONSTRAINT fk_t04_custody_operation_id FOREIGN KEY (company_id, custody_operation_id) REFERENCES ahdah.custody_operations (company_id, custody_operation_id) ON UPDATE NO ACTION ON DELETE NO ACTION DEFERRABLE INITIALLY DEFERRED;
ALTER TABLE ahdah.custody_movement_entries ADD CONSTRAINT fk_t04_project_custody_position_id FOREIGN KEY (company_id, project_custody_position_id) REFERENCES ahdah.project_custody_positions (company_id, project_custody_position_id) ON UPDATE NO ACTION ON DELETE NO ACTION DEFERRABLE INITIALLY DEFERRED;
ALTER TABLE ahdah.custody_movement_entries ADD CONSTRAINT fk_t04_balance_ledger_entry_id FOREIGN KEY (company_id, balance_ledger_entry_id) REFERENCES ahdah.balance_ledger_entries (company_id, balance_ledger_entry_id) ON UPDATE NO ACTION ON DELETE NO ACTION;
ALTER TABLE ahdah.custody_movement_entries ADD CONSTRAINT fk_t04_reverses_entry_id FOREIGN KEY (company_id, reverses_entry_id) REFERENCES ahdah.custody_movement_entries (company_id, custody_movement_entry_id) ON UPDATE NO ACTION ON DELETE NO ACTION DEFERRABLE INITIALLY DEFERRED;
ALTER TABLE ahdah.custody_movement_entries ADD CONSTRAINT fk_t04_actor_user_id FOREIGN KEY (company_id, actor_user_id) REFERENCES ahdah.app_users (company_id, user_id) ON UPDATE NO ACTION ON DELETE NO ACTION;
ALTER TABLE ahdah.custody_reservations ADD CONSTRAINT fk_t05_company_id FOREIGN KEY (company_id) REFERENCES ahdah.companies (company_id) ON UPDATE NO ACTION ON DELETE NO ACTION;
ALTER TABLE ahdah.custody_reservations ADD CONSTRAINT fk_t05_custody_operation_id FOREIGN KEY (company_id, custody_operation_id) REFERENCES ahdah.custody_operations (company_id, custody_operation_id) ON UPDATE NO ACTION ON DELETE NO ACTION DEFERRABLE INITIALLY DEFERRED;
ALTER TABLE ahdah.custody_reservations ADD CONSTRAINT fk_t05_project_custody_position_id FOREIGN KEY (company_id, project_custody_position_id) REFERENCES ahdah.project_custody_positions (company_id, project_custody_position_id) ON UPDATE NO ACTION ON DELETE NO ACTION DEFERRABLE INITIALLY DEFERRED;
ALTER TABLE ahdah.custody_reservations ADD CONSTRAINT fk_t05_expense_advance_allocation_id FOREIGN KEY (company_id, expense_advance_allocation_id) REFERENCES ahdah.expense_advance_allocations (company_id, expense_advance_allocation_id) ON UPDATE NO ACTION ON DELETE NO ACTION;
ALTER TABLE ahdah.custody_reservations ADD CONSTRAINT fk_t05_transfer_advance_allocation_id FOREIGN KEY (company_id, transfer_advance_allocation_id) REFERENCES ahdah.transfer_advance_allocations (company_id, transfer_advance_allocation_id) ON UPDATE NO ACTION ON DELETE NO ACTION;
ALTER TABLE ahdah.custody_reservations ADD CONSTRAINT fk_t05_resolved_by_operation_id FOREIGN KEY (company_id, resolved_by_operation_id) REFERENCES ahdah.custody_operations (company_id, custody_operation_id) ON UPDATE NO ACTION ON DELETE NO ACTION DEFERRABLE INITIALLY DEFERRED;
ALTER TABLE ahdah.project_settlements ADD CONSTRAINT fk_t06_company_id FOREIGN KEY (company_id) REFERENCES ahdah.companies (company_id) ON UPDATE NO ACTION ON DELETE NO ACTION;
ALTER TABLE ahdah.project_settlements ADD CONSTRAINT fk_t06_project_id FOREIGN KEY (company_id, project_id) REFERENCES ahdah.projects (company_id, project_id) ON UPDATE NO ACTION ON DELETE NO ACTION;
ALTER TABLE ahdah.project_settlements ADD CONSTRAINT fk_t06_previous_settlement_id FOREIGN KEY (company_id, project_id, previous_settlement_id) REFERENCES ahdah.project_settlements (company_id, project_id, project_settlement_id) ON UPDATE NO ACTION ON DELETE NO ACTION DEFERRABLE INITIALLY DEFERRED;
ALTER TABLE ahdah.project_settlements ADD CONSTRAINT fk_t06_policy_version_id FOREIGN KEY (company_id, policy_version_id) REFERENCES ahdah.settlement_policy_versions (company_id, settlement_policy_version_id) ON UPDATE NO ACTION ON DELETE NO ACTION DEFERRABLE INITIALLY DEFERRED;
ALTER TABLE ahdah.project_settlements ADD CONSTRAINT fk_t06_prepared_by_user_id FOREIGN KEY (company_id, prepared_by_user_id) REFERENCES ahdah.app_users (company_id, user_id) ON UPDATE NO ACTION ON DELETE NO ACTION;
ALTER TABLE ahdah.project_settlements ADD CONSTRAINT fk_t06_submitted_by_user_id FOREIGN KEY (company_id, submitted_by_user_id) REFERENCES ahdah.app_users (company_id, user_id) ON UPDATE NO ACTION ON DELETE NO ACTION;
ALTER TABLE ahdah.project_settlements ADD CONSTRAINT fk_t06_approved_by_user_id FOREIGN KEY (company_id, approved_by_user_id) REFERENCES ahdah.app_users (company_id, user_id) ON UPDATE NO ACTION ON DELETE NO ACTION;
ALTER TABLE ahdah.project_settlements ADD CONSTRAINT fk_t06_snapshot_id FOREIGN KEY (company_id, project_id, snapshot_id) REFERENCES ahdah.project_settlement_snapshots (company_id, project_id, project_settlement_snapshot_id) ON UPDATE NO ACTION ON DELETE NO ACTION DEFERRABLE INITIALLY DEFERRED;
ALTER TABLE ahdah.project_settlement_snapshots ADD CONSTRAINT fk_t07_company_id FOREIGN KEY (company_id) REFERENCES ahdah.companies (company_id) ON UPDATE NO ACTION ON DELETE NO ACTION;
ALTER TABLE ahdah.project_settlement_snapshots ADD CONSTRAINT fk_t07_project_id FOREIGN KEY (company_id, project_id) REFERENCES ahdah.projects (company_id, project_id) ON UPDATE NO ACTION ON DELETE NO ACTION;
ALTER TABLE ahdah.project_settlement_snapshots ADD CONSTRAINT fk_t07_project_settlement_id FOREIGN KEY (company_id, project_id, project_settlement_id) REFERENCES ahdah.project_settlements (company_id, project_id, project_settlement_id) ON UPDATE NO ACTION ON DELETE NO ACTION DEFERRABLE INITIALLY DEFERRED;
ALTER TABLE ahdah.project_settlement_snapshots ADD CONSTRAINT fk_t07_policy_version_id FOREIGN KEY (company_id, policy_version_id) REFERENCES ahdah.settlement_policy_versions (company_id, settlement_policy_version_id) ON UPDATE NO ACTION ON DELETE NO ACTION DEFERRABLE INITIALLY DEFERRED;
ALTER TABLE ahdah.project_settlement_snapshots ADD CONSTRAINT fk_t07_evaluated_by_user_id FOREIGN KEY (company_id, evaluated_by_user_id) REFERENCES ahdah.app_users (company_id, user_id) ON UPDATE NO ACTION ON DELETE NO ACTION;
ALTER TABLE ahdah.project_settlement_snapshot_categories ADD CONSTRAINT fk_t08_company_id FOREIGN KEY (company_id) REFERENCES ahdah.companies (company_id) ON UPDATE NO ACTION ON DELETE NO ACTION;
ALTER TABLE ahdah.project_settlement_snapshot_categories ADD CONSTRAINT fk_t08_project_id FOREIGN KEY (company_id, project_id) REFERENCES ahdah.projects (company_id, project_id) ON UPDATE NO ACTION ON DELETE NO ACTION;
ALTER TABLE ahdah.project_settlement_snapshot_categories ADD CONSTRAINT fk_t08_snapshot_id FOREIGN KEY (company_id, project_id, snapshot_id) REFERENCES ahdah.project_settlement_snapshots (company_id, project_id, project_settlement_snapshot_id) ON UPDATE NO ACTION ON DELETE NO ACTION DEFERRABLE INITIALLY DEFERRED;
ALTER TABLE ahdah.project_settlement_events ADD CONSTRAINT fk_t09_company_id FOREIGN KEY (company_id) REFERENCES ahdah.companies (company_id) ON UPDATE NO ACTION ON DELETE NO ACTION;
ALTER TABLE ahdah.project_settlement_events ADD CONSTRAINT fk_t09_project_id FOREIGN KEY (company_id, project_id) REFERENCES ahdah.projects (company_id, project_id) ON UPDATE NO ACTION ON DELETE NO ACTION;
ALTER TABLE ahdah.project_settlement_events ADD CONSTRAINT fk_t09_settlement_id FOREIGN KEY (company_id, project_id, settlement_id) REFERENCES ahdah.project_settlements (company_id, project_id, project_settlement_id) ON UPDATE NO ACTION ON DELETE NO ACTION DEFERRABLE INITIALLY DEFERRED;
ALTER TABLE ahdah.project_settlement_events ADD CONSTRAINT fk_t09_actor_user_id FOREIGN KEY (company_id, actor_user_id) REFERENCES ahdah.app_users (company_id, user_id) ON UPDATE NO ACTION ON DELETE NO ACTION;
ALTER TABLE ahdah.project_settlement_events ADD CONSTRAINT fk_t09_authorization_decision_id FOREIGN KEY (company_id, project_id, authorization_decision_id) REFERENCES ahdah.project_authorization_decisions (company_id, project_id, project_authorization_decision_id) ON UPDATE NO ACTION ON DELETE NO ACTION DEFERRABLE INITIALLY DEFERRED;
ALTER TABLE ahdah.project_settlement_events ADD CONSTRAINT fk_t09_snapshot_id FOREIGN KEY (company_id, project_id, snapshot_id) REFERENCES ahdah.project_settlement_snapshots (company_id, project_id, project_settlement_snapshot_id) ON UPDATE NO ACTION ON DELETE NO ACTION DEFERRABLE INITIALLY DEFERRED;
ALTER TABLE ahdah.project_settlement_events ADD CONSTRAINT fk_t09_idempotency_record_id FOREIGN KEY (company_id, idempotency_record_id) REFERENCES ahdah.idempotency_records (company_id, idempotency_record_id) ON UPDATE NO ACTION ON DELETE NO ACTION;
ALTER TABLE ahdah.project_closure_certificates ADD CONSTRAINT fk_t10_company_id FOREIGN KEY (company_id) REFERENCES ahdah.companies (company_id) ON UPDATE NO ACTION ON DELETE NO ACTION;
ALTER TABLE ahdah.project_closure_certificates ADD CONSTRAINT fk_t10_project_id FOREIGN KEY (company_id, project_id) REFERENCES ahdah.projects (company_id, project_id) ON UPDATE NO ACTION ON DELETE NO ACTION;
ALTER TABLE ahdah.project_closure_certificates ADD CONSTRAINT fk_t10_project_settlement_id FOREIGN KEY (company_id, project_id, project_settlement_id) REFERENCES ahdah.project_settlements (company_id, project_id, project_settlement_id) ON UPDATE NO ACTION ON DELETE NO ACTION DEFERRABLE INITIALLY DEFERRED;
ALTER TABLE ahdah.project_closure_certificates ADD CONSTRAINT fk_t10_approved_snapshot_id FOREIGN KEY (company_id, project_id, approved_snapshot_id) REFERENCES ahdah.project_settlement_snapshots (company_id, project_id, project_settlement_snapshot_id) ON UPDATE NO ACTION ON DELETE NO ACTION DEFERRABLE INITIALLY DEFERRED;
ALTER TABLE ahdah.project_closure_certificates ADD CONSTRAINT fk_t10_closure_snapshot_id FOREIGN KEY (company_id, project_id, closure_snapshot_id) REFERENCES ahdah.project_settlement_snapshots (company_id, project_id, project_settlement_snapshot_id) ON UPDATE NO ACTION ON DELETE NO ACTION DEFERRABLE INITIALLY DEFERRED;
ALTER TABLE ahdah.project_closure_certificates ADD CONSTRAINT fk_t10_policy_version_id FOREIGN KEY (company_id, policy_version_id) REFERENCES ahdah.settlement_policy_versions (company_id, settlement_policy_version_id) ON UPDATE NO ACTION ON DELETE NO ACTION DEFERRABLE INITIALLY DEFERRED;
ALTER TABLE ahdah.project_closure_certificates ADD CONSTRAINT fk_t10_closed_by_user_id FOREIGN KEY (company_id, closed_by_user_id) REFERENCES ahdah.app_users (company_id, user_id) ON UPDATE NO ACTION ON DELETE NO ACTION;
ALTER TABLE ahdah.project_closure_certificates ADD CONSTRAINT fk_t10_authorization_decision_id FOREIGN KEY (company_id, project_id, authorization_decision_id) REFERENCES ahdah.project_authorization_decisions (company_id, project_id, project_authorization_decision_id) ON UPDATE NO ACTION ON DELETE NO ACTION DEFERRABLE INITIALLY DEFERRED;
ALTER TABLE ahdah.project_closure_certificates ADD CONSTRAINT fk_t10_idempotency_record_id FOREIGN KEY (company_id, idempotency_record_id) REFERENCES ahdah.idempotency_records (company_id, idempotency_record_id) ON UPDATE NO ACTION ON DELETE NO ACTION;
ALTER TABLE ahdah.project_reopenings ADD CONSTRAINT fk_t11_company_id FOREIGN KEY (company_id) REFERENCES ahdah.companies (company_id) ON UPDATE NO ACTION ON DELETE NO ACTION;
ALTER TABLE ahdah.project_reopenings ADD CONSTRAINT fk_t11_project_id FOREIGN KEY (company_id, project_id) REFERENCES ahdah.projects (company_id, project_id) ON UPDATE NO ACTION ON DELETE NO ACTION;
ALTER TABLE ahdah.project_reopenings ADD CONSTRAINT fk_t11_previous_closure_id FOREIGN KEY (company_id, project_id, previous_closure_id) REFERENCES ahdah.project_closure_certificates (company_id, project_id, project_closure_certificate_id) ON UPDATE NO ACTION ON DELETE NO ACTION DEFERRABLE INITIALLY DEFERRED;
ALTER TABLE ahdah.project_reopenings ADD CONSTRAINT fk_t11_new_settlement_id FOREIGN KEY (company_id, project_id, new_settlement_id) REFERENCES ahdah.project_settlements (company_id, project_id, project_settlement_id) ON UPDATE NO ACTION ON DELETE NO ACTION DEFERRABLE INITIALLY DEFERRED;
ALTER TABLE ahdah.project_reopenings ADD CONSTRAINT fk_t11_actor_user_id FOREIGN KEY (company_id, actor_user_id) REFERENCES ahdah.app_users (company_id, user_id) ON UPDATE NO ACTION ON DELETE NO ACTION;
ALTER TABLE ahdah.project_reopenings ADD CONSTRAINT fk_t11_authorization_decision_id FOREIGN KEY (company_id, project_id, authorization_decision_id) REFERENCES ahdah.project_authorization_decisions (company_id, project_id, project_authorization_decision_id) ON UPDATE NO ACTION ON DELETE NO ACTION DEFERRABLE INITIALLY DEFERRED;
ALTER TABLE ahdah.project_reopenings ADD CONSTRAINT fk_t11_audit_log_id FOREIGN KEY (company_id, audit_log_id) REFERENCES ahdah.audit_logs (company_id, audit_log_id) ON UPDATE NO ACTION ON DELETE NO ACTION;
ALTER TABLE ahdah.project_reopenings ADD CONSTRAINT fk_t11_idempotency_record_id FOREIGN KEY (company_id, idempotency_record_id) REFERENCES ahdah.idempotency_records (company_id, idempotency_record_id) ON UPDATE NO ACTION ON DELETE NO ACTION;
ALTER TABLE ahdah.project_authorization_decisions ADD CONSTRAINT fk_t12_company_id FOREIGN KEY (company_id) REFERENCES ahdah.companies (company_id) ON UPDATE NO ACTION ON DELETE NO ACTION;
ALTER TABLE ahdah.project_authorization_decisions ADD CONSTRAINT fk_t12_project_id FOREIGN KEY (company_id, project_id) REFERENCES ahdah.projects (company_id, project_id) ON UPDATE NO ACTION ON DELETE NO ACTION;
ALTER TABLE ahdah.project_authorization_decisions ADD CONSTRAINT fk_t12_actor_user_id FOREIGN KEY (company_id, actor_user_id) REFERENCES ahdah.app_users (company_id, user_id) ON UPDATE NO ACTION ON DELETE NO ACTION;
ALTER TABLE ahdah.project_authorization_decisions ADD CONSTRAINT fk_t12_policy_version_id FOREIGN KEY (company_id, policy_version_id) REFERENCES ahdah.settlement_policy_versions (company_id, settlement_policy_version_id) ON UPDATE NO ACTION ON DELETE NO ACTION DEFERRABLE INITIALLY DEFERRED;
ALTER TABLE ahdah.project_authorization_decisions ADD CONSTRAINT fk_t12_preparer_user_id FOREIGN KEY (company_id, preparer_user_id) REFERENCES ahdah.app_users (company_id, user_id) ON UPDATE NO ACTION ON DELETE NO ACTION;
ALTER TABLE ahdah.project_authorization_decisions ADD CONSTRAINT fk_t12_submitter_user_id FOREIGN KEY (company_id, submitter_user_id) REFERENCES ahdah.app_users (company_id, user_id) ON UPDATE NO ACTION ON DELETE NO ACTION;
ALTER TABLE ahdah.project_authorization_decisions ADD CONSTRAINT fk_t12_beneficiary_user_id FOREIGN KEY (company_id, beneficiary_user_id) REFERENCES ahdah.app_users (company_id, user_id) ON UPDATE NO ACTION ON DELETE NO ACTION;
ALTER TABLE ahdah.project_authorization_decisions ADD CONSTRAINT fk_t12_evidence_id FOREIGN KEY (company_id, evidence_id) REFERENCES ahdah.verified_evidence_objects (company_id, verified_evidence_object_id) ON UPDATE NO ACTION ON DELETE NO ACTION DEFERRABLE INITIALLY DEFERRED;
ALTER TABLE ahdah.project_operational_events ADD CONSTRAINT fk_t13_company_id FOREIGN KEY (company_id) REFERENCES ahdah.companies (company_id) ON UPDATE NO ACTION ON DELETE NO ACTION;
ALTER TABLE ahdah.project_operational_events ADD CONSTRAINT fk_t13_project_id FOREIGN KEY (company_id, project_id) REFERENCES ahdah.projects (company_id, project_id) ON UPDATE NO ACTION ON DELETE NO ACTION;
ALTER TABLE ahdah.project_operational_events ADD CONSTRAINT fk_t13_actor_user_id FOREIGN KEY (company_id, actor_user_id) REFERENCES ahdah.app_users (company_id, user_id) ON UPDATE NO ACTION ON DELETE NO ACTION;
ALTER TABLE ahdah.project_operational_events ADD CONSTRAINT fk_t13_evidence_id FOREIGN KEY (company_id, evidence_id) REFERENCES ahdah.verified_evidence_objects (company_id, verified_evidence_object_id) ON UPDATE NO ACTION ON DELETE NO ACTION DEFERRABLE INITIALLY DEFERRED;
ALTER TABLE ahdah.project_operational_events ADD CONSTRAINT fk_t13_acknowledgment_event_id FOREIGN KEY (company_id, project_id, acknowledgment_event_id) REFERENCES ahdah.project_operational_events (company_id, project_id, project_operational_event_id) ON UPDATE NO ACTION ON DELETE NO ACTION DEFERRABLE INITIALLY DEFERRED;
ALTER TABLE ahdah.project_operational_events ADD CONSTRAINT fk_t13_authorization_decision_id FOREIGN KEY (company_id, project_id, authorization_decision_id) REFERENCES ahdah.project_authorization_decisions (company_id, project_id, project_authorization_decision_id) ON UPDATE NO ACTION ON DELETE NO ACTION DEFERRABLE INITIALLY DEFERRED;
ALTER TABLE ahdah.project_operational_events ADD CONSTRAINT fk_t13_idempotency_record_id FOREIGN KEY (company_id, idempotency_record_id) REFERENCES ahdah.idempotency_records (company_id, idempotency_record_id) ON UPDATE NO ACTION ON DELETE NO ACTION;
ALTER TABLE ahdah.settlement_policy_versions ADD CONSTRAINT fk_t14_company_id FOREIGN KEY (company_id) REFERENCES ahdah.companies (company_id) ON UPDATE NO ACTION ON DELETE NO ACTION;
ALTER TABLE ahdah.settlement_policy_versions ADD CONSTRAINT fk_t14_approved_by_user_id FOREIGN KEY (company_id, approved_by_user_id) REFERENCES ahdah.app_users (company_id, user_id) ON UPDATE NO ACTION ON DELETE NO ACTION;
ALTER TABLE ahdah.settlement_policy_versions ADD CONSTRAINT fk_t14_predecessor_id FOREIGN KEY (company_id, predecessor_id) REFERENCES ahdah.settlement_policy_versions (company_id, settlement_policy_version_id) ON UPDATE NO ACTION ON DELETE NO ACTION DEFERRABLE INITIALLY DEFERRED;
ALTER TABLE ahdah.company_settlement_policy_bindings ADD CONSTRAINT fk_t15_company_id FOREIGN KEY (company_id) REFERENCES ahdah.companies (company_id) ON UPDATE NO ACTION ON DELETE NO ACTION;
ALTER TABLE ahdah.company_settlement_policy_bindings ADD CONSTRAINT fk_t15_active_policy_version_id FOREIGN KEY (company_id, active_policy_version_id) REFERENCES ahdah.settlement_policy_versions (company_id, settlement_policy_version_id) ON UPDATE NO ACTION ON DELETE NO ACTION DEFERRABLE INITIALLY DEFERRED;
ALTER TABLE ahdah.company_settlement_policy_bindings ADD CONSTRAINT fk_t15_activated_by_user_id FOREIGN KEY (company_id, activated_by_user_id) REFERENCES ahdah.app_users (company_id, user_id) ON UPDATE NO ACTION ON DELETE NO ACTION;
ALTER TABLE ahdah.document_custody_events ADD CONSTRAINT fk_t16_company_id FOREIGN KEY (company_id) REFERENCES ahdah.companies (company_id) ON UPDATE NO ACTION ON DELETE NO ACTION;
ALTER TABLE ahdah.document_custody_events ADD CONSTRAINT fk_t16_project_id FOREIGN KEY (company_id, project_id) REFERENCES ahdah.projects (company_id, project_id) ON UPDATE NO ACTION ON DELETE NO ACTION;
ALTER TABLE ahdah.document_custody_events ADD CONSTRAINT fk_t16_evidence_id FOREIGN KEY (company_id, evidence_id) REFERENCES ahdah.verified_evidence_objects (company_id, verified_evidence_object_id) ON UPDATE NO ACTION ON DELETE NO ACTION DEFERRABLE INITIALLY DEFERRED;
ALTER TABLE ahdah.document_custody_events ADD CONSTRAINT fk_t16_expense_document_id FOREIGN KEY (company_id, expense_document_id) REFERENCES ahdah.expense_documents (company_id, expense_document_id) ON UPDATE NO ACTION ON DELETE NO ACTION;
ALTER TABLE ahdah.document_custody_events ADD CONSTRAINT fk_t16_actor_user_id FOREIGN KEY (company_id, actor_user_id) REFERENCES ahdah.app_users (company_id, user_id) ON UPDATE NO ACTION ON DELETE NO ACTION;
ALTER TABLE ahdah.document_custody_events ADD CONSTRAINT fk_t16_policy_version_id FOREIGN KEY (company_id, policy_version_id) REFERENCES ahdah.settlement_policy_versions (company_id, settlement_policy_version_id) ON UPDATE NO ACTION ON DELETE NO ACTION DEFERRABLE INITIALLY DEFERRED;
ALTER TABLE ahdah.document_custody_events ADD CONSTRAINT fk_t16_previous_event_id FOREIGN KEY (company_id, project_id, previous_event_id) REFERENCES ahdah.document_custody_events (company_id, project_id, document_custody_event_id) ON UPDATE NO ACTION ON DELETE NO ACTION DEFERRABLE INITIALLY DEFERRED;
ALTER TABLE ahdah.document_custody_events ADD CONSTRAINT fk_t16_authorization_decision_id FOREIGN KEY (company_id, project_id, authorization_decision_id) REFERENCES ahdah.project_authorization_decisions (company_id, project_id, project_authorization_decision_id) ON UPDATE NO ACTION ON DELETE NO ACTION DEFERRABLE INITIALLY DEFERRED;
ALTER TABLE ahdah.document_policy_exceptions ADD CONSTRAINT fk_t17_company_id FOREIGN KEY (company_id) REFERENCES ahdah.companies (company_id) ON UPDATE NO ACTION ON DELETE NO ACTION;
ALTER TABLE ahdah.document_policy_exceptions ADD CONSTRAINT fk_t17_project_id FOREIGN KEY (company_id, project_id) REFERENCES ahdah.projects (company_id, project_id) ON UPDATE NO ACTION ON DELETE NO ACTION;
ALTER TABLE ahdah.document_policy_exceptions ADD CONSTRAINT fk_t17_evidence_id FOREIGN KEY (company_id, evidence_id) REFERENCES ahdah.verified_evidence_objects (company_id, verified_evidence_object_id) ON UPDATE NO ACTION ON DELETE NO ACTION DEFERRABLE INITIALLY DEFERRED;
ALTER TABLE ahdah.document_policy_exceptions ADD CONSTRAINT fk_t17_expense_document_id FOREIGN KEY (company_id, expense_document_id) REFERENCES ahdah.expense_documents (company_id, expense_document_id) ON UPDATE NO ACTION ON DELETE NO ACTION;
ALTER TABLE ahdah.document_policy_exceptions ADD CONSTRAINT fk_t17_policy_version_id FOREIGN KEY (company_id, policy_version_id) REFERENCES ahdah.settlement_policy_versions (company_id, settlement_policy_version_id) ON UPDATE NO ACTION ON DELETE NO ACTION DEFERRABLE INITIALLY DEFERRED;
ALTER TABLE ahdah.document_policy_exceptions ADD CONSTRAINT fk_t17_approved_by_user_id FOREIGN KEY (company_id, approved_by_user_id) REFERENCES ahdah.app_users (company_id, user_id) ON UPDATE NO ACTION ON DELETE NO ACTION;
ALTER TABLE ahdah.document_policy_exceptions ADD CONSTRAINT fk_t17_authorization_decision_id FOREIGN KEY (company_id, project_id, authorization_decision_id) REFERENCES ahdah.project_authorization_decisions (company_id, project_id, project_authorization_decision_id) ON UPDATE NO ACTION ON DELETE NO ACTION DEFERRABLE INITIALLY DEFERRED;
ALTER TABLE ahdah.document_policy_exceptions ADD CONSTRAINT fk_t17_supersedes_exception_id FOREIGN KEY (company_id, project_id, supersedes_exception_id) REFERENCES ahdah.document_policy_exceptions (company_id, project_id, document_policy_exception_id) ON UPDATE NO ACTION ON DELETE NO ACTION DEFERRABLE INITIALLY DEFERRED;
ALTER TABLE ahdah.verified_evidence_objects ADD CONSTRAINT fk_t18_company_id FOREIGN KEY (company_id) REFERENCES ahdah.companies (company_id) ON UPDATE NO ACTION ON DELETE NO ACTION;
ALTER TABLE ahdah.verified_evidence_objects ADD CONSTRAINT fk_t18_project_id FOREIGN KEY (company_id, project_id) REFERENCES ahdah.projects (company_id, project_id) ON UPDATE NO ACTION ON DELETE NO ACTION;
ALTER TABLE ahdah.verified_evidence_objects ADD CONSTRAINT fk_t18_expense_document_id FOREIGN KEY (company_id, expense_document_id) REFERENCES ahdah.expense_documents (company_id, expense_document_id) ON UPDATE NO ACTION ON DELETE NO ACTION;
ALTER TABLE ahdah.verified_evidence_objects ADD CONSTRAINT fk_t18_verified_by_user_id FOREIGN KEY (company_id, verified_by_user_id) REFERENCES ahdah.app_users (company_id, user_id) ON UPDATE NO ACTION ON DELETE NO ACTION;
ALTER TABLE ahdah.verified_evidence_objects ADD CONSTRAINT fk_t18_source_evidence_id FOREIGN KEY (company_id, source_evidence_id) REFERENCES ahdah.verified_evidence_objects (company_id, verified_evidence_object_id) ON UPDATE NO ACTION ON DELETE NO ACTION DEFERRABLE INITIALLY DEFERRED;
ALTER TABLE ahdah.project_owner_reconciliations ADD CONSTRAINT fk_t19_company_id FOREIGN KEY (company_id) REFERENCES ahdah.companies (company_id) ON UPDATE NO ACTION ON DELETE NO ACTION;
ALTER TABLE ahdah.project_owner_reconciliations ADD CONSTRAINT fk_t19_project_id FOREIGN KEY (company_id, project_id) REFERENCES ahdah.projects (company_id, project_id) ON UPDATE NO ACTION ON DELETE NO ACTION;
ALTER TABLE ahdah.project_owner_reconciliations ADD CONSTRAINT fk_t19_policy_version_id FOREIGN KEY (company_id, policy_version_id) REFERENCES ahdah.settlement_policy_versions (company_id, settlement_policy_version_id) ON UPDATE NO ACTION ON DELETE NO ACTION DEFERRABLE INITIALLY DEFERRED;
ALTER TABLE ahdah.project_owner_reconciliations ADD CONSTRAINT fk_t19_prepared_by_user_id FOREIGN KEY (company_id, prepared_by_user_id) REFERENCES ahdah.app_users (company_id, user_id) ON UPDATE NO ACTION ON DELETE NO ACTION;
ALTER TABLE ahdah.project_owner_reconciliations ADD CONSTRAINT fk_t19_approved_by_user_id FOREIGN KEY (company_id, approved_by_user_id) REFERENCES ahdah.app_users (company_id, user_id) ON UPDATE NO ACTION ON DELETE NO ACTION;
ALTER TABLE ahdah.project_owner_reconciliations ADD CONSTRAINT fk_t19_evidence_id FOREIGN KEY (company_id, evidence_id) REFERENCES ahdah.verified_evidence_objects (company_id, verified_evidence_object_id) ON UPDATE NO ACTION ON DELETE NO ACTION DEFERRABLE INITIALLY DEFERRED;
ALTER TABLE ahdah.project_owner_reconciliations ADD CONSTRAINT fk_t19_supersedes_id FOREIGN KEY (company_id, project_id, supersedes_id) REFERENCES ahdah.project_owner_reconciliations (company_id, project_id, project_owner_reconciliation_id) ON UPDATE NO ACTION ON DELETE NO ACTION DEFERRABLE INITIALLY DEFERRED;
ALTER TABLE ahdah.project_owner_reconciliation_positions ADD CONSTRAINT fk_t20_company_id FOREIGN KEY (company_id) REFERENCES ahdah.companies (company_id) ON UPDATE NO ACTION ON DELETE NO ACTION;
ALTER TABLE ahdah.project_owner_reconciliation_positions ADD CONSTRAINT fk_t20_project_id FOREIGN KEY (company_id, project_id) REFERENCES ahdah.projects (company_id, project_id) ON UPDATE NO ACTION ON DELETE NO ACTION;
ALTER TABLE ahdah.project_owner_reconciliation_positions ADD CONSTRAINT fk_t20_owner_reconciliation_id FOREIGN KEY (company_id, project_id, owner_reconciliation_id) REFERENCES ahdah.project_owner_reconciliations (company_id, project_id, project_owner_reconciliation_id) ON UPDATE NO ACTION ON DELETE NO ACTION DEFERRABLE INITIALLY DEFERRED;
ALTER TABLE ahdah.project_owner_retention_items ADD CONSTRAINT fk_t21_company_id FOREIGN KEY (company_id) REFERENCES ahdah.companies (company_id) ON UPDATE NO ACTION ON DELETE NO ACTION;
ALTER TABLE ahdah.project_owner_retention_items ADD CONSTRAINT fk_t21_project_id FOREIGN KEY (company_id, project_id) REFERENCES ahdah.projects (company_id, project_id) ON UPDATE NO ACTION ON DELETE NO ACTION;
ALTER TABLE ahdah.project_owner_retention_items ADD CONSTRAINT fk_t21_owner_position_id FOREIGN KEY (company_id, project_id, owner_position_id) REFERENCES ahdah.project_owner_reconciliation_positions (company_id, project_id, project_owner_reconciliation_position_id) ON UPDATE NO ACTION ON DELETE NO ACTION DEFERRABLE INITIALLY DEFERRED;
ALTER TABLE ahdah.project_owner_retention_items ADD CONSTRAINT fk_t21_evidence_id FOREIGN KEY (company_id, evidence_id) REFERENCES ahdah.verified_evidence_objects (company_id, verified_evidence_object_id) ON UPDATE NO ACTION ON DELETE NO ACTION DEFERRABLE INITIALLY DEFERRED;
ALTER TABLE ahdah.project_owner_retention_items ADD CONSTRAINT fk_t21_approved_by_user_id FOREIGN KEY (company_id, approved_by_user_id) REFERENCES ahdah.app_users (company_id, user_id) ON UPDATE NO ACTION ON DELETE NO ACTION;
ALTER TABLE ahdah.financial_resolution_effects ADD CONSTRAINT fk_t22_company_id FOREIGN KEY (company_id) REFERENCES ahdah.companies (company_id) ON UPDATE NO ACTION ON DELETE NO ACTION;
ALTER TABLE ahdah.financial_resolution_effects ADD CONSTRAINT fk_t22_project_id FOREIGN KEY (company_id, project_id) REFERENCES ahdah.projects (company_id, project_id) ON UPDATE NO ACTION ON DELETE NO ACTION;
ALTER TABLE ahdah.financial_resolution_effects ADD CONSTRAINT fk_t22_expense_return_id FOREIGN KEY (company_id, expense_return_id) REFERENCES ahdah.expense_returns (company_id, expense_return_id) ON UPDATE NO ACTION ON DELETE NO ACTION;
ALTER TABLE ahdah.financial_resolution_effects ADD CONSTRAINT fk_t22_advance_settlement_resolution_id FOREIGN KEY (company_id, advance_settlement_resolution_id) REFERENCES ahdah.advance_settlement_resolutions (company_id, advance_settlement_resolution_id) ON UPDATE NO ACTION ON DELETE NO ACTION;
ALTER TABLE ahdah.financial_resolution_effects ADD CONSTRAINT fk_t22_custody_movement_entry_id FOREIGN KEY (company_id, custody_movement_entry_id) REFERENCES ahdah.custody_movement_entries (company_id, custody_movement_entry_id) ON UPDATE NO ACTION ON DELETE NO ACTION DEFERRABLE INITIALLY DEFERRED;
ALTER TABLE ahdah.financial_resolution_effects ADD CONSTRAINT fk_t22_funding_source_ledger_entry_id FOREIGN KEY (company_id, funding_source_ledger_entry_id) REFERENCES ahdah.funding_source_ledger_entries (company_id, funding_source_ledger_entry_id) ON UPDATE NO ACTION ON DELETE NO ACTION;
ALTER TABLE ahdah.financial_resolution_effects ADD CONSTRAINT fk_t22_personal_claim_adjustment_id FOREIGN KEY (company_id, personal_claim_adjustment_id) REFERENCES ahdah.personal_claim_adjustments (company_id, personal_claim_adjustment_id) ON UPDATE NO ACTION ON DELETE NO ACTION;
ALTER TABLE ahdah.financial_resolution_effects ADD CONSTRAINT fk_t22_personal_claim_write_off_id FOREIGN KEY (company_id, personal_claim_write_off_id) REFERENCES ahdah.personal_claim_write_offs (company_id, personal_claim_write_off_id) ON UPDATE NO ACTION ON DELETE NO ACTION;
ALTER TABLE ahdah.financial_resolution_effects ADD CONSTRAINT fk_t22_supplier_debt_write_off_id FOREIGN KEY (company_id, supplier_debt_write_off_id) REFERENCES ahdah.supplier_debt_write_offs (company_id, supplier_debt_write_off_id) ON UPDATE NO ACTION ON DELETE NO ACTION;
ALTER TABLE ahdah.financial_resolution_effects ADD CONSTRAINT fk_t22_supplier_credit_note_id FOREIGN KEY (company_id, supplier_credit_note_id) REFERENCES ahdah.supplier_credit_notes (company_id, supplier_credit_note_id) ON UPDATE NO ACTION ON DELETE NO ACTION;
ALTER TABLE ahdah.financial_resolution_effects ADD CONSTRAINT fk_t22_supplier_refund_id FOREIGN KEY (company_id, supplier_refund_id) REFERENCES ahdah.supplier_refunds (company_id, supplier_refund_id) ON UPDATE NO ACTION ON DELETE NO ACTION;
ALTER TABLE ahdah.financial_resolution_effects ADD CONSTRAINT fk_t22_authorization_decision_id FOREIGN KEY (company_id, project_id, authorization_decision_id) REFERENCES ahdah.project_authorization_decisions (company_id, project_id, project_authorization_decision_id) ON UPDATE NO ACTION ON DELETE NO ACTION DEFERRABLE INITIALLY DEFERRED;
ALTER TABLE ahdah.financial_resolution_effects ADD CONSTRAINT fk_t22_evidence_id FOREIGN KEY (company_id, evidence_id) REFERENCES ahdah.verified_evidence_objects (company_id, verified_evidence_object_id) ON UPDATE NO ACTION ON DELETE NO ACTION DEFERRABLE INITIALLY DEFERRED;
ALTER TABLE ahdah.financial_resolution_effects ADD CONSTRAINT fk_t22_reverses_effect_id FOREIGN KEY (company_id, project_id, reverses_effect_id) REFERENCES ahdah.financial_resolution_effects (company_id, project_id, financial_resolution_effect_id) ON UPDATE NO ACTION ON DELETE NO ACTION DEFERRABLE INITIALLY DEFERRED;
ALTER TABLE ahdah.supplier_credit_intents ADD CONSTRAINT fk_t23_company_id FOREIGN KEY (company_id) REFERENCES ahdah.companies (company_id) ON UPDATE NO ACTION ON DELETE NO ACTION;
ALTER TABLE ahdah.supplier_credit_intents ADD CONSTRAINT fk_t23_supplier_credit_note_id FOREIGN KEY (company_id, supplier_credit_note_id) REFERENCES ahdah.supplier_credit_notes (company_id, supplier_credit_note_id) ON UPDATE NO ACTION ON DELETE NO ACTION;
ALTER TABLE ahdah.supplier_credit_intents ADD CONSTRAINT fk_t23_project_id FOREIGN KEY (company_id, project_id) REFERENCES ahdah.projects (company_id, project_id) ON UPDATE NO ACTION ON DELETE NO ACTION;
ALTER TABLE ahdah.supplier_credit_intents ADD CONSTRAINT fk_t23_prepared_by_user_id FOREIGN KEY (company_id, prepared_by_user_id) REFERENCES ahdah.app_users (company_id, user_id) ON UPDATE NO ACTION ON DELETE NO ACTION;
ALTER TABLE ahdah.supplier_credit_intents ADD CONSTRAINT fk_t23_approved_by_user_id FOREIGN KEY (company_id, approved_by_user_id) REFERENCES ahdah.app_users (company_id, user_id) ON UPDATE NO ACTION ON DELETE NO ACTION;
ALTER TABLE ahdah.supplier_credit_intent_applications ADD CONSTRAINT fk_t24_company_id FOREIGN KEY (company_id) REFERENCES ahdah.companies (company_id) ON UPDATE NO ACTION ON DELETE NO ACTION;
ALTER TABLE ahdah.supplier_credit_intent_applications ADD CONSTRAINT fk_t24_supplier_credit_intent_id FOREIGN KEY (company_id, supplier_credit_intent_id) REFERENCES ahdah.supplier_credit_intents (company_id, supplier_credit_intent_id) ON UPDATE NO ACTION ON DELETE NO ACTION DEFERRABLE INITIALLY DEFERRED;
ALTER TABLE ahdah.supplier_credit_intent_applications ADD CONSTRAINT fk_t24_supplier_credit_note_allocation_id FOREIGN KEY (company_id, supplier_credit_note_allocation_id) REFERENCES ahdah.supplier_credit_note_allocations (company_id, supplier_credit_note_allocation_id) ON UPDATE NO ACTION ON DELETE NO ACTION;
ALTER TABLE ahdah.supplier_credit_intent_applications ADD CONSTRAINT fk_t24_applied_by_user_id FOREIGN KEY (company_id, applied_by_user_id) REFERENCES ahdah.app_users (company_id, user_id) ON UPDATE NO ACTION ON DELETE NO ACTION;
ALTER TABLE ahdah.supplier_credit_intent_applications ADD CONSTRAINT fk_t24_reverses_application_id FOREIGN KEY (company_id, reverses_application_id) REFERENCES ahdah.supplier_credit_intent_applications (company_id, supplier_credit_intent_application_id) ON UPDATE NO ACTION ON DELETE NO ACTION DEFERRABLE INITIALLY DEFERRED;
ALTER TABLE ahdah.legacy_attribution_reviews ADD CONSTRAINT fk_t25_company_id FOREIGN KEY (company_id) REFERENCES ahdah.companies (company_id) ON UPDATE NO ACTION ON DELETE NO ACTION;
ALTER TABLE ahdah.legacy_attribution_reviews ADD CONSTRAINT fk_t25_declared_by_user_id FOREIGN KEY (company_id, declared_by_user_id) REFERENCES ahdah.app_users (company_id, user_id) ON UPDATE NO ACTION ON DELETE NO ACTION;
ALTER TABLE ahdah.legacy_attribution_reviews ADD CONSTRAINT fk_t25_reviewed_by_user_id FOREIGN KEY (company_id, reviewed_by_user_id) REFERENCES ahdah.app_users (company_id, user_id) ON UPDATE NO ACTION ON DELETE NO ACTION;
ALTER TABLE ahdah.legacy_attribution_reviews ADD CONSTRAINT fk_t25_approved_by_user_id FOREIGN KEY (company_id, approved_by_user_id) REFERENCES ahdah.app_users (company_id, user_id) ON UPDATE NO ACTION ON DELETE NO ACTION;
ALTER TABLE ahdah.legacy_attribution_reviews ADD CONSTRAINT fk_t25_evidence_id FOREIGN KEY (company_id, evidence_id) REFERENCES ahdah.verified_evidence_objects (company_id, verified_evidence_object_id) ON UPDATE NO ACTION ON DELETE NO ACTION DEFERRABLE INITIALLY DEFERRED;
ALTER TABLE ahdah.legacy_attribution_reviews ADD CONSTRAINT fk_t25_supersedes_review_id FOREIGN KEY (company_id, supersedes_review_id) REFERENCES ahdah.legacy_attribution_reviews (company_id, legacy_attribution_review_id) ON UPDATE NO ACTION ON DELETE NO ACTION DEFERRABLE INITIALLY DEFERRED;
ALTER TABLE ahdah.legacy_attribution_review_projects ADD CONSTRAINT fk_t26_company_id FOREIGN KEY (company_id) REFERENCES ahdah.companies (company_id) ON UPDATE NO ACTION ON DELETE NO ACTION;
ALTER TABLE ahdah.legacy_attribution_review_projects ADD CONSTRAINT fk_t26_legacy_attribution_review_id FOREIGN KEY (company_id, legacy_attribution_review_id) REFERENCES ahdah.legacy_attribution_reviews (company_id, legacy_attribution_review_id) ON UPDATE NO ACTION ON DELETE NO ACTION DEFERRABLE INITIALLY DEFERRED;
ALTER TABLE ahdah.legacy_attribution_review_projects ADD CONSTRAINT fk_t26_project_id FOREIGN KEY (company_id, project_id) REFERENCES ahdah.projects (company_id, project_id) ON UPDATE NO ACTION ON DELETE NO ACTION;
ALTER TABLE ahdah.project_closure_certificates ADD CONSTRAINT fk_t10_cycle_approved_snapshot_id FOREIGN KEY (company_id, project_id, project_settlement_id, approved_snapshot_id) REFERENCES ahdah.project_settlement_snapshots (company_id, project_id, project_settlement_id, project_settlement_snapshot_id) ON UPDATE NO ACTION ON DELETE NO ACTION DEFERRABLE INITIALLY DEFERRED;
ALTER TABLE ahdah.project_closure_certificates ADD CONSTRAINT fk_t10_cycle_closure_snapshot_id FOREIGN KEY (company_id, project_id, project_settlement_id, closure_snapshot_id) REFERENCES ahdah.project_settlement_snapshots (company_id, project_id, project_settlement_id, project_settlement_snapshot_id) ON UPDATE NO ACTION ON DELETE NO ACTION DEFERRABLE INITIALLY DEFERRED;

-- workflow: company state queue
CREATE INDEX ix_t01_state ON ahdah.project_financial_controls (company_id, financial_state);

-- settlement reporting: current custody by project
CREATE INDEX ix_t02_project_balance ON ahdah.project_custody_positions (company_id, project_id, user_advance_balance_id);

-- settlement reporting: movement history
CREATE INDEX ix_t04_position_time ON ahdah.custody_movement_entries (company_id, project_custody_position_id, occurred_at);

-- uniqueness: allocation within slice
CREATE UNIQUE INDEX ux_t05_expense_allocation ON ahdah.custody_reservations (company_id, project_custody_position_id, expense_advance_allocation_id) WHERE expense_advance_allocation_id IS NOT NULL;

-- uniqueness: allocation within slice
CREATE UNIQUE INDEX ux_t05_transfer_allocation ON ahdah.custody_reservations (company_id, project_custody_position_id, transfer_advance_allocation_id) WHERE transfer_advance_allocation_id IS NOT NULL;

-- workflow: active reservations
CREATE INDEX ix_t05_position_status ON ahdah.custody_reservations (company_id, project_custody_position_id, status);

-- workflow: project cycles
CREATE INDEX ix_t06_workflow ON ahdah.project_settlements (company_id, project_id, status);

-- settlement reporting: evaluated revisions
CREATE INDEX ix_t07_revision ON ahdah.project_settlement_snapshots (company_id, project_id, financial_revision);

-- settlement reporting: paper custody chain
CREATE INDEX ix_t16_evidence_time ON ahdah.document_custody_events (company_id, project_id, evidence_id, occurred_at);

-- uniqueness: one exception successor
CREATE UNIQUE INDEX ux_t17_supersedes ON ahdah.document_policy_exceptions (company_id, supersedes_exception_id) WHERE supersedes_exception_id IS NOT NULL;

-- workflow: owner review
CREATE INDEX ix_t19_workflow ON ahdah.project_owner_reconciliations (company_id, project_id, status);

-- uniqueness: effect ordinal per origin
CREATE UNIQUE INDEX ux_t22_expense_return_id ON ahdah.financial_resolution_effects (company_id, expense_return_id, effect_number) WHERE expense_return_id IS NOT NULL;

-- uniqueness: effect ordinal per origin
CREATE UNIQUE INDEX ux_t22_advance_settlement_resolution_id ON ahdah.financial_resolution_effects (company_id, advance_settlement_resolution_id, effect_number) WHERE advance_settlement_resolution_id IS NOT NULL;

-- uniqueness: one economic use of posting target
CREATE UNIQUE INDEX ux_t22_custody_movement_entry_id ON ahdah.financial_resolution_effects (company_id, custody_movement_entry_id) WHERE custody_movement_entry_id IS NOT NULL;

-- uniqueness: one economic use of posting target
CREATE UNIQUE INDEX ux_t22_funding_source_ledger_entry_id ON ahdah.financial_resolution_effects (company_id, funding_source_ledger_entry_id) WHERE funding_source_ledger_entry_id IS NOT NULL;

-- uniqueness: one economic use of posting target
CREATE UNIQUE INDEX ux_t22_personal_claim_adjustment_id ON ahdah.financial_resolution_effects (company_id, personal_claim_adjustment_id) WHERE personal_claim_adjustment_id IS NOT NULL;

-- uniqueness: one economic use of posting target
CREATE UNIQUE INDEX ux_t22_personal_claim_write_off_id ON ahdah.financial_resolution_effects (company_id, personal_claim_write_off_id) WHERE personal_claim_write_off_id IS NOT NULL;

-- uniqueness: one economic use of posting target
CREATE UNIQUE INDEX ux_t22_supplier_debt_write_off_id ON ahdah.financial_resolution_effects (company_id, supplier_debt_write_off_id) WHERE supplier_debt_write_off_id IS NOT NULL;

-- uniqueness: one economic use of posting target
CREATE UNIQUE INDEX ux_t22_supplier_credit_note_id ON ahdah.financial_resolution_effects (company_id, supplier_credit_note_id) WHERE supplier_credit_note_id IS NOT NULL;

-- uniqueness: one compensating application
CREATE UNIQUE INDEX ux_t24_reversal ON ahdah.supplier_credit_intent_applications (company_id, reverses_application_id) WHERE reverses_application_id IS NOT NULL;

-- cutover/reconciliation: affected project scope
CREATE INDEX ix_t26_impact ON ahdah.legacy_attribution_review_projects (company_id, project_id, impact_code);

-- FK support: tenant-safe reference/restrict lookup
CREATE INDEX ix_t01_fk_active_settlement_id ON ahdah.project_financial_controls (company_id, project_id, active_settlement_id);

-- FK support: tenant-safe reference/restrict lookup
CREATE INDEX ix_t01_fk_active_closure_id ON ahdah.project_financial_controls (company_id, project_id, active_closure_id);

-- FK support: tenant-safe reference/restrict lookup
CREATE INDEX ix_t03_fk_requested_by_user_id ON ahdah.custody_operations (company_id, requested_by_user_id);

-- FK support: tenant-safe reference/restrict lookup
CREATE INDEX ix_t03_fk_approved_by_user_id ON ahdah.custody_operations (company_id, approved_by_user_id);

-- FK support: tenant-safe reference/restrict lookup
CREATE INDEX ix_t03_fk_authorization_decision_id ON ahdah.custody_operations (company_id, authorization_decision_id);

-- FK support: tenant-safe reference/restrict lookup
CREATE INDEX ix_t03_fk_source_operation_id ON ahdah.custody_operations (company_id, source_operation_id);

-- FK support: tenant-safe reference/restrict lookup
CREATE INDEX ix_t03_fk_money_transfer_id ON ahdah.custody_operations (company_id, money_transfer_id);

-- FK support: tenant-safe reference/restrict lookup
CREATE INDEX ix_t03_fk_expense_id ON ahdah.custody_operations (company_id, expense_id);

-- FK support: tenant-safe reference/restrict lookup
CREATE INDEX ix_t03_fk_expense_return_id ON ahdah.custody_operations (company_id, expense_return_id);

-- FK support: tenant-safe reference/restrict lookup
CREATE INDEX ix_t03_fk_legacy_attribution_review_id ON ahdah.custody_operations (company_id, legacy_attribution_review_id);

-- FK support: tenant-safe reference/restrict lookup
CREATE INDEX ix_t04_fk_balance_ledger_entry_id ON ahdah.custody_movement_entries (company_id, balance_ledger_entry_id);

-- FK support: tenant-safe reference/restrict lookup
CREATE INDEX ix_t04_fk_reverses_entry_id ON ahdah.custody_movement_entries (company_id, reverses_entry_id);

-- FK support: tenant-safe reference/restrict lookup
CREATE INDEX ix_t04_fk_actor_user_id ON ahdah.custody_movement_entries (company_id, actor_user_id);

-- FK support: tenant-safe reference/restrict lookup
CREATE INDEX ix_t05_fk_custody_operation_id ON ahdah.custody_reservations (company_id, custody_operation_id);

-- FK support: tenant-safe reference/restrict lookup
CREATE INDEX ix_t05_fk_expense_advance_allocation_id ON ahdah.custody_reservations (company_id, expense_advance_allocation_id);

-- FK support: tenant-safe reference/restrict lookup
CREATE INDEX ix_t05_fk_transfer_advance_allocation_id ON ahdah.custody_reservations (company_id, transfer_advance_allocation_id);

-- FK support: tenant-safe reference/restrict lookup
CREATE INDEX ix_t05_fk_resolved_by_operation_id ON ahdah.custody_reservations (company_id, resolved_by_operation_id);

-- FK support: tenant-safe reference/restrict lookup
CREATE INDEX ix_t06_fk_previous_settlement_id ON ahdah.project_settlements (company_id, project_id, previous_settlement_id);

-- FK support: tenant-safe reference/restrict lookup
CREATE INDEX ix_t06_fk_policy_version_id ON ahdah.project_settlements (company_id, policy_version_id);

-- FK support: tenant-safe reference/restrict lookup
CREATE INDEX ix_t06_fk_prepared_by_user_id ON ahdah.project_settlements (company_id, prepared_by_user_id);

-- FK support: tenant-safe reference/restrict lookup
CREATE INDEX ix_t06_fk_submitted_by_user_id ON ahdah.project_settlements (company_id, submitted_by_user_id);

-- FK support: tenant-safe reference/restrict lookup
CREATE INDEX ix_t06_fk_approved_by_user_id ON ahdah.project_settlements (company_id, approved_by_user_id);

-- FK support: tenant-safe reference/restrict lookup
CREATE INDEX ix_t06_fk_snapshot_id ON ahdah.project_settlements (company_id, project_id, snapshot_id);

-- FK support: tenant-safe reference/restrict lookup
CREATE INDEX ix_t07_fk_policy_version_id ON ahdah.project_settlement_snapshots (company_id, policy_version_id);

-- FK support: tenant-safe reference/restrict lookup
CREATE INDEX ix_t07_fk_evaluated_by_user_id ON ahdah.project_settlement_snapshots (company_id, evaluated_by_user_id);

-- FK support: tenant-safe reference/restrict lookup
CREATE INDEX ix_t08_fk_snapshot_id ON ahdah.project_settlement_snapshot_categories (company_id, project_id, snapshot_id);

-- FK support: tenant-safe reference/restrict lookup
CREATE INDEX ix_t09_fk_settlement_id ON ahdah.project_settlement_events (company_id, project_id, settlement_id);

-- FK support: tenant-safe reference/restrict lookup
CREATE INDEX ix_t09_fk_actor_user_id ON ahdah.project_settlement_events (company_id, actor_user_id);

-- FK support: tenant-safe reference/restrict lookup
CREATE INDEX ix_t09_fk_authorization_decision_id ON ahdah.project_settlement_events (company_id, project_id, authorization_decision_id);

-- FK support: tenant-safe reference/restrict lookup
CREATE INDEX ix_t09_fk_snapshot_id ON ahdah.project_settlement_events (company_id, project_id, snapshot_id);

-- FK support: tenant-safe reference/restrict lookup
CREATE INDEX ix_t10_fk_project_settlement_id ON ahdah.project_closure_certificates (company_id, project_id, project_settlement_id);

-- FK support: tenant-safe reference/restrict lookup
CREATE INDEX ix_t10_fk_approved_snapshot_id ON ahdah.project_closure_certificates (company_id, project_id, approved_snapshot_id);

-- FK support: tenant-safe reference/restrict lookup
CREATE INDEX ix_t10_fk_closure_snapshot_id ON ahdah.project_closure_certificates (company_id, project_id, closure_snapshot_id);

-- FK support: tenant-safe reference/restrict lookup
CREATE INDEX ix_t10_fk_policy_version_id ON ahdah.project_closure_certificates (company_id, policy_version_id);

-- FK support: tenant-safe reference/restrict lookup
CREATE INDEX ix_t10_fk_closed_by_user_id ON ahdah.project_closure_certificates (company_id, closed_by_user_id);

-- FK support: tenant-safe reference/restrict lookup
CREATE INDEX ix_t10_fk_authorization_decision_id ON ahdah.project_closure_certificates (company_id, project_id, authorization_decision_id);

-- FK support: tenant-safe reference/restrict lookup
CREATE INDEX ix_t11_fk_previous_closure_id ON ahdah.project_reopenings (company_id, project_id, previous_closure_id);

-- FK support: tenant-safe reference/restrict lookup
CREATE INDEX ix_t11_fk_new_settlement_id ON ahdah.project_reopenings (company_id, project_id, new_settlement_id);

-- FK support: tenant-safe reference/restrict lookup
CREATE INDEX ix_t11_fk_actor_user_id ON ahdah.project_reopenings (company_id, actor_user_id);

-- FK support: tenant-safe reference/restrict lookup
CREATE INDEX ix_t11_fk_authorization_decision_id ON ahdah.project_reopenings (company_id, project_id, authorization_decision_id);

-- FK support: tenant-safe reference/restrict lookup
CREATE INDEX ix_t11_fk_audit_log_id ON ahdah.project_reopenings (company_id, audit_log_id);

-- FK support: tenant-safe reference/restrict lookup
CREATE INDEX ix_t12_fk_actor_user_id ON ahdah.project_authorization_decisions (company_id, actor_user_id);

-- FK support: tenant-safe reference/restrict lookup
CREATE INDEX ix_t12_fk_policy_version_id ON ahdah.project_authorization_decisions (company_id, policy_version_id);

-- FK support: tenant-safe reference/restrict lookup
CREATE INDEX ix_t12_fk_preparer_user_id ON ahdah.project_authorization_decisions (company_id, preparer_user_id);

-- FK support: tenant-safe reference/restrict lookup
CREATE INDEX ix_t12_fk_submitter_user_id ON ahdah.project_authorization_decisions (company_id, submitter_user_id);

-- FK support: tenant-safe reference/restrict lookup
CREATE INDEX ix_t12_fk_beneficiary_user_id ON ahdah.project_authorization_decisions (company_id, beneficiary_user_id);

-- FK support: tenant-safe reference/restrict lookup
CREATE INDEX ix_t12_fk_evidence_id ON ahdah.project_authorization_decisions (company_id, evidence_id);

-- FK support: tenant-safe reference/restrict lookup
CREATE INDEX ix_t13_fk_actor_user_id ON ahdah.project_operational_events (company_id, actor_user_id);

-- FK support: tenant-safe reference/restrict lookup
CREATE INDEX ix_t13_fk_evidence_id ON ahdah.project_operational_events (company_id, evidence_id);

-- FK support: tenant-safe reference/restrict lookup
CREATE INDEX ix_t13_fk_acknowledgment_event_id ON ahdah.project_operational_events (company_id, project_id, acknowledgment_event_id);

-- FK support: tenant-safe reference/restrict lookup
CREATE INDEX ix_t13_fk_authorization_decision_id ON ahdah.project_operational_events (company_id, project_id, authorization_decision_id);

-- FK support: tenant-safe reference/restrict lookup
CREATE INDEX ix_t14_fk_approved_by_user_id ON ahdah.settlement_policy_versions (company_id, approved_by_user_id);

-- FK support: tenant-safe reference/restrict lookup
CREATE INDEX ix_t14_fk_predecessor_id ON ahdah.settlement_policy_versions (company_id, predecessor_id);

-- FK support: tenant-safe reference/restrict lookup
CREATE INDEX ix_t15_fk_active_policy_version_id ON ahdah.company_settlement_policy_bindings (company_id, active_policy_version_id);

-- FK support: tenant-safe reference/restrict lookup
CREATE INDEX ix_t15_fk_activated_by_user_id ON ahdah.company_settlement_policy_bindings (company_id, activated_by_user_id);

-- FK support: tenant-safe reference/restrict lookup
CREATE INDEX ix_t16_fk_evidence_id ON ahdah.document_custody_events (company_id, evidence_id);

-- FK support: tenant-safe reference/restrict lookup
CREATE INDEX ix_t16_fk_expense_document_id ON ahdah.document_custody_events (company_id, expense_document_id);

-- FK support: tenant-safe reference/restrict lookup
CREATE INDEX ix_t16_fk_actor_user_id ON ahdah.document_custody_events (company_id, actor_user_id);

-- FK support: tenant-safe reference/restrict lookup
CREATE INDEX ix_t16_fk_policy_version_id ON ahdah.document_custody_events (company_id, policy_version_id);

-- FK support: tenant-safe reference/restrict lookup
CREATE INDEX ix_t16_fk_previous_event_id ON ahdah.document_custody_events (company_id, project_id, previous_event_id);

-- FK support: tenant-safe reference/restrict lookup
CREATE INDEX ix_t16_fk_authorization_decision_id ON ahdah.document_custody_events (company_id, project_id, authorization_decision_id);

-- FK support: tenant-safe reference/restrict lookup
CREATE INDEX ix_t17_fk_evidence_id ON ahdah.document_policy_exceptions (company_id, evidence_id);

-- FK support: tenant-safe reference/restrict lookup
CREATE INDEX ix_t17_fk_expense_document_id ON ahdah.document_policy_exceptions (company_id, expense_document_id);

-- FK support: tenant-safe reference/restrict lookup
CREATE INDEX ix_t17_fk_policy_version_id ON ahdah.document_policy_exceptions (company_id, policy_version_id);

-- FK support: tenant-safe reference/restrict lookup
CREATE INDEX ix_t17_fk_approved_by_user_id ON ahdah.document_policy_exceptions (company_id, approved_by_user_id);

-- FK support: tenant-safe reference/restrict lookup
CREATE INDEX ix_t17_fk_authorization_decision_id ON ahdah.document_policy_exceptions (company_id, project_id, authorization_decision_id);

-- FK support: tenant-safe reference/restrict lookup
CREATE INDEX ix_t17_fk_supersedes_exception_id ON ahdah.document_policy_exceptions (company_id, project_id, supersedes_exception_id);

-- FK support: tenant-safe reference/restrict lookup
CREATE INDEX ix_t18_fk_project_id ON ahdah.verified_evidence_objects (company_id, project_id);

-- FK support: tenant-safe reference/restrict lookup
CREATE INDEX ix_t18_fk_expense_document_id ON ahdah.verified_evidence_objects (company_id, expense_document_id);

-- FK support: tenant-safe reference/restrict lookup
CREATE INDEX ix_t18_fk_verified_by_user_id ON ahdah.verified_evidence_objects (company_id, verified_by_user_id);

-- FK support: tenant-safe reference/restrict lookup
CREATE INDEX ix_t18_fk_source_evidence_id ON ahdah.verified_evidence_objects (company_id, source_evidence_id);

-- FK support: tenant-safe reference/restrict lookup
CREATE INDEX ix_t19_fk_policy_version_id ON ahdah.project_owner_reconciliations (company_id, policy_version_id);

-- FK support: tenant-safe reference/restrict lookup
CREATE INDEX ix_t19_fk_prepared_by_user_id ON ahdah.project_owner_reconciliations (company_id, prepared_by_user_id);

-- FK support: tenant-safe reference/restrict lookup
CREATE INDEX ix_t19_fk_approved_by_user_id ON ahdah.project_owner_reconciliations (company_id, approved_by_user_id);

-- FK support: tenant-safe reference/restrict lookup
CREATE INDEX ix_t19_fk_evidence_id ON ahdah.project_owner_reconciliations (company_id, evidence_id);

-- FK support: tenant-safe reference/restrict lookup
CREATE INDEX ix_t19_fk_supersedes_id ON ahdah.project_owner_reconciliations (company_id, project_id, supersedes_id);

-- FK support: tenant-safe reference/restrict lookup
CREATE INDEX ix_t20_fk_owner_reconciliation_id ON ahdah.project_owner_reconciliation_positions (company_id, project_id, owner_reconciliation_id);

-- FK support: tenant-safe reference/restrict lookup
CREATE INDEX ix_t21_fk_owner_position_id ON ahdah.project_owner_retention_items (company_id, project_id, owner_position_id);

-- FK support: tenant-safe reference/restrict lookup
CREATE INDEX ix_t21_fk_evidence_id ON ahdah.project_owner_retention_items (company_id, evidence_id);

-- FK support: tenant-safe reference/restrict lookup
CREATE INDEX ix_t21_fk_approved_by_user_id ON ahdah.project_owner_retention_items (company_id, approved_by_user_id);

-- FK support: tenant-safe reference/restrict lookup
CREATE INDEX ix_t22_fk_expense_return_id ON ahdah.financial_resolution_effects (company_id, expense_return_id);

-- FK support: tenant-safe reference/restrict lookup
CREATE INDEX ix_t22_fk_advance_settlement_resolution_id ON ahdah.financial_resolution_effects (company_id, advance_settlement_resolution_id);

-- FK support: tenant-safe reference/restrict lookup
CREATE INDEX ix_t22_fk_custody_movement_entry_id ON ahdah.financial_resolution_effects (company_id, custody_movement_entry_id);

-- FK support: tenant-safe reference/restrict lookup
CREATE INDEX ix_t22_fk_funding_source_ledger_entry_id ON ahdah.financial_resolution_effects (company_id, funding_source_ledger_entry_id);

-- FK support: tenant-safe reference/restrict lookup
CREATE INDEX ix_t22_fk_personal_claim_adjustment_id ON ahdah.financial_resolution_effects (company_id, personal_claim_adjustment_id);

-- FK support: tenant-safe reference/restrict lookup
CREATE INDEX ix_t22_fk_personal_claim_write_off_id ON ahdah.financial_resolution_effects (company_id, personal_claim_write_off_id);

-- FK support: tenant-safe reference/restrict lookup
CREATE INDEX ix_t22_fk_supplier_debt_write_off_id ON ahdah.financial_resolution_effects (company_id, supplier_debt_write_off_id);

-- FK support: tenant-safe reference/restrict lookup
CREATE INDEX ix_t22_fk_supplier_credit_note_id ON ahdah.financial_resolution_effects (company_id, supplier_credit_note_id);

-- FK support: tenant-safe reference/restrict lookup
CREATE INDEX ix_t22_fk_supplier_refund_id ON ahdah.financial_resolution_effects (company_id, supplier_refund_id);

-- FK support: tenant-safe reference/restrict lookup
CREATE INDEX ix_t22_fk_authorization_decision_id ON ahdah.financial_resolution_effects (company_id, project_id, authorization_decision_id);

-- FK support: tenant-safe reference/restrict lookup
CREATE INDEX ix_t22_fk_evidence_id ON ahdah.financial_resolution_effects (company_id, evidence_id);

-- FK support: tenant-safe reference/restrict lookup
CREATE INDEX ix_t22_fk_reverses_effect_id ON ahdah.financial_resolution_effects (company_id, project_id, reverses_effect_id);

-- FK support: tenant-safe reference/restrict lookup
CREATE INDEX ix_t23_fk_project_id ON ahdah.supplier_credit_intents (company_id, project_id);

-- FK support: tenant-safe reference/restrict lookup
CREATE INDEX ix_t23_fk_prepared_by_user_id ON ahdah.supplier_credit_intents (company_id, prepared_by_user_id);

-- FK support: tenant-safe reference/restrict lookup
CREATE INDEX ix_t23_fk_approved_by_user_id ON ahdah.supplier_credit_intents (company_id, approved_by_user_id);

-- FK support: tenant-safe reference/restrict lookup
CREATE INDEX ix_t24_fk_supplier_credit_intent_id ON ahdah.supplier_credit_intent_applications (company_id, supplier_credit_intent_id);

-- FK support: tenant-safe reference/restrict lookup
CREATE INDEX ix_t24_fk_applied_by_user_id ON ahdah.supplier_credit_intent_applications (company_id, applied_by_user_id);

-- FK support: tenant-safe reference/restrict lookup
CREATE INDEX ix_t24_fk_reverses_application_id ON ahdah.supplier_credit_intent_applications (company_id, reverses_application_id);

-- FK support: tenant-safe reference/restrict lookup
CREATE INDEX ix_t25_fk_declared_by_user_id ON ahdah.legacy_attribution_reviews (company_id, declared_by_user_id);

-- FK support: tenant-safe reference/restrict lookup
CREATE INDEX ix_t25_fk_reviewed_by_user_id ON ahdah.legacy_attribution_reviews (company_id, reviewed_by_user_id);

-- FK support: tenant-safe reference/restrict lookup
CREATE INDEX ix_t25_fk_approved_by_user_id ON ahdah.legacy_attribution_reviews (company_id, approved_by_user_id);

-- FK support: tenant-safe reference/restrict lookup
CREATE INDEX ix_t25_fk_evidence_id ON ahdah.legacy_attribution_reviews (company_id, evidence_id);

-- FK support: tenant-safe reference/restrict lookup
CREATE INDEX ix_t25_fk_supersedes_review_id ON ahdah.legacy_attribution_reviews (company_id, supersedes_review_id);

-- FK support: tenant-safe reference/restrict lookup
CREATE INDEX ix_t10_cycle_approved_snapshot_id ON ahdah.project_closure_certificates (company_id, project_id, project_settlement_id, approved_snapshot_id);

-- FK support: tenant-safe reference/restrict lookup
CREATE INDEX ix_t10_cycle_closure_snapshot_id ON ahdah.project_closure_certificates (company_id, project_id, project_settlement_id, closure_snapshot_id);

-- Validation functions are SECURITY INVOKER with fixed search_path, no DML.
-- Writers must acquire all project/balance locks in the reviewed global order
-- BEFORE posting. A deferred row trigger cannot know the entire future write set.
-- Each invocation orders old/new balance IDs; noncompliant direct writes may
-- deadlock and abort, never repair amounts. Retry the entire transaction.
CREATE FUNCTION ahdah.settlement_check_custody() RETURNS trigger
LANGUAGE plpgsql SET search_path = pg_catalog, ahdah AS $body$
DECLARE old_j jsonb; new_j jsonb; company uuid; balance_ids uuid[] := '{}';
 position_ids uuid[] := '{}'; bid uuid; b record; av numeric; rs numeric;
BEGIN
 IF TG_OP <> 'INSERT' THEN old_j := to_jsonb(OLD); END IF;
 IF TG_OP <> 'DELETE' THEN new_j := to_jsonb(NEW); END IF;
 company := coalesce((new_j->>'company_id')::uuid,(old_j->>'company_id')::uuid);
 IF TG_TABLE_NAME='custody_reservations' THEN
  position_ids := ARRAY[(old_j->>'project_custody_position_id')::uuid,(new_j->>'project_custody_position_id')::uuid];
  SELECT array_agg(DISTINCT user_advance_balance_id) INTO balance_ids
  FROM ahdah.project_custody_positions WHERE company_id=company AND project_custody_position_id=ANY(position_ids);
 ELSE
  balance_ids := ARRAY[(old_j->>'user_advance_balance_id')::uuid,(new_j->>'user_advance_balance_id')::uuid];
 END IF;
 FOR bid IN SELECT DISTINCT x FROM unnest(balance_ids) x WHERE x IS NOT NULL ORDER BY x LOOP
  SELECT * INTO b FROM ahdah.user_advance_balances
   WHERE company_id=company AND user_advance_balance_id=bid FOR UPDATE;
  IF NOT FOUND THEN
   IF EXISTS(SELECT 1 FROM ahdah.project_custody_positions WHERE company_id=company AND user_advance_balance_id=bid) THEN
    RAISE EXCEPTION 'Custody parent missing' USING ERRCODE='23514';
   END IF;
   CONTINUE;
  END IF;
  SELECT coalesce(sum(available_amount),0),coalesce(sum(reserved_amount),0) INTO av,rs
   FROM ahdah.project_custody_positions WHERE company_id=company AND user_advance_balance_id=bid;
  IF av<>b.available_amount OR rs<>b.reserved_amount THEN
   RAISE EXCEPTION 'Custody current totals disagree with authoritative balance' USING ERRCODE='23514';
  END IF;
  IF (SELECT count(*) FROM ahdah.project_custody_positions WHERE company_id=company AND user_advance_balance_id=bid AND project_id IS NULL) <> 1 THEN
   RAISE EXCEPTION 'Exactly one unassigned custody position required' USING ERRCODE='23514';
  END IF;
  IF EXISTS(SELECT 1 FROM ahdah.project_custody_positions p WHERE p.company_id=company AND p.user_advance_balance_id=bid
    AND p.reserved_amount<>(SELECT coalesce(sum(r.amount),0) FROM ahdah.custody_reservations r
     WHERE r.company_id=p.company_id AND r.project_custody_position_id=p.project_custody_position_id AND r.status='Reserved')) THEN
   RAISE EXCEPTION 'Slice reservation totals disagree' USING ERRCODE='23514';
  END IF;
 END LOOP;
 RETURN NULL;
END $body$;

CREATE FUNCTION ahdah.settlement_fixed_keys() RETURNS trigger
LANGUAGE plpgsql SET search_path = pg_catalog, ahdah AS $body$
DECLARE col text;
BEGIN
 FOREACH col IN ARRAY TG_ARGV LOOP
  IF to_jsonb(OLD)->col IS DISTINCT FROM to_jsonb(NEW)->col THEN
   RAISE EXCEPTION 'Immutable identity/reference: %',col USING ERRCODE='23514';
  END IF;
 END LOOP;
 RETURN NEW;
END $body$;

CREATE FUNCTION ahdah.settlement_freeze_status() RETURNS trigger
LANGUAGE plpgsql SET search_path = pg_catalog, ahdah AS $body$
BEGIN
 IF OLD.status=TG_ARGV[0] THEN
  RAISE EXCEPTION 'Finalized record is immutable; append a successor' USING ERRCODE='23514';
 END IF;
 IF TG_OP='DELETE' THEN RETURN OLD; END IF;
 RETURN NEW;
END $body$;

CREATE FUNCTION ahdah.settlement_child_freeze() RETURNS trigger
LANGUAGE plpgsql SET search_path = pg_catalog, ahdah AS $body$
DECLARE j jsonb; parent_status text; owner_id uuid;
BEGIN
 j := CASE WHEN TG_OP='DELETE' THEN to_jsonb(OLD) ELSE to_jsonb(NEW) END;
 IF TG_TABLE_NAME='custody_movement_entries' THEN
  SELECT status INTO parent_status FROM ahdah.custody_operations WHERE company_id=(j->>'company_id')::uuid AND custody_operation_id=(j->>'custody_operation_id')::uuid FOR UPDATE;
  IF parent_status='Posted' THEN RAISE EXCEPTION 'Posted movement group sealed' USING ERRCODE='23514'; END IF;
 ELSIF TG_TABLE_NAME='legacy_attribution_review_projects' THEN
  SELECT status INTO parent_status FROM ahdah.legacy_attribution_reviews WHERE company_id=(j->>'company_id')::uuid AND legacy_attribution_review_id=(j->>'legacy_attribution_review_id')::uuid FOR UPDATE;
  IF parent_status='Approved' THEN RAISE EXCEPTION 'Approved review sealed' USING ERRCODE='23514'; END IF;
 ELSE
  IF TG_TABLE_NAME='project_owner_reconciliation_positions' THEN owner_id := (j->>'owner_reconciliation_id')::uuid;
  ELSE
   SELECT owner_reconciliation_id INTO owner_id FROM ahdah.project_owner_reconciliation_positions WHERE company_id=(j->>'company_id')::uuid AND project_owner_reconciliation_position_id=(j->>'owner_position_id')::uuid;
  END IF;
  SELECT status INTO parent_status FROM ahdah.project_owner_reconciliations WHERE company_id=(j->>'company_id')::uuid AND project_owner_reconciliation_id=owner_id FOR UPDATE;
  IF parent_status='Approved' THEN RAISE EXCEPTION 'Approved owner schedule sealed' USING ERRCODE='23514'; END IF;
 END IF;
 -- Parent-first construction for these groups. Only snapshots use children-first.
 IF parent_status IS NULL THEN RAISE EXCEPTION 'Construct mutable parent before children' USING ERRCODE='23503'; END IF;
 IF TG_OP='DELETE' THEN RETURN OLD; END IF;
 RETURN NEW;
END $body$;

CREATE FUNCTION ahdah.settlement_owner_approval() RETURNS trigger
LANGUAGE plpgsql SET search_path = pg_catalog, ahdah AS $body$
BEGIN
 IF NEW.status='Approved' THEN
  IF NOT EXISTS(SELECT 1 FROM ahdah.project_owner_reconciliation_positions WHERE company_id=NEW.company_id AND owner_reconciliation_id=NEW.project_owner_reconciliation_id)
   OR EXISTS(SELECT 1 FROM ahdah.project_owner_reconciliation_positions p WHERE company_id=NEW.company_id AND owner_reconciliation_id=NEW.project_owner_reconciliation_id
    AND num_nulls(contract_amount,approved_change_amount,payments_received_amount,refunds_amount,immediate_receivable_amount,immediate_payable_amount,retention_amount,disputed_amount,unclassified_difference_amount)>0) THEN
   RAISE EXCEPTION 'Approved owner schedule requires complete amounts' USING ERRCODE='23514';
  END IF;
 END IF;
 RETURN NEW;
END $body$;

-- Children-first packet: preallocate snapshot UUID; insert T08; insert T07;
-- then bind T06.snapshot_id. All in ONE write subtransaction; FK is deferred.
-- xmin comparison is a construction-time check only, never persisted identity.
-- It prevents a racing transaction from attaching a category to another
-- transaction's newly sealed header even under a stale snapshot. No vacuum
-- dependence: validation happens at insertion commit, not years later.
CREATE FUNCTION ahdah.settlement_category_insert() RETURNS trigger
LANGUAGE plpgsql SET search_path = pg_catalog, ahdah AS $body$
BEGIN
 PERFORM 1 FROM ahdah.project_financial_controls WHERE company_id=NEW.company_id AND project_id=NEW.project_id FOR UPDATE;
 IF NOT FOUND THEN RAISE EXCEPTION 'Project gate missing' USING ERRCODE='23503'; END IF;
 IF EXISTS(SELECT 1 FROM ahdah.project_settlement_snapshots WHERE company_id=NEW.company_id AND project_settlement_snapshot_id=NEW.snapshot_id) THEN
  RAISE EXCEPTION 'Snapshot packet is sealed' USING ERRCODE='23514';
 END IF;
 RETURN NEW;
END $body$;
CREATE FUNCTION ahdah.settlement_packet_check() RETURNS trigger
LANGUAGE plpgsql SET search_path = pg_catalog, ahdah AS $body$
DECLARE sid uuid; cid uuid;
BEGIN
 cid:=NEW.company_id;
 IF TG_TABLE_NAME='project_settlement_snapshots' THEN sid:=NEW.project_settlement_snapshot_id; ELSE sid:=NEW.snapshot_id; END IF;
 IF NOT EXISTS(SELECT 1 FROM ahdah.project_settlement_snapshots s WHERE s.company_id=cid AND s.project_settlement_snapshot_id=sid) THEN
  RAISE EXCEPTION 'Snapshot packet has no header' USING ERRCODE='23503';
 END IF;
 IF NOT EXISTS(SELECT 1 FROM ahdah.project_settlement_snapshot_categories c WHERE c.company_id=cid AND c.snapshot_id=sid)
 OR EXISTS(SELECT 1 FROM ahdah.project_settlement_snapshot_categories c JOIN ahdah.project_settlement_snapshots s
  ON s.company_id=c.company_id AND s.project_settlement_snapshot_id=c.snapshot_id
  WHERE c.company_id=cid AND c.snapshot_id=sid AND c.xmin<>s.xmin) THEN
  RAISE EXCEPTION 'Packet children must be constructed with header in same write subtransaction' USING ERRCODE='23514';
 END IF;
 RETURN NULL;
END $body$;

CREATE FUNCTION ahdah.settlement_control_check() RETURNS trigger
LANGUAGE plpgsql SET search_path = pg_catalog, ahdah AS $body$
DECLARE cid uuid; pid uuid; j jsonb; p record; c record; st record; cert record; snap record;
BEGIN
 j:=CASE WHEN TG_OP='DELETE' THEN to_jsonb(OLD) ELSE to_jsonb(NEW) END;
 cid:=(j->>'company_id')::uuid; pid:=(j->>'project_id')::uuid;
 SELECT * INTO c FROM ahdah.project_financial_controls WHERE company_id=cid AND project_id=pid FOR UPDATE;
 SELECT * INTO p FROM ahdah.projects WHERE company_id=cid AND project_id=pid;
 IF NOT FOUND THEN RETURN NULL; END IF;
 IF c.project_id IS NULL THEN RAISE EXCEPTION 'Project financial control required' USING ERRCODE='23514'; END IF;
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

CREATE FUNCTION ahdah.settlement_advance_currency() RETURNS trigger
LANGUAGE plpgsql SET search_path = pg_catalog, ahdah AS $body$
BEGIN
 IF OLD.currency_code IS DISTINCT FROM NEW.currency_code AND EXISTS(
  SELECT 1 FROM ahdah.user_advance_balances WHERE company_id=OLD.company_id AND advance_id=OLD.advance_id) THEN
  RAISE EXCEPTION 'Advance currency immutable after custody exists' USING ERRCODE='23514';
 END IF;
 RETURN NEW;
END $body$;

-- Trigger attachments. Existing-table enforcement is installed disabled.
CREATE TRIGGER trg_t01_updated_at BEFORE UPDATE ON ahdah.project_financial_controls FOR EACH ROW EXECUTE FUNCTION ahdah.set_updated_at();
CREATE TRIGGER trg_t01_fixed_keys BEFORE UPDATE ON ahdah.project_financial_controls FOR EACH ROW EXECUTE FUNCTION ahdah.settlement_fixed_keys('company_id','project_id');
CREATE TRIGGER trg_t02_updated_at BEFORE UPDATE ON ahdah.project_custody_positions FOR EACH ROW EXECUTE FUNCTION ahdah.set_updated_at();
CREATE TRIGGER trg_t02_fixed_keys BEFORE UPDATE ON ahdah.project_custody_positions FOR EACH ROW EXECUTE FUNCTION ahdah.settlement_fixed_keys('project_custody_position_id','company_id','user_advance_balance_id','project_id');
CREATE TRIGGER trg_t03_updated_at BEFORE UPDATE ON ahdah.custody_operations FOR EACH ROW EXECUTE FUNCTION ahdah.set_updated_at();
CREATE TRIGGER trg_t03_fixed_keys BEFORE UPDATE ON ahdah.custody_operations FOR EACH ROW EXECUTE FUNCTION ahdah.settlement_fixed_keys('custody_operation_id','company_id','legacy_attribution_review_id');
CREATE TRIGGER trg_t04_immutable BEFORE UPDATE OR DELETE ON ahdah.custody_movement_entries FOR EACH ROW EXECUTE FUNCTION ahdah.prevent_immutable_record_change();
CREATE TRIGGER trg_t05_updated_at BEFORE UPDATE ON ahdah.custody_reservations FOR EACH ROW EXECUTE FUNCTION ahdah.set_updated_at();
CREATE TRIGGER trg_t05_fixed_keys BEFORE UPDATE ON ahdah.custody_reservations FOR EACH ROW EXECUTE FUNCTION ahdah.settlement_fixed_keys('custody_reservation_id','company_id','project_custody_position_id');
CREATE TRIGGER trg_t06_updated_at BEFORE UPDATE ON ahdah.project_settlements FOR EACH ROW EXECUTE FUNCTION ahdah.set_updated_at();
CREATE TRIGGER trg_t06_fixed_keys BEFORE UPDATE ON ahdah.project_settlements FOR EACH ROW EXECUTE FUNCTION ahdah.settlement_fixed_keys('project_settlement_id','company_id','project_id');
CREATE TRIGGER trg_t07_immutable BEFORE UPDATE OR DELETE ON ahdah.project_settlement_snapshots FOR EACH ROW EXECUTE FUNCTION ahdah.prevent_immutable_record_change();
CREATE TRIGGER trg_t08_immutable BEFORE UPDATE OR DELETE ON ahdah.project_settlement_snapshot_categories FOR EACH ROW EXECUTE FUNCTION ahdah.prevent_immutable_record_change();
CREATE TRIGGER trg_t09_immutable BEFORE UPDATE OR DELETE ON ahdah.project_settlement_events FOR EACH ROW EXECUTE FUNCTION ahdah.prevent_immutable_record_change();
CREATE TRIGGER trg_t10_immutable BEFORE UPDATE OR DELETE ON ahdah.project_closure_certificates FOR EACH ROW EXECUTE FUNCTION ahdah.prevent_immutable_record_change();
CREATE TRIGGER trg_t11_immutable BEFORE UPDATE OR DELETE ON ahdah.project_reopenings FOR EACH ROW EXECUTE FUNCTION ahdah.prevent_immutable_record_change();
CREATE TRIGGER trg_t12_immutable BEFORE UPDATE OR DELETE ON ahdah.project_authorization_decisions FOR EACH ROW EXECUTE FUNCTION ahdah.prevent_immutable_record_change();
CREATE TRIGGER trg_t13_immutable BEFORE UPDATE OR DELETE ON ahdah.project_operational_events FOR EACH ROW EXECUTE FUNCTION ahdah.prevent_immutable_record_change();
CREATE TRIGGER trg_t14_immutable BEFORE UPDATE OR DELETE ON ahdah.settlement_policy_versions FOR EACH ROW EXECUTE FUNCTION ahdah.prevent_immutable_record_change();
CREATE TRIGGER trg_t15_updated_at BEFORE UPDATE ON ahdah.company_settlement_policy_bindings FOR EACH ROW EXECUTE FUNCTION ahdah.set_updated_at();
CREATE TRIGGER trg_t15_fixed_keys BEFORE UPDATE ON ahdah.company_settlement_policy_bindings FOR EACH ROW EXECUTE FUNCTION ahdah.settlement_fixed_keys('company_id');
CREATE TRIGGER trg_t16_immutable BEFORE UPDATE OR DELETE ON ahdah.document_custody_events FOR EACH ROW EXECUTE FUNCTION ahdah.prevent_immutable_record_change();
CREATE TRIGGER trg_t17_immutable BEFORE UPDATE OR DELETE ON ahdah.document_policy_exceptions FOR EACH ROW EXECUTE FUNCTION ahdah.prevent_immutable_record_change();
CREATE TRIGGER trg_t18_immutable BEFORE UPDATE OR DELETE ON ahdah.verified_evidence_objects FOR EACH ROW EXECUTE FUNCTION ahdah.prevent_immutable_record_change();
CREATE TRIGGER trg_t19_updated_at BEFORE UPDATE ON ahdah.project_owner_reconciliations FOR EACH ROW EXECUTE FUNCTION ahdah.set_updated_at();
CREATE TRIGGER trg_t19_fixed_keys BEFORE UPDATE ON ahdah.project_owner_reconciliations FOR EACH ROW EXECUTE FUNCTION ahdah.settlement_fixed_keys('project_owner_reconciliation_id','company_id','project_id');
CREATE TRIGGER trg_t20_updated_at BEFORE UPDATE ON ahdah.project_owner_reconciliation_positions FOR EACH ROW EXECUTE FUNCTION ahdah.set_updated_at();
CREATE TRIGGER trg_t20_fixed_keys BEFORE UPDATE ON ahdah.project_owner_reconciliation_positions FOR EACH ROW EXECUTE FUNCTION ahdah.settlement_fixed_keys('project_owner_reconciliation_position_id','company_id','project_id','owner_reconciliation_id');
CREATE TRIGGER trg_t21_updated_at BEFORE UPDATE ON ahdah.project_owner_retention_items FOR EACH ROW EXECUTE FUNCTION ahdah.set_updated_at();
CREATE TRIGGER trg_t21_fixed_keys BEFORE UPDATE ON ahdah.project_owner_retention_items FOR EACH ROW EXECUTE FUNCTION ahdah.settlement_fixed_keys('project_owner_retention_item_id','company_id','project_id','owner_position_id');
CREATE TRIGGER trg_t22_immutable BEFORE UPDATE OR DELETE ON ahdah.financial_resolution_effects FOR EACH ROW EXECUTE FUNCTION ahdah.prevent_immutable_record_change();
CREATE TRIGGER trg_t23_updated_at BEFORE UPDATE ON ahdah.supplier_credit_intents FOR EACH ROW EXECUTE FUNCTION ahdah.set_updated_at();
CREATE TRIGGER trg_t23_fixed_keys BEFORE UPDATE ON ahdah.supplier_credit_intents FOR EACH ROW EXECUTE FUNCTION ahdah.settlement_fixed_keys('supplier_credit_intent_id','company_id','project_id');
CREATE TRIGGER trg_t24_immutable BEFORE UPDATE OR DELETE ON ahdah.supplier_credit_intent_applications FOR EACH ROW EXECUTE FUNCTION ahdah.prevent_immutable_record_change();
CREATE TRIGGER trg_t25_updated_at BEFORE UPDATE ON ahdah.legacy_attribution_reviews FOR EACH ROW EXECUTE FUNCTION ahdah.set_updated_at();
CREATE TRIGGER trg_t25_fixed_keys BEFORE UPDATE ON ahdah.legacy_attribution_reviews FOR EACH ROW EXECUTE FUNCTION ahdah.settlement_fixed_keys('legacy_attribution_review_id','company_id');
CREATE TRIGGER trg_t26_updated_at BEFORE UPDATE ON ahdah.legacy_attribution_review_projects FOR EACH ROW EXECUTE FUNCTION ahdah.set_updated_at();
CREATE TRIGGER trg_t26_fixed_keys BEFORE UPDATE ON ahdah.legacy_attribution_review_projects FOR EACH ROW EXECUTE FUNCTION ahdah.settlement_fixed_keys('legacy_attribution_review_project_id','company_id','legacy_attribution_review_id','project_id');
CREATE TRIGGER trg_t03_finalized BEFORE UPDATE OR DELETE ON ahdah.custody_operations FOR EACH ROW EXECUTE FUNCTION ahdah.settlement_freeze_status('Posted');
CREATE TRIGGER trg_t06_finalized BEFORE UPDATE OR DELETE ON ahdah.project_settlements FOR EACH ROW EXECUTE FUNCTION ahdah.settlement_freeze_status('Approved');
CREATE TRIGGER trg_t19_finalized BEFORE UPDATE OR DELETE ON ahdah.project_owner_reconciliations FOR EACH ROW EXECUTE FUNCTION ahdah.settlement_freeze_status('Approved');
CREATE TRIGGER trg_t25_finalized BEFORE UPDATE OR DELETE ON ahdah.legacy_attribution_reviews FOR EACH ROW EXECUTE FUNCTION ahdah.settlement_freeze_status('Approved');
CREATE TRIGGER trg_t04_parent_sealed BEFORE INSERT OR UPDATE OR DELETE ON ahdah.custody_movement_entries FOR EACH ROW EXECUTE FUNCTION ahdah.settlement_child_freeze();
CREATE TRIGGER trg_t26_parent_sealed BEFORE INSERT OR UPDATE OR DELETE ON ahdah.legacy_attribution_review_projects FOR EACH ROW EXECUTE FUNCTION ahdah.settlement_child_freeze();
CREATE TRIGGER trg_t20_parent_sealed BEFORE INSERT OR UPDATE OR DELETE ON ahdah.project_owner_reconciliation_positions FOR EACH ROW EXECUTE FUNCTION ahdah.settlement_child_freeze();
CREATE TRIGGER trg_t21_parent_sealed BEFORE INSERT OR UPDATE OR DELETE ON ahdah.project_owner_retention_items FOR EACH ROW EXECUTE FUNCTION ahdah.settlement_child_freeze();
CREATE TRIGGER trg_t19_complete BEFORE INSERT OR UPDATE ON ahdah.project_owner_reconciliations FOR EACH ROW EXECUTE FUNCTION ahdah.settlement_owner_approval();
CREATE TRIGGER trg_t08_insert BEFORE INSERT ON ahdah.project_settlement_snapshot_categories FOR EACH ROW EXECUTE FUNCTION ahdah.settlement_category_insert();
CREATE CONSTRAINT TRIGGER trg_t07_packet AFTER INSERT ON ahdah.project_settlement_snapshots DEFERRABLE INITIALLY DEFERRED FOR EACH ROW EXECUTE FUNCTION ahdah.settlement_packet_check();
CREATE CONSTRAINT TRIGGER trg_t08_packet AFTER INSERT ON ahdah.project_settlement_snapshot_categories DEFERRABLE INITIALLY DEFERRED FOR EACH ROW EXECUTE FUNCTION ahdah.settlement_packet_check();
CREATE CONSTRAINT TRIGGER trg_settlement_t02_conservation AFTER INSERT OR UPDATE OR DELETE ON ahdah.project_custody_positions DEFERRABLE INITIALLY DEFERRED FOR EACH ROW EXECUTE FUNCTION ahdah.settlement_check_custody();
CREATE CONSTRAINT TRIGGER trg_settlement_t05_conservation AFTER INSERT OR UPDATE OR DELETE ON ahdah.custody_reservations DEFERRABLE INITIALLY DEFERRED FOR EACH ROW EXECUTE FUNCTION ahdah.settlement_check_custody();
CREATE CONSTRAINT TRIGGER trg_settlement_balance_conservation AFTER INSERT OR UPDATE OR DELETE ON ahdah.user_advance_balances DEFERRABLE INITIALLY DEFERRED FOR EACH ROW EXECUTE FUNCTION ahdah.settlement_check_custody();
CREATE CONSTRAINT TRIGGER trg_settlement_t01_control AFTER INSERT OR UPDATE OR DELETE ON ahdah.project_financial_controls DEFERRABLE INITIALLY DEFERRED FOR EACH ROW EXECUTE FUNCTION ahdah.settlement_control_check();
CREATE CONSTRAINT TRIGGER trg_settlement_project_control AFTER INSERT OR UPDATE OR DELETE ON ahdah.projects DEFERRABLE INITIALLY DEFERRED FOR EACH ROW EXECUTE FUNCTION ahdah.settlement_control_check();
CREATE CONSTRAINT TRIGGER trg_settlement_t06_control AFTER INSERT OR UPDATE OR DELETE ON ahdah.project_settlements DEFERRABLE INITIALLY DEFERRED FOR EACH ROW EXECUTE FUNCTION ahdah.settlement_control_check();
CREATE CONSTRAINT TRIGGER trg_settlement_t10_control AFTER INSERT OR UPDATE OR DELETE ON ahdah.project_closure_certificates DEFERRABLE INITIALLY DEFERRED FOR EACH ROW EXECUTE FUNCTION ahdah.settlement_control_check();
CREATE TRIGGER trg_settlement_advance_currency BEFORE UPDATE ON ahdah.advances FOR EACH ROW EXECUTE FUNCTION ahdah.settlement_advance_currency();
ALTER TABLE ahdah.projects DISABLE TRIGGER trg_settlement_project_control;
ALTER TABLE ahdah.user_advance_balances DISABLE TRIGGER trg_settlement_balance_conservation;
ALTER TABLE ahdah.advances DISABLE TRIGGER trg_settlement_advance_currency;

-- Approval locks/cycle authority is application-owned; this checks the stored
-- structural binding and freezes exactly the approved packet.
CREATE FUNCTION ahdah.settlement_approval_packet() RETURNS trigger
LANGUAGE plpgsql SET search_path=pg_catalog,ahdah AS $body$
DECLARE s record;
BEGIN
 IF NEW.status IN ('Submitted','Approved') THEN
  SELECT * INTO s FROM ahdah.project_settlement_snapshots WHERE company_id=NEW.company_id AND project_id=NEW.project_id AND project_settlement_snapshot_id=NEW.snapshot_id;
  IF NOT FOUND OR s.project_settlement_id<>NEW.project_settlement_id OR s.financial_revision<>NEW.captured_financial_revision
   OR s.project_version<>NEW.captured_project_version OR s.policy_version_id<>NEW.policy_version_id OR s.snapshot_hash<>NEW.snapshot_hash THEN
   RAISE EXCEPTION 'Settlement packet binding mismatch' USING ERRCODE='23514';
  END IF;
  IF NEW.status='Approved' AND (s.blocker_count<>0 OR s.mandatory_gap_count<>0) THEN
   RAISE EXCEPTION 'Approved packet cannot contain blockers/gaps' USING ERRCODE='23514';
  END IF;
 END IF;
 RETURN NEW;
END $body$;
CREATE TRIGGER trg_t06_approval_packet BEFORE INSERT OR UPDATE ON ahdah.project_settlements
 FOR EACH ROW EXECUTE FUNCTION ahdah.settlement_approval_packet();

-- Approved Opening/Correction clarification: typed company correction source.
ALTER TABLE ahdah.custody_operations ADD CONSTRAINT fk_t03_advance_settlement_resolution_id FOREIGN KEY (company_id, advance_settlement_resolution_id) REFERENCES ahdah.advance_settlement_resolutions (company_id, advance_settlement_resolution_id) ON UPDATE NO ACTION ON DELETE NO ACTION;
-- FK support: approved company custody correction source
CREATE INDEX ix_t03_resolution_source ON ahdah.custody_operations (company_id, advance_settlement_resolution_id);
-- uniqueness: one posted Opening per confirmed delivery
CREATE UNIQUE INDEX ux_t03_live_opening ON ahdah.custody_operations (company_id, money_transfer_id) WHERE operation_type='Opening' AND status='Posted' AND money_transfer_id IS NOT NULL;
-- uniqueness: one posted Opening per reviewed legacy balance
CREATE UNIQUE INDEX ux_t03_legacy_opening ON ahdah.custody_operations (company_id, legacy_attribution_review_id) WHERE operation_type='Opening' AND status='Posted' AND legacy_attribution_review_id IS NOT NULL;

-- Physical-design clarification, 2026-09-17. Opening establishes ONLY company
-- unassigned custody. Real AdvanceDelivery confirmation or approved Balance
-- legacy review is its authority. InternalTransfer/BalanceReturn receipts remain
-- ConfirmTransfer movements (even when creating a recipient balance), preserving
-- prior designation; they must never be relabeled Opening.
CREATE FUNCTION ahdah.settlement_custody_authority() RETURNS trigger
LANGUAGE plpgsql SET search_path=pg_catalog,ahdah AS $body$
DECLARE opid uuid; cid uuid; op record; e record; b record; delivery record;
 review record; resolution record; auth record; pid uuid; bid uuid; target_balance uuid;
 entry_count integer; project_count integer; amount_available numeric; amount_reserved numeric;
 expected_action text;
BEGIN
 opid:=NEW.custody_operation_id; cid:=NEW.company_id;
 SELECT * INTO op FROM ahdah.custody_operations WHERE company_id=cid AND custody_operation_id=opid;
 IF NOT FOUND OR op.status<>'Posted' THEN RETURN NULL; END IF;
 -- Application already holds the full canonical gate/parent set. These locks
 -- validate the immutable posted group, not repair a writer's earlier lock order.
 FOR pid IN SELECT DISTINCT p.project_id FROM ahdah.custody_movement_entries x JOIN ahdah.project_custody_positions p
  ON p.company_id=x.company_id AND p.project_custody_position_id=x.project_custody_position_id
  WHERE x.company_id=cid AND x.custody_operation_id=opid AND p.project_id IS NOT NULL ORDER BY p.project_id LOOP
  PERFORM 1 FROM ahdah.project_financial_controls WHERE company_id=cid AND project_id=pid AND financial_state<>'FinanciallyClosed' FOR UPDATE;
  IF NOT FOUND THEN RAISE EXCEPTION 'Project gate missing or closed' USING ERRCODE='23514'; END IF;
 END LOOP;
 FOR bid IN SELECT DISTINCT p.user_advance_balance_id FROM ahdah.custody_movement_entries x JOIN ahdah.project_custody_positions p
  ON p.company_id=x.company_id AND p.project_custody_position_id=x.project_custody_position_id
  WHERE x.company_id=cid AND x.custody_operation_id=opid ORDER BY p.user_advance_balance_id LOOP
  PERFORM 1 FROM ahdah.user_advance_balances WHERE company_id=cid AND user_advance_balance_id=bid FOR UPDATE;
 END LOOP;
 SELECT count(*),count(DISTINCT p.project_id),coalesce(sum(x.delta_available_amount),0),coalesce(sum(x.delta_reserved_amount),0)
 INTO entry_count,project_count,amount_available,amount_reserved
 FROM ahdah.custody_movement_entries x JOIN ahdah.project_custody_positions p
 ON p.company_id=x.company_id AND p.project_custody_position_id=x.project_custody_position_id WHERE x.company_id=cid AND x.custody_operation_id=opid;
 IF EXISTS(SELECT 1 FROM ahdah.custody_movement_entries x JOIN ahdah.project_custody_positions p
 ON p.company_id=x.company_id AND p.project_custody_position_id=x.project_custody_position_id
 WHERE x.company_id=cid AND x.custody_operation_id=opid AND (p.project_id IS NULL)<>(x.financial_revision_after IS NULL)) THEN
  RAISE EXCEPTION 'Movement financial revision must match project scope' USING ERRCODE='23514';
 END IF;
 IF op.operation_type='Opening' THEN
  IF project_count<>0 OR entry_count>1 THEN RAISE EXCEPTION 'Opening targets one unassigned balance only' USING ERRCODE='23514'; END IF;
  IF op.money_transfer_id IS NOT NULL THEN
   SELECT * INTO delivery FROM ahdah.money_transfers WHERE company_id=cid AND money_transfer_id=op.money_transfer_id FOR UPDATE;
   IF NOT FOUND OR delivery.transfer_type<>'AdvanceDelivery' OR delivery.status<>'Confirmed' OR delivery.advance_id IS NULL
     OR delivery.confirmed_by_user_id IS DISTINCT FROM delivery.recipient_user_id OR delivery.confirmed_at IS NULL OR entry_count<>1 THEN
    RAISE EXCEPTION 'Live Opening requires real confirmed AdvanceDelivery authority' USING ERRCODE='23514';
   END IF;
   SELECT x.*,p.user_advance_balance_id INTO e FROM ahdah.custody_movement_entries x JOIN ahdah.project_custody_positions p
    ON p.company_id=x.company_id AND p.project_custody_position_id=x.project_custody_position_id WHERE x.company_id=cid AND x.custody_operation_id=opid;
   SELECT * INTO b FROM ahdah.user_advance_balances WHERE company_id=cid AND user_advance_balance_id=e.user_advance_balance_id;
   IF NOT FOUND OR b.advance_id<>delivery.advance_id OR b.user_id<>delivery.recipient_user_id
    OR e.delta_available_amount<>delivery.transfer_amount OR e.delta_reserved_amount<>0 OR e.received_amount<>delivery.transfer_amount
    OR e.restored_amount+e.expensed_amount+e.transferred_out_amount+e.returned_amount+e.adjustment_out_amount<>0
    OR NOT EXISTS(SELECT 1 FROM ahdah.advances a WHERE a.company_id=cid AND a.advance_id=b.advance_id AND a.currency_code=delivery.currency_code AND a.confirmed_by_user_id=delivery.recipient_user_id AND a.confirmed_at IS NOT NULL)
    OR NOT EXISTS(SELECT 1 FROM ahdah.balance_ledger_entries l WHERE l.company_id=cid AND l.balance_ledger_entry_id=e.balance_ledger_entry_id
      AND l.user_advance_balance_id=b.user_advance_balance_id AND l.entry_type='BalanceCreated' AND l.reference_type='Advance'
      AND l.reference_id=delivery.advance_id AND l.delta_received_amount=delivery.transfer_amount AND l.delta_available_amount=delivery.transfer_amount AND l.delta_reserved_amount=0) THEN
    RAISE EXCEPTION 'Live Opening lineage/amount/currency mismatch' USING ERRCODE='23514';
   END IF;
   target_balance:=b.user_advance_balance_id;
  ELSE
   SELECT * INTO review FROM ahdah.legacy_attribution_reviews WHERE company_id=cid AND legacy_attribution_review_id=op.legacy_attribution_review_id;
   IF NOT FOUND OR review.status<>'Approved' OR review.source_type<>'Balance' OR review.evidence_id IS NULL OR review.source_version IS NULL THEN
    RAISE EXCEPTION 'Legacy Opening requires approved evidenced Balance review' USING ERRCODE='23514';
   END IF;
   SELECT * INTO b FROM ahdah.user_advance_balances WHERE company_id=cid AND user_advance_balance_id=review.source_id FOR UPDATE;
   IF NOT FOUND OR b.version_number<>review.source_version OR amount_available<>b.available_amount OR amount_reserved<>b.reserved_amount THEN
    RAISE EXCEPTION 'Legacy Opening must match reviewed boundary balance/version' USING ERRCODE='23514';
   END IF;
   target_balance:=b.user_advance_balance_id;
   IF NOT EXISTS(SELECT 1 FROM ahdah.project_custody_positions p WHERE p.company_id=cid AND p.user_advance_balance_id=target_balance AND p.project_id IS NULL)
    OR EXISTS(SELECT 1 FROM ahdah.custody_movement_entries x JOIN ahdah.project_custody_positions p ON p.company_id=x.company_id AND p.project_custody_position_id=x.project_custody_position_id
      WHERE x.company_id=cid AND x.custody_operation_id=opid AND (p.user_advance_balance_id<>target_balance OR x.delta_available_amount<0 OR x.delta_reserved_amount<0 OR x.received_amount+x.restored_amount+x.expensed_amount+x.transferred_out_amount+x.returned_amount+x.adjustment_out_amount<>0 OR x.balance_ledger_entry_id IS NOT NULL)) THEN
    RAISE EXCEPTION 'Legacy Opening is current unassigned custody, not another receipt/ledger posting' USING ERRCODE='23514';
   END IF;
  END IF;
  -- Parent balance lock serializes this cross-operation uniqueness test, including
  -- zero legacy openings which intentionally have no all-zero movement row.
  IF EXISTS(SELECT 1 FROM ahdah.custody_operations o LEFT JOIN ahdah.legacy_attribution_reviews r
   ON r.company_id=o.company_id AND r.legacy_attribution_review_id=o.legacy_attribution_review_id
   WHERE o.company_id=cid AND o.custody_operation_id<>opid AND o.operation_type='Opening' AND o.status='Posted' AND
   ((r.source_type='Balance' AND r.source_id=target_balance) OR EXISTS(SELECT 1 FROM ahdah.custody_movement_entries x JOIN ahdah.project_custody_positions p
    ON p.company_id=x.company_id AND p.project_custody_position_id=x.project_custody_position_id WHERE x.company_id=o.company_id AND x.custody_operation_id=o.custody_operation_id AND p.user_advance_balance_id=target_balance))) THEN
   RAISE EXCEPTION 'Custody balance already opened' USING ERRCODE='23514';
  END IF;
 ELSIF op.operation_type IN ('Designate','Release','Reallocate','Correction') THEN
  IF entry_count=0 THEN RAISE EXCEPTION 'Posted operation requires movement effects' USING ERRCODE='23514'; END IF;
  IF op.operation_type='Correction' AND (op.source_operation_id=opid OR NOT EXISTS(SELECT 1 FROM ahdah.custody_operations WHERE company_id=cid AND custody_operation_id=op.source_operation_id AND status='Posted')) THEN
   RAISE EXCEPTION 'Correction requires a different original posted operation' USING ERRCODE='23514';
  END IF;
  IF op.operation_type='Correction' AND project_count=0 THEN
   IF op.authorization_decision_id IS NOT NULL THEN RAISE EXCEPTION 'Company-only Correction cannot fabricate project authorization' USING ERRCODE='23514'; END IF;
   IF NOT EXISTS(SELECT 1 FROM ahdah.custody_operations WHERE company_id=cid AND custody_operation_id=op.source_operation_id AND status='Posted') THEN
    RAISE EXCEPTION 'Correction requires original posted operation' USING ERRCODE='23514';
   END IF;
   IF op.legacy_attribution_review_id IS NOT NULL THEN
    SELECT * INTO review FROM ahdah.legacy_attribution_reviews WHERE company_id=cid AND legacy_attribution_review_id=op.legacy_attribution_review_id;
    IF NOT FOUND OR review.status<>'Approved' OR review.source_type<>'Balance' OR review.evidence_id IS NULL THEN RAISE EXCEPTION 'Company Correction requires evidenced approved legacy review' USING ERRCODE='23514'; END IF;
    target_balance:=review.source_id;
   ELSE
    SELECT r.*,s.user_advance_balance_id,s.currency_code INTO resolution FROM ahdah.advance_settlement_resolutions r JOIN ahdah.advance_settlements s ON s.company_id=r.company_id AND s.advance_settlement_id=r.advance_settlement_id
     WHERE r.company_id=cid AND r.advance_settlement_resolution_id=op.advance_settlement_resolution_id;
    IF NOT FOUND OR resolution.status<>'Approved' THEN RAISE EXCEPTION 'Company Correction requires approved advance difference resolution' USING ERRCODE='23514'; END IF;
    target_balance:=resolution.user_advance_balance_id;
    IF NOT EXISTS(SELECT 1 FROM ahdah.user_advance_balances b JOIN ahdah.advances a ON a.company_id=b.company_id AND a.advance_id=b.advance_id WHERE b.company_id=cid AND b.user_advance_balance_id=target_balance AND a.currency_code=resolution.currency_code) THEN
     RAISE EXCEPTION 'Correction currency source mismatch' USING ERRCODE='23514';
    END IF;
   END IF;
   IF EXISTS(SELECT 1 FROM ahdah.custody_movement_entries x JOIN ahdah.project_custody_positions p ON p.company_id=x.company_id AND p.project_custody_position_id=x.project_custody_position_id
    WHERE x.company_id=cid AND x.custody_operation_id=opid AND p.user_advance_balance_id<>target_balance) THEN
    RAISE EXCEPTION 'Company Correction target differs from authoritative source' USING ERRCODE='23514';
   END IF;
  ELSE
   expected_action:=CASE WHEN op.operation_type='Correction' THEN 'CorrectCustody' ELSE op.operation_type END;
   SELECT * INTO auth FROM ahdah.project_authorization_decisions WHERE company_id=cid AND project_authorization_decision_id=op.authorization_decision_id;
   IF NOT FOUND OR auth.action_code<>expected_action OR auth.decision<>'Allowed' OR auth.actor_role<>'Manager'
    OR op.approved_by_user_id IS DISTINCT FROM auth.actor_user_id OR op.approved_at IS NULL OR project_count=0 THEN
    RAISE EXCEPTION 'Project custody operation requires matching Manager action authorization' USING ERRCODE='23514';
   END IF;
   IF NOT EXISTS(SELECT 1 FROM ahdah.custody_movement_entries x JOIN ahdah.project_custody_positions p ON p.company_id=x.company_id AND p.project_custody_position_id=x.project_custody_position_id
    WHERE x.company_id=cid AND x.custody_operation_id=opid AND p.project_id=auth.project_id) THEN
    RAISE EXCEPTION 'Authorization project is not affected' USING ERRCODE='23514';
   END IF;
   FOR pid IN SELECT DISTINCT p.project_id FROM ahdah.custody_movement_entries x JOIN ahdah.project_custody_positions p ON p.company_id=x.company_id AND p.project_custody_position_id=x.project_custody_position_id
    WHERE x.company_id=cid AND x.custody_operation_id=opid AND p.project_id IS NOT NULL LOOP
    IF pid<>auth.project_id AND NOT EXISTS(SELECT 1 FROM ahdah.project_authorization_decisions d WHERE d.company_id=cid AND d.project_id=pid AND d.action_code=expected_action AND d.decision='Allowed' AND d.actor_role='Manager' AND d.actor_user_id=op.approved_by_user_id AND d.request_hash=auth.request_hash AND d.details @> jsonb_build_object('custodyOperationId',opid::text)) THEN
     RAISE EXCEPTION 'Every affected project requires its own authorization decision' USING ERRCODE='23514';
    END IF;
   END LOOP;
  END IF;
  IF op.operation_type IN ('Designate','Release','Reallocate') THEN
   IF EXISTS(SELECT 1 FROM ahdah.custody_movement_entries x JOIN ahdah.project_custody_positions p ON p.company_id=x.company_id AND p.project_custody_position_id=x.project_custody_position_id
    WHERE x.company_id=cid AND x.custody_operation_id=opid GROUP BY p.user_advance_balance_id HAVING sum(x.delta_available_amount)<>0 OR sum(x.delta_reserved_amount)<>0)
    OR EXISTS(SELECT 1 FROM ahdah.custody_movement_entries WHERE company_id=cid AND custody_operation_id=opid AND received_amount+restored_amount+expensed_amount+transferred_out_amount+returned_amount+adjustment_out_amount<>0) THEN
    RAISE EXCEPTION 'Designation disposition must preserve each balance and its gross history' USING ERRCODE='23514';
   END IF;
   IF op.operation_type='Designate' THEN
    IF project_count<>1 OR NOT EXISTS(SELECT 1 FROM ahdah.custody_movement_entries x JOIN ahdah.project_custody_positions p ON p.company_id=x.company_id AND p.project_custody_position_id=x.project_custody_position_id WHERE x.company_id=cid AND x.custody_operation_id=opid AND p.project_id IS NULL)
     OR EXISTS(SELECT 1 FROM ahdah.custody_movement_entries x JOIN ahdah.project_custody_positions p ON p.company_id=x.company_id AND p.project_custody_position_id=x.project_custody_position_id WHERE x.company_id=cid AND x.custody_operation_id=opid AND ((p.project_id IS NULL AND (x.delta_available_amount>0 OR x.delta_reserved_amount>0)) OR (p.project_id IS NOT NULL AND (x.delta_available_amount<0 OR x.delta_reserved_amount<0)))) THEN
     RAISE EXCEPTION 'Designate moves only unassigned custody to one project' USING ERRCODE='23514';
    END IF;
    -- Reserved legacy attribution is a boundary mapping, never reassignment of
    -- an existing live reservation: create final T05 rows in the same cutover TX.
    IF EXISTS(SELECT 1 FROM ahdah.custody_movement_entries WHERE company_id=cid AND custody_operation_id=opid AND delta_reserved_amount<>0) AND NOT EXISTS(
     SELECT 1 FROM ahdah.legacy_attribution_reviews r WHERE r.company_id=cid AND r.legacy_attribution_review_id=op.legacy_attribution_review_id AND r.status='Approved' AND r.source_type='Balance' AND r.evidence_id IS NOT NULL AND r.source_version IS NOT NULL
     AND NOT EXISTS(SELECT 1 FROM ahdah.custody_movement_entries x JOIN ahdah.project_custody_positions p ON p.company_id=x.company_id AND p.project_custody_position_id=x.project_custody_position_id JOIN ahdah.user_advance_balances b ON b.company_id=p.company_id AND b.user_advance_balance_id=p.user_advance_balance_id WHERE x.company_id=cid AND x.custody_operation_id=opid AND (p.user_advance_balance_id<>r.source_id OR b.version_number<>r.source_version))) THEN
     RAISE EXCEPTION 'Reserved designation requires reviewed legacy boundary' USING ERRCODE='23514';
    END IF;
   ELSIF op.operation_type='Release' THEN
    IF project_count<>1 OR EXISTS(SELECT 1 FROM ahdah.custody_movement_entries x JOIN ahdah.project_custody_positions p ON p.company_id=x.company_id AND p.project_custody_position_id=x.project_custody_position_id WHERE x.company_id=cid AND x.custody_operation_id=opid AND (x.delta_reserved_amount<>0 OR (p.project_id IS NULL AND x.delta_available_amount<0) OR (p.project_id IS NOT NULL AND x.delta_available_amount>0))) THEN
     RAISE EXCEPTION 'Release moves only available project custody to unassigned' USING ERRCODE='23514';
    END IF;
   ELSE
    IF project_count<>2 OR EXISTS(SELECT 1 FROM ahdah.custody_movement_entries x JOIN ahdah.project_custody_positions p ON p.company_id=x.company_id AND p.project_custody_position_id=x.project_custody_position_id WHERE x.company_id=cid AND x.custody_operation_id=opid AND (p.project_id IS NULL OR x.delta_reserved_amount<>0)) THEN
     RAISE EXCEPTION 'Reallocate uses two projects, no unassigned/reserved custody' USING ERRCODE='23514';
    END IF;
   END IF;
  END IF;
 END IF;
 RETURN NULL;
END $body$;
CREATE CONSTRAINT TRIGGER trg_t03_custody_authority AFTER INSERT OR UPDATE ON ahdah.custody_operations
 DEFERRABLE INITIALLY DEFERRED FOR EACH ROW EXECUTE FUNCTION ahdah.settlement_custody_authority();
CREATE CONSTRAINT TRIGGER trg_t04_custody_authority AFTER INSERT ON ahdah.custody_movement_entries
 DEFERRABLE INITIALLY DEFERRED FOR EACH ROW EXECUTE FUNCTION ahdah.settlement_custody_authority();
