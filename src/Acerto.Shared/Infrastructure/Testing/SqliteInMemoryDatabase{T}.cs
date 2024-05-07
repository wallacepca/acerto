using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace Acerto.Products.Service.Tests
{
    public sealed class SqliteInMemoryDatabase<T> : IDisposable
           where T : DbContext
    {
        private readonly string? _databaseName;
        private SqliteConnection? _connection;

        public SqliteInMemoryDatabase(string? databaseName = null)
        {
            _databaseName = databaseName;

            Options = GetOptions();

            CreateDatabase();
        }

        public DbContextOptions<T> Options { get; set; }

        public T GetContext()
        {
            return (T)Activator.CreateInstance(typeof(T), Options)!;
        }

        public void Dispose()
        {
            _connection?.Close();
        }

        private void CreateDatabase()
        {
            using var context = GetContext();
            context.Database.EnsureCreated();
        }

        private DbContextOptions<T> GetOptions()
        {
            return new DbContextOptionsBuilder<T>()
                .EnableDetailedErrors()
                .EnableSensitiveDataLogging()
                .UseSqlite(GetConnection())
                .UseLazyLoadingProxies()
                .Options;
        }

        private SqliteConnection GetConnection()
        {
            var connectionString = string.IsNullOrWhiteSpace(_databaseName) ? "DataSource=:memory:" : $"DataSource={_databaseName};Mode=Memory;Cache=Shared";

            _connection = new SqliteConnection(connectionString);

            _connection.Open();
            return _connection;
        }
    }
}
