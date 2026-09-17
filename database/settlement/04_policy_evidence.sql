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

-- T14
CREATE TABLE ahdah.settlement_policy_versions (
    settlement_policy_version_id uuid NOT NULL DEFAULT gen_random_uuid(),
    company_id uuid NOT NULL,
    policy_code varchar(40) NOT NULL,
    major_version integer NOT NULL,
    configuration_version integer NOT NULL,
    policy_text text NOT NULL,
    canonical_rules bytea NOT NULL,
    rules jsonb NOT NULL,
    content_hash bytea NOT NULL,
    approved_by_user_id uuid NOT NULL,
    approved_at timestamptz NOT NULL,
    effective_from timestamptz NOT NULL,
    predecessor_id uuid,
    created_at timestamptz NOT NULL DEFAULT CURRENT_TIMESTAMP,
    CONSTRAINT pk_t14 PRIMARY KEY (settlement_policy_version_id),
    CONSTRAINT uq_t14_tenant UNIQUE (company_id, settlement_policy_version_id),
    CONSTRAINT uq_t14_version UNIQUE (company_id, policy_code, major_version, configuration_version),
    CONSTRAINT ck_t14_bytes CHECK (octet_length(canonical_rules)>0),
    CONSTRAINT ck_t14_policy_text CHECK (btrim(policy_text)<>''),
    CONSTRAINT ck_t14_policy_code_text CHECK (policy_code IS NULL OR btrim(policy_code)<>''),
    CONSTRAINT ck_t14_major_version_range CHECK (major_version>=1),
    CONSTRAINT ck_t14_configuration_version_range CHECK (configuration_version>=1),
    CONSTRAINT ck_t14_rules_json CHECK (jsonb_typeof(rules)='object'),
    CONSTRAINT ck_t14_content_hash_hash CHECK (octet_length(content_hash)=32),
    CONSTRAINT ck_t14_approved_by_user_id_pair CHECK ((approved_by_user_id IS NULL)=(approved_at IS NULL)),
    CONSTRAINT ck_t14_approved_at_order CHECK (approved_at IS NULL OR approved_at>=created_at)
);

-- T15
CREATE TABLE ahdah.company_settlement_policy_bindings (
    company_id uuid NOT NULL,
    active_policy_version_id uuid NOT NULL,
    activated_by_user_id uuid NOT NULL,
    activated_at timestamptz NOT NULL,
    version_number integer NOT NULL DEFAULT 1,
    created_at timestamptz NOT NULL DEFAULT CURRENT_TIMESTAMP,
    updated_at timestamptz NOT NULL DEFAULT CURRENT_TIMESTAMP,
    CONSTRAINT pk_t15 PRIMARY KEY (company_id),
    CONSTRAINT ck_t15_version_number_range CHECK (version_number>=1)
);

-- T16
CREATE TABLE ahdah.document_custody_events (
    document_custody_event_id uuid NOT NULL DEFAULT gen_random_uuid(),
    company_id uuid NOT NULL,
    project_id uuid NOT NULL,
    evidence_id uuid NOT NULL,
    expense_document_id uuid,
    event_type varchar(40) NOT NULL,
    actor_user_id uuid NOT NULL,
    actor_role varchar(40) NOT NULL,
    occurred_at timestamptz NOT NULL,
    reason varchar(1000) NOT NULL,
    policy_version_id uuid NOT NULL,
    category_code varchar(80) NOT NULL,
    previous_event_id uuid,
    authorization_decision_id uuid,
    created_at timestamptz NOT NULL DEFAULT CURRENT_TIMESTAMP,
    CONSTRAINT pk_t16 PRIMARY KEY (document_custody_event_id),
    CONSTRAINT uq_t16_tenant UNIQUE (company_id, document_custody_event_id),
    CONSTRAINT uq_t16_project UNIQUE (company_id, project_id, document_custody_event_id),
    CONSTRAINT ck_t16_event_type CHECK (event_type IN ('OriginalReceived','OriginalTransferred','OriginalReturned','OriginalLost','SupplementalRecorded')),
    CONSTRAINT ck_t16_event_type_text CHECK (event_type IS NULL OR btrim(event_type)<>''),
    CONSTRAINT ck_t16_actor_role_text CHECK (actor_role IS NULL OR btrim(actor_role)<>''),
    CONSTRAINT ck_t16_reason_text CHECK (reason IS NULL OR btrim(reason)<>''),
    CONSTRAINT ck_t16_category_code_text CHECK (category_code IS NULL OR btrim(category_code)<>'')
);

