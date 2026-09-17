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

-- T19
CREATE TABLE ahdah.project_owner_reconciliations (
    project_owner_reconciliation_id uuid NOT NULL DEFAULT gen_random_uuid(),
    company_id uuid NOT NULL,
    project_id uuid NOT NULL,
    reconciliation_number integer NOT NULL,
    financial_revision bigint NOT NULL,
    policy_version_id uuid NOT NULL,
    status varchar(40) NOT NULL,
    prepared_by_user_id uuid NOT NULL,
    prepared_at timestamptz NOT NULL,
    approved_by_user_id uuid,
    approved_at timestamptz,
    evidence_id uuid NOT NULL,
    supersedes_id uuid,
    version_number integer NOT NULL DEFAULT 1,
    created_at timestamptz NOT NULL DEFAULT CURRENT_TIMESTAMP,
    updated_at timestamptz NOT NULL DEFAULT CURRENT_TIMESTAMP,
    CONSTRAINT pk_t19 PRIMARY KEY (project_owner_reconciliation_id),
    CONSTRAINT uq_t19_tenant UNIQUE (company_id, project_owner_reconciliation_id),
    CONSTRAINT uq_t19_project UNIQUE (company_id, project_id, project_owner_reconciliation_id),
    CONSTRAINT uq_t19_number UNIQUE (company_id, project_id, reconciliation_number),
    CONSTRAINT ck_t19_status CHECK (status IN ('Draft','Submitted','Approved','Rejected')),
    CONSTRAINT ck_t19_approval CHECK (status<>'Approved' OR (approved_by_user_id IS NOT NULL AND approved_by_user_id<>prepared_by_user_id)),
    CONSTRAINT ck_t19_reconciliation_number_range CHECK (reconciliation_number>=1),
    CONSTRAINT ck_t19_financial_revision_range CHECK (financial_revision>=1),
    CONSTRAINT ck_t19_status_text CHECK (status IS NULL OR btrim(status)<>''),
    CONSTRAINT ck_t19_version_number_range CHECK (version_number>=1),
    CONSTRAINT ck_t19_approved_by_user_id_pair CHECK ((approved_by_user_id IS NULL)=(approved_at IS NULL)),
    CONSTRAINT ck_t19_approved_at_order CHECK (approved_at IS NULL OR approved_at>=created_at)
);

-- T20
CREATE TABLE ahdah.project_owner_reconciliation_positions (
    project_owner_reconciliation_position_id uuid NOT NULL DEFAULT gen_random_uuid(),
    company_id uuid NOT NULL,
    project_id uuid NOT NULL,
    owner_reconciliation_id uuid NOT NULL,
    currency_code varchar(3) NOT NULL,
    contract_amount numeric(18,2),
    approved_change_amount numeric(18,2),
    payments_received_amount numeric(18,2),
    refunds_amount numeric(18,2),
    immediate_receivable_amount numeric(18,2),
    immediate_payable_amount numeric(18,2),
    retention_amount numeric(18,2),
    disputed_amount numeric(18,2),
    unclassified_difference_amount numeric(18,2),
    classification_status varchar(40) NOT NULL,
    source_details jsonb NOT NULL,
    version_number integer NOT NULL DEFAULT 1,
    created_at timestamptz NOT NULL DEFAULT CURRENT_TIMESTAMP,
    updated_at timestamptz NOT NULL DEFAULT CURRENT_TIMESTAMP,
    CONSTRAINT pk_t20 PRIMARY KEY (project_owner_reconciliation_position_id),
    CONSTRAINT uq_t20_tenant UNIQUE (company_id, project_owner_reconciliation_position_id),
    CONSTRAINT uq_t20_project UNIQUE (company_id, project_id, project_owner_reconciliation_position_id),
    CONSTRAINT uq_t20_currency UNIQUE (company_id, owner_reconciliation_id, currency_code),
    CONSTRAINT ck_t20_classification_status CHECK (classification_status IN ('Known','Disputed','Unclassified','Unknown')),
    CONSTRAINT ck_t20_equation CHECK (contract_amount + approved_change_amount - payments_received_amount + refunds_amount = immediate_receivable_amount - immediate_payable_amount + retention_amount + disputed_amount + unclassified_difference_amount),
    CONSTRAINT ck_t20_currency CHECK (currency_code ~ '^[A-Z]{3}$'),
    CONSTRAINT ck_t20_contract_amount_amount CHECK (contract_amount >= 0),
    CONSTRAINT ck_t20_payments_received_amount_amount CHECK (payments_received_amount >= 0),
    CONSTRAINT ck_t20_refunds_amount_amount CHECK (refunds_amount >= 0),
    CONSTRAINT ck_t20_immediate_receivable_amount_amount CHECK (immediate_receivable_amount >= 0),
    CONSTRAINT ck_t20_immediate_payable_amount_amount CHECK (immediate_payable_amount >= 0),
    CONSTRAINT ck_t20_retention_amount_amount CHECK (retention_amount >= 0),
    CONSTRAINT ck_t20_classification_status_text CHECK (classification_status IS NULL OR btrim(classification_status)<>''),
    CONSTRAINT ck_t20_source_details_json CHECK (jsonb_typeof(source_details)='array'),
    CONSTRAINT ck_t20_version_number_range CHECK (version_number>=1)
);

