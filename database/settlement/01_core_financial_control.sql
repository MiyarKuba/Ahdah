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

-- T01
CREATE TABLE ahdah.project_financial_controls (
    company_id uuid NOT NULL,
    project_id uuid NOT NULL,
    financial_revision bigint NOT NULL DEFAULT 1,
    financial_state varchar(40) NOT NULL,
    active_settlement_id uuid,
    active_closure_id uuid,
    last_material_change_at timestamptz NOT NULL,
    next_cycle_number integer NOT NULL DEFAULT 1,
    attribution_status varchar(40) NOT NULL,
    version_number integer NOT NULL DEFAULT 1,
    created_at timestamptz NOT NULL DEFAULT CURRENT_TIMESTAMP,
    updated_at timestamptz NOT NULL DEFAULT CURRENT_TIMESTAMP,
    CONSTRAINT pk_t01 PRIMARY KEY (company_id, project_id),
    CONSTRAINT ck_t01_financial_state CHECK (financial_state IN ('Open','InSettlement','Settled','FinanciallyClosed')),
    CONSTRAINT ck_t01_attribution_status CHECK (attribution_status IN ('PendingReview','Reviewed','Indeterminate')),
    CONSTRAINT ck_t01_closed_pointer CHECK ((financial_state = 'FinanciallyClosed') = (active_closure_id IS NOT NULL)),
    CONSTRAINT ck_t01_cycle_pointer CHECK (financial_state NOT IN ('Settled','FinanciallyClosed') OR active_settlement_id IS NOT NULL),
    CONSTRAINT ck_t01_financial_revision_range CHECK (financial_revision>=1),
    CONSTRAINT ck_t01_financial_state_text CHECK (financial_state IS NULL OR btrim(financial_state)<>''),
    CONSTRAINT ck_t01_next_cycle_number_range CHECK (next_cycle_number>=1),
    CONSTRAINT ck_t01_attribution_status_text CHECK (attribution_status IS NULL OR btrim(attribution_status)<>''),
    CONSTRAINT ck_t01_version_number_range CHECK (version_number>=1)
);
