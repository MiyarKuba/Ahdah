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

-- T02
CREATE TABLE ahdah.project_custody_positions (
    project_custody_position_id uuid NOT NULL DEFAULT gen_random_uuid(),
    company_id uuid NOT NULL,
    user_advance_balance_id uuid NOT NULL,
    project_id uuid,
    available_amount numeric(18,2) NOT NULL DEFAULT 0,
    reserved_amount numeric(18,2) NOT NULL DEFAULT 0,
    version_number integer NOT NULL DEFAULT 1,
    created_at timestamptz NOT NULL DEFAULT CURRENT_TIMESTAMP,
    updated_at timestamptz NOT NULL DEFAULT CURRENT_TIMESTAMP,
    CONSTRAINT pk_t02 PRIMARY KEY (project_custody_position_id),
    CONSTRAINT uq_t02_tenant UNIQUE (company_id, project_custody_position_id),
    CONSTRAINT uq_t02_balance_project UNIQUE NULLS NOT DISTINCT (company_id, user_advance_balance_id, project_id),
    CONSTRAINT ck_t02_available_amount_amount CHECK (available_amount >= 0),
    CONSTRAINT ck_t02_reserved_amount_amount CHECK (reserved_amount >= 0),
    CONSTRAINT ck_t02_version_number_range CHECK (version_number>=1)
);

-- T03
CREATE TABLE ahdah.custody_operations (
    custody_operation_id uuid NOT NULL DEFAULT gen_random_uuid(),
    company_id uuid NOT NULL,
    operation_type varchar(40) NOT NULL,
    status varchar(40) NOT NULL,
    requested_by_user_id uuid NOT NULL,
    requested_at timestamptz NOT NULL,
    approved_by_user_id uuid,
    approved_at timestamptz,
    authorization_decision_id uuid,
    reason varchar(1000) NOT NULL,
    idempotency_record_id uuid NOT NULL,
    source_operation_id uuid,
    money_transfer_id uuid,
    expense_id uuid,
    expense_return_id uuid,
    legacy_attribution_review_id uuid,
    advance_settlement_resolution_id uuid,
    version_number integer NOT NULL DEFAULT 1,
    created_at timestamptz NOT NULL DEFAULT CURRENT_TIMESTAMP,
    updated_at timestamptz NOT NULL DEFAULT CURRENT_TIMESTAMP,
    CONSTRAINT pk_t03 PRIMARY KEY (custody_operation_id),
    CONSTRAINT uq_t03_tenant UNIQUE (company_id, custody_operation_id),
    CONSTRAINT uq_t03_idempotency UNIQUE (company_id, idempotency_record_id, operation_type),
    CONSTRAINT ck_t03_operation_type CHECK (operation_type IN ('Opening','Designate','Release','Reallocate','ReserveExpense','ConsumeExpense','RejectExpense','ReserveTransfer','ConfirmTransfer','RejectTransfer','Restore','Correction')),
    CONSTRAINT ck_t03_status CHECK (status IN ('Draft','PendingApproval','Posted','Rejected','Cancelled')),
    CONSTRAINT ck_t03_origins CHECK ((operation_type NOT IN ('ReserveExpense','ConsumeExpense','RejectExpense') OR expense_id IS NOT NULL) AND (operation_type NOT IN ('ReserveTransfer','ConfirmTransfer','RejectTransfer') OR money_transfer_id IS NOT NULL) AND (operation_type <> 'Restore' OR expense_return_id IS NOT NULL) AND (operation_type <> 'Correction' OR source_operation_id IS NOT NULL)),
    CONSTRAINT ck_t03_posting_approval CHECK (status <> 'Posted' OR operation_type NOT IN ('Designate','Release','Reallocate') OR (approved_by_user_id IS NOT NULL AND approved_at IS NOT NULL AND authorization_decision_id IS NOT NULL)),
    CONSTRAINT ck_t03_opening_source CHECK (operation_type <> 'Opening' OR (authorization_decision_id IS NULL AND approved_by_user_id IS NULL AND approved_at IS NULL AND num_nonnulls(money_transfer_id,legacy_attribution_review_id)=1 AND source_operation_id IS NULL AND expense_id IS NULL AND expense_return_id IS NULL AND advance_settlement_resolution_id IS NULL)),
    CONSTRAINT ck_t03_company_correction CHECK (operation_type <> 'Correction' OR authorization_decision_id IS NOT NULL OR (num_nonnulls(legacy_attribution_review_id,advance_settlement_resolution_id)=1 AND money_transfer_id IS NULL AND expense_id IS NULL AND expense_return_id IS NULL)),
    CONSTRAINT ck_t03_operation_type_text CHECK (operation_type IS NULL OR btrim(operation_type)<>''),
    CONSTRAINT ck_t03_status_text CHECK (status IS NULL OR btrim(status)<>''),
    CONSTRAINT ck_t03_reason_text CHECK (reason IS NULL OR btrim(reason)<>''),
    CONSTRAINT ck_t03_version_number_range CHECK (version_number>=1),
    CONSTRAINT ck_t03_approved_by_user_id_pair CHECK ((approved_by_user_id IS NULL)=(approved_at IS NULL)),
    CONSTRAINT ck_t03_approved_at_order CHECK (approved_at IS NULL OR approved_at>=created_at)
);