-- T21
CREATE TABLE ahdah.project_owner_retention_items (
    project_owner_retention_item_id uuid NOT NULL DEFAULT gen_random_uuid(),
    company_id uuid NOT NULL,
    project_id uuid NOT NULL,
    owner_position_id uuid NOT NULL,
    amount numeric(18,2) NOT NULL,
    percentage numeric(9,6),
    percentage_base_amount numeric(18,2),
    contractual_basis varchar(1000) NOT NULL,
    release_condition varchar(1000) NOT NULL,
    due_date date,
    entitlement_status varchar(40) NOT NULL,
    classification varchar(40) NOT NULL,
    evidence_id uuid NOT NULL,
    approved_by_user_id uuid NOT NULL,
    approved_at timestamptz NOT NULL,
    version_number integer NOT NULL DEFAULT 1,
    created_at timestamptz NOT NULL DEFAULT CURRENT_TIMESTAMP,
    updated_at timestamptz NOT NULL DEFAULT CURRENT_TIMESTAMP,
    CONSTRAINT pk_t21 PRIMARY KEY (project_owner_retention_item_id),
    CONSTRAINT uq_t21_tenant UNIQUE (company_id, project_owner_retention_item_id),
    CONSTRAINT uq_t21_project UNIQUE (company_id, project_id, project_owner_retention_item_id),
    CONSTRAINT ck_t21_entitlement_status CHECK (entitlement_status IN ('Undisputed','Disputed','Unknown')),
    CONSTRAINT ck_t21_classification CHECK (classification IN ('OutsideImmediateSettlement','PendingReview')),
    CONSTRAINT ck_t21_percentage CHECK ((percentage IS NULL) = (percentage_base_amount IS NULL) AND (percentage IS NULL OR percentage BETWEEN 0 AND 100)),
    CONSTRAINT ck_t21_qualification CHECK (classification<>'OutsideImmediateSettlement' OR entitlement_status='Undisputed'),
    CONSTRAINT ck_t21_amount_amount CHECK (amount >= 0.01),
    CONSTRAINT ck_t21_percentage_base_amount_amount CHECK (percentage_base_amount >= 0),
    CONSTRAINT ck_t21_contractual_basis_text CHECK (contractual_basis IS NULL OR btrim(contractual_basis)<>''),
    CONSTRAINT ck_t21_release_condition_text CHECK (release_condition IS NULL OR btrim(release_condition)<>''),
    CONSTRAINT ck_t21_entitlement_status_text CHECK (entitlement_status IS NULL OR btrim(entitlement_status)<>''),
    CONSTRAINT ck_t21_classification_text CHECK (classification IS NULL OR btrim(classification)<>''),
    CONSTRAINT ck_t21_version_number_range CHECK (version_number>=1),
    CONSTRAINT ck_t21_approved_by_user_id_pair CHECK ((approved_by_user_id IS NULL)=(approved_at IS NULL)),
    CONSTRAINT ck_t21_approved_at_order CHECK (approved_at IS NULL OR approved_at>=created_at)
);
