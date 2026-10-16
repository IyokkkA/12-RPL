using Microsoft.Data.SqlClient;

namespace customer;

internal static class Db
{
    public static string ConnectionString { get; } =
        @"Server=(localdb)\MSSQLLocalDB;Database=BromoAirlines;Trusted_Connection=True;TrustServerCertificate=True;";

    public static SqlConnection GetConnection() => new(ConnectionString);
}
