# Index workload review

219 proposed indexes: 90 PK/UNIQUE-backed indexes and 129 standalone indexes. No existing index is recreated.

The reviewed design requires tenant-leading FK support; generation reuses any existing left-prefix PK/unique/nonpartial index before adding one. Nullable-reference indexes permit parent restriction checks without tenant-wide scans. This is substantial write amplification: every insert updates its table PK, tenant key, often project key, and applicable actor/reference/workflow indexes. Evidence/movement/event tables are append-heavy. Benchmark fixture posting and snapshot batches before execution approval; removal of reviewed FK support requires an explicit reviewed design adjustment, not silent omission. No indexes use CONCURRENTLY: new tables are empty in the atomic installation.

| Index | Table | Class / workload | Columns / predicate |
|---|---|---|---|
| pk_t01 | project_financial_controls | uniqueness: identity, tenant/project reference or workflow key | `PRIMARY KEY (company_id, project_id)` |
| pk_t02 | project_custody_positions | uniqueness: identity, tenant/project reference or workflow key | `PRIMARY KEY (project_custody_position_id)` |
| uq_t02_tenant | project_custody_positions | uniqueness: identity, tenant/project reference or workflow key | `UNIQUE (company_id, project_custody_position_id)` |
| uq_t02_balance_project | project_custody_positions | uniqueness: identity, tenant/project reference or workflow key | `UNIQUE NULLS NOT DISTINCT (company_id, user_advance_balance_id, project_id)` |
| pk_t03 | custody_operations | uniqueness: identity, tenant/project reference or workflow key | `PRIMARY KEY (custody_operation_id)` |
| uq_t03_tenant | custody_operations | uniqueness: identity, tenant/project reference or workflow key | `UNIQUE (company_id, custody_operation_id)` |
| uq_t03_idempotency | custody_operations | uniqueness: identity, tenant/project reference or workflow key | `UNIQUE (company_id, idempotency_record_id, operation_type)` |
| pk_t04 | custody_movement_entries | uniqueness: identity, tenant/project reference or workflow key | `PRIMARY KEY (custody_movement_entry_id)` |
| uq_t04_tenant | custody_movement_entries | uniqueness: identity, tenant/project reference or workflow key | `UNIQUE (company_id, custody_movement_entry_id)` |
| uq_t04_operation_entry | custody_movement_entries | uniqueness: identity, tenant/project reference or workflow key | `UNIQUE (company_id, custody_operation_id, entry_number)` |
| uq_t04_operation_position | custody_movement_entries | uniqueness: identity, tenant/project reference or workflow key | `UNIQUE (company_id, custody_operation_id, project_custody_position_id)` |
| pk_t05 | custody_reservations | uniqueness: identity, tenant/project reference or workflow key | `PRIMARY KEY (custody_reservation_id)` |
| uq_t05_tenant | custody_reservations | uniqueness: identity, tenant/project reference or workflow key | `UNIQUE (company_id, custody_reservation_id)` |
| pk_t06 | project_settlements | uniqueness: identity, tenant/project reference or workflow key | `PRIMARY KEY (project_settlement_id)` |
| uq_t06_tenant | project_settlements | uniqueness: identity, tenant/project reference or workflow key | `UNIQUE (company_id, project_settlement_id)` |
| uq_t06_project | project_settlements | uniqueness: identity, tenant/project reference or workflow key | `UNIQUE (company_id, project_id, project_settlement_id)` |
| uq_t06_cycle | project_settlements | uniqueness: identity, tenant/project reference or workflow key | `UNIQUE (company_id, project_id, cycle_number)` |
| pk_t07 | project_settlement_snapshots | uniqueness: identity, tenant/project reference or workflow key | `PRIMARY KEY (project_settlement_snapshot_id)` |
| uq_t07_tenant | project_settlement_snapshots | uniqueness: identity, tenant/project reference or workflow key | `UNIQUE (company_id, project_settlement_snapshot_id)` |
| uq_t07_project | project_settlement_snapshots | uniqueness: identity, tenant/project reference or workflow key | `UNIQUE (company_id, project_id, project_settlement_snapshot_id)` |
| uq_t07_number | project_settlement_snapshots | uniqueness: identity, tenant/project reference or workflow key | `UNIQUE (company_id, project_settlement_id, snapshot_number)` |
| uq_t07_cycle_reference | project_settlement_snapshots | uniqueness: identity, tenant/project reference or workflow key | `UNIQUE (company_id, project_id, project_settlement_id, project_settlement_snapshot_id)` |
| pk_t08 | project_settlement_snapshot_categories | uniqueness: identity, tenant/project reference or workflow key | `PRIMARY KEY (project_settlement_snapshot_category_id)` |
| uq_t08_tenant | project_settlement_snapshot_categories | uniqueness: identity, tenant/project reference or workflow key | `UNIQUE (company_id, project_settlement_snapshot_category_id)` |
| uq_t08_project | project_settlement_snapshot_categories | uniqueness: identity, tenant/project reference or workflow key | `UNIQUE (company_id, project_id, project_settlement_snapshot_category_id)` |
| uq_t08_category_amount | project_settlement_snapshot_categories | uniqueness: identity, tenant/project reference or workflow key | `UNIQUE NULLS NOT DISTINCT (company_id, snapshot_id, category_code, currency_code, amount_meaning)` |
| pk_t09 | project_settlement_events | uniqueness: identity, tenant/project reference or workflow key | `PRIMARY KEY (project_settlement_event_id)` |
| uq_t09_tenant | project_settlement_events | uniqueness: identity, tenant/project reference or workflow key | `UNIQUE (company_id, project_settlement_event_id)` |
| uq_t09_project | project_settlement_events | uniqueness: identity, tenant/project reference or workflow key | `UNIQUE (company_id, project_id, project_settlement_event_id)` |
| uq_t09_number | project_settlement_events | uniqueness: identity, tenant/project reference or workflow key | `UNIQUE (company_id, settlement_id, event_number)` |
| uq_t09_replay | project_settlement_events | uniqueness: identity, tenant/project reference or workflow key | `UNIQUE (company_id, idempotency_record_id, event_type, settlement_id)` |
| pk_t10 | project_closure_certificates | uniqueness: identity, tenant/project reference or workflow key | `PRIMARY KEY (project_closure_certificate_id)` |
| uq_t10_tenant | project_closure_certificates | uniqueness: identity, tenant/project reference or workflow key | `UNIQUE (company_id, project_closure_certificate_id)` |
| uq_t10_project | project_closure_certificates | uniqueness: identity, tenant/project reference or workflow key | `UNIQUE (company_id, project_id, project_closure_certificate_id)` |
| uq_t10_version | project_closure_certificates | uniqueness: identity, tenant/project reference or workflow key | `UNIQUE (company_id, project_id, closure_version)` |
| uq_t10_replay | project_closure_certificates | uniqueness: identity, tenant/project reference or workflow key | `UNIQUE (company_id, idempotency_record_id)` |
| pk_t11 | project_reopenings | uniqueness: identity, tenant/project reference or workflow key | `PRIMARY KEY (project_reopening_id)` |
| uq_t11_tenant | project_reopenings | uniqueness: identity, tenant/project reference or workflow key | `UNIQUE (company_id, project_reopening_id)` |
| uq_t11_project | project_reopenings | uniqueness: identity, tenant/project reference or workflow key | `UNIQUE (company_id, project_id, project_reopening_id)` |
| uq_t11_previous_closure_id | project_reopenings | uniqueness: identity, tenant/project reference or workflow key | `UNIQUE (company_id, previous_closure_id)` |
| uq_t11_new_settlement_id | project_reopenings | uniqueness: identity, tenant/project reference or workflow key | `UNIQUE (company_id, new_settlement_id)` |
| uq_t11_idempotency_record_id | project_reopenings | uniqueness: identity, tenant/project reference or workflow key | `UNIQUE (company_id, idempotency_record_id)` |
| pk_t12 | project_authorization_decisions | uniqueness: identity, tenant/project reference or workflow key | `PRIMARY KEY (project_authorization_decision_id)` |
| uq_t12_tenant | project_authorization_decisions | uniqueness: identity, tenant/project reference or workflow key | `UNIQUE (company_id, project_authorization_decision_id)` |
| uq_t12_project | project_authorization_decisions | uniqueness: identity, tenant/project reference or workflow key | `UNIQUE (company_id, project_id, project_authorization_decision_id)` |
| pk_t13 | project_operational_events | uniqueness: identity, tenant/project reference or workflow key | `PRIMARY KEY (project_operational_event_id)` |
| uq_t13_tenant | project_operational_events | uniqueness: identity, tenant/project reference or workflow key | `UNIQUE (company_id, project_operational_event_id)` |
| uq_t13_project | project_operational_events | uniqueness: identity, tenant/project reference or workflow key | `UNIQUE (company_id, project_id, project_operational_event_id)` |
| uq_t13_replay | project_operational_events | uniqueness: identity, tenant/project reference or workflow key | `UNIQUE (company_id, idempotency_record_id, event_type)` |
| pk_t14 | settlement_policy_versions | uniqueness: identity, tenant/project reference or workflow key | `PRIMARY KEY (settlement_policy_version_id)` |
| uq_t14_tenant | settlement_policy_versions | uniqueness: identity, tenant/project reference or workflow key | `UNIQUE (company_id, settlement_policy_version_id)` |
| uq_t14_version | settlement_policy_versions | uniqueness: identity, tenant/project reference or workflow key | `UNIQUE (company_id, policy_code, major_version, configuration_version)` |
| pk_t15 | company_settlement_policy_bindings | uniqueness: identity, tenant/project reference or workflow key | `PRIMARY KEY (company_id)` |
| pk_t16 | document_custody_events | uniqueness: identity, tenant/project reference or workflow key | `PRIMARY KEY (document_custody_event_id)` |
| uq_t16_tenant | document_custody_events | uniqueness: identity, tenant/project reference or workflow key | `UNIQUE (company_id, document_custody_event_id)` |
| uq_t16_project | document_custody_events | uniqueness: identity, tenant/project reference or workflow key | `UNIQUE (company_id, project_id, document_custody_event_id)` |
| pk_t17 | document_policy_exceptions | uniqueness: identity, tenant/project reference or workflow key | `PRIMARY KEY (document_policy_exception_id)` |
| uq_t17_tenant | document_policy_exceptions | uniqueness: identity, tenant/project reference or workflow key | `UNIQUE (company_id, document_policy_exception_id)` |
| uq_t17_project | document_policy_exceptions | uniqueness: identity, tenant/project reference or workflow key | `UNIQUE (company_id, project_id, document_policy_exception_id)` |
| pk_t18 | verified_evidence_objects | uniqueness: identity, tenant/project reference or workflow key | `PRIMARY KEY (verified_evidence_object_id)` |
| uq_t18_tenant | verified_evidence_objects | uniqueness: identity, tenant/project reference or workflow key | `UNIQUE (company_id, verified_evidence_object_id)` |
| uq_t18_object | verified_evidence_objects | uniqueness: identity, tenant/project reference or workflow key | `UNIQUE NULLS NOT DISTINCT (company_id, storage_object_key, storage_version, project_id)` |
| pk_t19 | project_owner_reconciliations | uniqueness: identity, tenant/project reference or workflow key | `PRIMARY KEY (project_owner_reconciliation_id)` |
| uq_t19_tenant | project_owner_reconciliations | uniqueness: identity, tenant/project reference or workflow key | `UNIQUE (company_id, project_owner_reconciliation_id)` |
| uq_t19_project | project_owner_reconciliations | uniqueness: identity, tenant/project reference or workflow key | `UNIQUE (company_id, project_id, project_owner_reconciliation_id)` |
| uq_t19_number | project_owner_reconciliations | uniqueness: identity, tenant/project reference or workflow key | `UNIQUE (company_id, project_id, reconciliation_number)` |
| pk_t20 | project_owner_reconciliation_positions | uniqueness: identity, tenant/project reference or workflow key | `PRIMARY KEY (project_owner_reconciliation_position_id)` |
| uq_t20_tenant | project_owner_reconciliation_positions | uniqueness: identity, tenant/project reference or workflow key | `UNIQUE (company_id, project_owner_reconciliation_position_id)` |
| uq_t20_project | project_owner_reconciliation_positions | uniqueness: identity, tenant/project reference or workflow key | `UNIQUE (company_id, project_id, project_owner_reconciliation_position_id)` |
| uq_t20_currency | project_owner_reconciliation_positions | uniqueness: identity, tenant/project reference or workflow key | `UNIQUE (company_id, owner_reconciliation_id, currency_code)` |
| pk_t21 | project_owner_retention_items | uniqueness: identity, tenant/project reference or workflow key | `PRIMARY KEY (project_owner_retention_item_id)` |
| uq_t21_tenant | project_owner_retention_items | uniqueness: identity, tenant/project reference or workflow key | `UNIQUE (company_id, project_owner_retention_item_id)` |
| uq_t21_project | project_owner_retention_items | uniqueness: identity, tenant/project reference or workflow key | `UNIQUE (company_id, project_id, project_owner_retention_item_id)` |
| pk_t22 | financial_resolution_effects | uniqueness: identity, tenant/project reference or workflow key | `PRIMARY KEY (financial_resolution_effect_id)` |
| uq_t22_tenant | financial_resolution_effects | uniqueness: identity, tenant/project reference or workflow key | `UNIQUE (company_id, financial_resolution_effect_id)` |
| uq_t22_project | financial_resolution_effects | uniqueness: identity, tenant/project reference or workflow key | `UNIQUE (company_id, project_id, financial_resolution_effect_id)` |
| uq_t22_economic_key | financial_resolution_effects | uniqueness: identity, tenant/project reference or workflow key | `UNIQUE (company_id, economic_effect_key)` |
| pk_t23 | supplier_credit_intents | uniqueness: identity, tenant/project reference or workflow key | `PRIMARY KEY (supplier_credit_intent_id)` |
| uq_t23_tenant | supplier_credit_intents | uniqueness: identity, tenant/project reference or workflow key | `UNIQUE (company_id, supplier_credit_intent_id)` |
| uq_t23_project | supplier_credit_intents | uniqueness: identity, tenant/project reference or workflow key | `UNIQUE NULLS NOT DISTINCT (company_id, supplier_credit_note_id, project_id)` |
| pk_t24 | supplier_credit_intent_applications | uniqueness: identity, tenant/project reference or workflow key | `PRIMARY KEY (supplier_credit_intent_application_id)` |
| uq_t24_tenant | supplier_credit_intent_applications | uniqueness: identity, tenant/project reference or workflow key | `UNIQUE (company_id, supplier_credit_intent_application_id)` |
| uq_t24_application | supplier_credit_intent_applications | uniqueness: identity, tenant/project reference or workflow key | `UNIQUE (company_id, supplier_credit_note_allocation_id, supplier_credit_intent_id)` |
| pk_t25 | legacy_attribution_reviews | uniqueness: identity, tenant/project reference or workflow key | `PRIMARY KEY (legacy_attribution_review_id)` |
| uq_t25_tenant | legacy_attribution_reviews | uniqueness: identity, tenant/project reference or workflow key | `UNIQUE (company_id, legacy_attribution_review_id)` |
| uq_t25_source | legacy_attribution_reviews | uniqueness: identity, tenant/project reference or workflow key | `UNIQUE (company_id, boundary_key, source_type, source_id)` |
| pk_t26 | legacy_attribution_review_projects | uniqueness: identity, tenant/project reference or workflow key | `PRIMARY KEY (legacy_attribution_review_project_id)` |
| uq_t26_tenant | legacy_attribution_review_projects | uniqueness: identity, tenant/project reference or workflow key | `UNIQUE (company_id, legacy_attribution_review_project_id)` |
| uq_t26_project | legacy_attribution_review_projects | uniqueness: identity, tenant/project reference or workflow key | `UNIQUE (company_id, project_id, legacy_attribution_review_project_id)` |
| uq_t26_review_project | legacy_attribution_review_projects | uniqueness: identity, tenant/project reference or workflow key | `UNIQUE (company_id, legacy_attribution_review_id, project_id)` |
| ix_t01_state | project_financial_controls | workflow: company state queue | `company_id, financial_state` |
| ix_t02_project_balance | project_custody_positions | settlement reporting: current custody by project | `company_id, project_id, user_advance_balance_id` |
| ix_t04_position_time | custody_movement_entries | settlement reporting: movement history | `company_id, project_custody_position_id, occurred_at` |
| ux_t05_expense_allocation | custody_reservations | uniqueness: allocation within slice | `company_id, project_custody_position_id, expense_advance_allocation_id WHERE expense_advance_allocation_id IS NOT NULL` |
| ux_t05_transfer_allocation | custody_reservations | uniqueness: allocation within slice | `company_id, project_custody_position_id, transfer_advance_allocation_id WHERE transfer_advance_allocation_id IS NOT NULL` |
| ix_t05_position_status | custody_reservations | workflow: active reservations | `company_id, project_custody_position_id, status` |
| ix_t06_workflow | project_settlements | workflow: project cycles | `company_id, project_id, status` |
| ix_t07_revision | project_settlement_snapshots | settlement reporting: evaluated revisions | `company_id, project_id, financial_revision` |
| ix_t16_evidence_time | document_custody_events | settlement reporting: paper custody chain | `company_id, project_id, evidence_id, occurred_at` |
| ux_t17_supersedes | document_policy_exceptions | uniqueness: one exception successor | `company_id, supersedes_exception_id WHERE supersedes_exception_id IS NOT NULL` |
| ix_t19_workflow | project_owner_reconciliations | workflow: owner review | `company_id, project_id, status` |
| ux_t22_expense_return_id | financial_resolution_effects | uniqueness: effect ordinal per origin | `company_id, expense_return_id, effect_number WHERE expense_return_id IS NOT NULL` |
| ux_t22_advance_settlement_resolution_id | financial_resolution_effects | uniqueness: effect ordinal per origin | `company_id, advance_settlement_resolution_id, effect_number WHERE advance_settlement_resolution_id IS NOT NULL` |
| ux_t22_custody_movement_entry_id | financial_resolution_effects | uniqueness: one economic use of posting target | `company_id, custody_movement_entry_id WHERE custody_movement_entry_id IS NOT NULL` |
| ux_t22_funding_source_ledger_entry_id | financial_resolution_effects | uniqueness: one economic use of posting target | `company_id, funding_source_ledger_entry_id WHERE funding_source_ledger_entry_id IS NOT NULL` |
| ux_t22_personal_claim_adjustment_id | financial_resolution_effects | uniqueness: one economic use of posting target | `company_id, personal_claim_adjustment_id WHERE personal_claim_adjustment_id IS NOT NULL` |
| ux_t22_personal_claim_write_off_id | financial_resolution_effects | uniqueness: one economic use of posting target | `company_id, personal_claim_write_off_id WHERE personal_claim_write_off_id IS NOT NULL` |
| ux_t22_supplier_debt_write_off_id | financial_resolution_effects | uniqueness: one economic use of posting target | `company_id, supplier_debt_write_off_id WHERE supplier_debt_write_off_id IS NOT NULL` |
| ux_t22_supplier_credit_note_id | financial_resolution_effects | uniqueness: one economic use of posting target | `company_id, supplier_credit_note_id WHERE supplier_credit_note_id IS NOT NULL` |
| ux_t24_reversal | supplier_credit_intent_applications | uniqueness: one compensating application | `company_id, reverses_application_id WHERE reverses_application_id IS NOT NULL` |
| ix_t26_impact | legacy_attribution_review_projects | cutover/reconciliation: affected project scope | `company_id, project_id, impact_code` |
| ix_t01_fk_active_settlement_id | project_financial_controls | FK support: tenant-safe reference/restrict lookup | `company_id, project_id, active_settlement_id` |
| ix_t01_fk_active_closure_id | project_financial_controls | FK support: tenant-safe reference/restrict lookup | `company_id, project_id, active_closure_id` |
| ix_t03_fk_requested_by_user_id | custody_operations | FK support: tenant-safe reference/restrict lookup | `company_id, requested_by_user_id` |
| ix_t03_fk_approved_by_user_id | custody_operations | FK support: tenant-safe reference/restrict lookup | `company_id, approved_by_user_id` |
| ix_t03_fk_authorization_decision_id | custody_operations | FK support: tenant-safe reference/restrict lookup | `company_id, authorization_decision_id` |
| ix_t03_fk_source_operation_id | custody_operations | FK support: tenant-safe reference/restrict lookup | `company_id, source_operation_id` |
| ix_t03_fk_money_transfer_id | custody_operations | FK support: tenant-safe reference/restrict lookup | `company_id, money_transfer_id` |
| ix_t03_fk_expense_id | custody_operations | FK support: tenant-safe reference/restrict lookup | `company_id, expense_id` |
| ix_t03_fk_expense_return_id | custody_operations | FK support: tenant-safe reference/restrict lookup | `company_id, expense_return_id` |
| ix_t03_fk_legacy_attribution_review_id | custody_operations | FK support: tenant-safe reference/restrict lookup | `company_id, legacy_attribution_review_id` |
| ix_t04_fk_balance_ledger_entry_id | custody_movement_entries | FK support: tenant-safe reference/restrict lookup | `company_id, balance_ledger_entry_id` |
| ix_t04_fk_reverses_entry_id | custody_movement_entries | FK support: tenant-safe reference/restrict lookup | `company_id, reverses_entry_id` |
| ix_t04_fk_actor_user_id | custody_movement_entries | FK support: tenant-safe reference/restrict lookup | `company_id, actor_user_id` |
| ix_t05_fk_custody_operation_id | custody_reservations | FK support: tenant-safe reference/restrict lookup | `company_id, custody_operation_id` |
| ix_t05_fk_expense_advance_allocation_id | custody_reservations | FK support: tenant-safe reference/restrict lookup | `company_id, expense_advance_allocation_id` |
| ix_t05_fk_transfer_advance_allocation_id | custody_reservations | FK support: tenant-safe reference/restrict lookup | `company_id, transfer_advance_allocation_id` |
| ix_t05_fk_resolved_by_operation_id | custody_reservations | FK support: tenant-safe reference/restrict lookup | `company_id, resolved_by_operation_id` |
| ix_t06_fk_previous_settlement_id | project_settlements | FK support: tenant-safe reference/restrict lookup | `company_id, project_id, previous_settlement_id` |
| ix_t06_fk_policy_version_id | project_settlements | FK support: tenant-safe reference/restrict lookup | `company_id, policy_version_id` |
| ix_t06_fk_prepared_by_user_id | project_settlements | FK support: tenant-safe reference/restrict lookup | `company_id, prepared_by_user_id` |
| ix_t06_fk_submitted_by_user_id | project_settlements | FK support: tenant-safe reference/restrict lookup | `company_id, submitted_by_user_id` |
| ix_t06_fk_approved_by_user_id | project_settlements | FK support: tenant-safe reference/restrict lookup | `company_id, approved_by_user_id` |
| ix_t06_fk_snapshot_id | project_settlements | FK support: tenant-safe reference/restrict lookup | `company_id, project_id, snapshot_id` |
| ix_t07_fk_policy_version_id | project_settlement_snapshots | FK support: tenant-safe reference/restrict lookup | `company_id, policy_version_id` |
| ix_t07_fk_evaluated_by_user_id | project_settlement_snapshots | FK support: tenant-safe reference/restrict lookup | `company_id, evaluated_by_user_id` |
| ix_t08_fk_snapshot_id | project_settlement_snapshot_categories | FK support: tenant-safe reference/restrict lookup | `company_id, project_id, snapshot_id` |
| ix_t09_fk_settlement_id | project_settlement_events | FK support: tenant-safe reference/restrict lookup | `company_id, project_id, settlement_id` |
| ix_t09_fk_actor_user_id | project_settlement_events | FK support: tenant-safe reference/restrict lookup | `company_id, actor_user_id` |
| ix_t09_fk_authorization_decision_id | project_settlement_events | FK support: tenant-safe reference/restrict lookup | `company_id, project_id, authorization_decision_id` |
| ix_t09_fk_snapshot_id | project_settlement_events | FK support: tenant-safe reference/restrict lookup | `company_id, project_id, snapshot_id` |
| ix_t10_fk_project_settlement_id | project_closure_certificates | FK support: tenant-safe reference/restrict lookup | `company_id, project_id, project_settlement_id` |
| ix_t10_fk_approved_snapshot_id | project_closure_certificates | FK support: tenant-safe reference/restrict lookup | `company_id, project_id, approved_snapshot_id` |
| ix_t10_fk_closure_snapshot_id | project_closure_certificates | FK support: tenant-safe reference/restrict lookup | `company_id, project_id, closure_snapshot_id` |
| ix_t10_fk_policy_version_id | project_closure_certificates | FK support: tenant-safe reference/restrict lookup | `company_id, policy_version_id` |
| ix_t10_fk_closed_by_user_id | project_closure_certificates | FK support: tenant-safe reference/restrict lookup | `company_id, closed_by_user_id` |
| ix_t10_fk_authorization_decision_id | project_closure_certificates | FK support: tenant-safe reference/restrict lookup | `company_id, project_id, authorization_decision_id` |
| ix_t11_fk_previous_closure_id | project_reopenings | FK support: tenant-safe reference/restrict lookup | `company_id, project_id, previous_closure_id` |
| ix_t11_fk_new_settlement_id | project_reopenings | FK support: tenant-safe reference/restrict lookup | `company_id, project_id, new_settlement_id` |
| ix_t11_fk_actor_user_id | project_reopenings | FK support: tenant-safe reference/restrict lookup | `company_id, actor_user_id` |
| ix_t11_fk_authorization_decision_id | project_reopenings | FK support: tenant-safe reference/restrict lookup | `company_id, project_id, authorization_decision_id` |
| ix_t11_fk_audit_log_id | project_reopenings | FK support: tenant-safe reference/restrict lookup | `company_id, audit_log_id` |
| ix_t12_fk_actor_user_id | project_authorization_decisions | FK support: tenant-safe reference/restrict lookup | `company_id, actor_user_id` |
| ix_t12_fk_policy_version_id | project_authorization_decisions | FK support: tenant-safe reference/restrict lookup | `company_id, policy_version_id` |
| ix_t12_fk_preparer_user_id | project_authorization_decisions | FK support: tenant-safe reference/restrict lookup | `company_id, preparer_user_id` |
| ix_t12_fk_submitter_user_id | project_authorization_decisions | FK support: tenant-safe reference/restrict lookup | `company_id, submitter_user_id` |
| ix_t12_fk_beneficiary_user_id | project_authorization_decisions | FK support: tenant-safe reference/restrict lookup | `company_id, beneficiary_user_id` |
| ix_t12_fk_evidence_id | project_authorization_decisions | FK support: tenant-safe reference/restrict lookup | `company_id, evidence_id` |
| ix_t13_fk_actor_user_id | project_operational_events | FK support: tenant-safe reference/restrict lookup | `company_id, actor_user_id` |
| ix_t13_fk_evidence_id | project_operational_events | FK support: tenant-safe reference/restrict lookup | `company_id, evidence_id` |
| ix_t13_fk_acknowledgment_event_id | project_operational_events | FK support: tenant-safe reference/restrict lookup | `company_id, project_id, acknowledgment_event_id` |
| ix_t13_fk_authorization_decision_id | project_operational_events | FK support: tenant-safe reference/restrict lookup | `company_id, project_id, authorization_decision_id` |
| ix_t14_fk_approved_by_user_id | settlement_policy_versions | FK support: tenant-safe reference/restrict lookup | `company_id, approved_by_user_id` |
| ix_t14_fk_predecessor_id | settlement_policy_versions | FK support: tenant-safe reference/restrict lookup | `company_id, predecessor_id` |
| ix_t15_fk_active_policy_version_id | company_settlement_policy_bindings | FK support: tenant-safe reference/restrict lookup | `company_id, active_policy_version_id` |
| ix_t15_fk_activated_by_user_id | company_settlement_policy_bindings | FK support: tenant-safe reference/restrict lookup | `company_id, activated_by_user_id` |
| ix_t16_fk_evidence_id | document_custody_events | FK support: tenant-safe reference/restrict lookup | `company_id, evidence_id` |
| ix_t16_fk_expense_document_id | document_custody_events | FK support: tenant-safe reference/restrict lookup | `company_id, expense_document_id` |
| ix_t16_fk_actor_user_id | document_custody_events | FK support: tenant-safe reference/restrict lookup | `company_id, actor_user_id` |
| ix_t16_fk_policy_version_id | document_custody_events | FK support: tenant-safe reference/restrict lookup | `company_id, policy_version_id` |
| ix_t16_fk_previous_event_id | document_custody_events | FK support: tenant-safe reference/restrict lookup | `company_id, project_id, previous_event_id` |
| ix_t16_fk_authorization_decision_id | document_custody_events | FK support: tenant-safe reference/restrict lookup | `company_id, project_id, authorization_decision_id` |
| ix_t17_fk_evidence_id | document_policy_exceptions | FK support: tenant-safe reference/restrict lookup | `company_id, evidence_id` |
| ix_t17_fk_expense_document_id | document_policy_exceptions | FK support: tenant-safe reference/restrict lookup | `company_id, expense_document_id` |
| ix_t17_fk_policy_version_id | document_policy_exceptions | FK support: tenant-safe reference/restrict lookup | `company_id, policy_version_id` |
| ix_t17_fk_approved_by_user_id | document_policy_exceptions | FK support: tenant-safe reference/restrict lookup | `company_id, approved_by_user_id` |
| ix_t17_fk_authorization_decision_id | document_policy_exceptions | FK support: tenant-safe reference/restrict lookup | `company_id, project_id, authorization_decision_id` |
| ix_t17_fk_supersedes_exception_id | document_policy_exceptions | FK support: tenant-safe reference/restrict lookup | `company_id, project_id, supersedes_exception_id` |
| ix_t18_fk_project_id | verified_evidence_objects | FK support: tenant-safe reference/restrict lookup | `company_id, project_id` |
| ix_t18_fk_expense_document_id | verified_evidence_objects | FK support: tenant-safe reference/restrict lookup | `company_id, expense_document_id` |
| ix_t18_fk_verified_by_user_id | verified_evidence_objects | FK support: tenant-safe reference/restrict lookup | `company_id, verified_by_user_id` |
| ix_t18_fk_source_evidence_id | verified_evidence_objects | FK support: tenant-safe reference/restrict lookup | `company_id, source_evidence_id` |
| ix_t19_fk_policy_version_id | project_owner_reconciliations | FK support: tenant-safe reference/restrict lookup | `company_id, policy_version_id` |
| ix_t19_fk_prepared_by_user_id | project_owner_reconciliations | FK support: tenant-safe reference/restrict lookup | `company_id, prepared_by_user_id` |
| ix_t19_fk_approved_by_user_id | project_owner_reconciliations | FK support: tenant-safe reference/restrict lookup | `company_id, approved_by_user_id` |
| ix_t19_fk_evidence_id | project_owner_reconciliations | FK support: tenant-safe reference/restrict lookup | `company_id, evidence_id` |
| ix_t19_fk_supersedes_id | project_owner_reconciliations | FK support: tenant-safe reference/restrict lookup | `company_id, project_id, supersedes_id` |
| ix_t20_fk_owner_reconciliation_id | project_owner_reconciliation_positions | FK support: tenant-safe reference/restrict lookup | `company_id, project_id, owner_reconciliation_id` |
| ix_t21_fk_owner_position_id | project_owner_retention_items | FK support: tenant-safe reference/restrict lookup | `company_id, project_id, owner_position_id` |
| ix_t21_fk_evidence_id | project_owner_retention_items | FK support: tenant-safe reference/restrict lookup | `company_id, evidence_id` |
| ix_t21_fk_approved_by_user_id | project_owner_retention_items | FK support: tenant-safe reference/restrict lookup | `company_id, approved_by_user_id` |
| ix_t22_fk_expense_return_id | financial_resolution_effects | FK support: tenant-safe reference/restrict lookup | `company_id, expense_return_id` |
| ix_t22_fk_advance_settlement_resolution_id | financial_resolution_effects | FK support: tenant-safe reference/restrict lookup | `company_id, advance_settlement_resolution_id` |
| ix_t22_fk_custody_movement_entry_id | financial_resolution_effects | FK support: tenant-safe reference/restrict lookup | `company_id, custody_movement_entry_id` |
| ix_t22_fk_funding_source_ledger_entry_id | financial_resolution_effects | FK support: tenant-safe reference/restrict lookup | `company_id, funding_source_ledger_entry_id` |
| ix_t22_fk_personal_claim_adjustment_id | financial_resolution_effects | FK support: tenant-safe reference/restrict lookup | `company_id, personal_claim_adjustment_id` |
| ix_t22_fk_personal_claim_write_off_id | financial_resolution_effects | FK support: tenant-safe reference/restrict lookup | `company_id, personal_claim_write_off_id` |
| ix_t22_fk_supplier_debt_write_off_id | financial_resolution_effects | FK support: tenant-safe reference/restrict lookup | `company_id, supplier_debt_write_off_id` |
| ix_t22_fk_supplier_credit_note_id | financial_resolution_effects | FK support: tenant-safe reference/restrict lookup | `company_id, supplier_credit_note_id` |
| ix_t22_fk_supplier_refund_id | financial_resolution_effects | FK support: tenant-safe reference/restrict lookup | `company_id, supplier_refund_id` |
| ix_t22_fk_authorization_decision_id | financial_resolution_effects | FK support: tenant-safe reference/restrict lookup | `company_id, project_id, authorization_decision_id` |
| ix_t22_fk_evidence_id | financial_resolution_effects | FK support: tenant-safe reference/restrict lookup | `company_id, evidence_id` |
| ix_t22_fk_reverses_effect_id | financial_resolution_effects | FK support: tenant-safe reference/restrict lookup | `company_id, project_id, reverses_effect_id` |
| ix_t23_fk_project_id | supplier_credit_intents | FK support: tenant-safe reference/restrict lookup | `company_id, project_id` |
| ix_t23_fk_prepared_by_user_id | supplier_credit_intents | FK support: tenant-safe reference/restrict lookup | `company_id, prepared_by_user_id` |
| ix_t23_fk_approved_by_user_id | supplier_credit_intents | FK support: tenant-safe reference/restrict lookup | `company_id, approved_by_user_id` |
| ix_t24_fk_supplier_credit_intent_id | supplier_credit_intent_applications | FK support: tenant-safe reference/restrict lookup | `company_id, supplier_credit_intent_id` |
| ix_t24_fk_applied_by_user_id | supplier_credit_intent_applications | FK support: tenant-safe reference/restrict lookup | `company_id, applied_by_user_id` |
| ix_t24_fk_reverses_application_id | supplier_credit_intent_applications | FK support: tenant-safe reference/restrict lookup | `company_id, reverses_application_id` |
| ix_t25_fk_declared_by_user_id | legacy_attribution_reviews | FK support: tenant-safe reference/restrict lookup | `company_id, declared_by_user_id` |
| ix_t25_fk_reviewed_by_user_id | legacy_attribution_reviews | FK support: tenant-safe reference/restrict lookup | `company_id, reviewed_by_user_id` |
| ix_t25_fk_approved_by_user_id | legacy_attribution_reviews | FK support: tenant-safe reference/restrict lookup | `company_id, approved_by_user_id` |
| ix_t25_fk_evidence_id | legacy_attribution_reviews | FK support: tenant-safe reference/restrict lookup | `company_id, evidence_id` |
| ix_t25_fk_supersedes_review_id | legacy_attribution_reviews | FK support: tenant-safe reference/restrict lookup | `company_id, supersedes_review_id` |
| ix_t10_cycle_approved_snapshot_id | project_closure_certificates | FK support: tenant-safe reference/restrict lookup | `company_id, project_id, project_settlement_id, approved_snapshot_id` |
| ix_t10_cycle_closure_snapshot_id | project_closure_certificates | FK support: tenant-safe reference/restrict lookup | `company_id, project_id, project_settlement_id, closure_snapshot_id` |
| ix_t03_resolution_source | custody_operations | Opening source uniqueness / correction FK support | `company_id, advance_settlement_resolution_id`  |
| ux_t03_live_opening | custody_operations | Opening source uniqueness / correction FK support | `company_id, money_transfer_id` operation_type='Opening' AND status='Posted' AND money_transfer_id IS NOT NULL |
| ux_t03_legacy_opening | custody_operations | Opening source uniqueness / correction FK support | `company_id, legacy_attribution_review_id` operation_type='Opening' AND status='Posted' AND legacy_attribution_review_id IS NOT NULL |
