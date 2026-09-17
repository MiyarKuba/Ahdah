\set ON_ERROR_STOP on
-- PREPARATION ONLY; run in the installation transaction, never independently.
\if :{?settlement_ddl_approved}
\else
\quit 3
\endif
\if :settlement_ddl_approved
\else
\echo 'STOP: settlement_ddl_approved must be true'
\quit 3
\endif
-- Nullable forever until an independently reviewed backfill and NOT NULL proposal.
-- No guessed currency/default; no existing constraint/index is removed.
ALTER TABLE ahdah.projects ADD COLUMN contract_currency_code varchar(3),
 ADD CONSTRAINT ck_settlement_project_currency CHECK (contract_currency_code ~ '^[A-Z]{3}$');
ALTER TABLE ahdah.project_contract_changes ADD COLUMN currency_code varchar(3),
 ADD CONSTRAINT ck_settlement_change_currency CHECK (currency_code ~ '^[A-Z]{3}$');
ALTER TABLE ahdah.owner_payment_refunds ADD COLUMN currency_code varchar(3),
 ADD CONSTRAINT ck_settlement_refund_currency CHECK (currency_code ~ '^[A-Z]{3}$');
-- The other two reviewed alterations are trigger attachments to
-- user_advance_balances and advances in 09. Baseline amounts never change here.
