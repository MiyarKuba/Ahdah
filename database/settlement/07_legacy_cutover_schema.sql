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

-- T25
CREATE TABLE ahdah.legacy_attribution_reviews (
    legacy_attribution_review_id uuid NOT NULL DEFAULT gen_random_uuid(),
    company_id uuid NOT NULL,
    boundary_key uuid NOT NULL,
    source_type varchar(40) NOT NULL,
    source_id uuid NOT NULL,
    source_version integer,
    status varchar(40) NOT NULL,
    uncertainty_scope varchar(40) NOT NULL,
    declared_by_user_id uuid,
    declared_at timestamptz,
    reviewed_by_user_id uuid,
    reviewed_at timestamptz,
    approved_by_user_id uuid,
    approved_at timestamptz,
    evidence_id uuid,
    declared_details jsonb NOT NULL,
    source_digest bytea NOT NULL,
    reason varchar(1000) NOT NULL,
    supersedes_review_id uuid,
    version_number integer NOT NULL DEFAULT 1,
    created_at timestamptz NOT NULL DEFAULT CURRENT_TIMESTAMP,
    updated_at timestamptz NOT NULL DEFAULT CURRENT_TIMESTAMP,
    CONSTRAINT pk_t25 PRIMARY KEY (legacy_attribution_review_id),
    CONSTRAINT uq_t25_tenant UNIQUE (company_id, legacy_attribution_review_id),
    CONSTRAINT uq_t25_source UNIQUE (company_id, boundary_key, source_type, source_id),
    CONSTRAINT ck_t25_status CHECK (status IN ('Pending','Indeterminate','Approved','Rejected')),
    CONSTRAINT ck_t25_uncertainty_scope CHECK (uncertainty_scope IN ('CompanyOnly','KnownProjects','Unbounded')),
    CONSTRAINT ck_t25_source_type CHECK (source_type IN ('Balance','Transfer','Return','Expense','Claim','SupplierDebt','ManagerContribution','OwnerPosition','Credit','Refund','Contract','ProjectCompleteness')),
    CONSTRAINT ck_t25_approval CHECK (status<>'Approved' OR (approved_by_user_id IS NOT NULL AND reviewed_by_user_id IS NOT NULL AND evidence_id IS NOT NULL AND approved_by_user_id<>reviewed_by_user_id)),
    CONSTRAINT ck_t25_source_type_text CHECK (source_type IS NULL OR btrim(source_type)<>''),
    CONSTRAINT ck_t25_source_version_range CHECK (source_version>=1),
    CONSTRAINT ck_t25_status_text CHECK (status IS NULL OR btrim(status)<>''),
    CONSTRAINT ck_t25_uncertainty_scope_text CHECK (uncertainty_scope IS NULL OR btrim(uncertainty_scope)<>''),
    CONSTRAINT ck_t25_declared_details_json CHECK (jsonb_typeof(declared_details)='object'),
    CONSTRAINT ck_t25_source_digest_hash CHECK (octet_length(source_digest)=32),
    CONSTRAINT ck_t25_reason_text CHECK (reason IS NULL OR btrim(reason)<>''),
    CONSTRAINT ck_t25_version_number_range CHECK (version_number>=1),
    CONSTRAINT ck_t25_approved_by_user_id_pair CHECK ((approved_by_user_id IS NULL)=(approved_at IS NULL)),
    CONSTRAINT ck_t25_approved_at_order CHECK (approved_at IS NULL OR approved_at>=created_at),
    CONSTRAINT ck_t25_declared_by_user_id_pair CHECK ((declared_by_user_id IS NULL)=(declared_at IS NULL)),
    CONSTRAINT ck_t25_declared_at_order CHECK (declared_at IS NULL OR declared_at>=created_at),
    CONSTRAINT ck_t25_reviewed_by_user_id_pair CHECK ((reviewed_by_user_id IS NULL)=(reviewed_at IS NULL)),
    CONSTRAINT ck_t25_reviewed_at_order CHECK (reviewed_at IS NULL OR reviewed_at>=created_at)
);

-- T26
CREATE TABLE ahdah.legacy_attribution_review_projects (
    legacy_attribution_review_project_id uuid NOT NULL DEFAULT gen_random_uuid(),
    company_id uuid NOT NULL,
    legacy_attribution_review_id uuid NOT NULL,
    project_id uuid NOT NULL,
    impact_code varchar(40) NOT NULL,
    version_number integer NOT NULL DEFAULT 1,
    created_at timestamptz NOT NULL DEFAULT CURRENT_TIMESTAMP,
    updated_at timestamptz NOT NULL DEFAULT CURRENT_TIMESTAMP,
    CONSTRAINT pk_t26 PRIMARY KEY (legacy_attribution_review_project_id),
    CONSTRAINT uq_t26_tenant UNIQUE (company_id, legacy_attribution_review_project_id),
    CONSTRAINT uq_t26_project UNIQUE (company_id, project_id, legacy_attribution_review_project_id),
    CONSTRAINT uq_t26_review_project UNIQUE (company_id, legacy_attribution_review_id, project_id),
    CONSTRAINT ck_t26_impact_code CHECK (impact_code IN ('PossibleAttribution','ConfirmedAttribution','ReviewedUnrelated')),
    CONSTRAINT ck_t26_impact_code_text CHECK (impact_code IS NULL OR btrim(impact_code)<>''),
    CONSTRAINT ck_t26_version_number_range CHECK (version_number>=1)
);
