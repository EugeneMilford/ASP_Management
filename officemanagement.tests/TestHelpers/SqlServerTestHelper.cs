using System;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using OfficeManagement.Data;

namespace OfficeManagement.Tests.TestHelpers
{
    public static class SqlServerTestHelper
    {
        public static async Task<(DbContextOptions<OfficeContext> Options, TestDatabase TestDb)> CreateUniqueTestDatabaseAsync(string masterConnectionString)
        {
            var dbName = "OfficeTest_" + Guid.NewGuid().ToString("N");
            var createSql = $@"CREATE DATABASE [{dbName}];";
            using (var conn = new SqlConnection(masterConnectionString))
            {
                await conn.OpenAsync();
                using var cmd = conn.CreateCommand();
                cmd.CommandText = createSql;
                await cmd.ExecuteNonQueryAsync();
            }

            var builder = new SqlConnectionStringBuilder(masterConnectionString)
            {
                InitialCatalog = dbName
            };

            var options = new DbContextOptionsBuilder<OfficeContext>()
                .UseSqlServer(builder.ConnectionString)
                .Options;

            using (var context = new OfficeContext(options))
            {
                // If you use migrations in your project prefer MigrateAsync
                await context.Database.EnsureCreatedAsync();
            }

            var testDb = new TestDatabase(dbName, masterConnectionString);
            return (options, testDb);
        }

        public sealed class TestDatabase : IDisposable
        {
            private readonly string _dbName;
            private readonly string _masterConn;
            private bool _disposed;

            public TestDatabase(string dbName, string masterConnectionString)
            {
                _dbName = dbName;
                _masterConn = masterConnectionString;
            }

            public void Dispose()
            {
                if (_disposed) return;

                using var conn = new SqlConnection(_masterConn);
                conn.Open();
                using var cmd1 = conn.CreateCommand();
                cmd1.CommandText = $@"
                    ALTER DATABASE [{_dbName}] SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
                    DROP DATABASE [{_dbName}];
                    ";
                cmd1.CommandTimeout = 60;
                try
                {
                    cmd1.ExecuteNonQuery();
                }
                catch
                {
                    // best-effort cleanup
                }

                _disposed = true;
            }
        }
    }
}