-- T04
CREATE TABLE ahdah.custody_movement_entries (
    custody_movement_entry_id uuid NOT NULL DEFAULT gen_random_uuid(),
    company_id uuid NOT NULL,
    custody_operation_id uuid NOT NULL,
    project_custody_position_id uuid NOT NULL,
    entry_number integer NOT NULL,
    delta_available_amount numeric(18,2) NOT NULL,
    delta_reserved_amount numeric(18,2) NOT NULL,
    received_amount numeric(18,2) NOT NULL DEFAULT 0,
    restored_amount numeric(18,2) NOT NULL DEFAULT 0,
    expensed_amount numeric(18,2) NOT NULL DEFAULT 0,
    transferred_out_amount numeric(18,2) NOT NULL DEFAULT 0,
    returned_amount numeric(18,2) NOT NULL DEFAULT 0,
    adjustment_out_amount numeric(18,2) NOT NULL DEFAULT 0,
    balance_ledger_entry_id uuid,
    reverses_entry_id uuid,
    occurred_at timestamptz NOT NULL,
    actor_user_id uuid NOT NULL,
    position_version_before integer NOT NULL,
    position_version_after integer NOT NULL,
    financial_revision_after bigint,
    created_at timestamptz NOT NULL DEFAULT CURRENT_TIMESTAMP,
    CONSTRAINT pk_t04 PRIMARY KEY (custody_movement_entry_id),
    CONSTRAINT uq_t04_tenant UNIQUE (company_id, custody_movement_entry_id),
    CONSTRAINT uq_t04_operation_entry UNIQUE (company_id, custody_operation_id, entry_number),
    CONSTRAINT uq_t04_operation_position UNIQUE (company_id, custody_operation_id, project_custody_position_id),
    CONSTRAINT ck_t04_position_version CHECK (position_version_after = position_version_before + 1),
    CONSTRAINT ck_t04_nonzero CHECK (delta_available_amount <> 0 OR delta_reserved_amount <> 0 OR received_amount + restored_amount + expensed_amount + transferred_out_amount + returned_amount + adjustment_out_amount > 0),
    CONSTRAINT ck_t04_entry_number_range CHECK (entry_number>=1),
    CONSTRAINT ck_t04_received_amount_amount CHECK (received_amount >= 0),
    CONSTRAINT ck_t04_restored_amount_amount CHECK (restored_amount >= 0),
    CONSTRAINT ck_t04_expensed_amount_amount CHECK (expensed_amount >= 0),
    CONSTRAINT ck_t04_transferred_out_amount_amount CHECK (transferred_out_amount >= 0),
    CONSTRAINT ck_t04_returned_amount_amount CHECK (returned_amount >= 0),
    CONSTRAINT ck_t04_adjustment_out_amount_amount CHECK (adjustment_out_amount >= 0),
    CONSTRAINT ck_t04_position_version_before_range CHECK (position_version_before>=1),
    CONSTRAINT ck_t04_position_version_after_range CHECK (position_version_after>=1),
    CONSTRAINT ck_t04_financial_revision_after_range CHECK (financial_revision_after>=1)
);

-- T05
CREATE TABLE ahdah.custody_reservations (
    custody_reservation_id uuid NOT NULL DEFAULT gen_random_uuid(),
    company_id uuid NOT NULL,
    custody_operation_id uuid NOT NULL,
    project_custody_position_id uuid NOT NULL,
    expense_advance_allocation_id uuid,
    transfer_advance_allocation_id uuid,
    amount numeric(18,2) NOT NULL,
    status varchar(40) NOT NULL,
    resolved_by_operation_id uuid,
    version_number integer NOT NULL DEFAULT 1,
    created_at timestamptz NOT NULL DEFAULT CURRENT_TIMESTAMP,
    updated_at timestamptz NOT NULL DEFAULT CURRENT_TIMESTAMP,
    CONSTRAINT pk_t05 PRIMARY KEY (custody_reservation_id),
    CONSTRAINT uq_t05_tenant UNIQUE (company_id, custody_reservation_id),
    CONSTRAINT ck_t05_status CHECK (status IN ('Reserved','Consumed','Released')),
    CONSTRAINT ck_t05_one_allocation CHECK (num_nonnulls(expense_advance_allocation_id,transfer_advance_allocation_id) = 1),
    CONSTRAINT ck_t05_resolution CHECK ((status = 'Reserved') = (resolved_by_operation_id IS NULL)),
    CONSTRAINT ck_t05_amount_amount CHECK (amount >= 0.01),
    CONSTRAINT ck_t05_status_text CHECK (status IS NULL OR btrim(status)<>''),
    CONSTRAINT ck_t05_version_number_range CHECK (version_number>=1)
);
