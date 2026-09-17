\set ON_ERROR_STOP on
-- PREPARATION ARTIFACT ONLY. NOT EXECUTED. Run only through the reviewed installation transaction.
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

-- T22
CREATE TABLE ahdah.financial_resolution_effects (
    financial_resolution_effect_id uuid NOT NULL DEFAULT gen_random_uuid(),
    company_id uuid NOT NULL,
    project_id uuid NOT NULL,
    expense_return_id uuid,
    advance_settlement_resolution_id uuid,
    effect_number integer NOT NULL,
    effect_type varchar(40) NOT NULL,
    currency_code varchar(3) NOT NULL,
    amount numeric(18,2) NOT NULL,
    direction varchar(40) NOT NULL,
    custody_movement_entry_id uuid,
    funding_source_ledger_entry_id uuid,
    personal_claim_adjustment_id uuid,
    personal_claim_write_off_id uuid,
    supplier_debt_write_off_id uuid,
    supplier_credit_note_id uuid,
    supplier_refund_id uuid,
    economic_effect_key uuid NOT NULL,
    polarity varchar(40) NOT NULL,
    authorization_decision_id uuid NOT NULL,
    evidence_id uuid,
    reverses_effect_id uuid,
    posted_at timestamptz NOT NULL,
    created_at timestamptz NOT NULL DEFAULT CURRENT_TIMESTAMP,
    CONSTRAINT pk_t22 PRIMARY KEY (financial_resolution_effect_id),
    CONSTRAINT uq_t22_tenant UNIQUE (company_id, financial_resolution_effect_id),
    CONSTRAINT uq_t22_project UNIQUE (company_id, project_id, financial_resolution_effect_id),
    CONSTRAINT uq_t22_economic_key UNIQUE (company_id, economic_effect_key),
    CONSTRAINT ck_t22_polarity CHECK (polarity IN ('Original','Reversal')),
    CONSTRAINT ck_t22_reversal CHECK ((polarity='Reversal')=(reverses_effect_id IS NOT NULL)),
    CONSTRAINT ck_t22_source CHECK (num_nonnulls(expense_return_id,advance_settlement_resolution_id)=1),
    CONSTRAINT ck_t22_target CHECK (num_nonnulls(custody_movement_entry_id,funding_source_ledger_entry_id,personal_claim_adjustment_id,personal_claim_write_off_id,supplier_debt_write_off_id,supplier_credit_note_id)=1),
    CONSTRAINT ck_t22_matrix CHECK ((effect_type='CustodyRestore' AND direction='IncreaseCustody' AND custody_movement_entry_id IS NOT NULL) OR (effect_type='FundingRestore' AND direction='IncreaseFunding' AND funding_source_ledger_entry_id IS NOT NULL) OR (effect_type='ClaimAdjustment' AND direction='ReduceClaim' AND personal_claim_adjustment_id IS NOT NULL) OR (effect_type='ClaimWriteOff' AND direction='ReduceClaim' AND personal_claim_write_off_id IS NOT NULL) OR (effect_type='DebtWriteOff' AND direction='ReduceDebt' AND supplier_debt_write_off_id IS NOT NULL) OR (effect_type='CreditGranted' AND direction='IncreaseCredit' AND supplier_credit_note_id IS NOT NULL) OR (effect_type='Other' AND direction='ExplicitOther' AND evidence_id IS NOT NULL)),
    CONSTRAINT ck_t22_refund_backing CHECK (supplier_refund_id IS NULL OR (effect_type='FundingRestore' AND funding_source_ledger_entry_id IS NOT NULL)),
    CONSTRAINT ck_t22_effect_number_range CHECK (effect_number>=1),
    CONSTRAINT ck_t22_effect_type_text CHECK (effect_type IS NULL OR btrim(effect_type)<>''),
    CONSTRAINT ck_t22_currency CHECK (currency_code ~ '^[A-Z]{3}$'),
    CONSTRAINT ck_t22_amount_amount CHECK (amount >= 0.01),
    CONSTRAINT ck_t22_direction_text CHECK (direction IS NULL OR btrim(direction)<>''),
    CONSTRAINT ck_t22_polarity_text CHECK (polarity IS NULL OR btrim(polarity)<>'')
);

