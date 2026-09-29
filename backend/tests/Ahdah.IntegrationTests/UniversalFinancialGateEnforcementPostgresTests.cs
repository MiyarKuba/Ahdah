using Npgsql;

namespace Ahdah.IntegrationTests;

[Collection("PostgreSQL custody serial")]
public sealed class UniversalFinancialGateEnforcementPostgresTests
{
    [Fact]
    public async Task Rehearsal_runtime_is_non_owner_and_has_exact_settlement_writer_privileges()
    {
        var connectionString = Environment.GetEnvironmentVariable("AHDAH_CUSTODY_TEST_CONNECTION");
        var expectedRole = Environment.GetEnvironmentVariable("AHDAH_CUSTODY_TEST_RUNTIME_ROLE");
        if (string.IsNullOrWhiteSpace(connectionString) || string.IsNullOrWhiteSpace(expectedRole)) return;

        await using var connection = new NpgsqlConnection(connectionString);
        await connection.OpenAsync();
        Assert.Equal(expectedRole, await Scalar<string>(connection, "SELECT current_user"));
        Assert.False(await Scalar<bool>(connection,
            "SELECT rolsuper OR rolcreatedb OR rolcreaterole OR rolbypassrls FROM pg_roles WHERE rolname=current_user"));
        Assert.False(await Scalar<bool>(connection,
            "SELECT datdba=(SELECT oid FROM pg_roles WHERE rolname=current_user) FROM pg_database WHERE datname=current_database()"));
        Assert.False(await Scalar<bool>(connection, "SELECT has_schema_privilege(current_user,'ahdah','CREATE')"));

        var expected = new Dictionary<string, string[]>(StringComparer.Ordinal)
        {
            ["project_financial_controls"] = ["INSERT", "SELECT", "UPDATE"],
            ["project_custody_positions"] = ["INSERT", "SELECT", "UPDATE"],
            ["custody_operations"] = ["INSERT", "SELECT", "UPDATE"],
            ["custody_movement_entries"] = ["INSERT", "SELECT"],
            ["custody_reservations"] = ["INSERT", "SELECT", "UPDATE"],
            ["project_authorization_decisions"] = ["INSERT", "SELECT"]
        };
        foreach (var (table, privileges) in expected)
        {
            await using var command = new NpgsqlCommand("""
                SELECT array_agg(privilege_type ORDER BY privilege_type)
                FROM information_schema.role_table_grants
                WHERE grantee=current_user AND table_schema='ahdah' AND table_name=$1
                """, connection);
            command.Parameters.AddWithValue(table);
            Assert.Equal(privileges, (string[])(await command.ExecuteScalarAsync())!);
        }

        await AssertDenied(connectionString, "ALTER TABLE ahdah.projects DISABLE TRIGGER trg_settlement_project_control");
        await AssertDenied(connectionString, "TRUNCATE ahdah.custody_movement_entries");
        await AssertDenied(connectionString, "DELETE FROM ahdah.custody_movement_entries WHERE false");
        await AssertDenied(connectionString, "UPDATE ahdah.custody_movement_entries SET delta_available_amount=delta_available_amount WHERE false");
        await AssertDenied(connectionString, "CREATE TABLE ahdah.runtime_must_not_create(id integer)");
        await AssertDenied(connectionString, "INSERT INTO ahdah.project_settlements DEFAULT VALUES");
    }

    [Fact]
    public async Task Enabled_triggers_reject_direct_protocol_bypasses_atomically()
    {
        var connectionString = Environment.GetEnvironmentVariable("AHDAH_CUSTODY_TEST_CONNECTION");
        if (string.IsNullOrWhiteSpace(connectionString)
            || !string.Equals(Environment.GetEnvironmentVariable("AHDAH_ENFORCEMENT_TEST"), "true",
                StringComparison.OrdinalIgnoreCase)) return;

        await AssertTriggerEnabled(connectionString, "projects", "trg_settlement_project_control");
        await AssertTriggerEnabled(connectionString, "user_advance_balances", "trg_settlement_balance_conservation");
        await AssertTriggerEnabled(connectionString, "advances", "trg_settlement_advance_currency");

        await AssertCommitRejected(connectionString,
            "UPDATE ahdah.projects SET contract_date=contract_date+1 WHERE project_id=(SELECT project_id FROM ahdah.projects LIMIT 1)");
        await AssertCommitRejected(connectionString,
            "UPDATE ahdah.user_advance_balances SET available_amount=available_amount+1 WHERE user_advance_balance_id=(SELECT user_advance_balance_id FROM ahdah.user_advance_balances LIMIT 1)");
        await AssertCommitRejected(connectionString,
            "UPDATE ahdah.advances SET currency_code='USD' WHERE advance_id=(SELECT b.advance_id FROM ahdah.user_advance_balances b LIMIT 1)");
    }

    private static async Task<T> Scalar<T>(NpgsqlConnection connection, string sql)
    {
        await using var command = new NpgsqlCommand(sql, connection);
        return (T)(await command.ExecuteScalarAsync())!;
    }

    private static async Task AssertDenied(string connectionString, string sql)
    {
        await using var connection = new NpgsqlConnection(connectionString);
        await connection.OpenAsync();
        var error = await Assert.ThrowsAsync<PostgresException>(async () =>
        {
            await using var command = new NpgsqlCommand(sql, connection);
            await command.ExecuteNonQueryAsync();
        });
        Assert.Equal(PostgresErrorCodes.InsufficientPrivilege, error.SqlState);
    }

    private static async Task AssertTriggerEnabled(string connectionString, string table, string trigger)
    {
        await using var connection = new NpgsqlConnection(connectionString);
        await connection.OpenAsync();
        await using var command = new NpgsqlCommand("""
            SELECT t.tgenabled='O'
            FROM pg_trigger t JOIN pg_class c ON c.oid=t.tgrelid JOIN pg_namespace n ON n.oid=c.relnamespace
            WHERE n.nspname='ahdah' AND c.relname=$1 AND t.tgname=$2
            """, connection);
        command.Parameters.AddWithValue(table);
        command.Parameters.AddWithValue(trigger);
        Assert.True((bool)(await command.ExecuteScalarAsync())!);
    }

    private static async Task AssertCommitRejected(string connectionString, string sql)
    {
        await using var connection = new NpgsqlConnection(connectionString);
        await connection.OpenAsync();
        await using var transaction = await connection.BeginTransactionAsync();
        PostgresException? rejection = null;
        try
        {
            await using var command = new NpgsqlCommand(sql, connection, transaction);
            Assert.True(await command.ExecuteNonQueryAsync() > 0, "The bypass probe found no fixture row.");
            await transaction.CommitAsync();
        }
        catch (PostgresException error)
        {
            rejection = error;
        }
        Assert.NotNull(rejection);
    }
}
