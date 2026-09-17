\set ON_ERROR_STOP on
\echo 'AUTHORIZED CUTOVER USE ONLY — BUSINESS-ROW RECONCILIATION — NOT RUN DURING PREPARATION'
\if :{?authorized_cutover}
\else
\quit 3
\endif
\if :authorized_cutover
\else
\echo 'STOP: authorized_cutover must be true'
\quit 3
\endif
\if :{?company_id}
\else
\echo 'Provide one explicitly authorized company_id'
\quit 3
\endif
BEGIN ISOLATION LEVEL REPEATABLE READ READ ONLY;
WITH violations(code,n) AS (
 SELECT 'opening_scope_or_source',count(*) FROM ahdah.custody_operations o
 WHERE o.company_id=:'company_id'::uuid AND o.operation_type='Opening' AND o.status='Posted' AND
 (o.authorization_decision_id IS NOT NULL OR num_nonnulls(o.money_transfer_id,o.legacy_attribution_review_id)<>1
 OR EXISTS(SELECT 1 FROM ahdah.custody_movement_entries e JOIN ahdah.project_custody_positions p ON p.company_id=e.company_id AND p.project_custody_position_id=e.project_custody_position_id WHERE e.company_id=o.company_id AND e.custody_operation_id=o.custody_operation_id AND p.project_id IS NOT NULL)
 OR (o.money_transfer_id IS NOT NULL AND NOT EXISTS(SELECT 1 FROM ahdah.money_transfers t WHERE t.company_id=o.company_id AND t.money_transfer_id=o.money_transfer_id AND t.transfer_type='AdvanceDelivery' AND t.status='Confirmed' AND t.confirmed_by_user_id=t.recipient_user_id AND t.confirmed_at IS NOT NULL))
 OR (o.legacy_attribution_review_id IS NOT NULL AND NOT EXISTS(SELECT 1 FROM ahdah.legacy_attribution_reviews r WHERE r.company_id=o.company_id AND r.legacy_attribution_review_id=o.legacy_attribution_review_id AND r.status='Approved' AND r.source_type='Balance' AND r.evidence_id IS NOT NULL AND r.source_version IS NOT NULL)))
 UNION ALL
 SELECT 'control_missing',count(*) FROM ahdah.projects p LEFT JOIN ahdah.project_financial_controls c USING(company_id,project_id) WHERE p.company_id=:'company_id'::uuid AND c.project_id IS NULL
 UNION ALL SELECT 'unassigned_position',count(*) FROM ahdah.user_advance_balances b WHERE b.company_id=:'company_id'::uuid AND (SELECT count(*) FROM ahdah.project_custody_positions p WHERE p.company_id=b.company_id AND p.user_advance_balance_id=b.user_advance_balance_id AND p.project_id IS NULL)<>1
 UNION ALL SELECT 'available_reserved',count(*) FROM ahdah.user_advance_balances b WHERE b.company_id=:'company_id'::uuid AND (b.available_amount<>(SELECT coalesce(sum(p.available_amount),0) FROM ahdah.project_custody_positions p WHERE p.company_id=b.company_id AND p.user_advance_balance_id=b.user_advance_balance_id) OR b.reserved_amount<>(SELECT coalesce(sum(p.reserved_amount),0) FROM ahdah.project_custody_positions p WHERE p.company_id=b.company_id AND p.user_advance_balance_id=b.user_advance_balance_id))
 UNION ALL SELECT 'negative_or_reservation_mismatch',count(*) FROM ahdah.project_custody_positions p WHERE p.company_id=:'company_id'::uuid AND (p.available_amount<0 OR p.reserved_amount<0 OR p.reserved_amount<>(SELECT coalesce(sum(r.amount),0) FROM ahdah.custody_reservations r WHERE r.company_id=p.company_id AND r.project_custody_position_id=p.project_custody_position_id AND r.status='Reserved'))
 UNION ALL SELECT 'closure_authority',count(*) FROM ahdah.project_financial_controls c LEFT JOIN ahdah.project_closure_certificates x ON x.company_id=c.company_id AND x.project_id=c.project_id AND x.project_closure_certificate_id=c.active_closure_id LEFT JOIN ahdah.project_settlements s ON s.company_id=c.company_id AND s.project_id=c.project_id AND s.project_settlement_id=c.active_settlement_id WHERE c.company_id=:'company_id'::uuid AND ((c.financial_state='FinanciallyClosed')<>(c.active_closure_id IS NOT NULL) OR (c.active_closure_id IS NOT NULL AND (x.project_closure_certificate_id IS NULL OR x.financial_revision<>c.financial_revision OR s.status IS DISTINCT FROM 'Approved' OR s.captured_financial_revision<>c.financial_revision OR x.project_settlement_id<>s.project_settlement_id)))
 UNION ALL SELECT 'duplicate_authority',count(*) FROM (SELECT company_id,project_id FROM ahdah.project_financial_controls WHERE company_id=:'company_id'::uuid GROUP BY company_id,project_id HAVING count(*)>1) d
 UNION ALL SELECT 'duplicate_effect',count(*) FROM (SELECT economic_effect_key FROM ahdah.financial_resolution_effects WHERE company_id=:'company_id'::uuid GROUP BY economic_effect_key HAVING count(*)>1) d
 UNION ALL SELECT 'effect_other_requires_review',count(*) FROM ahdah.financial_resolution_effects WHERE company_id=:'company_id'::uuid AND effect_type='Other'
 UNION ALL SELECT 'credit_intent_budget',count(*) FROM ahdah.supplier_credit_intents WHERE company_id=:'company_id'::uuid AND (consumed_amount<0 OR released_amount<0 OR consumed_amount+released_amount>intended_amount)
 UNION ALL SELECT 'credit_note_overintent',count(*) FROM ahdah.supplier_credit_notes n WHERE n.company_id=:'company_id'::uuid AND (SELECT coalesce(sum(i.intended_amount-i.released_amount),0) FROM ahdah.supplier_credit_intents i WHERE i.company_id=n.company_id AND i.supplier_credit_note_id=n.supplier_credit_note_id)>n.credit_note_amount
 UNION ALL SELECT 'credit_application_budget',count(*) FROM ahdah.supplier_credit_note_allocations a WHERE a.company_id=:'company_id'::uuid AND (SELECT coalesce(sum(CASE WHEN i.reverses_application_id IS NULL THEN i.amount ELSE -i.amount END),0) FROM ahdah.supplier_credit_intent_applications i WHERE i.company_id=a.company_id AND i.supplier_credit_note_allocation_id=a.supplier_credit_note_allocation_id)<>a.allocated_amount
 UNION ALL SELECT 'policy_missing',CASE WHEN EXISTS(SELECT 1 FROM ahdah.company_settlement_policy_bindings b JOIN ahdah.settlement_policy_versions p ON p.company_id=b.company_id AND p.settlement_policy_version_id=b.active_policy_version_id WHERE b.company_id=:'company_id'::uuid AND p.effective_from<=CURRENT_TIMESTAMP) THEN 0 ELSE 1 END
 UNION ALL SELECT 'unsealed_snapshot',count(*) FROM ahdah.project_settlement_snapshots s WHERE s.company_id=:'company_id'::uuid AND NOT EXISTS(SELECT 1 FROM ahdah.project_settlement_snapshot_categories c WHERE c.company_id=s.company_id AND c.snapshot_id=s.project_settlement_snapshot_id)
 UNION ALL SELECT 'legacy_not_reviewed',count(*) FROM ahdah.legacy_attribution_reviews WHERE company_id=:'company_id'::uuid AND status<>'Approved'
 UNION ALL SELECT 'project_attribution_incomplete',count(*) FROM ahdah.project_financial_controls WHERE company_id=:'company_id'::uuid AND attribution_status<>'Reviewed'
 UNION ALL SELECT 'cross_tenant_position',count(*) FROM ahdah.project_custody_positions p LEFT JOIN ahdah.user_advance_balances b ON b.company_id=p.company_id AND b.user_advance_balance_id=p.user_advance_balance_id WHERE p.company_id=:'company_id'::uuid AND b.user_advance_balance_id IS NULL
)
SELECT jsonb_object_agg(code,n) AS violation_counts, bool_and(n=0) AS invariants_ok FROM violations \gset
\echo :violation_counts
ROLLBACK;
\if :invariants_ok
\echo 'Structural checks pass; semantic review and opening completeness attestation still required.'
\else
\echo 'STOP: review reported violations; no automatic repair.'
\quit 3
\endif
-- FK metadata in 90 proves all tenant relationships are validated; this script
-- adds one representative orphan query. Ledger/effect budgets and category
-- completeness still require the full approved evaluator. This is not canClose.
