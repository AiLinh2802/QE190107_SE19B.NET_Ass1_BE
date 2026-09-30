using Npgsql;

namespace TaskTrack.API.Infrastructure;

public static class DatabaseUrlParser
{
    public static string Normalize(string connection)
    {
        if (!connection.StartsWith("postgres://", StringComparison.OrdinalIgnoreCase) &&
            !connection.StartsWith("postgresql://", StringComparison.OrdinalIgnoreCase))
            return connection;

        var uri = new Uri(connection);
        var credentials = uri.UserInfo.Split(':', 2);
        if (credentials.Length != 2) throw new InvalidOperationException("DATABASE_URL must include a username and password.");

        return new NpgsqlConnectionStringBuilder
        {
            Host = uri.Host,
            Port = uri.IsDefaultPort ? 5432 : uri.Port,
            Username = Uri.UnescapeDataString(credentials[0]),
            Password = Uri.UnescapeDataString(credentials[1]),
            Database = Uri.UnescapeDataString(uri.AbsolutePath.TrimStart('/')),
            SslMode = SslMode.Require,
            TrustServerCertificate = true,
            Timeout = 30
        }.ConnectionString;
    }
}
