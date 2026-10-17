using System.Text.RegularExpressions;
using Microsoft.EntityFrameworkCore;

namespace HCareVault.Database;

/// <summary>
/// The app uses EnsureCreated (no migrations), which never changes an existing database.
/// This brings an existing medicines.db up to date with the model: it creates missing tables
/// (and their indexes) from EF's own create script, and adds missing nullable columns.
/// </summary>
public static partial class SchemaUpgrader
{
    [GeneratedRegex(@"^CREATE TABLE ""(?<t>[^""]+)""", RegexOptions.IgnoreCase)]
    private static partial Regex CreateTableRx();

    [GeneratedRegex(@"^CREATE (UNIQUE )?INDEX ""[^""]+"" ON ""(?<t>[^""]+)""", RegexOptions.IgnoreCase)]
    private static partial Regex CreateIndexRx();

    public static void Upgrade(HCareDBContext db)
    {
        if (db.Database.EnsureCreated()) return; // brand-new database, already complete

        var existingTables = db.Database
            .SqlQueryRaw<string>("SELECT name AS Value FROM sqlite_master WHERE type = 'table'")
            .ToHashSet(StringComparer.OrdinalIgnoreCase);
        var createdTables = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        var statements = db.Database.GenerateCreateScript()
            .Split(';', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

        foreach (var sql in statements)
        {
            var table = CreateTableRx().Match(sql);
            if (table.Success && !existingTables.Contains(table.Groups["t"].Value))
            {
                db.Database.ExecuteSqlRaw(sql);
                createdTables.Add(table.Groups["t"].Value);
                continue;
            }

            var index = CreateIndexRx().Match(sql);
            if (index.Success && createdTables.Contains(index.Groups["t"].Value))
                db.Database.ExecuteSqlRaw(sql);
        }

        foreach (var entity in db.Model.GetEntityTypes())
        {
            var tableName = entity.GetTableName();
            if (tableName is null || createdTables.Contains(tableName)) continue;

            var columns = db.Database
                .SqlQuery<string>($"SELECT name AS Value FROM pragma_table_info({tableName})")
                .ToHashSet(StringComparer.OrdinalIgnoreCase);

            foreach (var prop in entity.GetProperties())
            {
                var column = prop.GetColumnName();
                if (columns.Contains(column)) continue;

                var type = prop.GetColumnType();
                var nullability = prop.IsNullable ? "NULL" : $"NOT NULL DEFAULT {DefaultFor(type)}";
                db.Database.ExecuteSqlRaw($"ALTER TABLE \"{tableName}\" ADD COLUMN \"{column}\" {type} {nullability}");
            }
        }
    }

    private static string DefaultFor(string columnType) =>
        columnType.ToUpperInvariant() switch
        {
            "INTEGER" or "REAL" or "NUMERIC" => "0",
            _ => "''"
        };
}