-- T23
CREATE TABLE ahdah.supplier_credit_intents (
    supplier_credit_intent_id uuid NOT NULL DEFAULT gen_random_uuid(),
    company_id uuid NOT NULL,
    supplier_credit_note_id uuid NOT NULL,
    project_id uuid,
    intent_kind varchar(40) NOT NULL,
    intended_amount numeric(18,2) NOT NULL,
    consumed_amount numeric(18,2) NOT NULL DEFAULT 0,
    released_amount numeric(18,2) NOT NULL DEFAULT 0,
    status varchar(40) NOT NULL,
    prepared_by_user_id uuid NOT NULL,
    approved_by_user_id uuid,
    approved_at timestamptz,
    reason varchar(1000) NOT NULL,
    version_number integer NOT NULL DEFAULT 1,
    created_at timestamptz NOT NULL DEFAULT CURRENT_TIMESTAMP,
    updated_at timestamptz NOT NULL DEFAULT CURRENT_TIMESTAMP,
    CONSTRAINT pk_t23 PRIMARY KEY (supplier_credit_intent_id),
    CONSTRAINT uq_t23_tenant UNIQUE (company_id, supplier_credit_intent_id),
    CONSTRAINT uq_t23_project UNIQUE NULLS NOT DISTINCT (company_id, supplier_credit_note_id, project_id),
    CONSTRAINT ck_t23_intent_kind CHECK (intent_kind IN ('Project','CompanyUnassigned')),
    CONSTRAINT ck_t23_status CHECK (status IN ('Pending','Approved','Exhausted','Cancelled')),
    CONSTRAINT ck_t23_project_kind CHECK ((intent_kind='Project')=(project_id IS NOT NULL)),
    CONSTRAINT ck_t23_budget CHECK (consumed_amount+released_amount<=intended_amount),
    CONSTRAINT ck_t23_approval CHECK (status NOT IN ('Approved','Exhausted') OR approved_by_user_id IS NOT NULL),
    CONSTRAINT ck_t23_intent_kind_text CHECK (intent_kind IS NULL OR btrim(intent_kind)<>''),
    CONSTRAINT ck_t23_intended_amount_amount CHECK (intended_amount >= 0.01),
    CONSTRAINT ck_t23_consumed_amount_amount CHECK (consumed_amount >= 0),
    CONSTRAINT ck_t23_released_amount_amount CHECK (released_amount >= 0),
    CONSTRAINT ck_t23_status_text CHECK (status IS NULL OR btrim(status)<>''),
    CONSTRAINT ck_t23_reason_text CHECK (reason IS NULL OR btrim(reason)<>''),
    CONSTRAINT ck_t23_version_number_range CHECK (version_number>=1),
    CONSTRAINT ck_t23_approved_by_user_id_pair CHECK ((approved_by_user_id IS NULL)=(approved_at IS NULL)),
    CONSTRAINT ck_t23_approved_at_order CHECK (approved_at IS NULL OR approved_at>=created_at)
);

-- T24
CREATE TABLE ahdah.supplier_credit_intent_applications (
    supplier_credit_intent_application_id uuid NOT NULL DEFAULT gen_random_uuid(),
    company_id uuid NOT NULL,
    supplier_credit_intent_id uuid NOT NULL,
    supplier_credit_note_allocation_id uuid NOT NULL,
    amount numeric(18,2) NOT NULL,
    applied_by_user_id uuid NOT NULL,
    applied_at timestamptz NOT NULL,
    reverses_application_id uuid,
    created_at timestamptz NOT NULL DEFAULT CURRENT_TIMESTAMP,
    CONSTRAINT pk_t24 PRIMARY KEY (supplier_credit_intent_application_id),
    CONSTRAINT uq_t24_tenant UNIQUE (company_id, supplier_credit_intent_application_id),
    CONSTRAINT uq_t24_application UNIQUE (company_id, supplier_credit_note_allocation_id, supplier_credit_intent_id),
    CONSTRAINT ck_t24_amount_amount CHECK (amount >= 0.01)
);