-- T17
CREATE TABLE ahdah.document_policy_exceptions (
    document_policy_exception_id uuid NOT NULL DEFAULT gen_random_uuid(),
    company_id uuid NOT NULL,
    project_id uuid NOT NULL,
    evidence_id uuid NOT NULL,
    expense_document_id uuid,
    category_code varchar(80) NOT NULL,
    requirement_code varchar(40) NOT NULL,
    policy_version_id uuid NOT NULL,
    approved_by_user_id uuid NOT NULL,
    actor_role varchar(40) NOT NULL,
    approved_at timestamptz NOT NULL,
    reason varchar(1000) NOT NULL,
    authorization_decision_id uuid NOT NULL,
    supersedes_exception_id uuid,
    created_at timestamptz NOT NULL DEFAULT CURRENT_TIMESTAMP,
    CONSTRAINT pk_t17 PRIMARY KEY (document_policy_exception_id),
    CONSTRAINT uq_t17_tenant UNIQUE (company_id, document_policy_exception_id),
    CONSTRAINT uq_t17_project UNIQUE (company_id, project_id, document_policy_exception_id),
    CONSTRAINT ck_t17_category_code_text CHECK (category_code IS NULL OR btrim(category_code)<>''),
    CONSTRAINT ck_t17_requirement_code_text CHECK (requirement_code IS NULL OR btrim(requirement_code)<>''),
    CONSTRAINT ck_t17_actor_role_text CHECK (actor_role IS NULL OR btrim(actor_role)<>''),
    CONSTRAINT ck_t17_reason_text CHECK (reason IS NULL OR btrim(reason)<>''),
    CONSTRAINT ck_t17_approved_by_user_id_pair CHECK ((approved_by_user_id IS NULL)=(approved_at IS NULL)),
    CONSTRAINT ck_t17_approved_at_order CHECK (approved_at IS NULL OR approved_at>=created_at)
);

-- T18
CREATE TABLE ahdah.verified_evidence_objects (
    verified_evidence_object_id uuid NOT NULL DEFAULT gen_random_uuid(),
    company_id uuid NOT NULL,
    project_id uuid,
    expense_document_id uuid,
    storage_object_key varchar(500) NOT NULL,
    storage_version varchar(200) NOT NULL,
    sha256_hash bytea NOT NULL,
    media_type varchar(150) NOT NULL,
    byte_length bigint NOT NULL,
    verified_by_user_id uuid NOT NULL,
    verified_at timestamptz NOT NULL,
    verification_method varchar(40) NOT NULL,
    source_evidence_id uuid,
    created_at timestamptz NOT NULL DEFAULT CURRENT_TIMESTAMP,
    CONSTRAINT pk_t18 PRIMARY KEY (verified_evidence_object_id),
    CONSTRAINT uq_t18_tenant UNIQUE (company_id, verified_evidence_object_id),
    CONSTRAINT uq_t18_object UNIQUE NULLS NOT DISTINCT (company_id, storage_object_key, storage_version, project_id),
    CONSTRAINT ck_t18_stable_key CHECK (storage_object_key !~* '(^https?://|[?]|password=|token=|signature=)'),
    CONSTRAINT ck_t18_storage_object_key_text CHECK (storage_object_key IS NULL OR btrim(storage_object_key)<>''),
    CONSTRAINT ck_t18_storage_version_text CHECK (storage_version IS NULL OR btrim(storage_version)<>''),
    CONSTRAINT ck_t18_sha256_hash_hash CHECK (octet_length(sha256_hash)=32),
    CONSTRAINT ck_t18_media_type_text CHECK (media_type IS NULL OR btrim(media_type)<>''),
    CONSTRAINT ck_t18_byte_length_range CHECK (byte_length>=1),
    CONSTRAINT ck_t18_verification_method_text CHECK (verification_method IS NULL OR btrim(verification_method)<>'')
);
